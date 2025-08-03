using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Model.Collidables.Trash;
using _Scripts.Model.Pickables;
using _Scripts.Model.Trackables;
using _Scripts.State.Pausing;
using _Scripts.Util;
using _Scripts.Util.Pools;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace _Scripts.Model.Entities.Snake
{
    public class SnakeEntity : GameEntity, IPickableVisitor, IPausable
    {
        public bool isGhost;
        readonly InputHandler _inputHandler = InputHandler.Instance;
        #region Position Properties
        public Vector3 InitialPosition { get; private set; }
        public Quaternion InitialRotation { get; private set; }
        List<Vector3> PositionsHistory = new();
        SnakeInputFrame _currentInputFrame;
        bool firstInputFrameMade = false;
        int maximumPositionHistoryLength = 20000;
        #endregion

        #region Speed Properties
        float baseSpeed;
        float boosterSpeed;
        float moveSpeed;
        Coroutine? speedBoostCoroutine;

        [FormerlySerializedAs("SteerSpeed")]
        public float steerSpeed = 180;
        #endregion

        #region BodyProperties
        public float distanceBetween;

        [FormerlySerializedAs("Gap")]
        public int gap = 310;

        [FormerlySerializedAs("BodyPrefab")]
        public SnakeBody bodyPrefab;
        List<SnakeBody> BodyParts = new();
        bool _bodySpawnedThisFrame = false;
        
        public event Action<Collider>? OnCollision; 

        #endregion

        #region Module Properties
        public TrackingModule TrackingModule { get; set; }
        #endregion

        #region Unity Events
        void Awake()
        {
            InitialPosition = transform.position;
            InitialRotation = transform.rotation;
        }

        void Start()
        {
            for (int i = 0; i < 100; i++)
            {
                PositionsHistory.Add(transform.position);
            }
            
        } 

        void OnEnable()
        {
            GamePauseLogicManager.Instance.Register(this);
        }
        
        void OnDisable()
        {
            GamePauseLogicManager.Instance.Unregister(this);
            RemoveBodies();
        }

        void FixedUpdate()
        {
            {
                if (GamePauseLogicManager.Instance.IsPaused) return;
                Vector2? inputVector = _inputHandler.GetMovementDirection(transform.position);
                Vector3? moveVectorDirection = null;
                if (inputVector.HasValue)
                {
                    moveVectorDirection = new Vector3(inputVector.Value.x, 0, inputVector.Value.y);
                }
                
                
                float steerDirection = Input.GetAxis("Horizontal");
                float currentSpeed = moveSpeed + boosterSpeed;
                if (TrackingModule.IsTracking())
                {
                    _currentInputFrame = new SnakeInputFrame(moveVectorDirection,
                        steerDirection,
                        Time.fixedDeltaTime,
                        _bodySpawnedThisFrame,
                        currentSpeed
                    );

                    TrackingModule.TrackAction(_currentInputFrame);
                    if (!firstInputFrameMade)
                        firstInputFrameMade = true;

                    _bodySpawnedThisFrame = false;

                    CameraController.Instance.UpdateCameraPosition(transform.position);
                }
                else if (TrackingModule.IsReplaying() && isGhost)
                {
                    _currentInputFrame = TrackingModule.GetFrameAction();
                    if (!firstInputFrameMade)
                        firstInputFrameMade = true;

                    if (_currentInputFrame.spawnedObject)
                    {
                        GrowSnake();
                    }
                }
                else
                {
                    _currentInputFrame = new SnakeInputFrame(
                        moveVectorDirection,
                        steerDirection,
                        Time.fixedDeltaTime,
                        _bodySpawnedThisFrame,
                        currentSpeed
                    );
                }

                if (!firstInputFrameMade)
                    return;

                float steer = _currentInputFrame.steerInput;
                float dt = _currentInputFrame.deltaTime;
                float currentMoveSpeed = _currentInputFrame.moveSpeed;

                transform.position += transform.forward * (currentMoveSpeed * dt);
                
                //transform.Rotate(Vector3.up * (steer * steerSpeed * dt));

                if (_currentInputFrame.moveDirection.HasValue)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(_currentInputFrame.moveDirection.Value);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        steerSpeed * dt
                    );
                }
                
                
                

                Vector3 currentFrontPosition = PositionsHistory.First();
                Vector3 difference = transform.position - currentFrontPosition;
                float distanceMoved = difference.magnitude;

                if (distanceMoved >= distanceBetween)
                {
                    int pointsToInsert = Mathf.FloorToInt(distanceMoved / distanceBetween);
                    Vector3 direction = difference.normalized;

                    for (int i = 1; i <= pointsToInsert; i++)
                    {
                        Vector3 newPoint = currentFrontPosition + direction * (distanceBetween * i);
                        PositionsHistory.Insert(0, newPoint);
                    }
                }

                PositionsHistory.Add(transform.position);

                int index = 1;
                foreach (var body in BodyParts)
                {
                    Vector3 point = PositionsHistory[
                        Mathf.Clamp(index * gap, 0, PositionsHistory.Count - 1)
                    ];
                    Vector3 moveDirection = point - body.transform.position;
                    Vector3 newPosition = body.transform.position + moveDirection;

                    if (moveDirection.sqrMagnitude > 0.001f)
                    {
                        Quaternion newRotation = Quaternion.LookRotation(moveDirection);
                        body.transform.SetPositionAndRotation(newPosition, newRotation);
                        if (!body.isInCorrectLocation)
                        {
                            body.isInCorrectLocation = true;
                            VisualEffect effect = body.GetComponentInChildren<VisualEffect>();
                            if (effect != null)
                            {
                                effect.gameObject.SetActive(true);
                                effect.Play();
                                StartCoroutine(DisableAfterDelay(effect.gameObject, 1f));
                            }
                   
                        }
                    }
                    else
                    {
                        body.transform.position = newPosition;
                    }

                    index++;
                }
#if UNITY_EDITOR

                debugTrailPoints.Add(transform.position);

#endif

                if (PositionsHistory.Count < maximumPositionHistoryLength)
                    return;
                PositionsHistory.RemoveRange(maximumPositionHistoryLength, PositionsHistory.Count - (maximumPositionHistoryLength+1));
            }
        }
        IEnumerator DisableAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            obj.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            bool bothGhosts = false;;
            bool isSnake = other.CompareTag("Snake");
            SnakeEntity? collidedSnake;
            SnakeBody? snakeBody;
            
            if (other.gameObject.TryGetComponent(out collidedSnake))
            {
                if (collidedSnake != null)
                    bothGhosts = collidedSnake.isGhost && isGhost;
            }

            if (other.gameObject.TryGetComponent(out snakeBody))
            {
                if (snakeBody != null)
                {
                    bothGhosts = snakeBody.isGhost && isGhost;
                }
            }

            if (isGhost && other.TryGetComponent(out SnakeEntity entity))
            {
                Debug.Log("The main collider was a ghost and was hitting a non Ghost player");
                return;
                
            }
         
            if (isSnake && !bothGhosts || (other.CompareTag("Wall") && !isGhost))
            {
                if (other.gameObject.TryGetComponent(out SnakeBody body) && body.isSpawning)
                {
                    body.isSpawning = false;
                    return;
                }
                OnCollision?.Invoke(other);
                return;
            }

            if (!isGhost && other.CompareTag("Trash"))
            {
                GrowSnake();
            }
        }

        void OnDestroy()
        {
            GamePauseLogicManager.Instance.Unregister(this);
            RemoveBodies();
        }

        public void RemoveBodies()
        {
            foreach (var bodyPart in BodyParts)
            {
                bodyPart.isInCorrectLocation = false;
                VisualEffect effect = bodyPart.GetComponentInChildren<VisualEffect>();
                if (effect != null && effect.gameObject != null)
                {
                    bodyPart.GetComponentInChildren<VisualEffect>().gameObject.SetActive(false);
                }
               
                ObjectPoolManager.Instance.ReturnObjectToPool(bodyPart);
            }
            BodyParts.Clear(); 
        }
        #endregion

        #region SpeedBoostLogic
        public void ApplySpeedBoost(float boostAmount, float duration)
        {
            if (speedBoostCoroutine != null)
            {
                StopCoroutine(speedBoostCoroutine);
            }

            speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(boostAmount, duration));
        }

        IEnumerator SpeedBoostRoutine(float boostAmount, float duration)
        {
            moveSpeed = baseSpeed + boostAmount;
            yield return new WaitForSeconds(duration);
            moveSpeed = baseSpeed;
            speedBoostCoroutine = null;
        }
        #endregion
        public override void Initialize(EntityDefinition definition)
        {
            Debug.Log("Snake Initialization!");
            base.Initialize(definition);

            baseSpeed = definition.BaseSpeed;
            moveSpeed = baseSpeed;
            try
            {
                TrackingModule =
                    GetModule<TrackingModule>() ?? throw new InvalidOperationException();
            }
            catch (InvalidOperationException e)
            {
                Debug.LogWarning($"Tracking module is not present on snake entity:  {name}");
            }
        }

        public void GrowSnake()
        {
            int index = BodyParts.Count * gap;
            Vector3 spawnPosition;

            if (index < PositionsHistory.Count)
            {
                spawnPosition = PositionsHistory[index];
            }
            else
            {
                spawnPosition =
                    BodyParts.Count > 0
                        ? BodyParts[BodyParts.Count - 1].transform.position
                        : transform.position - transform.forward * (gap * 0.1f);
            }
            
            GameObject body = ObjectPoolManager.Instance.SpawnObject(
                bodyPrefab,
                spawnPosition,
                Quaternion.identity, definition.spawnEffect
            );
            SnakeBody bodyToAdd = body.GetComponent<SnakeBody>();
            if (BodyParts.Count != 0)
                bodyToAdd.isSpawning = false;

            if (isGhost)
            {
                bodyToAdd.isGhost = true;
            }
            BodyParts.Add(bodyToAdd);
            _bodySpawnedThisFrame = true;
        }

        public void Visit(IPickable pickable)
        {
            if (pickable is TrashItem trash)
            {
                trash.Effects().ForEach(effect => effect.ApplyEffect(this));
            }
        }

        private List<Vector3> debugTrailPoints = new();

// #if UNITY_EDITOR
//         void OnDrawGizmos()
//         {
//             if (debugTrailPoints == null || debugTrailPoints.Count < 2)
//                 return;
//
//             if (!isGhost)
//             {
//                 Gizmos.color = Color.green;
//
//             }
//             else
//             {
//                 Gizmos.color = Color.red;
//             }
//
//             foreach (var point in debugTrailPoints)
//             {
//                 Gizmos.DrawSphere(point, 0.1f);
//             }
//         }
// #endif
        public void HandleResume()
        {
            //TODO logic to resume animations and sounds etc
        }

        public void HandlePause()
        {
            //TODO logic to pause animations and sounds etc
        }
    }
}

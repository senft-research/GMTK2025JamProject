using System;
using System.Collections.Generic;
using _Scripts.Model.Trackables;
using _Scripts.Util.Pools;
using UnityEngine;

namespace _Scripts.Model.Entities.Snake
{
    public class SnakeEntity : GameEntity
    {
        public Vector3 InitialPosition { get; private set; }
        public Quaternion InitialRotation { get; private set; }
        SnakeInputFrame currentInputFrame;

        public float MoveSpeed = 5;
        public float SteerSpeed = 180;
        public float BodySpeed = 5;
        public int Gap = 310;
        public SnakeBody BodyPrefab;
        public bool isGhost;

        List<SnakeBody> BodyParts = new();
        List<Vector3> PositionsHistory = new();
        int playbackIndex = 0;

        bool isRecording = false;
        bool isReplaying = false;
        bool spawnedObject = false;
        public TrackingModule TrackingModule { get; set; }

        private void Awake()
        {
            InitialPosition = transform.position;
            InitialRotation = transform.rotation;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Snake") || other.CompareTag("Wall"))
            {
                if (other.gameObject.TryGetComponent(out SnakeBody body) && body.isSpawning)
                {
                    body.isSpawning = false;
                    return;
                }
                Debug.Log("ROUND OVER! HIT AN OBJECT!");
                //TODO need level reset logic here
                return;
            }

            if (!isGhost && other.CompareTag("Trash"))
            {
                GrowSnake();
            }
        }

        void Start()
        {
            for (int i = 0; i < 100; i++)
            {
                PositionsHistory.Add(transform.position);
            }
        }

        void FixedUpdate()
        {
            float steerDirection = currentInputFrame.steerInput;
            float deltaTime = currentInputFrame.deltaTime;

            // Move the head
            transform.position += transform.forward * MoveSpeed * deltaTime;
            transform.Rotate(Vector3.up * steerDirection * SteerSpeed * deltaTime);
            PositionsHistory.Insert(0, transform.position);

            // Move the body
            int index = 0;
            foreach (var body in BodyParts)
            {
                Vector3 point = PositionsHistory[
                    Mathf.Clamp(index * Gap, 0, PositionsHistory.Count - 1)
                ];
                Vector3 moveDirection = point - body.transform.position;
                Vector3 newPosition =
                    body.transform.position + moveDirection * BodySpeed * deltaTime;

                if (moveDirection.sqrMagnitude > 0.001f)
                {
                    Quaternion newRotation = Quaternion.LookRotation(moveDirection);
                    body.transform.SetPositionAndRotation(newPosition, newRotation);
                }
                else
                {
                    body.transform.position = newPosition;
                }

                index++;
            }
        }

        void Update()
        {
            float steerDirection = 0f;
            float deltaTime = Time.deltaTime;

            if (TrackingModule.IsTracking())
            {
                steerDirection = Input.GetAxis("Horizontal");
                currentInputFrame = new SnakeInputFrame(
                    steerDirection,
                    Time.fixedDeltaTime,
                    spawnedObject
                );
                TrackingModule.TrackAction(currentInputFrame);
                spawnedObject = false;
            }
            else if (TrackingModule.IsReplaying() && isGhost)
            {
                currentInputFrame = TrackingModule.GetFrameAction();
                if (currentInputFrame.spawnedObject)
                {
                    GrowSnake();
                }
            }
            else
            {
                steerDirection = Input.GetAxis("Horizontal");
                currentInputFrame = new SnakeInputFrame(steerDirection, Time.fixedDeltaTime, false);
            }
        }

        public override void Initialize(EntityDefinition definition)
        {
            Debug.Log("Snake Initialization!");
            base.Initialize(definition);
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
            int index = BodyParts.Count * Gap;
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
                        : transform.position - transform.forward * (Gap * 0.1f);
            }
            GameObject body = ObjectPoolManager.Instance.SpawnObject(
                BodyPrefab,
                spawnPosition,
                Quaternion.identity
            );
            if (BodyParts.Count != 0)
                body.GetComponent<SnakeBody>().isSpawning = false;
            BodyParts.Add(body.GetComponent<SnakeBody>());
            spawnedObject = true;
        }

        void OnDestroy()
        {
            foreach (var bodyPart in BodyParts)
            {
                ObjectPoolManager.Instance.ReturnObjectToPool(bodyPart);
            }
        }
    }
}

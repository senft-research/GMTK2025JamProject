using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Util
{
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler Instance { get; private set; }

        public Camera RaycastCamera { private get; set; }
        MainPlayerInput _playerInput;
        MainPlayerInput.SnakeGameActions _snakeGame;

        public delegate void OnPauseDelegate();
        public static event OnPauseDelegate OnPause;

        Vector2? lastHitOnWorldPoint;
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                Debug.LogWarning("InputHandler: An instance of InputHandler already exists!");
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            _playerInput = new MainPlayerInput();
            _snakeGame = _playerInput.SnakeGame;

            _snakeGame.Pause.performed += HandlePause;
        }

        private void HandlePause(InputAction.CallbackContext context) => OnPause?.Invoke();

        public Vector2? GetMovementDirection(Vector3 entityPosition)
        {
            lastHitOnWorldPoint = _snakeGame.MoveDirection.ReadValue<Vector2>();
            if (_snakeGame.MoveRaycast.IsPressed())
            {
                return GetVectorFromRaycast(entityPosition);
            }

            if (_snakeGame.MoveDirection.IsPressed())
            {
                return _snakeGame.MoveDirection.ReadValue<Vector2>();
            }
            return null;
        }

        void OnEnable()
        {
            _snakeGame.Enable();
        }

        void OnDisable()
        {
            _snakeGame.Disable();
        }

        Vector2? GetVectorFromRaycast(Vector3 entityPosition)
        {
            Vector2 origin = new Vector2(entityPosition.x, entityPosition.z);
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = RaycastCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPoint = hit.point;
                Vector2 worldHit = new Vector2(hitPoint.x, hitPoint.z);
           
                lastHitOnWorldPoint = (worldHit - origin).normalized;
                return lastHitOnWorldPoint;
            }
            return null;
        }
        
        
        void OnDrawGizmos()
        {
            if (lastHitOnWorldPoint.HasValue)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(lastHitOnWorldPoint.Value, 1f);
            }
        }
    }
}

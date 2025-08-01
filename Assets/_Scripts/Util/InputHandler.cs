using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Util
{
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler Instance { get; private set; }

        private MainPlayerInput _playerInput;
        private MainPlayerInput.SnakeGameActions _snakeGame;

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
            }

            _playerInput = new MainPlayerInput();
            _snakeGame = _playerInput.SnakeGame;
        }

        public Vector2 GetMovementDirection()
        {
            return _snakeGame.MoveDirection.ReadValue<Vector2>();
        }

        private void OnEnable()
        {
            _snakeGame.Enable();
        }

        private void OnDisable()
        {
            _snakeGame.Disable();
        }
    }
}
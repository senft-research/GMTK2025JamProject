using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.State
{
    public class GameManger : MonoBehaviour
    {
        public static GameManger Instance { get; private set; }

        readonly Dictionary<GameState, Action> _actions = new();

        public GameState gameState;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            SubscribeGameStates();
            ChangeState(GameState.MainMenu);
        }

        public void ChangeState(GameState newState)
        {
            if (_actions.TryGetValue(newState, out var action))
            {
                gameState = newState;
                action?.Invoke();
            }
            else
            {
                Debug.LogWarning($"No action defined for the state: {newState}");
            }
        }

        void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }

        void LoadMainGame()
        {
            SceneManager.LoadScene("SnakeTestScene", LoadSceneMode.Single);
            SceneManager.LoadScene("GameGui", LoadSceneMode.Additive);
        }

        void QuitGame()
        {
            Debug.Log("Quitting Game!");
            Application.Quit();
        }

        void SubscribeGameStates()
        {
            _actions.Add(GameState.MainMenu, LoadMainMenu);
            _actions.Add(GameState.MainGame, LoadMainGame);
            _actions.Add(GameState.Quitting, QuitGame);
        }
    }
}

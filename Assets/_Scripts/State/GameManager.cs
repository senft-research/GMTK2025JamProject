using System;
using System.Collections.Generic;
using _Scripts.Util.Pools.Audio;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.State
{
    public class GameManger : MonoBehaviour
    {
        public static GameManger Instance { get; private set; }

        readonly Dictionary<GameState, Action> _actions = new();

        public GameState gameState;

        [SerializeField, Child]
        MusicManager musicManager;
        
        public MainGameManager MainGameManager { private get; set; }

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
            SceneManager.sceneLoaded += OnSceneLoaded;
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

        void MainGameStartTest()
        {
            SceneManager.LoadScene("GameGui", LoadSceneMode.Additive);
            MainGameManager.StartNewGame();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {

            if (scene.name == "MainGameScene")
            {
                MainGameStartTest();
            }
        }

        void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);

        }

        void QuitGame()
        {
            Debug.Log("Quitting Game!");
            Application.Quit();
        }

        void SubscribeGameStates()
        {
            _actions.Add(
                GameState.MainGame,
                (
                    () =>
                    {
                        SceneManager.LoadScene("MainGameScene", LoadSceneMode.Single);
                    }
                )
            );
            _actions.Add(GameState.MainMenu, LoadMainMenu);
            _actions.Add(GameState.Quitting, QuitGame);
            _actions.Add(GameState.UnPause, () => MainGameManager.ResumeGameLogic());
        }

        public int Score
        {
            get { return MainGameManager.CurrentPoints; }
            set { MainGameManager.CurrentPoints = value; }
        }
    }
}

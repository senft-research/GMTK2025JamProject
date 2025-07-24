using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.State
{
    public class GameManger : MonoBehaviour
    {
        public static GameManger Instance { get; private set; }
    
        readonly Dictionary<GameState, Action> _actions = new();

        [SerializeField] GameObject _playerPrefab;
        
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
                Debug.LogWarning($"No action defined for the state: {gameState}");
            }
        }

        void SubscribeGameStates()
        {
            throw new NotImplementedException("GameStates not implemented yet");

        }
    }
}

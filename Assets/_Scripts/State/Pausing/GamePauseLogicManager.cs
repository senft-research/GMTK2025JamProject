using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace _Scripts.State.Pausing
{
    public class GamePauseLogicManager : ValidatedMonoBehaviour
    {
        public static GamePauseLogicManager Instance { get; private set; }

        readonly HashSet<IPausable> pausables = new();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        public void Register(IPausable pausable) => pausables.Add(pausable);
        public void Unregister(IPausable pausable) => pausables.Remove(pausable);

        public bool IsPaused { get; private set; }

        public void PauseGame(bool isPauseMenu)
        {
            if (IsPaused) return;

            IsPaused = true;
            Time.timeScale = 0f;
            foreach (var pausable in pausables)
                pausable.HandlePause();
            if (!isPauseMenu)
            {
                return;
            }
            //TODO (ChroniclerDelta) put a pause menu call here to pause!
            
        }

        public void ResumeGame(bool isPauseMenu)
        {
            if (!IsPaused) return;

            IsPaused = false;
            Time.timeScale = 1f;
            foreach (var pausable in pausables)
                pausable.HandleResume();
            if (!isPauseMenu)
            {
                return;
            }
            //TODO (ChroniclerDelta) put a pause menu call here to unpause!

            
        }

        public void TogglePause(bool isPauseMenu)
        {
            if (IsPaused) ResumeGame(isPauseMenu);
            else PauseGame(isPauseMenu);
        }
        
    }
}

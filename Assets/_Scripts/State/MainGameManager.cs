using System;
using System.Collections.Generic;
using _Scripts.Model.Collidables;
using _Scripts.Model.Collidables.Trash;
using _Scripts.Model.Entities;
using _Scripts.Model.Entities.Snake;
using _Scripts.Model.Level;
using _Scripts.State.Pausing;
using _Scripts.UI;
using _Scripts.Util;
using _Scripts.Util.Pools.Audio;
using KBCore.Refs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.State
{
    public class MainGameManager : ValidatedMonoBehaviour
    {
        //TODO Need to add the ghost logic in, including its pool.
        //TODO Need to add logic for different types of trash body.
        //TODO Need to add animation / effects logic.
        //TODO Need to set up level music / game music logic.
        //TODO Need to POTENTIALLY set up "rewind" logic for physically rewinding the round to the beginning. 
        #region Timer Logic

        GameLogicTimer _roundTimer;
        GameLogicTimer _trashSpawnTimer;

        #endregion

        SnakeEntity _playerEntity;

        [SerializeField]
        EntityDefinition playerDefinition;

        [SerializeField]
        List<LevelDefinition> levels;

        LevelDefinition? _currentLevelDefinition;

        [SerializeField, Child]
        Canvas _canvas;

        int currentLevel = 0;

        int currentScore = 0;

        public void OnEnable()
        {
            GameManger.Instance.MainGameManager = this;
        }

        void Update()
        {
            if (GamePauseLogicManager.Instance.IsPaused)
                return;

            CheckTimersRunning();
            UpdateTimers();
        }

        void CheckTimersRunning()
        {
            if (!_trashSpawnTimer.IsRunning)
            {
                _trashSpawnTimer.Start();
            }

            if (!_roundTimer.IsRunning)
            {
                _trashSpawnTimer.Start();
            }
        }

        void UpdateTimers()
        {
            _roundTimer.Update(Time.deltaTime);
            _trashSpawnTimer.Update(Time.deltaTime);
        }

        public void StartNewGame()
        {
            ChangeLevel(currentLevel);
            SpawnPlayer();
            PauseGameLogic();
            ShowGameStartUI();
        }

        public void StartNewRound()
        {
            DespawnPlayer();
            ChangeLevel(currentLevel + 1);
            PauseGameLogic();
            ShowGameStartUI();
        }

        public void EndRound()
        {
            EndRound(true);
        }

        public void EndRound(bool timeOver)
        {
            if (!timeOver)
            {
                RoundFailureLogic();
            }
            StartNewRound();
        }

        void RoundFailureLogic()
        {
            //TODO Remove this before we public
            Debug.Log("You lose this round asshole!");
        }
        void PauseGameLogic()
        {
            GamePauseLogicManager.Instance.PauseGame(false);
            _canvas.gameObject.SetActive(true);
        }

        public void ResumeGameLogic(bool isPauseMenu = false)
        {
            if (_canvas.gameObject.activeSelf)
            {
                _canvas.gameObject.SetActive(false);
            }
            GamePauseLogicManager.Instance.ResumeGame(isPauseMenu);
        }

        public void EndGame(bool playerWon) { }

        GameObject activeTerrain;

        void LoadTerrain()
        {
            if (_currentLevelDefinition == null)
            {
                Debug.LogError("No level selected!");
                return;
            }

            activeTerrain = Instantiate(
                _currentLevelDefinition.terrainPrefab,
                new Vector3(0, -2, 0),
                Quaternion.identity
            );
        }

        void SetTimers()
        {
            _roundTimer = new GameLogicTimer(_currentLevelDefinition.roundDuration);
            _trashSpawnTimer = new GameLogicTimer(_currentLevelDefinition.trashSpawnInterval);
            _roundTimer.OnTimerFinished += EndRound;
            _roundTimer.OnTimeChanged += timeRemaining =>
            {
                UiManager.Instance.ChangeBarPercent(
                    UiElementType.Timer,
                    timeRemaining / _roundTimer.Duration
                );
            };

            _trashSpawnTimer.OnTimerFinished += () =>
            {
                Vector3 minRange = new Vector3(-41f, 0f, -21f);
                Vector3 maxRange = new Vector3(41f, 0f, 21f);
                SpawnRandomTrash(GetRandomSpawnPosition(minRange, maxRange));
            };
        }

        void UnloadTerrain()
        {
            if (activeTerrain != null)
                Destroy(activeTerrain);
        }

        void SpawnPlayer()
        {
            _playerEntity = EntitySpawner.CreateEntity<SnakeEntity>(
                playerDefinition,
                Vector3.zero,
                transform,
                Quaternion.identity
            );

            _playerEntity.TrackingModule.StartTracking();
            _playerEntity.OnCollision += other =>
            {
                EndRound(false);
            };
        }

        void DespawnPlayer()
        {
            _playerEntity.TrackingModule.StopTracking();
        }

        void ShowGameStartUI()
        {
            _canvas.gameObject.SetActive(true);
        }

        public void ShowGameOverScreen() { }

        public void ResetGame() { }

        public void Cleanup() { }

        void ChangeLevel(int levelNumber)
        {
            if (levelNumber < 0 || levelNumber >= levels.Count)
            {
                Debug.LogError("Invalid level number!");
                return;
            }

            currentLevel = levelNumber;
            _currentLevelDefinition = levels[levelNumber];
            SetTimers();
            UnloadTerrain();
            LoadTerrain();
        }

        void SpawnRandomTrash(Vector3 spawnPosition)
        {
            if (_currentLevelDefinition == null)
            {
                throw new InvalidOperationException($"The current level definition is not set!");
            }
            TrashItem trash = TrashSpawner.CreateTrash<TrashItem>(
                _currentLevelDefinition.GetRandomTrash(),
                spawnPosition,
                gameObject.transform,
                Quaternion.identity
            );
        }

        Vector3 GetRandomSpawnPosition(Vector3 minRange, Vector3 maxRange)
        {
            return new Vector3(
                Random.Range(minRange.x, maxRange.x),
                Random.Range(minRange.y, maxRange.y),
                Random.Range(minRange.z, maxRange.z)
            );
        }

        public int CurrentPoints
        {
            get { return currentScore; }
            set { currentScore += value; }
        }
    }
}

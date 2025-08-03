using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using _Scripts.Model.Collidables;
using _Scripts.Model.Collidables.Trash;
using _Scripts.Model.Entities;
using _Scripts.Model.Entities.Snake;
using _Scripts.Model.Level;
using _Scripts.Model.Level.Canvas;
using _Scripts.Model.Trackables;
using _Scripts.State.Pausing;
using _Scripts.UI;
using _Scripts.Util;
using KBCore.Refs;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace _Scripts.State
{
    public class MainGameManager : ValidatedMonoBehaviour
    {
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
        
        [SerializeField, Child]
        LevelCanvas _canvas;
        
        [SerializeField]
        List<LevelDefinition> levels;
        Vector3 minRange = new(-35f, 0f, -26f);
        Vector3 maxRange = new(35f, 0f, 26f);
        List<TrackingModule> ghostPool = new();
        int _maxLives = 3;
        int currentLives;
        int currentRoundLevel = 1;
        LevelDefinition? _currentLevelDefinition;
        GameObject activeTerrain;
        int currentLevel = 0;
        int currentScore = 0;
        public int CurrentPoints
        {
            get { return currentScore; }
            set { currentScore += value; }
        }

        #region Unity Events
        public void OnEnable()
        {
            GameManger.Instance.MainGameManager = this;
            InputHandler.OnPause += TogglePause;
        }
        public void OnDisable()
        {
            InputHandler.OnPause -= TogglePause;
        }
        void Update()
        {
            
            if (GamePauseLogicManager.Instance.IsPaused)
                return;
            if(currentLives == 0) ChangeLives(_maxLives);
            CheckTimersRunning();
            UpdateTimers();
        }
        #endregion

        #region Timer Logic
        void CheckTimersRunning()
        {
            if (!_trashSpawnTimer.IsRunning)
            {
                _trashSpawnTimer.Start();
            }

            if (!_roundTimer.IsRunning)
            {
                _roundTimer.Start();
            }
        }
        void UpdateTimers()
        {
            _roundTimer.Update(Time.deltaTime);
            _trashSpawnTimer.Update(Time.deltaTime);
        }
        void SetTimers()
        {
            _roundTimer = new GameLogicTimer(_currentLevelDefinition.roundDuration);
            _trashSpawnTimer = new GameLogicTimer(_currentLevelDefinition.trashSpawnInterval);
            _roundTimer.OnTimerFinished += () =>
            { EndRound("The Timer Ran out!");

            };
            _roundTimer.OnTimeChanged += timeRemaining =>
            {
                UiManager.Instance.ChangeBarPercent(
                    UiElementType.Timer,
                    timeRemaining / _roundTimer.Duration
                );
            };

            _trashSpawnTimer.OnTimerFinished += () =>
            {
                SpawnRandomTrash(GetRandomSpawnPosition(minRange, maxRange));
            };
        }
        #endregion

        #region Main Game Start Logic
        public void StartNewGame()
        {
            
            ChangeLevel(currentLevel);
            SpawnEntites(false);
            PauseGameLogic();
            ShowGameStartUI();
        }
        void StartNewRound()
        {
            DespawnPlayer();
            ChangeLevel(currentLevel);
            SpawnEntites(true);
            PauseGameLogic();
            currentRoundLevel += 1;

            ShowGameStartUI();
        }
        void ChangeLevel(int levelNumber)
        {
            if (levelNumber < 0 || levelNumber >= levels.Count)
            {
                return;
            }

            currentLevel = levelNumber;
            _currentLevelDefinition = levels[levelNumber];
            _canvas.LockCanvas(false);
            if (_currentLevelDefinition != null)
            {
                _canvas.SetObjectives(_currentLevelDefinition.levelInfo.objectives);
                _canvas.SetLevelText(_currentLevelDefinition.levelInfo.InfoText);
            }
            _canvas.SetMainMenuButton(false, false);
            SetTimers();
            UnloadTerrain();
            LoadTerrain();
            GameManger.Instance.SetInputHandlerCamera(Camera.main);
        }

        void LoadTerrain()
        {
            if (_currentLevelDefinition == null)
            {
                return;
            }

            activeTerrain = Instantiate(
                _currentLevelDefinition.terrainPrefab,
                new Vector3(0, -2, 0),
                Quaternion.identity
            );
        }
        void SpawnEntites(bool includeGhosts = false)
        {
            SpawnPlayer();
            if(includeGhosts) SpawnGhosts(ghostPool.Count);
        }
        void SpawnPlayer()
        {
            Vector3 randomSpawnPosition = GetRandomSpawnPosition(minRange, maxRange);
            Quaternion rotation = CenterRotation(randomSpawnPosition);
            _playerEntity = EntitySpawner.CreateEntity<SnakeEntity>(
                playerDefinition,
                randomSpawnPosition,
                transform,
                rotation
            );

            _playerEntity.TrackingModule.StartTracking();
            _playerEntity.OnCollision += CollisionEndRound;
        }
        Quaternion CenterRotation(Vector3 position)
        {       var dx = -position.x;
            var dz = -position.z;
            float angleRadians = Mathf.Atan2(dx, dz);
            var angleDegrees = angleRadians * Mathf.Rad2Deg;
            Quaternion angleRot = Quaternion.Euler(0f, angleDegrees, 0f);
            return angleRot;
        }
        void SpawnGhosts(int amount = 1)
        {
            
            for (int i = 0; i < amount; i++)
            {
                ghostPool[i].ResetReplay();
            }
        }
        #endregion

        #region End Round Logic
        void EndRound(string endRoundReason = "")
        {
            EndRound(true, endRoundReason);
        }
        void EndRound(bool timeOver, string endRoundReason = "")
        {
            Debug.Log($"END OF ROUND!: REASON: {endRoundReason}");
            if (timeOver)
            {
                StartNewRound();
                return;
            }
            
            ChangeLives(-1);
            
            if (currentLives > 0)
            {
                StartNewRound();
                return;
            }
            RoundFailureLogic();
            
        }
        public void CollisionEndRound(Collider other)
        {
            EndRound(false, $"{other.name}");
        }

        void DespawnPlayer()
        {
            _playerEntity.TrackingModule.StopTracking();
            _playerEntity.TrackingModule.InitReplayableEntity(CollisionEndRound,false);
            _playerEntity.RemoveBodies();
            _playerEntity.gameObject.SetActive(false);
            if (_playerEntity.TrackingModule.TrackingDataCount == 0)
            {
                return;
            }
            ghostPool.Add(_playerEntity.TrackingModule);
        }
        void RoundFailureLogic()
        {
            PauseGameLogic();
            _canvas.SetLevelText(
                "SIMULATION FAULT\nIf you are seeing this message, then the machine has collided into a wall or other machine, meaning this simulation has finished.\nPlease shut down the device.");
            _canvas.SetObjectives(null);
            _canvas.SetMainMenuButton(true, false);
            _canvas.LockCanvas();
            ShowGameStartUI();
        }
        void UnloadTerrain()
        {
            if (activeTerrain != null)
                Destroy(activeTerrain);
        }
        void ChangeLives(int changeAmount)
        {
            currentLives += changeAmount;
            UiManager.Instance.ChangeText(UiElementType.Lives, $"{currentLives}/{_maxLives}");
        }

        #endregion
        
        #region Pause Logic
        void TogglePause()
        {
            if (_canvas == null)
            {
                return;
            }
            
            if (_canvas.IsLocked())
            {
                return;
            }
            if (_canvas.gameObject.activeInHierarchy)
            {
                ResumeGameLogic();
                return;
            }
            _canvas.SetObjectives(null);
            _canvas.SetLevelText("Paused");
            _canvas.SetMainMenuButton(true, true);
            PauseGame();
        }
        void PauseGame()
        {
            GamePauseLogicManager.Instance.PauseGame(true);
            
            _canvas.gameObject.SetActive(true);
        }
        void PauseGameLogic()
        {
            GamePauseLogicManager.Instance.PauseGame(true);
        }
        public void ResumeGameLogic(bool isPauseMenu = false)
        {
            if (_canvas.gameObject.activeSelf)
            {
                _canvas.gameObject.SetActive(false);
            }
            UiManager.Instance.ChangeText(UiElementType.Rounds, currentRoundLevel.ToString());
            GamePauseLogicManager.Instance.ResumeGame(isPauseMenu);
        }
        #endregion

        #region Trash Spawn Logic
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
        #endregion
        
        void ShowGameStartUI()
        { 
            _canvas.gameObject.SetActive(true);
        }
  
    }
}

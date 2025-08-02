using System;
using System.Collections.Generic;
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
using Random = UnityEngine.Random;

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

        [SerializeField]
        List<LevelDefinition> levels;
        Vector3 minRange = new(-35f, 0f, -26f);
        Vector3 maxRange = new(35f, 0f, 26f);

        List<TrackingModule> ghostPool = new();
        int _maxLives = 3;
        int currentLives;
        LevelDefinition? _currentLevelDefinition;

        [SerializeField, Child]
        LevelCanvas _canvas;

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
            currentLives = _maxLives;
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
            ShowGameStartUI();
        }

        void SpawnEntites(bool includeGhosts = false)
        {
            SpawnPlayer();
            if(includeGhosts) SpawnGhosts();
        }

        public void StartNewLevel()
        {
            DespawnPlayer();
            ChangeLevel(currentLevel);
            PauseGameLogic();
            SpawnPlayer();
            ShowGameStartUI();
        }

        void EndRound(string endRoundReason = "")
        {
            EndRound(true, endRoundReason);
        }

        void EndRound(bool timeOver, string endRoundReason = "")
        {
            Debug.Log($"END OF ROUND!: REASON: {endRoundReason}");
            if (!timeOver)
            {
                RoundFailureLogic();
                return;
            }
            StartNewRound();
        }

        void RoundFailureLogic()
        {
            PauseGameLogic();
            Debug.Log("You lose this round asshole!");
            //TODO: Make this a levelInfo?
            _canvas.SetLevelText(
                "SIMULATION FAULT\nIf you are seeing this message, then the machine has collided into a wall or other machine, meaning this simulation has finished.\nPlease shut down the device.");
            _canvas.SetObjectives(null);
            _canvas.SetMainMenuButton(true);
            ShowGameStartUI();
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

        public void SpawnGhosts(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                ghostPool[Random.Range(0,ghostPool.Count)].ResetReplay();
            }
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

        void UnloadTerrain()
        {
            if (activeTerrain != null)
                Destroy(activeTerrain);
        }

        void SpawnPlayer()
        {
            Vector3 randomSpawnPosition = GetRandomSpawnPosition(minRange, maxRange);
            Quaternion rotation = CenterRotation(randomSpawnPosition);
            _playerEntity = EntitySpawner.CreateEntity<SnakeEntity>(
                playerDefinition,
                GetRandomSpawnPosition(minRange, maxRange),
                transform,
                rotation
            );

            _playerEntity.TrackingModule.StartTracking();
            _playerEntity.OnCollision += CollisionEndRound;
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
            if (_currentLevelDefinition != null)
            {
                _canvas.SetObjectives(_currentLevelDefinition.levelInfo.objectives);
                _canvas.SetLevelText(_currentLevelDefinition.levelInfo.InfoText);
            }
            _canvas.SetMainMenuButton(false);
            SetTimers();
            UnloadTerrain();
            LoadTerrain();
            GameManger.Instance.SetInputHandlerCamera(Camera.main);
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

        

        public int CurrentPoints
        {
            get { return currentScore; }
            set { currentScore += value; }
        }
        
        Vector3 GetRandomSpawnPosition(Vector3 minRange, Vector3 maxRange)
        {
            return new Vector3(
                Random.Range(minRange.x, maxRange.x),
                Random.Range(minRange.y, maxRange.y),
                Random.Range(minRange.z, maxRange.z)
            );
        }
        
        Quaternion CenterRotation(Vector3 currentPosition)
        {
            Vector3 center = (minRange + maxRange) / 2f;


            Vector3 directionToCenter = center - currentPosition;

            return Quaternion.LookRotation(directionToCenter.normalized, Vector3.up);

        }
    }
}

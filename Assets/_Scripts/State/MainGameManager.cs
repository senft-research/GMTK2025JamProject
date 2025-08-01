using System;
using System.Collections.Generic;
using _Scripts.Model.Collidables;
using _Scripts.Model.Collidables.Trash;
using _Scripts.Model.Entities;
using _Scripts.Model.Entities.Snake;
using _Scripts.Model.Level;
using _Scripts.State.Pausing;
using KBCore.Refs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.State
{
    public class MainGameManager : ValidatedMonoBehaviour
    {
        
        SnakeEntity _playerEntity;

        [SerializeField]
        EntityDefinition playerDefinition;
        
        [SerializeField]
        List<LevelDefinition> levels;

        LevelDefinition? _currentLevelDefinition;

        [SerializeField, Child]
        Canvas _canvas;
        
        int currentLevel = 0;
        public void OnEnable()
        {
            GameManger.Instance.MainGameManager = this;
            
        }

        void UpdateLoop()
        {
            
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
        
        public void EndRound() { }

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

            activeTerrain = Instantiate(_currentLevelDefinition.terrainPrefab);
            Vector3 minRange = new Vector3(-45f, 0f, -26f);
            Vector3 maxRange = new Vector3(45f, 0f, 26f);

            for (int i = 0; i < 6; i++)
            {
                Debug.Log("Spawning Trash!");
                SpawnRandomTrash(GetRandomSpawnPosition(minRange,maxRange));
            }
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
            
            UnloadTerrain();
            LoadTerrain();
        }

        void SpawnRandomTrash(Vector3 spawnPosition)
        {
            if (_currentLevelDefinition == null)
            {
                throw new InvalidOperationException($"The current level definition is not set!");
            }
            TrashItem trash = TrashSpawner.CreateTrash<TrashItem>(_currentLevelDefinition.GetRandomTrash(),
                spawnPosition,
                gameObject.transform, Quaternion.identity);
        }

        Vector3 GetRandomSpawnPosition(Vector3 minRange, Vector3 maxRange)
        {
            return new Vector3(
                Random.Range(minRange.x, maxRange.x),
                Random.Range(minRange.y, maxRange.y),
                Random.Range(minRange.z, maxRange.z)
            );
        }

    }
}

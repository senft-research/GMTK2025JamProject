using System;
using System.Collections.Generic;
using _Scripts.Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Model.Level
{
    [Serializable]
    public class LevelDefinition
    {
        public string levelName;
        public GameObject terrainPrefab;
        public Vector3 playerStartPosition;
        public List<TrashDefinition> potentialTrashItems;
        public int roundDuration;
        public int trashSpawnInterval;
        int _trashTypesAmount;

        int TrashTypesAmount()
        {
            if (_trashTypesAmount == 0)
            {
                _trashTypesAmount = potentialTrashItems.Count;
                Debug.Log($"Potential Trash Type Amount: {_trashTypesAmount}");
            }
            return _trashTypesAmount;

        }

        public TrashDefinition GetRandomTrash()
        {
            return potentialTrashItems[Random.Range(0, TrashTypesAmount())];
        }
    }

}

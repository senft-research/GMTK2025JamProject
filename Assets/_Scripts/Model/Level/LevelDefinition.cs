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
        int _trashTypesAmount;
        int TrashTypesAmount()
        {
            if (_trashTypesAmount == 0)
            {
                _trashTypesAmount = potentialTrashItems.Count;
            }
            return _trashTypesAmount;

        }

        public TrashDefinition GetRandomTrash()
        {
            return potentialTrashItems[Random.Range(0, TrashTypesAmount() - 1)];
        }
    }

}

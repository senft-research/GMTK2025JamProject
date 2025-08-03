using System;
using _Scripts.Model.Collidables.Trash;
using _Scripts.Scriptables;
using _Scripts.Util.Pools;
using UnityEngine;

namespace _Scripts.Model.Collidables
{
    public class TrashSpawner
    {
        public static T CreateTrash<T>(TrashDefinition definition,
            Vector3 position,
            Transform parent, Quaternion? rotation = null)
            where T : TrashItem
        {

            if (!definition.trashPrefab.TryGetComponent<IPoolable>(out var poolable))
            {
                throw new InvalidOperationException(
                    $"The prefab '{definition.trashPrefab.name}' provided in '{definition.name}' does not have a required IPoolable component."
                );
            }
            GameObject trashItem = ObjectPoolManager.Instance.SpawnObject(poolable,
                position,
                rotation ?? Quaternion.identity);
            
            
            var trashComponent = trashItem.GetComponent<T>();
            trashComponent.SetupTrash(definition);
            trashComponent.transform.SetParent(parent);
            return trashComponent;
        }
    }
}

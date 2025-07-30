using System;
using _Scripts.Model.Entities;
using KBCore.Refs;
using UnityEngine;

namespace _Scripts.Model
{
    public class EntitySpawner : ValidatedMonoBehaviour
    {
        public static GameObject CreateEntity(
            EntityDefinition definition,
            Vector3 position,
            Transform parent,
            Quaternion? rotation = null
        )
        {
            if (definition.entityPrefab == null)
            {
                throw new ArgumentNullException(nameof(definition), "Entity prefab must be assigned in PlayerDefinition.");
            }
            if (parent == null)
            {
                throw new ArgumentException($"Parent transform for {nameof(definition)} should not be null!");
            }
            var spawnRotation = rotation ?? Quaternion.identity;
            GameObject entityObject = Instantiate(
                definition.entityPrefab, position, spawnRotation, parent
            );

            GameEntity gameEntity = entityObject.GetComponent<GameEntity>();
            if (gameEntity == null)
            {
                throw new ArgumentException(
                    $"Player prefab '{definition.entityPrefab.name}' is missing the required PlayerEntity component.");
            }
            gameEntity.Initialize(definition);
            return entityObject;
        }
    }
}

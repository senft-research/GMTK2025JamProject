using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.VFX;

namespace _Scripts.Model.Entities
{
    public class EntitySpawner : ValidatedMonoBehaviour
    {
        public static T CreateEntity<T>(
            EntityDefinition definition,
            Vector3 position,
            Transform? parent,
            Quaternion? rotation = null
        )
            where T : GameEntity
        {
            if (definition.entityPrefab == null)
            {
                throw new ArgumentNullException(
                    nameof(definition),
                    "Entity prefab must be assigned in EntityDefinition."
                );
            }

            var spawnRotation = rotation ?? Quaternion.identity;
            
       
            
            GameObject entityObject = Instantiate(
                definition.entityPrefab,
                position,
                spawnRotation,
                parent
            );
            entityObject.transform.SetPositionAndRotation(position, spawnRotation);

            if (!entityObject.TryGetComponent<T>(out var entity))
            {
                throw new ArgumentException(
                    $"Entity prefab '{definition.entityPrefab.name}' is missing required component of type {typeof(T).Name}."
                );
            }

            entity.Initialize(definition);
            return entity;
        }
    }
}

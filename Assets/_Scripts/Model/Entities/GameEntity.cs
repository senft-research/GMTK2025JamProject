using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace _Scripts.Model.Entities
{
    public class GameEntity : ValidatedMonoBehaviour
    {
        readonly Dictionary<Type, IEntityModule> _modules = new();

        public virtual void Initialize(EntityDefinition definition)
        {
            IEntityModule[] modules = GetComponentsInChildren<IEntityModule>(true);
            Debug.Log($"Modules found: {modules.Length}");
            foreach (var module in modules)
            {
                module.Initialize(definition);
                _modules.Add(module.GetType(), module);
            }
        }

        public T? GetModule<T>()
            where T : class, IEntityModule
        {
            if (_modules.TryGetValue(typeof(T), out var module))
            {
                return module as T;
            }

            Debug.LogWarning($"Module of type {typeof(T).Name} not found on player.");
            return null;
        }
    }
}

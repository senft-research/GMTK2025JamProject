using System;
using System.Collections.Generic;
using _Scripts.Model.Entities.Snake;
using _Scripts.Scriptables;
using _Scripts.Util.Pools;
using _Scripts.Util.Pools.Audio;
using KBCore.Refs;
using UnityEngine;

namespace _Scripts.Model.Collidables.Trash
{
    public class TrashItem : CollideablePickup, IPoolable
    {
        SoundData soundData;

        List<ScriptableObject> trashEffectsScriptables;

        List<ITrashEffect> _trashEffects = new();
        public List<ITrashEffect> Effects() => _trashEffects;
 
        protected override void OnCollide(Collider? other = null)
        {
            base.OnCollide(other);
            PickupTrash();
        }

        void PickupTrash()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(gameObject.transform.position).Play(soundData);
            ObjectPoolManager.Instance.ReturnObjectToPool(this);
        }

        public void SetupTrash(TrashDefinition definition)
        {
            soundData = definition.pickupAudioData;
            trashEffectsScriptables = definition.trashEffects;

            _trashEffects.Clear();
            foreach (var scriptableObject in trashEffectsScriptables)
            {
                if (scriptableObject is ITrashEffect effect)
                {
                    _trashEffects.Add(effect);
                }
            }
        }


        public void InitReturnLogic()
        {
            
        }

        public GameObject PoolableObject() => gameObject;
    }
}

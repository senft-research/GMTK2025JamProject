using System;
using System.Collections.Generic;
using _Scripts.Model.Entities.Snake;
using _Scripts.Scriptables;
using _Scripts.Util.Pools.Audio;
using KBCore.Refs;
using UnityEngine;

namespace _Scripts.Model.Collidables.Trash
{
    public class TrashItem : CollideablePickup
    {
        [SerializeField]
        SoundData soundData;

        [SerializeField]
        List<ScriptableObject> trashEffects;

        List<ITrashEffect> _trashEffects = new();

        void Start()
        {
            foreach (var scriptableObject in trashEffects)
            {
                if (scriptableObject is ITrashEffect effect)
                {
                    _trashEffects.Add(effect);
                }
            }
        }

        public List<ITrashEffect> Effects() => _trashEffects;
        public void PrintTest()
        {
            Debug.Log($"Hello, you visited {gameObject.name}!");
        } 
        
        protected override void OnCollide(Collider? other = null)
        {
            base.OnCollide(other);
            PickupTrash();
        }

        void PickupTrash()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(gameObject.transform.position).Play(soundData);
            Destroy(gameObject);
        }
        
        
    }
}

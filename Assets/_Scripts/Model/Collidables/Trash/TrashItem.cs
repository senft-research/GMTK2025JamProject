using System;
using System.Collections.Generic;
using _Scripts.Model.Entities.Snake;
using _Scripts.Scriptables;
using _Scripts.State;
using _Scripts.UI;
using _Scripts.Util.Pools;
using _Scripts.Util.Pools.Audio;
using UnityEngine;

namespace _Scripts.Model.Collidables.Trash
{
    public class TrashItem : CollideablePickup, IPoolable
    {
        SoundData soundData;

        List<ScriptableObject> trashEffectsScriptables;

        List<ITrashEffect> _trashEffects = new();
        int _pointsValue;
        public List<ITrashEffect> Effects() => _trashEffects;
 
        protected override void OnCollide(Collider? other = null)
        {
            base.OnCollide(other);
            if (other != null)
            {
                if (other.TryGetComponent(out SnakeEntity entity) && entity.isGhost)
                {
                    return;
                }

                if (other.TryGetComponent(out SnakeBody body))
                {
                    return;
                }
            }
            
            PickupTrash();
        }

        void PickupTrash()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(gameObject.transform.position).Play(soundData);
            GameManger.Instance.Score = _pointsValue;
            UiManager.Instance.ChangeText(UiElementType.Score, GameManger.Instance.Score.ToString());
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
            _pointsValue = definition.pointsValue;
        }


        public void InitReturnLogic()
        {
            
        }

        public GameObject PoolableObject() => gameObject;
    }
}

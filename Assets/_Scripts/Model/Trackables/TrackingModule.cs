using System;
using System.Collections.Generic;
using _Scripts.Model.Entities;
using _Scripts.Model.Entities.Snake;
using KBCore.Refs;
using UnityEngine;

namespace _Scripts.Model.Trackables
{
    public class TrackingModule : ValidatedMonoBehaviour, IEntityModule, ITrackable
    {

        [SerializeField, Parent]
        SnakeEntity _entity;

        SnakeEntity ghostSnake;
        
        EntityDefinition definition;
        Action<Collider>? collisionHandler;
        [SerializeField]
        EntityDefinition? ghostDefinition;
        
        List<SnakeInputFrame> _trackingData = new();
        int _playbackIndex = 0;
        bool _isTracking = false;
        bool _isReplaying = false;
        public int TrackingDataCount
        {
            get { return _trackingData.Count; }
        }

        public void Initialize(EntityDefinition definition)
        {
            this.definition = definition;
        }

        public void TrackAction(SnakeInputFrame action)
        {
            if (!_isTracking) return;
            _trackingData.Add(action);
        }

        public SnakeInputFrame GetFrameAction()
        {
            if (_playbackIndex > _trackingData.Count - 1)
            {
                ResetReplay();
            }
            try
            {
                SnakeInputFrame currentFrameAction = _trackingData[_playbackIndex];
                _playbackIndex += 1;
                return currentFrameAction;
            }
            catch (ArgumentOutOfRangeException e)
            {
#if UNITY_EDITOR
                Debug.LogError($"Playback index out of range on object: {gameObject.name}", gameObject);
                UnityEditor.Selection.activeGameObject = gameObject; // Selects it in Hierarchy
                UnityEditor.EditorGUIUtility.PingObject(gameObject); // Pings it (flashes blue)
#endif
                throw;
            }
        }

        public bool IsTracking() => _isTracking;

        public void StartTracking()
        {
            _isTracking = true;
        }

        public void StopTracking()
        {
            _isTracking = false;
        }

        public bool IsReplaying() => _isReplaying;

        public void InitReplayableEntity(Action<Collider>? collisionHandler,bool startActive = true)
        {
            this.collisionHandler ??= collisionHandler;
            
            _isReplaying = true;
            _isTracking = false;
            if (ghostDefinition != null)
                ghostSnake =
                    EntitySpawner.CreateEntity<SnakeEntity>(ghostDefinition,
                        _entity.InitialPosition,
                        null,
                        _entity.InitialRotation);
            ghostSnake.TrackingModule = this;
            ghostSnake.isGhost = true;
            if(collisionHandler != null) ghostSnake.OnCollision += this.collisionHandler;
            ghostSnake.gameObject.SetActive(startActive);

        }

        public void ResetReplay()
        {
            _playbackIndex = 0;
            Destroy(ghostSnake.gameObject);
            InitReplayableEntity(collisionHandler);
            
        }

        public void StopReplay()
        {
            _playbackIndex = 0;
            Destroy(ghostSnake.gameObject);
        }
    }
}

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
        
        [SerializeField]
        EntityDefinition? ghostDefinition;
        
        List<SnakeInputFrame> _trackingData = new();
        int _playbackIndex = 0;
        bool _isTracking = false;
        bool _isReplaying = false;


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
            SnakeInputFrame currentFrameAction = _trackingData[_playbackIndex];
            _playbackIndex += 1;
            return currentFrameAction;
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

        public void StartReplay()
        {
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
            ghostSnake.gameObject.SetActive(true);

        }

        public void ResetReplay()
        {
            _playbackIndex = 0;
            Destroy(ghostSnake.gameObject);
            StartReplay();
        }

        public void StopReplay()
        {
            throw new System.NotImplementedException();
        }
    }
}

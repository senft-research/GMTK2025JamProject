using System;
using _Scripts.Model.Entities.Snake;
using UnityEngine;

namespace _Scripts.Model.Trackables
{
    public interface ITrackable
    {
        void TrackAction(SnakeInputFrame action);
        SnakeInputFrame GetFrameAction();
        bool IsTracking();
        void StartTracking();
        void StopTracking();
        bool IsReplaying();
        void InitReplayableEntity(Action<Collider>? collisionHandler, bool startActive = true);
        void ResetReplay();
        void StopReplay();
    }
}

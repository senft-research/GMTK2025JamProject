using _Scripts.Model.Entities.Snake;

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
        void StartReplay();
        void ResetReplay();
        void StopReplay();
    }
}

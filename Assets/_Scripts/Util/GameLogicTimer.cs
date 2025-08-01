using System;
using _Scripts.State.Pausing;
using UnityEngine;

namespace _Scripts.Util
{
    public class GameLogicTimer: IPausable
    {
        public float Duration { get; private set; }
        public float TimeRemaining { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsLooping { get; set; }
    
        public event Action OnTimerFinished;

        public event Action<float> OnTimeChanged; 
    
        public GameLogicTimer(float duration, bool isLooping = false)
        {
            Duration = duration;
            TimeRemaining = duration;
            IsLooping = isLooping;
            GamePauseLogicManager.Instance.Register(this);
        }
    
        public void Start()
        {
            TimeRemaining = Duration;
            IsRunning = true;
        }
    
        public void Stop()
        {
            IsRunning = false;
        }

        public void Resume()
        {
            IsRunning = true;
        }
    
        public void Update(float deltaTime)
        {
            if (!IsRunning) return;
    
            TimeRemaining -= deltaTime;
            TimeRemaining = Mathf.Max(0f, TimeRemaining);
            OnTimeChanged?.Invoke(TimeRemaining);
            if (TimeRemaining <= 0f)
            {
                OnTimerFinished?.Invoke();
    
                if (IsLooping)
                {
                    TimeRemaining = Duration;
                }
                else
                {
                    IsRunning = false;
                }
            }
        }
    
        public void Restart(float? newDuration = null)
        {
            if (newDuration.HasValue)
                Duration = newDuration.Value;
    
            TimeRemaining = Duration;
            IsRunning = true;
        }
    
        public void SetDuration(float newDuration)
        {
            Duration = newDuration;
            TimeRemaining = Mathf.Min(TimeRemaining, Duration);
        }

        public void HandleResume() => Resume();

        public void HandlePause() => Stop();
    }

}

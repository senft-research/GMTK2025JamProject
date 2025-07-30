using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Pool;

namespace _Scripts.Util.Pools.Audio
{
    public class SoundManager : ValidatedMonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        IObjectPool<SoundEmitter> _soundEmitterPool;
        readonly List<SoundEmitter> _activeSoundEmitters = new();
        Dictionary<SoundData, int> Counts = new();

        public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new();

        public SoundBuilder CreateSoundBuilder() => new SoundBuilder(this);

        [SerializeField]
        bool collectionCheck = true;

        [SerializeField]
        int defaultCapacity = 10;

        [SerializeField]
        int maxPoolSize = 100;

        [SerializeField]
        int maxSoundInstances = 30;

        [SerializeField]
        SoundEmitter _soundEmitterPrefab;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            InitializePool();
        }

        public bool CanPlaySound(SoundData data)
        {
            if (!data.frequentSound)
                return true;

            if (FrequentSoundEmitters.Count >= maxSoundInstances)
            {
                try
                {
                    FrequentSoundEmitters.First.Value.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }
                return false;
            }
            return true;
        }

        public SoundEmitter Get()
        {
            return _soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            _soundEmitterPool.Release(soundEmitter);
        }

        public void StopAll()
        {
            foreach (var soundEmitter in _activeSoundEmitters)
            {
                soundEmitter.Stop();
            }

            FrequentSoundEmitters.Clear();
        }

        void InitializePool()
        {
            _soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
        }

        SoundEmitter CreateSoundEmitter()
        {
            var soundEmitter = Instantiate(_soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            _activeSoundEmitters.Add(soundEmitter);
        }

        void OnReturnToPool(SoundEmitter soundEmitter)
        {
            if (soundEmitter.Node != null)
            {
                FrequentSoundEmitters.Remove(soundEmitter.Node);
                soundEmitter.Node = null;
            }
            soundEmitter.gameObject.SetActive(false);
            _activeSoundEmitters.Remove(soundEmitter);
        }

        void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }
    }
}

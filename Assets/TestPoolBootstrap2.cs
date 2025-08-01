using _Scripts.Util.Pools;
using _Scripts.Util.Pools.Audio;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPoolBootStrap2 : ValidatedMonoBehaviour
{
    [SerializeField, Child]
    InterfaceRef<IPoolable> objectToSpawn;

    [SerializeField]
    SoundData _soundData;

    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            GameObject obj = ObjectPoolManager.Instance.SpawnObject(
                objectToSpawn.Value,
                Vector3.zero,
                Quaternion.identity
            );

            if (obj.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = Vector3.forward * 10f;
            }
            SoundManager
                .Instance.CreateSoundBuilder()
                .WithPosition(Vector3.zero)
                .WithRandomPitch()
                .Play(_soundData);
        }
    }
}

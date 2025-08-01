using _Scripts.Util.Pools;
using _Scripts.Util.Pools.Audio;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPoolBootStrap : ValidatedMonoBehaviour
{
    [SerializeField, Child]
    InterfaceRef<IPoolable> objectToSpawn;

    [SerializeField]
    SoundData _soundData;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            GameObject obj = ObjectPoolManager.Instance.SpawnObject(
                objectToSpawn.Value,
                Vector3.zero,
                Quaternion.Euler(90f, 0f, 0f)
            );

            if (obj.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.freezeRotation = true;
                rb.linearVelocity = Vector3.forward * 100f;
            }
            SoundManager
                .Instance.CreateSoundBuilder()
                .WithPosition(Vector3.zero)
                .WithRandomPitch()
                .Play(_soundData);
        }
    }
}

using _Scripts.Util.Pools;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPoolBootStrap : ValidatedMonoBehaviour
{
    [SerializeField, Child]
    InterfaceRef<IPoolable> objectToSpawn;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
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
        }
    }
}

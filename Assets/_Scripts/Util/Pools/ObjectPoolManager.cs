using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace _Scripts.Util.Pools
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }

        readonly Dictionary<string, PooledObjectInfo> _objectPools = new();

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

        public GameObject SpawnObject(
            IPoolable poolableToSpawn,
            Vector3 spawnPosition,
            Quaternion spawnRotation, VisualEffect? spawnEffect = null
        )
        {
            GameObject objectToSpawn = poolableToSpawn.PoolableObject();
            PooledObjectInfo pool;
            if (_objectPools.TryGetValue(objectToSpawn.name, out var objectPool))
            {
                pool = objectPool;
            }
            else
            {
                pool = new PooledObjectInfo()
                {
                    LookupString = objectToSpawn.name,
                    PoolContainer = new GameObject(objectToSpawn.name),
                };
                pool.PoolContainer.transform.SetParent(gameObject.transform);
                _objectPools.Add(pool.LookupString, pool);
            }

            GameObject? spawnableObject = pool.InactiveObjects.FirstOrDefault();
            if (spawnableObject == null)
            {
                
                spawnableObject = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
            }
            else
            {
                Debug.Log($"Found pool item {spawnableObject.name}");
                spawnableObject.transform.position = spawnPosition;
                spawnableObject.transform.rotation = spawnRotation;
                pool.InactiveObjects.Remove(spawnableObject);
            }
            Debug.Log("Instantiating new object!");
            if (spawnEffect != null)
            {
                VisualEffect effect = Instantiate(spawnEffect, spawnPosition, spawnRotation);
                effect.transform.SetParent(spawnableObject.transform);
                effect.Stop();
            }
            spawnableObject.SetActive(true);
            spawnableObject.GetComponent<IPoolable>().InitReturnLogic();
            return spawnableObject;
        }

        public void ReturnObjectToPool(IPoolable poolableToReturn)
        {
            GameObject objectToReturn = poolableToReturn.PoolableObject();

            string objectName = objectToReturn.name.Substring(0, objectToReturn.name.Length - 7);

            if (!_objectPools.TryGetValue(objectName, out var pool))
            {
                throw new ArgumentException(
                    $"Object with name {objectToReturn.name} does not have a pool to release to!"
                );
            }

            objectToReturn.SetActive(false);
            pool.InactiveObjects.Add(objectToReturn);
            objectToReturn.transform.SetParent(pool.PoolContainer.transform);
        }
    }

    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new();
        public GameObject PoolContainer;
    }
}

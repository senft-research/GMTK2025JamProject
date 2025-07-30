using System;
using System.Collections.Generic;
using System.Linq;
using KBCore.Refs;
using UnityEngine;

namespace _Scripts.Util.Pools
{
    public class ObjectPoolManager : ValidatedMonoBehaviour
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
            Quaternion spawnRotation
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
                spawnableObject.SetActive(true);
            }
            else
            {
                spawnableObject.transform.position = spawnPosition;
                spawnableObject.transform.rotation = spawnRotation;
                pool.InactiveObjects.Remove(spawnableObject);
                spawnableObject.SetActive(true);
            }
            spawnableObject.GetComponent<IPoolable>().InitReturnLogic();
            return spawnableObject;
        }

        public void ReturnObjectToPool(GameObject objectToReturn)
        {
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

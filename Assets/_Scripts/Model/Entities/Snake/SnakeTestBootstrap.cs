using System.Collections;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace _Scripts.Model.Entities.Snake
{
    public class SnakeTestBootstrap : MonoBehaviour
    {
        SnakeEntity entity;

        [SerializeField]
        SnakeEntity ghostPrefab;

        [SerializeField]
        EntityDefinition definition;

        void Start()
        {
            entity = EntitySpawner.CreateEntity<SnakeEntity>(
                definition,
                Vector3.zero,
                transform,
                Quaternion.identity
            );
            entity.TrackingModule.StartTracking();
            StartCoroutine(RecordAndSpawnGhost());
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                entity.GrowSnake();
            }
        }

        IEnumerator RecordAndSpawnGhost()
        {
            yield return new WaitForSeconds(15f);
            entity.TrackingModule.StartReplay();
        }
    }
}

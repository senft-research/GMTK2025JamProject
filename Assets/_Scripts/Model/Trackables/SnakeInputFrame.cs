using UnityEngine;

namespace _Scripts.Model.Entities.Snake
{
    [System.Serializable]
    public struct SnakeInputFrame
    {
        public float steerInput;
        public float deltaTime;
        public float moveSpeed;
        public bool spawnedObject;
        public Vector3? moveDirection;

        public SnakeInputFrame(Vector3? moveDirection, float steerInput, float deltaTime, bool spawnedObject, float moveSpeed)
        {
            this.moveDirection = moveDirection;
            this.steerInput = steerInput;
            this.deltaTime = deltaTime;
            this.spawnedObject = spawnedObject;
            this.moveSpeed = moveSpeed;

        }
    }
}

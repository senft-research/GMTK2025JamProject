namespace _Scripts.Model.Entities.Snake
{
    [System.Serializable]
    public struct SnakeInputFrame
    {
        public float steerInput;
        public float deltaTime;
        public bool spawnedObject;

        public SnakeInputFrame(float steerInput, float deltaTime, bool spawnedObject)
        {
            this.steerInput = steerInput;
            this.deltaTime = deltaTime;
            this.spawnedObject = spawnedObject;
        }
    }
}

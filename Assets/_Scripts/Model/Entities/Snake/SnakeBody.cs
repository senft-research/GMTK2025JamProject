using _Scripts.Util.Pools;
using UnityEngine;

namespace _Scripts.Model.Entities.Snake
{
    public class SnakeBody : Collideable, IPoolable
    {
        public bool isSpawning = true;

        public void InitReturnLogic() { }

        public GameObject PoolableObject() => gameObject;

        protected override void OnCollide()
        {
            if (isSpawning)
                return;
            //TODO cancel logic here
        }
    }
}

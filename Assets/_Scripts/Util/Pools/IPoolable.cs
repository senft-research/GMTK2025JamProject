using UnityEngine;

namespace _Scripts.Util.Pools
{
    public interface IPoolable
    {
        void InitReturnLogic();
        GameObject PoolableObject();
    }
}

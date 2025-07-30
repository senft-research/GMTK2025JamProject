using System.Collections;
using UnityEngine;

namespace _Scripts.Util.Pools
{
    public class ReturnToPoolAfterTime : MonoBehaviour, IPoolable
    {
        private Coroutine? returnCoroutine;

        public void InitReturnLogic()
        {
            StartReturnTimer(2f);
        }

        void StartReturnTimer(float seconds)
        {
            if (returnCoroutine != null)
                StopCoroutine(returnCoroutine);

            returnCoroutine = StartCoroutine(ReturnAfterDelay(seconds));
        }

        IEnumerator ReturnAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ObjectPoolManager.Instance.ReturnObjectToPool(this);
        }

        void OnDisable()
        {
            if (returnCoroutine == null)
                return;
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        public GameObject PoolableObject()
        {
            return gameObject;
        }
    }
}

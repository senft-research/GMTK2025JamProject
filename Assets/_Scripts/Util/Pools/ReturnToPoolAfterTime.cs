using System.Collections;
using _Scripts.Util.Pools;
using KBCore.Refs;
using UnityEngine;

public class ReturnToPoolAfterTime : ValidatedMonoBehaviour, IPoolable
{
    private Coroutine returnCoroutine;

    void StartReturnTimer(float seconds)
    {
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);

        returnCoroutine = StartCoroutine(ReturnAfterDelay(seconds));
    }

    IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPoolManager.Instance.ReturnObjectToPool(gameObject);
    }

    void OnDisable()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }

    public void InitReturnLogic()
    {
        StartReturnTimer(2f);
    }

    public GameObject PoolableObject()
    {
        return gameObject;
    }
}

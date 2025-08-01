using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Collideable : MonoBehaviour
{
    [SerializeField]
    List<string> collisionTags;

    HashSet<string> collisionTagNames = new();

    void Awake()
    {
        foreach (var tag in collisionTags)
        {
            collisionTagNames.Add(tag);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (collisionTagNames.Contains(other.tag))
        {
            OnCollide(other);
        }
    }

    protected abstract void OnCollide(Collider? other = null);
}

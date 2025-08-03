using System;
using KBCore.Refs;
using UnityEngine;

namespace _Scripts.Model.Entities
{
    public class CameraController : ValidatedMonoBehaviour
    {
        public static CameraController Instance { get; private set; }
        Vector3 referencePosition;
        Quaternion referenceRotation;

        void Awake()
        {
            Instance = this;
            referencePosition = gameObject.transform.position;
            referenceRotation = gameObject.transform.rotation;
        }

        public void UpdateCameraPosition(Vector3 position)
        {
            transform.position = Vector3.Lerp(referencePosition, position, 0.5f);
            transform.LookAt(position);
            transform.rotation = Quaternion.Lerp(referenceRotation, transform.rotation, 0.5f);
        }
    }
}

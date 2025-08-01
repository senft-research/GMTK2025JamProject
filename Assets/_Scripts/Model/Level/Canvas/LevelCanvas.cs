using KBCore.Refs;
using UnityEngine;

namespace _Scripts.Model.Level.Canvas
{
    public class LevelCanvas : MonoBehaviour
    {
        [SerializeField, Child]
        LevelObjectives levelObjectives;

        [SerializeField, Child]
        LevelText levelText;
    }
}

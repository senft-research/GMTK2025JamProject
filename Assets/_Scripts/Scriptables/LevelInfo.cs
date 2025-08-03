using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Scriptables
{
    [CreateAssetMenu(
        fileName = "New Level Info",
        menuName = "Game/Level/Level Info",
        order = 0
    )]
    public class LevelInfo : ScriptableObject
    {
        public string InfoText;
        public List<string> objectives;
    }
}
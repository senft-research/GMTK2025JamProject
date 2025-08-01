using System.Collections.Generic;
using _Scripts.Util.Pools.Audio;
using UnityEngine;

namespace _Scripts.Scriptables
{   
    [CreateAssetMenu(
        fileName = "New Trash Definition",
        menuName = "Game/Trash/Trash Definition",
        order = 0
    )]
    public class TrashDefinition : ScriptableObject
    {
        public GameObject trashPrefab;
        public List<ScriptableObject> trashEffects;
        public int points;
        public SoundData pickupAudioData;
    }
}

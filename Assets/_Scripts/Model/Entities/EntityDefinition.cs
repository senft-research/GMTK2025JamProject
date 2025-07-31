using UnityEngine;

namespace _Scripts.Model.Entities
{
    [CreateAssetMenu(
        fileName = "New EntityDefinition",
        menuName = "Game/Entity Definition",
        order = 0
    )]
    public class EntityDefinition : ScriptableObject
    {
        public GameObject entityPrefab;
        public string entityName;
    }
}

using _Scripts.Model.Entities.Snake;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Scriptables
{
    [CreateAssetMenu(
        fileName = "New Speed Effect",
        menuName = "Game/Trash/Effects/Speed Effect",
        order = 0
    )]
    public class SpeedEffect : ScriptableObject, ITrashEffect
    {

        [FormerlySerializedAs("MoveSpeedAmmount")]
        public float SpeedBoostAmmount = 2f;

        public float BoostDuration = 5;
        public void ApplyEffect(SnakeEntity entityToEffect)
        {
            entityToEffect.ApplySpeedBoost(SpeedBoostAmmount, BoostDuration);
        }
    }
}

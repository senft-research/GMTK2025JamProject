using _Scripts.Util.Pools.Audio;
using UnityEngine;

namespace _Scripts.Model.Collidables.Trash
{
    public class TrashItem : CollideablePickup
    {
        [SerializeField]
        SoundData soundData;

        public float speedBoost = 10f;
        public void PrintTest()
        {
            Debug.Log($"Hello, you visited {gameObject.name}!");
        }
        
        protected override void OnCollide(Collider? other = null)
        {
            base.OnCollide(other);
            PickupTrash();
        }

        void PickupTrash()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(gameObject.transform.position).Play(soundData);
            Destroy(gameObject);
        }
    }
}

using _Scripts.Util.Pools.Audio;
using UnityEngine;

namespace _Scripts.Model.Collidables.Trash
{
    public class TrashItem : Collideable
    {
        [SerializeField]
        SoundData soundData;
        
        protected override void OnCollide()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(gameObject.transform.position).Play(soundData);
            Destroy(gameObject);
        }
    }
}

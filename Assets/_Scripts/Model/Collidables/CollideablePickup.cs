using System.Linq;
using _Scripts.Model.Pickables;
using UnityEngine;

namespace _Scripts.Model.Collidables
{
    public abstract class CollideablePickup : Collideable, IPickable
    {
        
        protected override void OnCollide(Collider? other = null)
        {
            Debug.Log($"Vist attempt on {gameObject.name}");
            if (!(other?.TryGetComponent<IPickableVisitor>(out var visitor) ?? false))
            {
                return;
            }
            
            Debug.Log($"Accepted the visitor!");
            Accept(visitor);
        }
 
        public void Accept(IPickableVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

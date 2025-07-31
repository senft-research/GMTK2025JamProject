namespace _Scripts.Model.Collidables.Trash
{
    public class TrashItem : Collideable
    {
        protected override void OnCollide()
        {
            Destroy(gameObject);
        }
    }
}

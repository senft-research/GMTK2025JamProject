namespace _Scripts.Model.Pickables
{
    public interface IPickableVisitor
    {
        void Visit(IPickable pickable);
    }
}

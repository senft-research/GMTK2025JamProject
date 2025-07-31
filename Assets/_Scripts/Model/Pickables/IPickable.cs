namespace _Scripts.Model.Pickables
{
    public interface IPickable
    {
        void Accept(IPickableVisitor visitor);
        
    }
}

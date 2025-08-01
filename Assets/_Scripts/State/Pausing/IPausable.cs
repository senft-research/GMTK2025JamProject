namespace _Scripts.State.Pausing
{
    public interface IPausable
    {
        void HandleResume();
        void HandlePause();
    }
}

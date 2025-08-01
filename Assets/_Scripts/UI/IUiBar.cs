namespace _Scripts.UI
{
    public interface IUiBar : IUiElement
    {
        public void ChangeBarPercent(float percent);

        public float GetBarPercent();
    }
}
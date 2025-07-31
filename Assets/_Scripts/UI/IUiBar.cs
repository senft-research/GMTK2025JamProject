namespace _Scripts.UI.Buttons
{
    public interface IUiBar
    {
        public BarType GetBarType();

        public void ChangeBarPercent(float percent);

        public float GetBarPercent();
    }
}
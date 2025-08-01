namespace _Scripts.UI
{
    public interface IUIText : IUiElement
    {
        public void ChangeText(string text);

        public int GetIntegerText();
    }
}
using _Scripts.State;
using UnityEngine;

namespace _Scripts.UI.Buttons
{
    public class GameStateButton : ButtonLogic
    {
        [SerializeField]
        GameState gameState;

        protected override void OnClick()
        {
            GameManger.Instance.ChangeState(gameState);
        }
    }
}

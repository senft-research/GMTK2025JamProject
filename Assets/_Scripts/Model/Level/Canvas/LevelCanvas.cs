using System.Collections.Generic;
using _Scripts.UI.Buttons;
using KBCore.Refs;
using UnityEngine;

namespace _Scripts.Model.Level.Canvas
{
    public class LevelCanvas : ValidatedMonoBehaviour
    {
        [SerializeField, Child]
        LevelObjectives levelObjectives;

        [SerializeField, Child]
        LevelText levelText;

        [SerializeField]
        GameStateButton button;

        public void SetObjectives(List<string>? objectives)
        {
            levelObjectives.SetObjectives(objectives);
        }

        public void SetLevelText(string text)
        {
            levelText.SetText(text);
        }

        public void SetMainMenuButton(bool isActive)
        {
            button.gameObject.SetActive(isActive);
        }
    }
}

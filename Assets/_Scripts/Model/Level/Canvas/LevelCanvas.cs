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

        private bool isLocked = false;

        public void SetObjectives(List<string>? objectives)
        {
            if (!isLocked)
            {
                levelObjectives.SetObjectives(objectives);
            }
        }

        public void SetLevelText(string text)
        {
            if (!isLocked)
            {
                levelText.SetText(text);
            }
        }

        public void SetMainMenuButton(bool isActive)
        {
            if (!isLocked)
            {
                button.gameObject.SetActive(isActive);
            }
        }

        public void LockCanvas(bool willLock = true)
        {
            isLocked = willLock;
        }

        public bool IsLocked()
        {
            return isLocked;
        }
    }
}

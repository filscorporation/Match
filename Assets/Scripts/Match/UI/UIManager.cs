﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Match.UI
{
    /// <summary>
    /// Base class for UI managers
    /// </summary>
    public abstract class UIManager : MonoBehaviour
    {
        protected const string mainMenuSceneName = "MainMenuScene";
        protected const string gameSceneName = "GameScene";

        /// <summary>
        /// List of all buttons in control of this manager
        /// Child class should overload it with button names for warm up
        /// </summary>
        protected abstract Dictionary<string, Button> Buttons { get; set; }

        public void Start()
        {
            WarmUpButtons();
            WarmUp();
        }

        /// <summary>
        /// Is called on start
        /// </summary>
        protected abstract void WarmUp();

        /// <summary>
        /// Searches for button according to Button dictionary
        /// </summary>
        private void WarmUpButtons()
        {
            List<string> buttonNames = Buttons.Keys.ToList();
            List<GameObject> objects = Resources.FindObjectsOfTypeAll<Button>().Select(b => b.gameObject).ToList();
            foreach (string button in buttonNames)
            {
                Buttons[button] = objects.FirstOrDefault(o => o.name == button)?.GetComponent<Button>();
            }
        }

        /// <summary>
        /// Enables button by name
        /// </summary>
        /// <param name="button"></param>
        protected void Enable(string button)
        {
            Buttons[button].gameObject.SetActive(true);
        }

        /// <summary>
        /// Disables button by name
        /// </summary>
        /// <param name="button"></param>
        protected void Disable(string button)
        {
            Buttons[button].gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets button interactable by name
        /// </summary>
        /// <param name="button"></param>
        /// <param name="interactable"></param>
        protected void SetInteractable(string button, bool interactable)
        {
            Buttons[button].interactable = interactable;
        }
    }
}

﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Match
{
    public class UIManager : MonoBehaviour
    {
        public GameManager GameManager;
        
        private const string menuButton = "MenuButton";
        private const string backButton = "BackButton";
        private const string toMainMenuButton = "ToMainMenuButton";
        private const string playButton = "PlayButton";

        private const string blurImageName = "Blur";

        private const string mainMenuSceneName = "MainMenuScene";
        private const string gameSceneName = "GameScene";

        private GameObject blur;
        private bool uiMode = false;

        private readonly Dictionary<string, GameObject> buttons = new Dictionary<string, GameObject>
        {
            { menuButton, null },
            { backButton, null },
            { toMainMenuButton, null },
            { playButton, null },
        };

        public void Start()
        {
            WarmUpButtons();
        }

        public bool IsUIMode() => uiMode;

        private void WarmUpButtons()
        {
            List<string> buttonNames = buttons.Keys.ToList();
            List<GameObject> objects = Resources.FindObjectsOfTypeAll<Button>().Select(b => b.gameObject).ToList();
            foreach (string button in buttonNames)
            {
                buttons[button] = objects.FirstOrDefault(o => o.name == button);
            }

            blur = Resources.FindObjectsOfTypeAll<CanvasRenderer>().FirstOrDefault(o => o.name == blurImageName)?.gameObject;
        }

        private void Disable(string button)
        {
            buttons[button].SetActive(false);
        }

        private void Enable(string button)
        {
            buttons[button].SetActive(true);
        }

        private void ToUIMode()
        {
            uiMode = true;
            blur.SetActive(true);
        }

        private void FromUIMode()
        {
            uiMode = false;
            blur.SetActive(false);
        }

        public void MenuButtonClick()
        {
            ToUIMode();
            Disable(menuButton);
            Enable(backButton);
            Enable(toMainMenuButton);
        }

        public void BackButtonClick()
        {
            FromUIMode();
            Disable(backButton);
            Disable(toMainMenuButton);
            Enable(menuButton);
        }

        public void ToMainMenuButtonClick()
        {
            GameManager.EndGame();
            SceneManager.LoadScene(mainMenuSceneName);
        }

        public void PlayButtonClick()
        {
            // TODO: fill GameSettings
            GameSettings.FieldWidth = 6;
            GameSettings.FieldHeigth = 5;
            GameSettings.PlayersCount = 1;
            SceneManager.LoadScene(gameSceneName);
        }
    }
}

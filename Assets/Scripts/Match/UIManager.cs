using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Match
{
    public class UIManager : MonoBehaviour
    {
        private const string menuButton = "MenuButton";
        private const string backButton = "BackButton";
        private const string toMainMenuButton = "ToMainMenuButton";
        private const string playButton = "PlayButton";
        private const string restartButton = "RestartButton";

        private const string blurImageName = "Blur";
        private const string activePlayerFrameName = "ActivePlayerFrame";

        private const string mainMenuSceneName = "MainMenuScene";
        private const string gameSceneName = "GameScene";

        private const string defaultCardPackage = "DefaultPack";
        private const string geometryCardPackage = "GeometryPack";
        private const string colorsCardPackage = "ColorsPack";

        private GameObject blur;
        private bool uiMode = false;
        private GameObject activePlayerFrame;
        private Dictionary<string, Text> playersTexts;

        private readonly Dictionary<string, GameObject> buttons = new Dictionary<string, GameObject>
        {
            { menuButton, null },
            { backButton, null },
            { toMainMenuButton, null },
            { playButton, null },
            { restartButton, null },
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

            CanvasRenderer[] elements = Resources.FindObjectsOfTypeAll<CanvasRenderer>();
            blur = elements.FirstOrDefault(o => o.name == blurImageName)?.gameObject;
            if (blur != null)
            {
                blur.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            }
            activePlayerFrame = elements.FirstOrDefault(o => o.name == activePlayerFrameName)?.gameObject;
        }

        public void DrawPlayers(List<Player> players, int active)
        {
            if (playersTexts == null)
            {
                if (players.Count > 2)
                    throw new NotSupportedException("Too much players");
                playersTexts = new Dictionary<string, Text>();
                Text[] elements = Resources.FindObjectsOfTypeAll<Text>();
                foreach (Player player in players)
                {
                    Text text = elements.First(t => t.gameObject.name == player.Name + " Text");
                    text.gameObject.SetActive(true);
                    playersTexts.Add(player.Name, text);
                }
            }

            foreach (Player player in players)
            {
                playersTexts[player.Name].text = player.Name + ": " + player.Score;
            }

            if (players.Count == 1)
            {
                activePlayerFrame.SetActive(false);
            }
            else
            {
                activePlayerFrame.SetActive(true);
                activePlayerFrame.transform.SetParent(playersTexts[players[active].Name].transform);
                activePlayerFrame.transform.localPosition = Vector3.zero;
            }
        }

        public void OpenRestartMenu(float delay)
        {
            Disable(menuButton);
            Invoke(nameof(OpenRestartMenu), delay);
        }

        public void OpenRestartMenu()
        {
            ToUIMode();
            Disable(menuButton);
            Disable(backButton);
            Enable(toMainMenuButton);
            Enable(restartButton);
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
            SceneManager.LoadScene(mainMenuSceneName);
        }

        public void PlayButtonClick()
        {
            // TODO: fill GameSettings
            //GameSettings.FieldWidth = 6;
            //GameSettings.FieldHeigth = 5;
            //GameSettings.PlayersCount = 2;
            GameSettings.CardPackageName = geometryCardPackage;
            SceneManager.LoadScene(gameSceneName);
        }

        public void RestartButtonClick()
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}

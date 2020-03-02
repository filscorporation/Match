using System.Collections.Generic;
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
        private const string activePlayerFrameName = "ActivePlayerFrame";

        private const string mainMenuSceneName = "MainMenuScene";
        private const string gameSceneName = "GameScene";

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
            activePlayerFrame = elements.FirstOrDefault(o => o.name == activePlayerFrameName)?.gameObject;
        }

        public void DrawPlayers(List<Player> players, int active)
        {
            if (playersTexts == null)
            {
                playersTexts = new Dictionary<string, Text>();
                int i = 0;
                foreach (Player player in players)
                {
                    GameObject playerText = new GameObject(player.Name + " Text");
                    playerText.AddComponent<CanvasRenderer>();
                    Text text = playerText.AddComponent<Text>();
                    text.transform.SetParent(transform);
                    text.resizeTextForBestFit = true;
                    text.resizeTextMaxSize = 260;
                    text.font = Resources.FindObjectsOfTypeAll<Font>().FirstOrDefault();
                    text.color = Color.black;
                    text.rectTransform.anchorMin = new Vector2(0, 1);
                    text.rectTransform.anchorMax = new Vector2(0, 1);
                    text.rectTransform.pivot = new Vector2(0.5F, 0.5F);
                    text.rectTransform.sizeDelta = new Vector2(600, 120);
                    text.rectTransform.anchoredPosition = new Vector2(360, -100 - 120*i);
                    playersTexts.Add(player.Name, text);
                    i++;
                }
            }

            foreach (Player player in players)
            {
                playersTexts[player.Name].text = player.Name + ": " + player.Score;
            }
            activePlayerFrame.transform.SetParent(playersTexts[players[active].Name].transform);
            activePlayerFrame.transform.localPosition = Vector3.zero;
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
            GameSettings.PlayersCount = 2;
            SceneManager.LoadScene(gameSceneName);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Match.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Match.UI
{
    /// <summary>
    /// UI manager used in game
    /// </summary>
    public class GameUIManager : UIManager
    {
        private const string menuButton = "MenuButton";
        private const string backButton = "BackButton";
        private const string toMainMenuButton = "ToMainMenuButton";
        private const string restartButton = "RestartButton";

        private const string blurImageName = "Blur";
        private const string activePlayerFrameName = "ActivePlayerFrame";

        private string turnsCounterTextName = "Turns Text";
        private string timeMeterTextName = "Time Text";
        private Text turnsCounterText;
        private Text timeMeterText;

        private GameObject blur;
        private bool uiMode = false;
        private GameObject activePlayerFrame;
        private Dictionary<string, Text> playersTexts;

        /// <summary>
        /// Check if UI menu is opened
        /// </summary>
        /// <returns></returns>
        public bool IsUIMode() => uiMode;

        protected override Dictionary<string, Button> Buttons { get; set; } = new Dictionary<string, Button>
        {
            { menuButton, null },
            { backButton, null },
            { toMainMenuButton, null },
            { restartButton, null },
        };

        protected override void WarmUp()
        {
            CanvasRenderer[] elements = Resources.FindObjectsOfTypeAll<CanvasRenderer>();
            blur = elements.FirstOrDefault(o => o.name == blurImageName).gameObject;
            if (blur != null)
            {
                blur.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            }
            activePlayerFrame = elements.FirstOrDefault(o => o.name == activePlayerFrameName).gameObject;

            turnsCounterText = elements.FirstOrDefault(o => o.name == turnsCounterTextName).gameObject.GetComponent<Text>();
            timeMeterText = elements.FirstOrDefault(o => o.name == timeMeterTextName).gameObject.GetComponent<Text>();
        }

        /// <summary>
        /// Draws all players icons and score
        /// </summary>
        /// <param name="players">Players list</param>
        /// <param name="active">Index of active player (whos turn)</param>
        public void DrawPlayers(List<Player> players, int active)
        {
            if (playersTexts == null)
            {
                if (players.Count > 4)
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
                playersTexts[player.Name].text = $"{player.Name}: {player.Score}";
            }

            activePlayerFrame.SetActive(true);
            activePlayerFrame.transform.SetParent(playersTexts[players[active].Name].transform);
            activePlayerFrame.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// Draws player stats
        /// </summary>
        /// <param name="stats"></param>
        public void DrawSinglePlayerStats(SinglePlayerStats stats)
        {
            turnsCounterText.gameObject.SetActive(true);
            timeMeterText.gameObject.SetActive(true);
            turnsCounterText.text = $"Turns: {stats.Turns}";
            if (stats.TurnsHighscore != -1)
                turnsCounterText.text += $" (Best: {stats.TurnsHighscore})";
            TimeSpan tt = TimeSpan.FromSeconds(stats.Time);
            string ttString = tt.ToString("mm':'ss");
            timeMeterText.text = $"Time: {ttString}";
            if (stats.TimeHighscore != -1)
            {
                TimeSpan th = TimeSpan.FromSeconds(stats.TimeHighscore);
                string thString = th.ToString("mm':'ss");
                timeMeterText.text += $" (Best: {thString})";
            }
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

        public void MenuButtonClick()
        {
            ToUIMode();
            Disable(menuButton);
            Enable(backButton);
            Enable(toMainMenuButton);
        }

        public void RestartButtonClick()
        {
            if (GameSettings.IsOnline)
            {
                NetworkManager.Instance.RestartGame();
            }
            else
            {
                SceneManager.LoadScene(gameSceneName);
            }
        }
    }
}

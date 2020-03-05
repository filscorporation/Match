using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Match.UI
{
    /// <summary>
    /// UI manager used in main menu
    /// </summary>
    public class MainMenuUIManager : UIManager
    {
        private const string playButton = "PlayButton";
        private const string singlePlayerButton = "SinglePlayerButton";
        private const string multiPlayerButton = "MultiPlayerButton";

        private const string defaultCardPackage = "DefaultPack";
        private const string geometryCardPackage = "GeometryPack";
        private const string colorsCardPackage = "ColorsPack";

        protected override Dictionary<string, GameObject> Buttons { get; set; } = new Dictionary<string, GameObject>
        {
            { playButton, null },
            { singlePlayerButton, null },
            { multiPlayerButton, null },
        };

        protected override void WarmUp()
        {
            switch (GameSettings.PlayersCount)
            {
                case 1:
                    SetInteractable(singlePlayerButton, false);
                    SetInteractable(multiPlayerButton, true);
                    break;
                case 2:
                    SetInteractable(singlePlayerButton, true);
                    SetInteractable(multiPlayerButton, false);
                    break;
                default:
                    throw new NotSupportedException("Too much players");
            }
        }

        public void PlayButtonClick()
        {
            //GameSettings.FieldWidth = 6;
            //GameSettings.FieldHeigth = 5;
            GameSettings.CardPackageName = colorsCardPackage;
            SceneManager.LoadScene(gameSceneName);
        }

        public void SinglePlayerButtonClick()
        {
            SetInteractable(singlePlayerButton, false);
            SetInteractable(multiPlayerButton, true);
            GameSettings.PlayersCount = 1;
        }

        public void MultiPlayerButtonClick()
        {
            SetInteractable(singlePlayerButton, true);
            SetInteractable(multiPlayerButton, false);
            GameSettings.PlayersCount = 2;
        }
    }
}

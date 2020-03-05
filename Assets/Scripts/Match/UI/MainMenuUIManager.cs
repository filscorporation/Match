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
        private const string defaultCardPackButton = "DefaultCardPackButton";
        private const string colorsCardPackButton = "ColorsCardPackButton";
        private const string geometryCardPackButton = "GeometryCardPackButton";

        private const string defaultCardPackage = "DefaultPack";
        private const string geometryCardPackage = "GeometryPack";
        private const string colorsCardPackage = "ColorsPack";

        protected override Dictionary<string, GameObject> Buttons { get; set; } = new Dictionary<string, GameObject>
        {
            { playButton, null },
            { singlePlayerButton, null },
            { multiPlayerButton, null },
            { defaultCardPackButton, null },
            { colorsCardPackButton, null },
            { geometryCardPackButton, null },
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

            switch (GameSettings.CardPackageName)
            {
                case defaultCardPackage:
                    SetInteractable(defaultCardPackButton, false);
                    SetInteractable(colorsCardPackButton, true);
                    SetInteractable(geometryCardPackButton, true);
                    GameSettings.FieldHeigth = 5;
                    GameSettings.FieldWidth = 6;
                    break;
                case colorsCardPackage:
                    SetInteractable(defaultCardPackButton, true);
                    SetInteractable(colorsCardPackButton, false);
                    SetInteractable(geometryCardPackButton, true);
                    GameSettings.FieldHeigth = 5;
                    GameSettings.FieldWidth = 6;
                    break;
                case geometryCardPackage:
                    SetInteractable(defaultCardPackButton, true);
                    SetInteractable(colorsCardPackButton, true);
                    SetInteractable(geometryCardPackButton, false);
                    GameSettings.FieldHeigth = 4;
                    GameSettings.FieldWidth = 5;
                    break;
                default:
                    throw new NotSupportedException($"Unknown card package name: {GameSettings.CardPackageName}");
            }
        }

        public void PlayButtonClick()
        {

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

        public void DefaultCardPackButtonClick()
        {
            SetInteractable(defaultCardPackButton, false);
            SetInteractable(colorsCardPackButton, true);
            SetInteractable(geometryCardPackButton, true);
            GameSettings.FieldHeigth = 5;
            GameSettings.FieldWidth = 6;
            GameSettings.CardPackageName = defaultCardPackage;
        }

        public void ColorsCardPackButtonClick()
        {
            SetInteractable(defaultCardPackButton, true);
            SetInteractable(colorsCardPackButton, false);
            SetInteractable(geometryCardPackButton, true);
            GameSettings.FieldHeigth = 5;
            GameSettings.FieldWidth = 6;
            GameSettings.CardPackageName = colorsCardPackage;
        }

        public void GeometryCardPackButtonClick()
        {
            SetInteractable(defaultCardPackButton, true);
            SetInteractable(colorsCardPackButton, true);
            SetInteractable(geometryCardPackButton, false);
            GameSettings.FieldHeigth = 4;
            GameSettings.FieldWidth = 5;
            GameSettings.CardPackageName = geometryCardPackage;
        }
    }
}

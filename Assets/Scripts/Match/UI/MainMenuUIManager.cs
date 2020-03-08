using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Match.UI
{
    /// <summary>
    /// UI manager used in main menu
    /// </summary>
    public class MainMenuUIManager : UIManager
    {
        private const string playButton = "PlayButton";
        private const string singlePlayerButton = "SinglePlayerButton";
        private const string twoPlayerButton = "TwoPlayerButton";
        private const string threePlayerButton = "ThreePlayerButton";
        private const string fourPlayerButton = "FourPlayerButton";
        private const string defaultCardPackButton = "DefaultCardPackButton";
        private const string colorsCardPackButton = "ColorsCardPackButton";
        private const string geometryCardPackButton = "GeometryCardPackButton";

        private CardPack defaultCardPackage = new CardPack("DefaultPack", 2, 2);
        private CardPack geometryCardPackage = new CardPack("GeometryPack", 5, 4);
        private CardPack colorsCardPackage = new CardPack("ColorsPack", 6, 5);

        private OptionsButtonsManager<CardPack> cardPacksButtons;
        private OptionsButtonsManager<int> playersCountButtons;

        protected override Dictionary<string, Button> Buttons { get; set; } = new Dictionary<string, Button>
        {
            { playButton, null },
            { singlePlayerButton, null },
            { twoPlayerButton, null },
            { threePlayerButton, null },
            { fourPlayerButton, null },
            { defaultCardPackButton, null },
            { colorsCardPackButton, null },
            { geometryCardPackButton, null },
        };

        protected override void WarmUp()
        {
            SetDefaultCardPackage();

            List<ButtonWrapper<int>> pcbs = new List<ButtonWrapper<int>>();
            pcbs.Add(new ButtonWrapper<int>(Buttons[singlePlayerButton], 1));
            pcbs.Add(new ButtonWrapper<int>(Buttons[twoPlayerButton], 2));
            pcbs.Add(new ButtonWrapper<int>(Buttons[threePlayerButton], 3));
            pcbs.Add(new ButtonWrapper<int>(Buttons[fourPlayerButton], 4));
            playersCountButtons = new OptionsButtonsManager<int>(pcbs, GameSettings.PlayersCount);

            List<ButtonWrapper<CardPack>> cpbs = new List<ButtonWrapper<CardPack>>();
            cpbs.Add(new ButtonWrapper<CardPack>(Buttons[defaultCardPackButton], defaultCardPackage));
            cpbs.Add(new ButtonWrapper<CardPack>(Buttons[colorsCardPackButton], colorsCardPackage));
            cpbs.Add(new ButtonWrapper<CardPack>(Buttons[geometryCardPackButton], geometryCardPackage));
            cardPacksButtons = new OptionsButtonsManager<CardPack>(cpbs, GameSettings.CardPackage);
        }

        private void SetDefaultCardPackage()
        {
            if (GameSettings.CardPackage == null)
            {
                GameSettings.CardPackage = defaultCardPackage;
                GameSettings.FieldWidth = defaultCardPackage.MaxWidth;
                GameSettings.FieldHeigth = defaultCardPackage.MaxHeigth;
            }
        }

        public void PlayButtonClick()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void SinglePlayerButtonClick()
        {
            GameSettings.PlayersCount = 1;
            playersCountButtons.SelectOption(GameSettings.PlayersCount);
        }

        public void TwoPlayerButtonClick()
        {
            GameSettings.PlayersCount = 2;
            playersCountButtons.SelectOption(GameSettings.PlayersCount);
        }

        public void ThreePlayerButtonClick()
        {
            GameSettings.PlayersCount = 3;
            playersCountButtons.SelectOption(GameSettings.PlayersCount);
        }

        public void FourPlayerButtonClick()
        {
            GameSettings.PlayersCount = 4;
            playersCountButtons.SelectOption(GameSettings.PlayersCount);
        }

        public void DefaultCardPackButtonClick()
        {
            GameSettings.CardPackage = defaultCardPackage;
            GameSettings.FieldHeigth = GameSettings.CardPackage.MaxHeigth;
            GameSettings.FieldWidth = GameSettings.CardPackage.MaxWidth;
            cardPacksButtons.SelectOption(GameSettings.CardPackage);
        }

        public void ColorsCardPackButtonClick()
        {
            GameSettings.CardPackage = colorsCardPackage;
            GameSettings.FieldHeigth = GameSettings.CardPackage.MaxHeigth;
            GameSettings.FieldWidth = GameSettings.CardPackage.MaxWidth;
            cardPacksButtons.SelectOption(GameSettings.CardPackage);
        }

        public void GeometryCardPackButtonClick()
        {
            GameSettings.CardPackage = geometryCardPackage;
            GameSettings.FieldHeigth = GameSettings.CardPackage.MaxHeigth;
            GameSettings.FieldWidth = GameSettings.CardPackage.MaxWidth;
            cardPacksButtons.SelectOption(GameSettings.CardPackage);
        }
    }
}

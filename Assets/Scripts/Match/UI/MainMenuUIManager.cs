using System.Collections.Generic;
using Assets.Scripts.Match.CardManagement;
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
        private const string onlineButton = "OnlineButton";

        private const string kitchenCardPackButton = "KitchenCardPackButton";
        private const string artCardPackButton = "ArtCardPackButton";
        private const string oceanCardPackButton = "OceanCardPackButton";

        private readonly CardPack kitchenCardPackage = CardPackages.Packages["KitchenPack"];
        private readonly CardPack artCardPackage = CardPackages.Packages["ArtPack"];
        private readonly CardPack oceanCardPackage = CardPackages.Packages["OceanPack"];

        private OptionsButtonsManager<CardPack> cardPacksButtons;
        private OptionsButtonsManager<int> playersCountButtons;

        protected override Dictionary<string, Button> Buttons { get; set; } = new Dictionary<string, Button>
        {
            { playButton, null },
            { singlePlayerButton, null },
            { twoPlayerButton, null },
            { threePlayerButton, null },
            { fourPlayerButton, null },
            { onlineButton, null },
            { kitchenCardPackButton, null },
            { artCardPackButton, null },
            { oceanCardPackButton, null },
        };

        protected override void WarmUp()
        {
            SetDefaultCardPackage();

            List<ButtonWrapper<int>> pcbs = new List<ButtonWrapper<int>>();
            pcbs.Add(new ButtonWrapper<int>(Buttons[singlePlayerButton], 1));
            pcbs.Add(new ButtonWrapper<int>(Buttons[twoPlayerButton], 2));
            pcbs.Add(new ButtonWrapper<int>(Buttons[threePlayerButton], 3));
            pcbs.Add(new ButtonWrapper<int>(Buttons[fourPlayerButton], 4));
            pcbs.Add(new ButtonWrapper<int>(Buttons[onlineButton], 0));
            playersCountButtons = new OptionsButtonsManager<int>(pcbs, GameSettings.IsOnline ? 0 : GameSettings.PlayersCount);

            List<ButtonWrapper<CardPack>> cpbs = new List<ButtonWrapper<CardPack>>();
            cpbs.Add(new ButtonWrapper<CardPack>(Buttons[kitchenCardPackButton], kitchenCardPackage));
            cpbs.Add(new ButtonWrapper<CardPack>(Buttons[artCardPackButton], artCardPackage));
            cpbs.Add(new ButtonWrapper<CardPack>(Buttons[oceanCardPackButton], oceanCardPackage));
            cardPacksButtons = new OptionsButtonsManager<CardPack>(cpbs, GameSettings.CardPackage);
        }

        private void SetDefaultCardPackage()
        {
            if (GameSettings.CardPackage == null)
            {
                GameSettings.CardPackage = kitchenCardPackage;
                GameSettings.FieldWidth = kitchenCardPackage.MaxWidth;
                GameSettings.FieldHeight = kitchenCardPackage.MaxHeight;
            }
        }

        public void PlayButtonClick()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void SinglePlayerButtonClick()
        {
            GameSettings.PlayersCount = 1;
            GameSettings.IsOnline = false;
            playersCountButtons.SelectOption(GameSettings.PlayersCount);
        }

        public void TwoPlayerButtonClick()
        {
            GameSettings.PlayersCount = 2;
            GameSettings.IsOnline = false;
            playersCountButtons.SelectOption(GameSettings.PlayersCount);
        }

        public void ThreePlayerButtonClick()
        {
            GameSettings.PlayersCount = 3;
            GameSettings.IsOnline = false;
            playersCountButtons.SelectOption(GameSettings.PlayersCount);
        }

        public void FourPlayerButtonClick()
        {
            GameSettings.PlayersCount = 4;
            GameSettings.IsOnline = false;
            playersCountButtons.SelectOption(GameSettings.PlayersCount);
        }

        public void OnlineButtonClick()
        {
            GameSettings.PlayersCount = 2;
            GameSettings.IsOnline = true;
            playersCountButtons.SelectOption(0);
        }

        public void KitchenCardPackButtonClick()
        {
            GameSettings.CardPackage = kitchenCardPackage;
            GameSettings.FieldHeight = GameSettings.CardPackage.MaxHeight;
            GameSettings.FieldWidth = GameSettings.CardPackage.MaxWidth;
            cardPacksButtons.SelectOption(GameSettings.CardPackage);
        }

        public void ArtCardPackButtonClick()
        {
            GameSettings.CardPackage = artCardPackage;
            GameSettings.FieldHeight = GameSettings.CardPackage.MaxHeight;
            GameSettings.FieldWidth = GameSettings.CardPackage.MaxWidth;
            cardPacksButtons.SelectOption(GameSettings.CardPackage);
        }

        public void OceanCardPackButtonClick()
        {
            GameSettings.CardPackage = oceanCardPackage;
            GameSettings.FieldHeight = GameSettings.CardPackage.MaxHeight;
            GameSettings.FieldWidth = GameSettings.CardPackage.MaxWidth;
            cardPacksButtons.SelectOption(GameSettings.CardPackage);
        }
    }
}

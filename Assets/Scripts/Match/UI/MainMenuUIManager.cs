using System.Collections.Generic;
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

        private const string kitchenCardPackButton = "KitchenCardPackButton";
        private const string artCardPackButton = "ArtCardPackButton";
        private const string oceanCardPackButton = "OceanCardPackButton";

        private readonly CardPack kitchenCardPackage = new CardPack("KitchenPack", 5, 6);
        private readonly CardPack artCardPackage = new CardPack("ArtPack", 5, 6);
        private readonly CardPack oceanCardPackage = new CardPack("OceanPack", 5, 6);

        private OptionsButtonsManager<CardPack> cardPacksButtons;
        private OptionsButtonsManager<int> playersCountButtons;

        protected override Dictionary<string, Button> Buttons { get; set; } = new Dictionary<string, Button>
        {
            { playButton, null },
            { singlePlayerButton, null },
            { twoPlayerButton, null },
            { threePlayerButton, null },
            { fourPlayerButton, null },
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
            playersCountButtons = new OptionsButtonsManager<int>(pcbs, GameSettings.PlayersCount);

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
                GameSettings.FieldHeigth = kitchenCardPackage.MaxHeigth;
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

        public void KitchenCardPackButtonClick()
        {
            GameSettings.CardPackage = kitchenCardPackage;
            GameSettings.FieldHeigth = GameSettings.CardPackage.MaxHeigth;
            GameSettings.FieldWidth = GameSettings.CardPackage.MaxWidth;
            cardPacksButtons.SelectOption(GameSettings.CardPackage);
        }

        public void ArtCardPackButtonClick()
        {
            GameSettings.CardPackage = artCardPackage;
            GameSettings.FieldHeigth = GameSettings.CardPackage.MaxHeigth;
            GameSettings.FieldWidth = GameSettings.CardPackage.MaxWidth;
            cardPacksButtons.SelectOption(GameSettings.CardPackage);
        }

        public void OceanCardPackButtonClick()
        {
            GameSettings.CardPackage = oceanCardPackage;
            GameSettings.FieldHeigth = GameSettings.CardPackage.MaxHeigth;
            GameSettings.FieldWidth = GameSettings.CardPackage.MaxWidth;
            cardPacksButtons.SelectOption(GameSettings.CardPackage);
        }
    }
}

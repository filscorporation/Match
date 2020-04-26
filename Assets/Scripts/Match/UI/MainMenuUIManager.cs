using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Match.CardManagement;
using Assets.Scripts.Match.Networking;
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
        private static MainMenuUIManager instance;

        public static MainMenuUIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<MainMenuUIManager>();
                }
                return instance;
            }
        }

        private const string playButton = "PlayButton";
        private const string singlePlayerButton = "SinglePlayerButton";
        private const string twoPlayerButton = "TwoPlayerButton";
        private const string threePlayerButton = "ThreePlayerButton";
        private const string fourPlayerButton = "FourPlayerButton";

        private const string kitchenCardPackButton = "KitchenCardPackButton";
        private const string artCardPackButton = "ArtCardPackButton";
        private const string oceanCardPackButton = "OceanCardPackButton";

        private const string onlineButton = "OnlineButton";
        private const string createButton = "CreateButton";
        private const string joinButton = "JoinButton";
        private const string onlineMenuPanelName = "OnlineMenuPanel";
        private const string idTextBoxName = "IDTextBox";
        private const string idTextBoxPlaceholderName = "IDTextBoxPlaceholder";
        private const string playerNameTextBoxName = "PlayerNameTextBox";

        private const string defaultPlayerName = "Player";
        private const string playerNamePlayerPref = "PlayerName";

        private readonly CardPack kitchenCardPackage = CardPackages.Packages["KitchenPack"];
        private readonly CardPack artCardPackage = CardPackages.Packages["ArtPack"];
        private readonly CardPack oceanCardPackage = CardPackages.Packages["OceanPack"];

        private OptionsButtonsManager<CardPack> cardPacksButtons;
        private OptionsButtonsManager<int> playersCountButtons;

        private GameObject onlineMenuPanel;
        private InputField idTextBox;
        private Text idTextBoxPlaceholder;
        private InputField playerNameTextBox;

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
            { onlineButton, null },
            { createButton, null },
            { joinButton, null },
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

            onlineMenuPanel = Resources.FindObjectsOfTypeAll<Image>()
                .FirstOrDefault(o => o.name == onlineMenuPanelName)?.gameObject;
            idTextBox = Resources.FindObjectsOfTypeAll<InputField>()
                .FirstOrDefault(o => o.name == idTextBoxName);
            idTextBoxPlaceholder = Resources.FindObjectsOfTypeAll<Text>()
                .FirstOrDefault(o => o.name == idTextBoxPlaceholderName);
            playerNameTextBox = Resources.FindObjectsOfTypeAll<InputField>()
                .FirstOrDefault(o => o.name == playerNameTextBoxName);

            playerNameTextBox.text = GetPlayerName();
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

        #region Base Menu Buttons

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

        #endregion

        #region Online Menu Buttons

        public void OnlineButtonClick()
        {
            NetworkManager.Instance.ConnectIfNot();

            idTextBox.readOnly = true;
            idTextBox.text = string.Empty;
            idTextBoxPlaceholder.text = string.Empty;
            onlineMenuPanel.SetActive(true);
            idTextBox.gameObject.SetActive(false);
            SetInteractable(createButton, true);
            SetInteractable(joinButton, true);
        }

        public void CreateButtonClick()
        {
            SetInteractable(joinButton, false);
            SetInteractable(createButton, false);

            idTextBox.gameObject.SetActive(true);
            NetworkManager.Instance.CreateGame(GetPlayerName());
            idTextBox.readOnly = true;
            idTextBoxPlaceholder.text = "Creating..";
        }

        public void RoomCreated(string roomID)
        {
            idTextBox.text = roomID;
            idTextBox.readOnly = true;
            idTextBoxPlaceholder.text = string.Empty;
        }

        public void JoinButtonClick()
        {
            if (string.IsNullOrEmpty(idTextBox.text))
            {
                SetInteractable(createButton, false);
                idTextBoxPlaceholder.text = "Enter room number";
                idTextBox.readOnly = false;
                idTextBox.gameObject.SetActive(true);

                return;
            }

            SetInteractable(createButton, false);
            SetInteractable(joinButton, false);
            NetworkManager.Instance.JoinGame(idTextBox.text, GetPlayerName());

            CancelInvoke(nameof(UnlockCreateJoinButtons));
            Invoke(nameof(UnlockCreateJoinButtons), 3F);
        }

        private void UnlockCreateJoinButtons()
        {
            SetInteractable(createButton, true);
            SetInteractable(joinButton, true);
        }

        public void BackButtonClick()
        {
            onlineMenuPanel.SetActive(false);
            idTextBox.readOnly = true;
            idTextBox.text = string.Empty;
            idTextBoxPlaceholder.text = string.Empty;
        }

        public void OnPlayerNameChanged()
        {
            if (string.IsNullOrWhiteSpace(playerNameTextBox.text))
            {
                playerNameTextBox.text = defaultPlayerName;
                return;
            }

            SetPlayerName(playerNameTextBox.text);
        }

        private void SetPlayerName(string playerName)
        {
            PlayerPrefs.SetString(playerNamePlayerPref, playerName);
        }

        private string GetPlayerName()
        {
            string savedName = PlayerPrefs.GetString(playerNamePlayerPref);

            if (!string.IsNullOrWhiteSpace(savedName))
                return savedName;

            return defaultPlayerName;
        }

        #endregion
    }
}

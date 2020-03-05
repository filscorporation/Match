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

        private const string defaultCardPackage = "DefaultPack";
        private const string geometryCardPackage = "GeometryPack";
        private const string colorsCardPackage = "ColorsPack";

        protected override Dictionary<string, GameObject> Buttons { get; set; } = new Dictionary<string, GameObject>
        {
            { playButton, null },
        };

        public void PlayButtonClick()
        {
            // TODO: fill GameSettings
            //GameSettings.FieldWidth = 6;
            //GameSettings.FieldHeigth = 5;
            //GameSettings.PlayersCount = 2;
            GameSettings.CardPackageName = colorsCardPackage;
            SceneManager.LoadScene(gameSceneName);
        }
    }
}

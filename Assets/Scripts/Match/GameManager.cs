using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Match
{
    public class GameManager : MonoBehaviour
    {
        public CardManager CardManager;
        public UIManager UIManager;
        public IInputManager InputManager;

        private List<Player> players;

        private const string defaultCardPackage = "DefaultPack";

        private bool getInput = true;

        public void Start()
        {
            StartGame();
        }

        public void Update()
        {
            if (IsGetInput())
                InputManager.CheckForInput();
        }

        private bool IsGetInput()
        {
            return getInput && !UIManager.IsUIMode();
        }

        public void StartGame()
        {
            players = new List<Player>(GameSettings.PlayersCount);

            CardManager = new CardManager(this);
            FieldParams fieldParams = new FieldParams { Height = GameSettings.FieldHeigth, Width = GameSettings.FieldWidth };
            CardManager.InitializeField(fieldParams, defaultCardPackage);
            InputManager = new PCInputManager();
            InputManager.AddSubscriber(CardManager);
            UIManager = FindObjectOfType<UIManager>();
            UIManager.GameManager = this;
        }

        public void Score()
        {

        }

        public void EndGame()
        {
            
        }
    }
}

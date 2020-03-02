using System;
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
        private int activePlayer = 0;

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

            UIManager.DrawPlayers(players, activePlayer);
        }

        private bool IsGetInput()
        {
            return getInput && !UIManager.IsUIMode();
        }

        public void StartGame()
        {
            InitializePlayers(GameSettings.PlayersCount);

            CardManager = new CardManager(this);
            FieldParams fieldParams = new FieldParams { Height = GameSettings.FieldHeigth, Width = GameSettings.FieldWidth };
            CardManager.InitializeField(fieldParams, defaultCardPackage);
            InputManager = new PCInputManager();
            InputManager.AddSubscriber(CardManager);
            UIManager = FindObjectOfType<UIManager>();
            UIManager.GameManager = this;
        }

        private void InitializePlayers(int count)
        {
            if (count != 2)
                throw new NotImplementedException();

            activePlayer = 0;
            players = new List<Player> { new Player {Name = "Player 1"}, new Player {Name = "Player 2"} };
        }

        public void Match()
        {
            players[activePlayer].Score++;
        }

        public void Unmatch()
        {
            activePlayer = activePlayer + 1 > players.Count - 1 ? 0 : activePlayer + 1;
        }

        public void EndGame()
        {
            
        }
    }
}

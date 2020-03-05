using System;
using System.Collections.Generic;
using Assets.Scripts.Match.UI;
using UnityEngine;

namespace Assets.Scripts.Match
{
    public class GameManager : MonoBehaviour
    {
        public CardManager CardManager;
        public GameUIManager UIManager;
        public IInputManager InputManager;

        private List<Player> players;
        private int activePlayer = 0;

        private const bool isFreezeOnAnimation = true;

        private bool getInput = true;

        private bool isInitialized = false;

        public void Start()
        {
            StartGame();
        }

        public void Update()
        {
            if (!isInitialized)
                return;

            if (IsGetInput())
                InputManager.CheckForInput();

            UIManager.DrawPlayers(players, activePlayer);
        }

        private bool IsGetInput()
        {
            return getInput && !UIManager.IsUIMode() && (!isFreezeOnAnimation || !CardManager.IsAnimating());
        }

        public void StartGame()
        {
            InitializePlayers(GameSettings.PlayersCount);

            CardManager = new CardManager(this);
            FieldParams fieldParams = new FieldParams { Height = GameSettings.FieldHeigth, Width = GameSettings.FieldWidth };
            CardManager.InitializeField(fieldParams, GameSettings.CardPackageName);
            InputManager = new PCInputManager();
            InputManager.AddSubscriber(CardManager);
            UIManager = FindObjectOfType<GameUIManager>();

            isInitialized = true;
        }

        private void InitializePlayers(int count)
        {
            activePlayer = 0;
            switch (count)
            {
                case 1:
                    players = new List<Player> { new Player { Name = "Player 1" } };
                    break;
                case 2:
                    players = new List<Player> { new Player { Name = "Player 1" }, new Player { Name = "Player 2" } };
                    break;
                default:
                    throw new NotImplementedException();
            }

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
            UIManager.OpenRestartMenu(1.5F);
        }
    }
}

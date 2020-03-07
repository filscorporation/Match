using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<string> defaultPlayersNames = new List<string> //TODO: remove
        {
            "Player 1", "Player 2", "Player 3", "Player 4"
        };

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
            CardManager.InitializeField(fieldParams, GameSettings.CardPackage);
            InputManager = new PCInputManager();
            InputManager.AddSubscriber(CardManager);
            UIManager = FindObjectOfType<GameUIManager>();

            isInitialized = true;
        }

        private void InitializePlayers(int count)
        {
            activePlayer = 0;
            if (count > 4)
                throw new NotImplementedException();
            players = defaultPlayersNames.GetRange(0, count).Select(n => new Player { Name = n }).ToList();
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

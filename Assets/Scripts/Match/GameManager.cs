using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Match.CardManagement;
using Assets.Scripts.Match.InputManagement;
using Assets.Scripts.Match.Networking;
using Assets.Scripts.Match.UI;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Starts the game, controls players, turns and score
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                }
                return instance;
            }
        }

        public CardManager CardManager;
        public GameUIManager UIManager;
        public IInputManager InputManager;

        private List<Player> players;
        private int activePlayer = 0;
        private List<string> defaultPlayersNames = new List<string> //TODO: remove
        {
            "Player 1", "Player 2", "Player 3", "Player 4"
        };

        private const string turnsHighscorePlayerPref = "TurnsHighscore";
        private const string timeHighscorePlayerPref = "TimeHighscore";
        private int turnsPassed;
        private float timePassed;
        private SinglePlayerStats stats;

        private const bool isFreezeOnAnimation = true;

        private bool getInput = true;

        private bool isInitialized = false;
        private bool isInGame = false;

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

            if (isInGame)
                UpdateTimePassed();
            if (GameSettings.PlayersCount == 1)
                UIManager.DrawSinglePlayerStats(GetPlayerStats());
            else
                UIManager.DrawPlayers(players, activePlayer);
        }

        private bool IsGetInput()
        {
            return getInput
                   && !UIManager.IsUIMode()
                   && (!isFreezeOnAnimation || !CardManager.IsAnimating())
                   && isInGame
                   && (!GameSettings.IsOnline || NetworkManager.Instance.ThisPlayerID == activePlayer);
        }

        public void StartGame()
        {
            InitializePlayers(GameSettings.PlayersCount);
            InitializePlayerStats();

            CardManager = new CardManager(this);
            FieldParams fieldParams = new FieldParams { Height = GameSettings.FieldHeight, Width = GameSettings.FieldWidth };
            if (GameSettings.IsFromData)
            {
                GameSettings.IsFromData = false;
                CardManager.InitializeField(fieldParams, GameSettings.CardPackage, GameSettings.FieldData);
                NetworkManager.Instance.ThisPlayerID = 0;
            }
            else
            {
                if (GameSettings.IsOnline)
                    NetworkManager.Instance.ConnectPlayer();
                CardManager.InitializeField(fieldParams, GameSettings.CardPackage);
            }

            InputManager = new PCInputManager();
            InputManager.AddSubscriber(CardManager);
            UIManager = FindObjectOfType<GameUIManager>();

            isInGame = true;
            isInitialized = true;
        }

        private void InitializePlayers(int count)
        {
            activePlayer = 0;
            if (count > 4)
                throw new NotImplementedException();
            players = defaultPlayersNames.GetRange(0, count).Select(n => new Player { Name = n }).ToList();
        }

        private void InitializePlayerStats()
        {
            timePassed = 0;
            turnsPassed = 0;
            stats = new SinglePlayerStats();
            // Adding cardpack name to load highscore for current pack
            stats.TurnsHighscore = PlayerPrefs.GetInt(turnsHighscorePlayerPref + GameSettings.CardPackage.Name, -1);
            stats.TimeHighscore = PlayerPrefs.GetInt(timeHighscorePlayerPref + GameSettings.CardPackage.Name, -1);
        }

        private SinglePlayerStats GetPlayerStats()
        {
            stats.Turns = turnsPassed;
            stats.Time = (int)Math.Round(timePassed);

            return stats;
        }

        private void SavePlayerStats()
        {
            if (stats.TurnsHighscore == -1 || turnsPassed < stats.TurnsHighscore)
            {
                PlayerPrefs.SetInt(turnsHighscorePlayerPref + GameSettings.CardPackage.Name, turnsPassed);
                stats.TurnsHighscore = turnsPassed;
            }
            if (stats.TimeHighscore == -1 || timePassed < stats.TimeHighscore)
            {
                PlayerPrefs.SetInt(timeHighscorePlayerPref + GameSettings.CardPackage.Name, (int)Math.Round(timePassed));
                stats.TimeHighscore = (int)Math.Round(timePassed);
            }
        }

        private void UpdateTimePassed()
        {
            timePassed += Time.deltaTime;
        }

        private void UpdateTurnsPassed()
        {
            turnsPassed++;
        }

        public void Match()
        {
            players[activePlayer].Score++;
            UpdateTurnsPassed();
        }

        public void Unmatch()
        {
            PassPlayerTurn();
            UpdateTurnsPassed();
        }

        private void PassPlayerTurn()
        {
            activePlayer = activePlayer + 1 > players.Count - 1 ? 0 : activePlayer + 1;
        }

        public void EndGame()
        {
            SavePlayerStats();
            isInGame = false;
            UIManager.OpenRestartMenu(1.5F);
        }
    }
}

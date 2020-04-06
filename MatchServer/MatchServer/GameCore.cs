using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NetworkShared.Core;
using NetworkShared.Data;
using NetworkShared.Network;

namespace MatchServer
{
    public class GameCore : IServerListener
    {
        public bool IsRunning = false;
        public const int TicksPerSec = 30;
        public const int MsPerTick = 1000 / TicksPerSec;

        public Server Server;

        private readonly List<Player> players;
        private readonly List<GameMatch> matches;

        public GameCore()
        {
            players = new List<Player>();
            matches = new List<GameMatch>();
        }

        public void Start()
        {
            Console.WriteLine($"Game thread started. Running at {TicksPerSec} ticks per second.");
            DateTime nextLoop = DateTime.Now;

            IsRunning = true;

            while (IsRunning)
            {
                while (nextLoop < DateTime.Now)
                {
                    Update();

                    nextLoop = nextLoop.AddMilliseconds(MsPerTick);

                    if (nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }

        private void Update()
        {
            ThreadManager.UpdateMain();
        }

        public void ClientConnected(int clientID)
        {
            if (players.Any(p => p.ID == clientID))
            {
                Console.WriteLine($"Player {clientID} already exists");
                return;
            }

            Console.WriteLine($"Adding player {clientID} to the game");
            players.Add(new Player(clientID));

            TryCreateMatch();
        }

        public void ClientDisconnected(int clientID)
        {
            if (players.RemoveAll(p => p.ID == clientID) == 0)
            {
                Console.WriteLine($"Player {clientID} doesn't exist");
                return;
            }

            Console.WriteLine($"Removing player {clientID} from the game");
        }

        public void ClientSentData(int clientID, int dataType, object data)
        {
            switch ((DataTypes)dataType)
            {
                case DataTypes.GameData:
                    Player player = players.FirstOrDefault(p => p.ID == clientID);
                    if (player == null)
                    {
                        Console.WriteLine($"Player {clientID} doesn't exist");
                        return;
                    }
                    if (!player.IsInGame)
                    {
                        Console.WriteLine($"Player {clientID} isn't in game");
                        return;
                    }

                    GameMatch match = matches.FirstOrDefault(m => m.Player1 == player);
                    if (match == null)
                    {
                        Console.WriteLine($"Error finding players {clientID} match");
                        return;
                    }
                    if (match.IsInitialized)
                    {
                        Console.WriteLine($"Match already initialized");
                        return;
                    }

                    match.IsInitialized = true;

                    Server.SendDataToClient(match.Player2.ID, (int)DataTypes.GameData, data);
                    break;
                case DataTypes.PlayersTurnData:
                    break;
                case DataTypes.StartGame:
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
            }
        }

        private bool TryCreateMatch()
        {
            Player player1 = null;

            foreach (Player player in players.Where(p => !p.IsInGame))
            {
                if (player1 == null)
                    player1 = player;
                else
                {
                    CreateMatch(player1, player);
                    return true;
                }
            }

            return false;
        }

        private void CreateMatch(Player player1, Player player2)
        {
            player1.IsInGame = true;
            player2.IsInGame = true;

            GameMatch match = new GameMatch();
            match.Player1 = player1;
            match.Player2 = player2;

            matches.Add(match);

            Server.SendDataToClient(player1.ID, (int)DataTypes.StartGame, null);

            Console.WriteLine($"Match between player {player1.ID} and player {player2.ID}");
        }
    }
}

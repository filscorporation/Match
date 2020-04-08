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

        #region Core

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

        #endregion

        #region Clients

        public void ClientConnected(int clientID)
        {
            if (players.Any(p => p.ClientID == clientID))
            {
                Console.WriteLine($"Player {clientID} already exists");
                return;
            }

            Console.WriteLine($"Adding player {clientID} to the game");
            players.Add(new Player(clientID));
        }

        public void ClientDisconnected(int clientID)
        {
            GameMatch match = matches.FirstOrDefault(m => m.ContainsPlayer(clientID));
            if (match != null)
            {
                Console.WriteLine($"Dropping room {match.RoomID}");
                if (match.Player1 != null) match.Player1.Match = null;
                if (match.Player2 != null) match.Player2.Match = null;
                matches.Remove(match);
            }

            if (players.RemoveAll(p => p.ClientID == clientID) == 0)
            {
                Console.WriteLine($"Player {clientID} doesn't exist");
                return;
            }

            Console.WriteLine($"Removing player {clientID} from the game");
        }

        #endregion

        #region Data

        public void ClientSentData(int clientID, int dataType, object data)
        {
            try
            {
                switch ((DataTypes)dataType)
                {
                    case DataTypes.CreateGameRequest:
                        ProcessCreateGameRequest((CreateGameRequest)data, clientID);
                        break;
                    case DataTypes.JoinGameRequest:
                        ProcessJoinGameRequest((JoinGameRequest)data, clientID);
                        break;
                    case DataTypes.PlayersTurnData:
                        ProcessPlayerTurnData((PlayersTurnData)data, clientID);
                        break;
                    case DataTypes.RestartGameRequest:
                        ProcessRestartGameRequest((RestartGameRequest)data, clientID);
                        break;
                    case DataTypes.StartGameResponse:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing message from clint {clientID}:{e}");
            }
        }

        private void ProcessCreateGameRequest(CreateGameRequest request, int clientID)
        {
            Console.WriteLine($"Request to create room {request.RoomID} from player {clientID}");

            Player player = players.FirstOrDefault(p => p.ClientID == clientID);
            if (player == null)
            {
                Console.WriteLine($"Player {clientID} doesn't exist");
                return;
            }
            if (player.Match != null)
            {
                Console.WriteLine($"Player {clientID} is already in the game");

                Console.WriteLine($"Dropping room {player.Match.RoomID}");
                GameMatch oldMatch = player.Match;

                if (oldMatch.Player1 != null) oldMatch.Player1.Match = null;
                if (oldMatch.Player2 != null) oldMatch.Player2.Match = null;
                matches.Remove(oldMatch);
            }
            if (string.IsNullOrWhiteSpace(request.RoomID))
            {
                Console.WriteLine($"Room ID can't be empty");
                return;
            }

            GameMatch match = new GameMatch();
            match.Player1 = player;
            match.RoomID = request.RoomID;
            match.CardPackName = request.CardPack;
            match.Width = request.Width;
            match.Height = request.Height;

            match.IsRunning = false;
            player.Match = match;

            matches.Add(match);

            Console.WriteLine($"Room {request.RoomID} created");
        }

        private void ProcessJoinGameRequest(JoinGameRequest request, int clientID)
        {
            Console.WriteLine($"Request to join room {request.RoomID} from player {clientID}");

            Player player = players.FirstOrDefault(p => p.ClientID == clientID);
            if (player == null)
            {
                Console.WriteLine($"Player {clientID} doesn't exist");
                return;
            }
            if (player.Match != null)
            {
                Console.WriteLine($"Player {clientID} is already in the game");
                return;
            }
            if (string.IsNullOrWhiteSpace(request.RoomID))
            {
                Console.WriteLine($"Room ID can't be empty");
                return;
            }

            GameMatch match = matches.FirstOrDefault(m => m.RoomID == request.RoomID);
            if (match == null)
            {
                Console.WriteLine($"Room {request.RoomID} doesn't exist");
                return;
            }

            match.Player2 = player;
            match.IsRunning = true;
            player.Match = match;

            int[,] field = CreateField(match.Width, match.Height);
            StartGameResponse response = new StartGameResponse();
            response.CardPackName = match.CardPackName;
            response.PlayerID = 0;
            response.Field = field;
            Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.StartGameResponse, response);
            response.PlayerID = 1;
            Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.StartGameResponse, response);

            Console.WriteLine($"Player {clientID} successfully joined");
        }

        private void ProcessRestartGameRequest(RestartGameRequest request, int clientID)
        {
            Console.WriteLine($"Request to restart game from player {clientID}");

            Player player = players.FirstOrDefault(p => p.ClientID == clientID);
            if (player == null)
            {
                Console.WriteLine($"Player {clientID} doesn't exist");
                return;
            }
            if (player.Match == null)
            {
                Console.WriteLine($"Player {clientID} is not in the game");
                return;
            }

            GameMatch match = player.Match;
            if (!match.IsRunning)
            {
                Console.WriteLine($"Match isn't running");
                return;
            }

            int[,] field = CreateField(match.Width, match.Height);
            StartGameResponse response = new StartGameResponse();
            response.CardPackName = match.CardPackName;
            response.PlayerID = 0;
            response.Field = field;
            Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.StartGameResponse, response);
            response.PlayerID = 1;
            Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.StartGameResponse, response);

            Console.WriteLine($"Player {clientID} successfully joined");
        }

        private void ProcessPlayerTurnData(PlayersTurnData data, int clientID)
        {
            Player player = players.FirstOrDefault(p => p.ClientID == clientID);
            if (player == null)
            {
                Console.WriteLine($"Player {clientID} doesn't exist");
                return;
            }
            if (player.Match == null)
            {
                Console.WriteLine($"Player {clientID} isn't in game");
                return;
            }

            GameMatch match = player.Match;
            if (!match.IsRunning)
            {
                Console.WriteLine($"Match isn't running");
                return;
            }

            if (match.Player1 == player)
                Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.PlayersTurnData, data);
            else
                Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.PlayersTurnData, data);
        }

        #endregion

        private int[,] CreateField(int w, int h)
        {
            int[,] field = new int[h,w];
            List<int> seed = new List<int>(w * h);
            for (int i = 0; i < w * h; i++)
            {
                seed.Add(i);
            }

            seed.Shuffle();

            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    field[j, i] = seed[j * w + i] / 2;
                }
            }

            return field;
        }
    }
}

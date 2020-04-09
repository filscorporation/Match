using System;
using Assets.Scripts.Match.CardManagement;
using Assets.Scripts.Match.UI;
using NetworkShared.Core;
using NetworkShared.Data;
using NetworkShared.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Match.Networking
{
    /// <summary>
    /// Connects client to the server
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager instance;

        public static NetworkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<NetworkManager>();
                }
                return instance;
            }
        }

        private static Client client;
        public int ThisPlayerID;

        public void Update()
        {
            ThreadManager.UpdateMain();
        }

        public void OnApplicationQuit()
        {
            DisconnectPlayer();
        }

        public void CreateGame()
        {
            if (client == null || !client.IsConnected())
                ConnectPlayer();

            string id = Random.Range(0, 1000).ToString();
            CreateGameRequest request = new CreateGameRequest();
            request.CardPack = GameSettings.CardPackage.Name;
            request.Width = GameSettings.CardPackage.MaxWidth;
            request.Height = GameSettings.CardPackage.MaxHeight;

            client.SendData((int)DataTypes.CreateGameRequest, request);
        }

        public void JoinGame(string id)
        {
            if (client == null || !client.IsConnected())
                ConnectPlayer();

            JoinGameRequest request = new JoinGameRequest();
            request.RoomID = id;

            client.SendData((int)DataTypes.JoinGameRequest, request);
        }

        public void RestartGame()
        {
            if (client == null || !client.IsConnected())
                ConnectPlayer();

            RestartGameRequest request = new RestartGameRequest();

            client.SendData((int)DataTypes.RestartGameRequest, request);
        }

        public void ConnectIfNot()
        {
            if (client == null || !client.IsConnected())
                ConnectPlayer();
        }

        public void ConnectPlayer()
        {
            client = new Client(this);
            client.ConnectToServer();
        }

        public void DisconnectPlayer()
        {
            client?.Disconnect();
            client = null;
        }

        public void ClientReceiveData(int type, object data)
        {
            switch ((DataTypes)type)
            {
                case DataTypes.StartGameResponse:
                    StartGameResponse startGameResponse = (StartGameResponse) data;

                    GameSettings.PlayersCount = 2;
                    GameSettings.IsOnline = true;
                    GameSettings.PlayerID = startGameResponse.PlayerID;
                    GameSettings.CardPackage = CardPackages.Packages[startGameResponse.CardPackName];
                    GameSettings.FieldHeight = startGameResponse.Field.GetLength(0);
                    GameSettings.FieldWidth = startGameResponse.Field.GetLength(1);
                    GameSettings.FieldData = startGameResponse.Field;

                    SceneManager.LoadScene("GameScene");
                    break;
                case DataTypes.PlayersTurnData:
                    GameManager.Instance.CardManager.Handle((PlayersTurnData)data);
                    break;
                case DataTypes.CreateGameResponse:
                    CreateGameResponse createGameResponse = (CreateGameResponse)data;

                    MainMenuUIManager.Instance.RoomCreated(createGameResponse.RoomID);
                    break;
                case DataTypes.CreateGameRequest:
                case DataTypes.JoinGameRequest:
                case DataTypes.RestartGameRequest:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void SendPlayersTurn(PlayersTurnData playersTurnData)
        {
            client.SendData((int)DataTypes.PlayersTurnData, playersTurnData);
        }
    }
}

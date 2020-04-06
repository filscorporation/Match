﻿using System;
using Assets.Scripts.Match.CardManagement;
using NetworkShared.Core;
using NetworkShared.Data;
using NetworkShared.Network;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private Client client;

        public void Update()
        {
            ThreadManager.UpdateMain();
        }

        public void OnApplicationQuit()
        {
            DisconnectPlayer();
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
                case DataTypes.StartGame:
                    GameData newGameData = new GameData();
                    newGameData.Field = GameManager.Instance.CardManager.GetFieldData();
                    newGameData.CardPackName = GameSettings.CardPackage.Name;
                    client.SendData((int)DataTypes.GameData, newGameData);
                    break;
                case DataTypes.GameData:
                    GameData gameData = (GameData) data;
                    GameSettings.CardPackage = CardPackages.Packages[gameData.CardPackName];
                    GameSettings.FieldData = gameData.Field;
                    GameSettings.IsFromData = true;
                    SceneManager.LoadScene("GameScene");
                    break;
                case DataTypes.PlayersTurnData:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static void Log(string msg)
        {
            Debug.Log(msg);
        }
    }
}

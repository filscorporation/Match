using System;
using Assets.Scripts.Match.CardManagement;

namespace Assets.Scripts.Match
{
    [Serializable]
    public static class GameSettings
    {
        public static string[] PlayersNames;

        public static int Difficulty = 0;

        public static int FieldWidth;

        public static int FieldHeight;

        public static int PlayersCount = 2;

        public static CardPack CardPackage;

        public static bool IsOnline = false;

        public static int[,] FieldData;

        public static int PlayerID;
    }
}

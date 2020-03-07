using System;

namespace Assets.Scripts.Match
{
    [Serializable]
    public static class GameSettings
    {
        public static int FieldWidth = 6;

        public static int FieldHeigth = 5;

        public static int PlayersCount = 2;

        public static CardPack CardPackage;
    }
}

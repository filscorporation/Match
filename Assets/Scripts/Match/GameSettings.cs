using System;

namespace Assets.Scripts.Match
{
    [Serializable]
    public static class GameSettings
    {
        public static int FieldWidth;

        public static int FieldHeigth;

        public static int PlayersCount = 2;

        public static CardPack CardPackage;
    }
}

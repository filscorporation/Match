using System;

namespace NetworkShared.Data
{
    [Serializable]
    public class StartGameResponse
    {
        public string[] PlayersNames;

        public int PlayerID;

        public int[,] Field;

        public string CardPackName;

        public int Difficulty;
    }
}

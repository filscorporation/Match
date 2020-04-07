using System;

namespace NetworkShared.Data
{
    [Serializable]
    public class StartGameResponse
    {
        public int PlayerID;

        public int[,] Field;

        public string CardPackName;
    }
}

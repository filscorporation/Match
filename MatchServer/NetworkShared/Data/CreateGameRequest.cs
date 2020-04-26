using System;

namespace NetworkShared.Data
{
    [Serializable]
    public class CreateGameRequest
    {
        public string PlayerName;

        public string CardPack;

        public int Width;

        public int Height;
    }
}

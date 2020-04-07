using System;

namespace NetworkShared.Data
{
    [Serializable]
    public class CreateGameRequest
    {
        public string RoomID;

        public string CardPack;

        public int Width;

        public int Height;
    }
}

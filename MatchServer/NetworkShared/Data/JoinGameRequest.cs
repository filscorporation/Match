﻿using System;

namespace NetworkShared.Data
{
    [Serializable]
    public class JoinGameRequest
    {
        public string PlayerName;

        public string RoomID;
    }
}
namespace MatchServer
{
    public class GameMatch
    {
        public string RoomID;

        public string CardPackName;

        public int Width;

        public int Height;

        public bool IsRunning = false;

        public Player Player1;

        public Player Player2;

        public bool ContainsPlayer(int id)
        {
            return Player1 != null && Player1.ClientID == id || Player2 != null && Player2.ClientID == id;
        }
    }
}

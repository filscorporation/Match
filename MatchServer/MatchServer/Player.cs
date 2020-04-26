namespace MatchServer
{
    public class Player
    {
        public string Name;

        public int ClientID;

        public GameMatch Match;

        public Player(int clientID)
        {
            ClientID = clientID;
        }
    }
}

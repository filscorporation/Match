namespace MatchServer
{
    public class Player
    {
        public int ClientID;

        public GameMatch Match;

        public Player(int clientID)
        {
            ClientID = clientID;
        }
    }
}

namespace MatchServer
{
    public class GameMatch
    {
        public bool IsInitialized = false;

        public Player Player1;

        public Player Player2;

        public bool ContainsPlayer(int id)
        {
            return Player1.ID == id || Player2.ID == id;
        }
    }
}

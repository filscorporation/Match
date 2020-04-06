using System;
using System.Threading;
using System.Threading.Tasks;
using NetworkShared.Network;

namespace MatchServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Match Game Server";

            GameCore core = new GameCore();
            Task.Factory.StartNew(core.Start);

            Server server = new Server();
            server.Listener = core;
            server.Start(2, 26950);

            core.Server = server;
            
            Console.ReadKey();
        }
    }
}

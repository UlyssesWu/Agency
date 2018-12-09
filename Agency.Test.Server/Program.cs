using System;
using Agency.Handlers;

namespace Agency.Test.Server
{
    class Program
    {
        private static Agent47 agent;
        static void Main(string[] args)
        {
            agent = new Agent47();
            var handler = new TcpHandler(port: 0);
            Agency.RegisterAgent("47", agent, handler);
            Console.WriteLine("Agent 47 is on his way.");
            Console.WriteLine($"Access port: {handler.ServerPort}");
            Console.ReadLine();
            Console.WriteLine($"Current Weapon: {agent.Weapon}");
            Console.ReadLine();
        }
    }
}


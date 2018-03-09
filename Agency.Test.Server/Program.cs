using System;

namespace Agency.Test.Server
{
    class Program
    {
        private static Agent47 agent;
        static void Main(string[] args)
        {
            agent = new Agent47();
            Agency.RegisterAgent("47", agent, new IpcHandler());
            Console.WriteLine("Agent 47 is on his way.");
            Console.ReadLine();
            Console.WriteLine($"Current Weapon: {agent.Weapon}");
            Console.ReadLine();
        }
    }
}


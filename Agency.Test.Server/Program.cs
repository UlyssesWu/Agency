using System;

namespace Agency.Test.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var agent = new Agent47();
            Agency.RegisterAgent("47", agent, new IpcHandler());
            Console.ReadLine();
            Console.WriteLine($"Current Weapon: {agent.Weapon}");
            Console.ReadLine();
        }
    }
}


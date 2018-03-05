using System;

namespace Agency.Test.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            dynamic agent;
            try
            {
                agent = Agency.SpawnAgent("47", new IpcHandler());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            var weapon = "Silverballer";
            Console.WriteLine($"Changing Weapon from {agent.Weapon} to {weapon}");
            agent.Weapon = weapon;
            for (int i = 0; i < 5; i++)
            {
                agent.PointShooting(i);
            }
            Console.WriteLine(agent.Name + ": " + agent.IsDianaDead());
            Console.ReadLine();
        }
    }
}

using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

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
            //This will not work since lambda's code is only in Client assembly. Looking for a solution
            //agent.OnContractSigned += new Func<int, string>(i => $"Account += ${i}"); 
            for (int i = 0; i < 5; i++)
            {
                agent.PointShooting(i);
            }
            Console.WriteLine(agent.Name + ": " + agent.IsDianaDead());
            Console.ReadLine();
        }
    }
}

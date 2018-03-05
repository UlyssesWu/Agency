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
            //This will pass the expression as a Func to be executed in server side event, but it's very limited.
            Expression<Func<int, string>> exp = i => $"Account += ${i}";
            agent.OnContractSigned += exp.GetDelegateDesc();
            for (int i = 0; i < 5; i++)
            {
                agent.PointShooting(i);
            }
            Console.WriteLine(agent.Name + ": " + agent.IsDianaDead());
            Console.ReadLine();
        }
    }
}

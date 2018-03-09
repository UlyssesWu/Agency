using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Agency.Test.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is the Handler, sending instructions to Agent.");
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
            //Get from server side
            Console.WriteLine($"Please change Weapon from {agent.Weapon} to {weapon}");
            //Set to server side
            agent.Weapon = weapon;
            
            Func<int, float, string> func = (i, f) =>
            {
                Console.WriteLine("Good job.");
                return $"Account: ${i * f}";
            };

            Func<int, float, string> func2 = (i, f) =>
            {
                Console.WriteLine("Well done.");
                return $"Money: ${i * f}";
            };
            
            //This will pass the expression as a Func to be executed in server side event, but it's very limited.
            Expression<Func<int, float, string>> exp = (i,f) => $"Price: ${f} per target.";
            agent.OnContractSigned += exp;
            
            //Run on server side and get return value
            Console.WriteLine(agent.PointShooting(0));

            //This will be executed in client side, but the return value can be passed to server side!
            agent.OnContractSigned += func;
            agent.OnContractSigned += func2;
            for (int i = 1; i < 5; i++)
            {
                agent.PointShooting(i);
            }

            agent.OnContractSigned -= func2;
            agent.OnContractSigned -= exp;

            for (int i = 5; i < 9; i++)
            {
                agent.PointShooting(i);
            }
            Console.WriteLine(agent.Name + ": " + agent.IsDianaDead());
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency.Test.Server
{
    class Agent47
    {
        public event Func<int, float, string> OnContractSigned;
        public string Name { get; set; } = "Agent 47";

        public string Weapon { get; set; } = "Fiber Wire";

        public string IsDianaDead()
        {
            return "You'll never know.";
        }

        public string PointShooting(int enemy)
        {
            var str = "Point Shooting";
            Console.WriteLine($"Target eliminated: {enemy}");
            if (OnContractSigned != null)
            {
                str = OnContractSigned.Invoke(enemy, 1000);
                Console.WriteLine(str);
            }

            return str;
        }
    }
}

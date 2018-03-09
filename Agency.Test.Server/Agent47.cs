using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency.Test.Server
{
    /// <summary>
    /// Agent 47
    /// </summary>
    class Agent47
    {
        public event Func<int, float, string> OnContractFulfilled;
        public event Action<int> OnContractSigned;
        public string Name { get; set; } = "Agent 47";

        public string Weapon { get; set; } = "Fiber Wire";

        public string IsDianaDead()
        {
            return "You'll never know.";
        }

        public string PointShooting(int enemy)
        {
            OnContractSigned?.Invoke(1000);
            Console.WriteLine($"Target eliminated: {enemy}");
            if (OnContractFulfilled != null)
            {
                Console.WriteLine(OnContractFulfilled.Invoke(enemy, 1000));
            }

            return "Point Shooting...";
        }
    }
}

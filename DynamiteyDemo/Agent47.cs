using System;

namespace DynamiteyDemo
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

        public void PointShooting(int enemy)
        {
            Console.WriteLine($"Target eliminated: {enemy}");
        }
    }

    class Travis
    {
        public string Name { get; set; } = "Benjamin Travis";

        public string Weapon { get; set; } = "Swiss 3000";

        public bool IsDianaDead()
        {
            return false;
        }

        public void PointShooting(int shoot)
        {
            if (shoot > 3)
            {
                Weapon = "None";
            }
        }
    }
}

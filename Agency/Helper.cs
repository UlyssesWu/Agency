using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency
{
    public static class Helper
    {
        public static dynamic GetAgent(this Agent agent)
        {
            return new RemoteProxy() { Agent = agent };
        }
    }
}

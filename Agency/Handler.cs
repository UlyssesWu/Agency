using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamitey.DynamicObjects;

namespace Agency
{
    /// <summary>
    /// <para>How Contract is passed</para>
    /// </summary>
    public interface IHandler
    {
        void Host(string address, object obj);
        Agent Connect(string address);
    }
}

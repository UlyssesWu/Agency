using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Security.AccessControl;
using System.Security.Principal;
using Dynamitey.DynamicObjects;

namespace Agency
{
    public static class Agency
    {
        internal const string AgencyLambdaToken = "@{lambda}";
        internal const string AgencyEventToken = "@{event}";
        internal static Dictionary<string, object> Agents { get; } = new Dictionary<string, object>();
        public static void RegisterAgent(string address, object obj, IHandler handler)
        {
            Agents[address] = obj;
            handler.Host(address, obj);
            return;
        }

        public static dynamic SpawnAgent(string address, IHandler handler)
        {
            var agent = handler.Connect(address);
            agent.Dispatcher = new ContractDispatcher();
            return agent.GetAgent();
        }
    }
}

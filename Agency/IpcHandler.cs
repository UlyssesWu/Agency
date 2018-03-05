using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

namespace Agency
{
    public class IpcHandler : IHandler
    {
        private const string AgencyDomain = "ICA-PF";
        private static Dictionary<string, IpcServerChannel> _serverChannels = new Dictionary<string, IpcServerChannel>();
        //private static List<IpcClientChannel> _clientChannels = new List<IpcClientChannel>();
        private static IpcClientChannel _clientChannel;

        public IContract<byte[], object> Contract { get; } = new BinaryContract();
        public void Host(string address, object obj)
        {
            Agent agent = new Agent(address, obj);
            var channel = IpcCreateServer(AgencyDomain, address, WellKnownObjectMode.Singleton, agent);
            _serverChannels[address] = channel;
            return;
        }

        public Agent Connect(string address)
        {
            if (_clientChannel == null)
            {
                _clientChannel = new IpcClientChannel();
                ChannelServices.RegisterChannel(_clientChannel, false);
            }
            var agent = IpcConnectClient<Agent>(AgencyDomain, address);
            return agent;
        }

        internal static IpcServerChannel IpcCreateServer<T>(string domain, string portName, WellKnownObjectMode mode, T obj) where T : MarshalByRefObject
        {
            IpcServerChannel channel = ChannelServices.GetChannel(domain) as IpcServerChannel;
            if (channel == null)
            {
                //Instantiate our server channel.
                channel = new IpcServerChannel(domain);
                //Register the server channel.
                ChannelServices.RegisterChannel(channel, false);
            }

            //Register this service type.
            if (obj == null)
            {
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), portName, mode);
            }
            else
            {
                RemotingServices.Marshal(obj, portName);
            }
            return channel;
        }

        /// <summary>
        /// Get interface object from host.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private static T IpcConnectClient<T>(string channel, string port)
        {
            T client =
                (T)Activator.GetObject(typeof(T), $"ipc://{channel}/{port}");

            if (client == null)
                throw new ArgumentException("Unable to create remote interface.");

            return client;
        }
    }
}

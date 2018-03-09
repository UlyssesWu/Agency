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
        private static IpcServerChannel _clientChannel;

        public void Host(string address, object obj)
        {
            Agent agent = new Agent(address, obj);
            var channel = IpcCreateServer(AgencyDomain, address, WellKnownObjectMode.Singleton, agent);
            _serverChannels[address] = channel;
            return;
        }

        public Agent Connect(string address)
        {
            var agent = IpcConnectClient<Agent>(AgencyDomain, address);
            return agent;
        }

        internal static IpcServerChannel IpcCreateServer<T>(string domain, string portName, WellKnownObjectMode mode, T obj) where T : MarshalByRefObject
        {
            IpcServerChannel channel = ChannelServices.GetChannel(domain) as IpcServerChannel;
            if (channel == null)
            {
                BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                //Instantiate our server channel.
                channel = new IpcServerChannel(domain, domain, serverProvider);
                //Register the server channel.
                ChannelServices.RegisterChannel(channel, false);
            }

            var c = channel.GetChannelUri();
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
        /// <param name="domain"></param>
        /// <param name="portName"></param>
        /// <returns></returns>
        private static T IpcConnectClient<T>(string domain, string portName)
        {
            if (_clientChannel == null)
            {
                BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                var channelName = $"{domain}C";
                //Instantiate our server channel.
                var channel = new IpcServerChannel(channelName, channelName, serverProvider);
                //Register the server channel.
                ChannelServices.RegisterChannel(channel, false);
                _clientChannel = channel;
            }

            T client =
                (T)Activator.GetObject(typeof(T), $"ipc://{domain}/{portName}");

            if (client == null)
                throw new ArgumentException("Unable to create remote interface.");

            return client;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Agency
{
    public class TcpHandler : IHandler
    {
        private uint _port;
        private string _ip;
        private TcpServerChannel _clientChannel;
        private TcpServerChannel _serverChannel;
        private const string AgencyDomain = "ICA-TCP";
        private static Dictionary<string, TcpServerChannel> _serverChannels = new Dictionary<string, TcpServerChannel>();
        //private static List<IpcClientChannel> _clientChannels = new List<IpcClientChannel>();
        public TcpHandler(string ip = "localhost", uint port = 40147)
        {
            _ip = ip;
            _port = port;
        }

        public void Host(string address, object obj)
        {
            Agent agent = new Agent(address, obj);
            _serverChannel = TcpCreateServer(AgencyDomain, address, WellKnownObjectMode.Singleton, agent);
            _serverChannels[address] = _serverChannel;
            return;
        }

        private TcpServerChannel TcpCreateServer<T>(string name, string address, WellKnownObjectMode mode, T obj) where T : MarshalByRefObject
        {
            TcpServerChannel channel = ChannelServices.GetChannel(name) as TcpServerChannel;
            if (channel == null)
            {
                BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                //Instantiate our server channel.
                channel = new TcpServerChannel(name, (int)_port, serverProvider);
                //Register the server channel.
                ChannelServices.RegisterChannel(channel, false);
            }

            var c = channel.GetChannelUri();
            //Register this service type.
            if (obj == null)
            {
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), address, mode);
            }
            else
            {
                RemotingServices.Marshal(obj, address);
            }
            return channel;
        }

        public Agent Connect(string address)
        {
            var agent = TcpConnectClient<Agent>(_ip, _port, address);
            return agent;
        }

        /// <summary>
        /// Get interface object from host.
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="portName"></param>
        /// <returns></returns>
        private T TcpConnectClient<T>(string ip, uint port, string name)
        {
            if (_clientChannel == null)
            {
                BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                var channelName = $"{AgencyDomain}-C";
                //Instantiate our server channel.
                var channel = new TcpServerChannel(channelName, 0, serverProvider);
                //Register the server channel.
                ChannelServices.RegisterChannel(channel, false);
                _clientChannel = channel;
            }
            T client =
                (T)Activator.GetObject(typeof(T), $"tcp://{ip}:{port}/{name}");

            if (client == null)
                throw new ArgumentException("Unable to create remote interface.");

            return client;
        }

        public void Dispose()
        {
            try
            {
                if (_serverChannel != null)
                {
                    ChannelServices.UnregisterChannel(_serverChannel);
                }
            }
            catch (Exception e)
            {
                //
            }
        }
    }
}

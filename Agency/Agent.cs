using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices.Expando;
using System.Text;
using System.Threading.Tasks;
using Dynamitey;
using Dynamitey.DynamicObjects;

namespace Agency
{
    public class Agent : MarshalByRefObject
    {
        private readonly RemoteRecorder _recorder = new RemoteRecorder(new Mimic());
        private readonly object _target;
        public string Address { get; set; }

        public Agent(string address, object obj)
        {
            Address = address;
            _target = obj;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public object Execute(RemoteInvocation remoteInvocation)
        {
            return _recorder.ReplayOn(_target, remoteInvocation);
        }
    }
}

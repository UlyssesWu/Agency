using System;
using System.Collections.Generic;
using Agency.Contracts;
using Dynamitey;
using Dynamitey.DynamicObjects;
using LinFu.Delegates;

namespace Agency
{
    public class Agent : MarshalByRefObject
    {
        private readonly Dictionary<string, Delegate> LocalDelegates = new Dictionary<string, Delegate>();
        public ContractDispatcher Dispatcher { get; set; }
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

        public List<string> GetTargetEvents()
        {
            List<string> result = new List<string>();
            foreach (var eventInfo in _target.GetType().GetEvents())
            {
                result.Add(eventInfo.Name);
            }
            return result;
        }

        public object Execute(RemoteInvocation ins)
        {
            if (ins.Args != null)
            {
                for (var i = 0; i < ins.Args.Length; i++)
                {
                    var arg = ins.Args[i];
                    if (arg is ExpressionContract expression)
                    {
                        if (expression.IsAdd)
                        {
                            dynamic exp = expression.Expression;
                            Delegate d = exp.Compile();
                            LocalDelegates[expression.Description] = d;
                            ins = new RemoteInvocation(InvocationKind.AddAssign, ins.Name, d);
                            break;
                        }
                        else
                        {
                            if (LocalDelegates.ContainsKey(expression.Description) && LocalDelegates[expression.Description] is Delegate d)
                            {
                                ins = new RemoteInvocation(InvocationKind.SubtractAssign, ins.Name, d);
                                break;
                            }
                        }
                    }

                    if (arg is EventContract eventContract)
                    {
                        if (eventContract.IsAdd)
                        {
                            Func<string, object[], object> c = Dispatcher.AssignContract;
                            Func<object[], object> cc = Dynamic.Curry(c)(eventContract.Description);
                            LocalDelegates[eventContract.Description] = EventBinder.BindToEvent(eventContract.Event, _target, cc);
                        }
                        else
                        {
                            if (LocalDelegates.ContainsKey(eventContract.Description) && LocalDelegates[eventContract.Description] is MulticastDelegate d)
                            {
                                EventBinder.UnbindFromEvent(eventContract.Event, _target, d);
                                LocalDelegates.Remove(eventContract.Description);
                            }
                        }
                        return null;
                    }
                }
            }
            return _recorder.ReplayOn(_target, ins);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Agency.Contracts;
using Dynamitey;
using Dynamitey.DynamicObjects;

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
                            //Func<string, object[], object> c = Dispatcher.AssignContract;
                            //Func<object[], object> cc = Dynamic.Curry(c)(eventContract.Description);
                            //LocalDelegates[eventContract.Description] = EventBinder.BindToEvent(eventContract.Event, _target, cc);

                            //REF: https://stackoverflow.com/questions/7935306/compiling-a-lambda-expression-results-in-delegate-with-closure-argument

                            var c = CreateProxyDelegateForEvent(_target, eventContract.Event, out var dType, out var pCount);
                            Delegate cc = Dynamic.CoerceToDelegate(Dynamic.Curry(c, pCount)(eventContract.Description), dType);
                            LocalDelegates[eventContract.Description] = cc;
                            ins = new RemoteInvocation(InvocationKind.AddAssign, ins.Name, cc);
                            break;
                        }
                        else
                        {
                            //if (LocalDelegates.ContainsKey(eventContract.Description) && LocalDelegates[eventContract.Description] is MulticastDelegate d)
                            if (LocalDelegates.ContainsKey(eventContract.Description) && LocalDelegates[eventContract.Description] is Delegate d)
                            {
                                //EventBinder.UnbindFromEvent(eventContract.Event, _target, d);
                                //LocalDelegates.Remove(eventContract.Description);

                                ins = new RemoteInvocation(InvocationKind.SubtractAssign, ins.Name, d);
                                LocalDelegates.Remove(eventContract.Description);
                                break;
                            }
                        }
                        //return null;
                    }
                }
            }
            return _recorder.ReplayOn(_target, ins);
        }

        //LinFu.Delegates won't works on mono since it throws error when calling `MethodHandle.GetFunctionPointer();` on method without body
        //REF: https://github.com/mono/mono/commit/9e96492f47a1d47137e921ac6c4a27f73ad779a8

        //REF: https://stackoverflow.com/questions/939956/howto-emit-a-delegate-or-lambda-expression
        private Delegate CreateDelegate(Type returnType, Type[] parameterTypes)
        {
            //m_Type = returnType;
            var nameExp = Expression.Parameter(typeof(string), "name");

            var i = 0;
            List<ParameterExpression> paras = new List<ParameterExpression>() { nameExp };
            var param = Array.ConvertAll(parameterTypes, arg => Expression.Parameter(arg, "arg" + i++));
            paras.AddRange(param);
            UnaryExpression[] asObj = Array.ConvertAll(param, p => Expression.Convert(p, typeof(object)));
            var argsArray = Expression.NewArrayInit(typeof(object), asObj);

            var callEx = Expression.Call(Expression.Constant(Dispatcher), typeof(ContractDispatcher).GetMethod("AssignContract") ?? throw new InvalidOperationException(), nameExp, argsArray);
            Expression body;
            if (returnType == null || returnType == typeof(void))
            {
                body = callEx;
            }
            else
            {
                body = Expression.Convert(callEx, returnType);
            }
            var lambda = Expression.Lambda(body, paras);
            var s = lambda.ToString();
            var ret = lambda.Compile();
            return ret;
        }

        private Delegate CreateProxyDelegateForEvent(object source, string eventName, out Type delegateType, out int paramCount)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            // Find the matching event defined on that type
            Type sourceType = source.GetType();
            EventInfo targetEvent = sourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            if (targetEvent == null)
                throw new ArgumentException(
                    string.Format("Event '{0}' was not found on source type '{1}'", eventName, sourceType.FullName));

            delegateType = targetEvent.EventHandlerType;
            var invokeMethod = delegateType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);

            var returnType = invokeMethod.ReturnType;
            var parameterTypes = invokeMethod.GetParameters().Select(param => param.ParameterType);
            var paras = parameterTypes.ToArray();
            paramCount = paras.Length;
            return CreateDelegate(returnType, paras);
        }
    }
}

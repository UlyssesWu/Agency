// 
//  Copyright 2011  Ekon Benefits
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.


using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Agency.Contracts;
using Dynamitey;
using Dynamitey.DynamicObjects;
using Dynamitey.Internal.Optimization;

namespace Agency
{
    /// <summary>
    /// Proxy that Replay Dynamic Invocations on an object
    /// </summary>
    public class RemoteProxy : BaseForwarder
    {
        public List<string> RemoteEvents { get; internal set; } = new List<string>();
        //public Dictionary<string, List<Delegate>> RemoteDelegates { get; } = new Dictionary<string, List<Delegate>>();
        public Dictionary<string, Delegate> RemoteDelegates { get; } = new Dictionary<string, Delegate>();

        public Agent Agent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteProxy"/> class.
        /// </summary>
        public RemoteProxy() : base(new Dummy())
        {
        }

        public object FireEvent(string name, object[] objects)
        {
            return RemoteDelegates.ContainsKey(name) ? RemoteDelegates[name].FastDynamicInvoke(objects) : null;
            //if (!RemoteDelegates.ContainsKey(name)) return null;
            //object result = null;
            //foreach (var d in RemoteDelegates[name])
            //{
            //    result = d.FastDynamicInvoke(objects);

            //}
            //return result;
        }

        //private void AddCacheDelegate(string name, Delegate d)
        //{
        //    if (!RemoteDelegates.ContainsKey(name))
        //    {
        //        RemoteDelegates[name] = new List<Delegate>();
        //    }
        //    RemoteDelegates[name].Add(d);
        //}

        //private void RemoveCacheDelegate(string name, Delegate d)
        //{
        //    if (!RemoteDelegates.ContainsKey(name))
        //    {
        //        return;
        //    }
        //    RemoteDelegates[name].Remove(d);
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteProxy"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public RemoteProxy(object target) : base(target)
        {
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (RemoteEvents.Contains(binder.Name))
            {
                result = new AddRemoveMarker();
                return true;
            }
            if (base.TryGetMember(binder, out result))
            {
                try
                {
                    result = Agent.Execute(new RemoteInvocation(InvocationKind.Get, binder.Name));
                }
                catch (SerializationException)
                {
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries the set member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (base.TrySetMember(binder, value))
            {
                if (value is AddRemoveMarker m)
                {
                    if (m.IsAdding)
                    {
                        if (m.Delegate is Delegate md)
                        {
                            value = md;
                        }
                        else if (m.Delegate is Expression exp)
                        {
                            value = exp.GetContract(binder.Name);
                        }
                        else
                        {
                            value = null;
                            return false;
                        }
                    }
                    else // -=
                    {
                        if (m.Delegate is Delegate md)
                        {
                            var contract = new EventContract(binder.Name, md.Method.Name, false);
                            RemoteDelegates.Remove(contract.Description);
                            value = contract;
                        }
                        else if (m.Delegate is Expression exp)
                        {
                            var contract = exp.GetContract(binder.Name);
                            contract.IsAdd = false;
                            value = contract;
                        }
                        else
                        {
                            value = null;
                        }
                    }
                }
                if (value is Delegate d)
                {
                    var contract = new EventContract(binder.Name, d.Method.Name, true);
                    RemoteDelegates[contract.Description] = d;
                    value = contract;
                }
                Agent.Execute(new RemoteInvocation(InvocationKind.Set, binder.Name, value));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries the invoke member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="args">The args.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (base.TryInvokeMember(binder, args, out result))
            {
                result = Agent.Execute(new RemoteInvocation(InvocationKind.InvokeMemberUnknown, binder.Name, Util.NameArgsIfNecessary(binder.CallInfo, args)));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries the index of the get.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="indexes">The indexes.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (base.TryGetIndex(binder, indexes, out result))
            {
                result = Agent.Execute(new RemoteInvocation(InvocationKind.GetIndex, Invocation.IndexBinderName, Util.NameArgsIfNecessary(binder.CallInfo, indexes)));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries the index of the set.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="indexes">The indexes.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (base.TrySetIndex(binder, indexes, value))
            {
                var tCombinedArgs = indexes.Concat(new[] { value }).ToArray();
                Agent.Execute(new RemoteInvocation(InvocationKind.GetIndex, Invocation.IndexBinderName, Util.NameArgsIfNecessary(binder.CallInfo, tCombinedArgs)));
                return true;
            }
            return false;
        }

    }
}
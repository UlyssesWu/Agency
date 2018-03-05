﻿// 
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
using System.Linq;
using System.Linq.Expressions;
using Dynamitey;
using Microsoft.CSharp.RuntimeBinder;

namespace Agency
{
    /// <summary>
    /// Storable representation of an invocation without the target
    /// </summary>
    [Serializable]
    public class RemoteInvocation
    {

        /// <summary>
        /// Defacto Binder Name for Explicit Convert Op
        /// </summary>
        public static readonly string ExplicitConvertBinderName = "(Explicit)";

        /// <summary>
        /// Defacto Binder Name for Implicit Convert Op
        /// </summary>
        public static readonly string ImplicitConvertBinderName = "(Implicit)";

        /// <summary>
        /// Defacto Binder Name for Indexer
        /// </summary>
        public static readonly string IndexBinderName = "Item";


        /// <summary>
        /// Defacto Binder Name for Construvter
        /// </summary>
        public static readonly string ConstructorBinderName = "new()";

        /// <summary>
        /// Gets or sets the kind.
        /// </summary>
        /// <value>The kind.</value>
        public InvocationKind Kind { get; protected set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the args.
        /// </summary>
        /// <value>The args.</value>
        public object[] Args { get; protected set; }

        /// <summary>
        /// Creates the invocation.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="name">The name.</param>
        /// <param name="storedArgs">The args.</param>
        /// <returns></returns>
        public static RemoteInvocation Create(InvocationKind kind, String_OR_InvokeMemberName name, params object[] storedArgs)
        {
            return new RemoteInvocation(kind,name.Name, storedArgs);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteInvocation"/> class.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="name">The name.</param>
        /// <param name="storedArgs">The args.</param>
        public RemoteInvocation(InvocationKind kind, string name, params object[] storedArgs)
        {
            Kind = kind;
            Name = name;
            Args = storedArgs;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(RemoteInvocation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Kind, Kind) && Equals(other.Name, Name) && (Equals(other.Args, Args) || Enumerable.SequenceEqual(other.Args, Args));
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RemoteInvocation)) return false;
            return Equals((RemoteInvocation) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = Kind.GetHashCode();
                result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ (Args != null ? Args.GetHashCode() : 0);
                return result;
            }
        }

        /// <summary>
        /// Invokes the invocation on specified target with specific args.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public virtual object Invoke(object target, params object[] args)
        {
            switch (Kind)
            {
                case InvocationKind.Constructor:
                    return Dynamic.InvokeConstructor((Type)target, args);
                case InvocationKind.Convert:
                    bool tExplict = false;
                    if (Args.Length == 2)
                        tExplict = (bool)args[1];
                    return Dynamic.InvokeConvert(target, (Type)args[0], tExplict);
                case InvocationKind.Get:
                    return Dynamic.InvokeGet(target, Name);
                case InvocationKind.Set:
                    Dynamic.InvokeSet(target, Name, args.FirstOrDefault());
                    return null;
                case InvocationKind.GetIndex:
                    return Dynamic.InvokeGetIndex(target, args);
                case InvocationKind.SetIndex:
                    Dynamic.InvokeSetIndex(target, args);
                    return null;
                case InvocationKind.InvokeMember:
                    return Dynamic.InvokeMember(target, Name, args);
                case InvocationKind.InvokeMemberAction:
                    Dynamic.InvokeMemberAction(target, Name, args);
                    return null;
                case InvocationKind.InvokeMemberUnknown:
                    {
                        try
                        {
                            return Dynamic.InvokeMember(target, Name, args);
                        }
                        catch (RuntimeBinderException)
                        {

                            Dynamic.InvokeMemberAction(target, Name, args);
                            return null;
                        }
                    }
                case InvocationKind.Invoke:
                    return Dynamic.Invoke(target, args);
                case InvocationKind.InvokeAction:
                    Dynamic.InvokeAction(target, args);
                    return null;
                case InvocationKind.InvokeUnknown:
                    {
                        try
                        {
                            return Dynamic.Invoke(target, args);
                        }
                        catch (RuntimeBinderException)
                        {

                            Dynamic.InvokeAction(target, args);
                            return null;
                        }
                    }
                case InvocationKind.AddAssign:
                    Dynamic.InvokeAddAssignMember(target, Name, args.FirstOrDefault());
                    return null;
                case InvocationKind.SubtractAssign:
                    Dynamic.InvokeSubtractAssignMember(target, Name, args.FirstOrDefault());
                    return null;
                case InvocationKind.IsEvent:
                    return Dynamic.InvokeIsEvent(target, Name);
                default:
                    throw new InvalidOperationException("Unknown Invocation Kind: " + Kind);
            }

        }

        /// <summary>
        /// Invokes the invocation on specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public virtual object InvokeWithStoredArgs(object target)
        {
            if (Args == null)
            {
                try
                {
                    return Invoke(target, new object[0]);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            else
            {
                for (var i = 0; i < Args.Length; i++)
                {
                    var arg = Args[i];
                    if (arg is ExpressionContract contract)
                    {
                        dynamic exp = contract.Expression;
                        Args[i] = exp.Compile();
                    }
                }
            }
            return Invoke(target, Args);
        }
    }
}

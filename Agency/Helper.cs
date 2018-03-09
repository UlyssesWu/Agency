using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Agency.Contracts;
using Serialize.Linq.Serializers;

namespace Agency
{
    public static class Helper
    {
        private static readonly ExpressionSerializer Serializer = new ExpressionSerializer(new JsonSerializer());

        public static dynamic GetAgent(this Agent agent)
        {
            var proxy = new RemoteProxy() { Agent = agent, RemoteEvents = new List<string>(agent.GetTargetEvents()) };
            agent.Dispatcher.Contract += (name, objs) => proxy.FireEvent(name, objs);
            return proxy;
        }

        //public static ExpressionContract GetDelegateBytes(this Expression expression)
        //{
        //    return new ExpressionContract(_serializer.SerializeBinary(expression));
        //}


        //internal static Expression GetExpression(this byte[] bytes)
        //{
        //    return _serializer.DeserializeBinary(bytes);
        //}

        public static ExpressionContract GetContract(this Expression expression, string @event = null)
        {
            return new ExpressionContract(Serializer.SerializeText(expression), @event);
        }
        internal static Expression GetExpression(this string desc)
        {
            return Serializer.DeserializeText(desc);
        }
    }
}

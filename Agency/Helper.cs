using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Serialize.Linq.Serializers;

namespace Agency
{
    public static class Helper
    {
        private static ExpressionSerializer _serializer = new ExpressionSerializer(new JsonSerializer());

        public static dynamic GetAgent(this Agent agent)
        {
            return new RemoteProxy() { Agent = agent };
        }

        //public static ExpressionContract GetDelegateBytes(this Expression expression)
        //{
        //    return new ExpressionContract(_serializer.SerializeBinary(expression));
        //}


        //internal static Expression GetExpression(this byte[] bytes)
        //{
        //    return _serializer.DeserializeBinary(bytes);
        //}

        public static ExpressionContract GetDelegateDesc(this Expression expression)
        {
            return new ExpressionContract(_serializer.SerializeText(expression));
        }
        internal static Expression GetExpression(this string desc)
        {
            return _serializer.DeserializeText(desc);
        }
    }
}

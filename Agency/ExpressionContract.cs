using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Agency
{
    [Serializable]
    public class ExpressionContract
    {
        public Expression Expression => Content?.GetExpression();

        public string Content { get; private set; }

        public ExpressionContract(string content)
        {
            Content = content;
        }

        public override string ToString()
        {
            return Agency.AgencyLambdaToken + Content;
        }
    }
}

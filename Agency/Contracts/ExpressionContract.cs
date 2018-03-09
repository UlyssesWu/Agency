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
        public string Event { get; set; }
        public string Description => $"[{Event}] {_desc}";
        public bool IsAdd { get; set; } = true;
        public Expression Expression => Content?.GetExpression();
        public string Content { get; private set; }
        private readonly string _desc;

        public ExpressionContract(string content, string @event = null)
        {
            if (content.StartsWith("+="))
            {
                IsAdd = true;
                content = content.Substring(2);
            }
            else if (content.StartsWith("-="))
            {
                IsAdd = false;
                content = content.Substring(2);
            }
            Content = content;
            Event = @event;
            _desc = Expression.ToString();
        }

        public override string ToString()
        {
            return $"{Agency.AgencyLambdaToken}{(IsAdd ? "+=" : "-=")}" + Content;
        }
    }
}

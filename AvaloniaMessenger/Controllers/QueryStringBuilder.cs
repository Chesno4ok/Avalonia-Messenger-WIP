using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaMessenger.Controllers
{
    class QueryStringBuilder
    {
        public string GetQueryString(Dictionary<string, object> query)
        {
            string queryString = "";

            foreach (var p in query)
            {
                Type type = p.GetType();


                var m = GetType().GetMethods().FirstOrDefault(i => i.GetCustomAttribute<QueryTypeInfo>().queryType == p.Value.GetType());

                if (m == null)
                    queryString += $"{p.Key}={p.Value}&";
                else
                    queryString += $"{p.Key}={m.Invoke(this, new object[] {p.Value})}";
            }

            return queryString;
        }
        [QueryTypeInfo("string")]
        public string GetString(object value)
        {
            return $"\"{value}\"";
        }

        private static Dictionary<string, string> GetQueryDictionary(params string[] queryValues)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

            StackTrace stackTrace = new StackTrace();

            var stackFrame = stackTrace.GetFrame(1);
            if (stackFrame == null)
                throw new Exception("Stack trace is empty!");

            var callMethod = stackFrame.GetMethod();
            if (callMethod == null)
                throw new Exception("Previous method is empty!");

            var queryInfo = callMethod.GetCustomAttribute<QueryInfo>();
            if (queryInfo == null)
                throw new Exception("Calling method must have QueryInfo attribute!");

            string[] queryNames = queryInfo.QueryParams;
            if (queryNames.Length != queryValues.Length)
                throw new Exception("QueryNames and QueryValues must have the same size!");

            for (int i = 0; i < queryNames.Length; i++)
                query.Add(queryNames[i], queryValues[i]);

            return query;
        }

    }
    class QueryInfo : Attribute
    {
        public string[] QueryParams { get; private set; }

        public QueryInfo(params string[] query)
        {
            if (query.Length == 0)
                throw new Exception("Query cannot be empty.");

            QueryParams = query;
        }
    }
    class QueryTypeInfo : Attribute
    {
        public Type? queryType;
        public QueryTypeInfo(string type)
        {
            queryType = Type.GetType(type);

        }
    }
}


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaMessenger.Controllers
{
    class QueryStringBuilder
    {
        [QueryTypeInfo(typeof(string))]
        public string GetString(object value)
        {
            return $"{value}";
        }
        [QueryTypeInfo(typeof(int))]
        public string GetInteger(object value)
        {
            return $"{value}";
        }

        public string GetQueryString(Dictionary<string, object> query)
        {
            string queryString = "";

            foreach (var p in query)
            {
                MethodInfo?[] methods;
                string s = p.Value.GetType().ToString();

                methods = typeof(QueryStringBuilder).GetMethods().Where(i => i.GetCustomAttribute<QueryTypeInfo>() != null).ToArray();

                MethodInfo? m;
                try
                {
                    m = methods.FirstOrDefault(i => i.GetCustomAttribute<QueryTypeInfo>().QueryType.Any(i => i == p.Value.GetType()));
                }
                catch
                {
                    m = null;
                }


                if (m == null)
                    queryString += $"{p.Key}={p.Value}&";
                else
                    queryString += $"{p.Key}={m.Invoke(this, new object[] { p.Value })}&";
            }

            return queryString;
        }
        
        

        public string GetQueryString(params object[] queryValues)
        {
            Dictionary<string, object> queryDictionary = new Dictionary<string, object>();

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
                queryDictionary.Add(queryNames[i], queryValues[i]);

            return GetQueryString(queryDictionary);
        }

        public static Dictionary<string, string> GetQueryDictionary(params string[] queryValues)
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
        public Type?[] QueryType;
        public QueryTypeInfo(params Type[] type)
        {
            QueryType = type;

            if (QueryType == null)
                throw new Exception("Type cannot be null");
        }
    }
}

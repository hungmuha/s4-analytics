using Dapper;
using System.Collections.Generic;

namespace S4Analytics.Models
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extension method that works like AddDynamicParams() but
        /// does not have the limitation/bug that parameter names are
        /// missing from the ParameterNames collection.
        /// </summary>
        /// <param name="dynamicParams">DynamicParameters instance</param>
        /// <param name="parameters">Parameters object whose fields should be added to the DynamicParameters instance</param>
        public static void Add(this DynamicParameters dynamicParams, object parameters)
        {
            // add a parameter for each field in the template object
            foreach (var prop in parameters.GetType().GetProperties())
            {
                dynamicParams.Add(prop.Name, prop.GetValue(parameters));
            }
        }

        /// <summary>
        /// Generate a text dump of parameter keys and values.
        /// </summary>
        /// <param name="dynamicParams">DynamicParameters instance</param>
        /// <returns>Text dump of parameter keys and values</returns>
        public static string DumpText(this DynamicParameters dynamicParams)
        {
            var lines = new List<string>();
            foreach (var paramName in dynamicParams.ParameterNames)
            {
                var paramValue = dynamicParams.Get<object>(paramName);
                string paramValueStr;
                if (paramValue.GetType() == typeof(List<int>))
                {
                    paramValueStr = string.Join(", ", ((List<int>)paramValue).ToArray());
                }
                else if (paramValue.GetType() == typeof(List<string>))
                {
                    paramValueStr = string.Join(", ", ((List<string>)paramValue).ToArray());
                }
                else
                {
                    paramValueStr = paramValue.ToString();
                }
                lines.Add(string.Format("{0}: {1}", paramName, paramValueStr));
            }
            var text = string.Join("\r\n", lines);
            return text;
        }
    }
}

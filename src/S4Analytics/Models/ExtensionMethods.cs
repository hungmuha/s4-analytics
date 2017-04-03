using Dapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace S4Analytics.Models
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extension method that adds Dictionary contents to DynamicParameters instance.
        /// </summary>
        /// <param name="dynamicParams">DynamicParameters instance</param>
        /// <param name="parameters">Dictionary whose values should be added to the DynamicParameters instance</param>
        public static void Add(this DynamicParameters dynamicParams, Dictionary<string, object> parameters)
        {
            // add a parameter for each field in the template object
            foreach (var key in parameters.Keys)
            {
                dynamicParams.Add(key, parameters[key]);
            }
        }

        /// <summary>
        /// Extension method that allows fields of an object to be added to a dictionary.
        /// </summary>
        /// <param name="paramDict">Dictionary to be added to</param>
        /// <param name="parameters">Parameters object whose fields should be added to the Dictionary instance</param>
        public static void Add(this Dictionary<string, object> paramDict, object parameters)
        {
            // add a parameter for each field in the template object
            foreach (var prop in parameters.GetType().GetProperties())
            {
                paramDict.Add(prop.Name, prop.GetValue(parameters));
            }
        }

        /// <summary>
        /// Generate a text dump of Dictionary keys and values.
        /// </summary>
        /// <param name="dict">Dictionary instance</param>
        /// <returns>Text dump of Dictionary keys and values</returns>
        public static string DumpText(this Dictionary<string, object> dict)
        {
            var lines = new List<string>();
            foreach (var key in dict.Keys)
            {
                var value = dict[key];
                string valueStr;
                if (value.GetType() == typeof(List<int>))
                {
                    valueStr = string.Join(", ", ((List<int>)value).ToArray());
                }
                else if (value.GetType() == typeof(List<string>))
                {
                    valueStr = string.Join(", ", ((List<string>)value).ToArray());
                }
                else
                {
                    valueStr = value.ToString();
                }
                lines.Add(string.Format("{0}: {1}", key, valueStr));
            }
            var text = string.Join("\r\n", lines);
            return text;
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            session.SetString(key, JsonConvert.SerializeObject(value, serializerSettings));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var value = session.GetString(key);
            return value == null
                ? default(T)
                : JsonConvert.DeserializeObject<T>(value, serializerSettings);
        }
    }
}

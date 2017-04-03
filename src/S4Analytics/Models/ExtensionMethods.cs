﻿using Dapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace S4Analytics.Models
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Add Dictionary contents to DynamicParameters instance.
        /// </summary>
        /// <param name="dynamicParams">DynamicParameters instance.</param>
        /// <param name="parameters">Dictionary whose values should be added to the DynamicParameters instance.</param>
        public static void AddDict(this DynamicParameters dynamicParams, Dictionary<string, object> parameters)
        {
            // add a parameter for each field in the template object
            foreach (var key in parameters.Keys)
            {
                dynamicParams.Add(key, parameters[key]);
            }
        }

        /// <summary>
        /// Add all fields of an object to a dictionary.
        /// </summary>
        /// <param name="dict">Dictionary instance.</param>
        /// <param name="item">Item whose fields should be added to the Dictionary instance.</param>
        public static void AddFields(this Dictionary<string, object> dict, object item)
        {
            // add a parameter for each field in the template object
            foreach (var prop in item.GetType().GetProperties())
            {
                dict.Add(prop.Name, prop.GetValue(item));
            }
        }

        /// <summary>
        /// Serialize object to JSON.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>JSON representation of object.</returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Serialize object to pretty JSON.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>Pretty JSON representation of object.</returns>
        public static string ToPrettyJson(this object obj)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(obj, serializerSettings);
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

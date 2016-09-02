using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S4Analytics.Models
{
    public class Utils
    {
        /// <summary>
        /// Convert a numeric value from the database to an enum value.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="dbValue">Numeric value from the database</param>
        /// <returns></returns>
        static public T ConvertToEnum<T>(decimal dbValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) { throw new ArgumentException("T must be an enumeration."); }
            return (T)(object)Convert.ToInt32(dbValue);
        }

        /// <summary>
        /// Convert a nullable numeric value from the database to an enum value.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="dbValue">Numeric value from the database</param>
        /// <param name="defaultValue">Default enum value to use if database value is DBNull</param>
        /// <returns></returns>
        static public T ConvertToEnumX<T>(decimal? dbValue, T defaultValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) { throw new ArgumentException("T must be an enumeration."); }
            return dbValue == null
                ? defaultValue
                : (T)(object)Convert.ToInt32((decimal)dbValue);
        }
    }
}

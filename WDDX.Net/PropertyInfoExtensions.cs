using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Mueller.Wddx
{
    /// <summary>
    /// Extension methods for the <c>PropertyInfo</c> class.
    /// </summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Sets the property value of a specified object with optional index values for index properties.
        /// It will also attempt to convert the object to the destination type if the type is different.
        /// </summary>
        /// <param name="propertyInfo">The instance of the <c>PropertyInfo</c> class this applies to.</param>
        /// <param name="obj">The object whose property value will be set. </param>
        /// <param name="value">The new property value. </param>
        /// <param name="index">Optional index values for indexed properties. This value should be null for non-indexed properties. </param>
        public static void SetAndConvertValue(this PropertyInfo propertyInfo, object obj, object value, object[] index)
        {
            if (propertyInfo.PropertyType == value.GetType())
            {
                propertyInfo.SetValue(obj, value, index);
            }
            else
            {
                var convertedValue = Convert.ChangeType(value, propertyInfo.PropertyType);
                propertyInfo.SetValue(obj, convertedValue, index);
            }
        }

        /// <summary>
        /// Creates a dictionary for a PropertyInfo collection with a Key being the name of the property
        /// and the value being the PropertyInfo itself. If the property has a [DataMember] attribute
        /// overriding the Name, then this is used instead of the property name.
        /// </summary>
        /// <param name="properties">The collection of properties to create a name lookup for.</param>
        public static Dictionary<string, PropertyInfo> ToNameDictionary(this IEnumerable<PropertyInfo> properties)
        {
            var result = new Dictionary<string, PropertyInfo>();
            foreach (var property in properties)
            {
                var dataMemberAttribute = property
                    .GetCustomAttributes(typeof(DataMemberAttribute), false)
                    .Cast<DataMemberAttribute>()
                    .FirstOrDefault();

                if (dataMemberAttribute != null && !result.ContainsKey(dataMemberAttribute.Name))
                {
                    result.Add(dataMemberAttribute.Name, property);
                }
                else if (!result.ContainsKey(property.Name))
                {
                    result.Add(property.Name, property);
                }
            }
            return result;
        }
    }
}

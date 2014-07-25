using System;
using System.Collections;
using System.Collections.Generic;

namespace Mueller.Wddx
{
    /// <summary>
    ///		This class provides the appropriate typed deserializer
    ///		based on the tag name of the element being processed.
    ///     ie: deserialize to a specific type
    /// </summary>
    internal static class WddxElementTypedDeserializerFactory
    {
        /// <summary>
        ///		Gets the appropriate typed deserializer based on the tag name
        ///		of the element being processed and the destination type it is
        ///     being deserialized to.
        /// </summary>
        /// <param name="tagName">A valid WDDX tag name.</param>
        /// <param name="destinationType">The <c>Type</c> of the destination</param>
        public static IWddxElementDeserializer GetDeserializer(string tagName, Type destinationType)
        {
            if ((tagName == "struct") && (destinationType != typeof(Hashtable)))
            {
                if (destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == typeof (Dictionary<,>))
                {
                    return TypedDictionaryDeserializer.Instance;
                }

                return ClassDeserializer.Instance;
            }

            if (tagName == "array")
            {
                if (destinationType.IsArray)
                {
                    return TypedArrayDeserializer.Instance;
                }
            }

            return WddxElementDeserializerFactory.GetDeserializer(tagName);
        }
    }
}
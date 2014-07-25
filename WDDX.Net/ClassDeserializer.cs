using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections;

namespace Mueller.Wddx
{
    /// <summary>
    ///		Deserializes a WDDX struct element into a <see cref="Hashtable"/> object.
    /// </summary>
    /// <remarks>
    ///		This class is a Singleton class - only one instance of it will ever exist
    ///		in a given AppDomain.
    ///	</remarks>
    ///	<seealso cref="IWddxElementDeserializer"/>
    internal class ClassDeserializer : IWddxElementTypedDeserializer
    {
        private static ClassDeserializer _instance;

        /// <summary>
        ///		Provides access to the instance of this class.
        /// </summary>
        public static ClassDeserializer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(ClassDeserializer))
                    {
                        if (_instance == null)
                            _instance = new ClassDeserializer();
                    }
                }

                return _instance;
            }
        }

        private ClassDeserializer() { }

        /// <summary>
        ///		Parses the WDDX element and returns the deserialized content as 
        ///     a specific typed object, advancing the reader to the next element.
        /// </summary>
        /// <typeparam name="T">The type of the object to be returned.</typeparam>
        /// <param name="input">The pre-initialized <see cref="System.Xml.XmlTextReader"/> pointing to the WDDX to be parsed.</param>
        public T ParseElement<T>(XmlReader input)
        {
            return (T)ParseElement(input, typeof(T));
        }

        /// <summary>
        ///		Parses the WDDX element and returns the deserialized content as 
        ///     a specific typed object, advancing the reader to the next element.
        /// </summary>
        /// <param name="input">The pre-initialized <see cref="System.Xml.XmlTextReader"/> pointing to the WDDX to be parsed.</param>
        /// <param name="elementType">The type of the object to be returned.</param>
        public object ParseElement(XmlReader input, Type elementType)
        {
            object result = Activator.CreateInstance(elementType);

            if (input.IsEmptyElement)
            {
                input.Skip();
                return result;
            }

            var properties = elementType.GetProperties();
            var propertyMap = properties.ToNameDictionary();

            while (input.Read() && (!(input.Name == "struct" && input.NodeType == XmlNodeType.EndElement)))
            {
                if (input.Name == "var" && input.NodeType != XmlNodeType.EndElement)
                {
                    string elementName = input.GetAttribute("name");
                    input.Read();  // move to contents of <var>

                    if (elementName != null && propertyMap.ContainsKey(elementName))
                    {
                        var property = propertyMap[elementName];

                        var deserializer = WddxElementTypedDeserializerFactory.GetDeserializer(input.Name, property.PropertyType);
                        var typedDeserializer = deserializer as IWddxElementTypedDeserializer;
                        var elementValue = (typedDeserializer != null)
                            ? typedDeserializer.ParseElement(input, property.PropertyType)
                            : deserializer.ParseElement(input);

                        if (elementValue != null)
                        {
                            property.SetAndConvertValue(result, elementValue, null);
                        }
                    }
                    else
                    {
                        var deserializer = WddxElementDeserializerFactory.GetDeserializer(input.Name);
                        deserializer.ParseElement(input);
                    }
                }
            }
            input.ReadEndElement();

            return result;

        }

        /// <summary>
        ///		Parses the WDDX element and returns the deserialized
        ///		content as an object, advancing the reader to the next
        ///		element.
        /// </summary>
        /// <param name="input">The pre-initialized <see cref="System.Xml.XmlTextReader"/> pointing to the WDDX to be parsed.</param>
        /// <remarks>This is not supported for this class.</remarks>
        public object ParseElement(XmlReader input)
        {
            throw new NotSupportedException();
        }
    }
}

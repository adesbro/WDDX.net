using System;
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
    internal class TypedDictionaryDeserializer : IWddxElementTypedDeserializer
    {
        private static TypedDictionaryDeserializer _instance;

        /// <summary>
        ///		Provides access to the instance of this class.
        /// </summary>
        public static TypedDictionaryDeserializer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(TypedDictionaryDeserializer))
                    {
                        if (_instance == null)
                            _instance = new TypedDictionaryDeserializer();
                    }
                }

                return _instance;
            }
        }

        private TypedDictionaryDeserializer() { }

        /// <summary>
        ///		Parses the WDDX element and returns the deserialized content as an instance of a 
        ///     specified type, advancing the reader to the next element.
        /// </summary>
        /// <typeparam name="T">The type of the instance to deserialize into.</typeparam>
        /// <param name="input">The pre-initialized <see cref="System.Xml.XmlTextReader"/> pointing to the WDDX to be parsed.</param>
        /// <remarks>The return type will be a typed Dictionary.</remarks>
        public T ParseElement<T>(XmlReader input)
        {
            return (T)ParseElement(input, typeof(T));
        }

        /// <summary>
        ///		Parses the WDDX element and returns the deserialized content as an instance of a 
        ///     specified type, advancing the reader to the next element.
        /// </summary>
        /// <param name="input">The pre-initialized <see cref="System.Xml.XmlTextReader"/> pointing to the WDDX to be parsed.</param>
        /// <param name="elementType">The type of the instance to deserialize into.</param>
        /// <remarks>The return type will be a typed Dictionary.</remarks>
        public object ParseElement(XmlReader input, Type elementType)
        {
            object thisTable = Activator.CreateInstance(elementType);

            if (input.IsEmptyElement)
            {
                input.Skip();
                return thisTable;
            }

            var dictValueType = elementType.GetGenericArguments()[1];

            while (input.Read() && (!(input.Name == "struct" && input.NodeType == XmlNodeType.EndElement)))
            {
                if (input.Name == "var" && input.NodeType != XmlNodeType.EndElement)
                {
                    string elementName = input.GetAttribute("name");
                    input.Read();  // move to contents of <var>
                    var deserializer = WddxElementTypedDeserializerFactory.GetDeserializer(input.Name, dictValueType);
                    var typedDeserializer = deserializer as IWddxElementTypedDeserializer;
                    var elementValue = (typedDeserializer != null)
                        ? typedDeserializer.ParseElement(input, dictValueType)
                        : deserializer.ParseElement(input);

                    var dictionaryKey = elementName;
                    var dictionaryValue = elementValue;
                    var add = thisTable.GetType()
                        .GetMethod("Add", new[] { dictionaryKey.GetType(), dictionaryValue.GetType() });
                    add.Invoke(thisTable, new[] { dictionaryKey, dictionaryValue }); 
                }
            }
            input.ReadEndElement();

            return thisTable;
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

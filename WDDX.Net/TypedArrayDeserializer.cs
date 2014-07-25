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
    internal class TypedArrayDeserializer : IWddxElementTypedDeserializer
    {
        private static TypedArrayDeserializer _instance;

        /// <summary>
        ///		Provides access to the instance of this class.
        /// </summary>
        public static TypedArrayDeserializer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(TypedArrayDeserializer))
                    {
                        if (_instance == null)
                            _instance = new TypedArrayDeserializer();
                    }
                }

                return _instance;
            }
        }

        private TypedArrayDeserializer() { }

        /// <summary>
        ///		Parses the WDDX element and returns the deserialized content as an instance of a 
        ///     specified type, advancing the reader to the next element.
        /// </summary>
        /// <typeparam name="T">The type of the instance to deserialize into.</typeparam>
        /// <param name="input">The pre-initialized <see cref="System.Xml.XmlTextReader"/> pointing to the WDDX to be parsed.</param>
        /// <remarks>The return type will be an Array.</remarks>
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
        /// <remarks>The return type will be an Array.</remarks>
        public object ParseElement(XmlReader input, Type elementType)
        {
            var arrayItemType = elementType.GetElementType();
            var result = new ArrayList();

            if (input.IsEmptyElement)
            {
                input.Skip();
                return result;
            }

            input.Read();
            while (!(input.Name == "array" && input.NodeType == XmlNodeType.EndElement))
            {
                var deserializer = WddxElementTypedDeserializerFactory.GetDeserializer(input.Name, arrayItemType);
                var typedDeserializer = deserializer as IWddxElementTypedDeserializer;
                var elementValue = (typedDeserializer != null)
                    ? typedDeserializer.ParseElement(input, arrayItemType)
                    : deserializer.ParseElement(input);

                result.Add(elementValue);
            }
            input.ReadEndElement();

            return result.ToArray(arrayItemType);
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

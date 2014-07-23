using System.Xml;

namespace Mueller.Wddx
{
    /// <summary>
    ///    Interface that defines the behavior for typed deserializers.
    /// </summary>
    internal interface IWddxElementTypedDeserializer 
    {
        /// <summary>
        ///		Parses the WDDX element and returns the deserialized content as an instance of a 
        ///     specified type, advancing the reader to the next element.
        /// </summary>
        /// <typeparam name="T">The type of the instance to deserialize into.</typeparam>
        /// <param name="input">The pre-initialized <see cref="System.Xml.XmlTextReader"/> pointing to the WDDX to be parsed.</param>
        T ParseElement<T>(XmlReader input);
    }
}
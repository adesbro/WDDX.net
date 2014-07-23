using System;
using System.Linq;
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
    internal class TypedDeserializer : IWddxElementTypedDeserializer
    {
		private static TypedDeserializer instance = null;

		/// <summary>
		///		Provides access to the instance of this class.
		/// </summary>
		public static TypedDeserializer Instance
		{
			get
			{
				if (instance == null)
				{
					lock (typeof(TypedDeserializer))
					{
						if (instance == null)
							instance = new TypedDeserializer();
					}
				}

				return instance;
			}
		}

        private TypedDeserializer() { }

		/// <summary>
		///		Parses the WDDX element and returns the deserialized
		///		content as a <see cref="Hashtable"/> object, advancing the reader to the next
		///		element.
		/// </summary>
		/// <param name="input">The pre-initialized <see cref="System.Xml.XmlTextReader"/> pointing to the WDDX to be parsed.</param>
	    public T ParseElement<T>(XmlReader input)
	    {
            T result = Activator.CreateInstance<T>();

            if (input.IsEmptyElement)
            {
                input.Skip();
                return result;
            }

            var type = result.GetType();
            var properties = type.GetProperties();
            // TODO: allow [DataMember] to overrule Name
		    var propertyMap = properties.ToDictionary(p => p.Name);

            //foreach (var property in properties)
            //{
            //    var dataMember = property.GetAttribute<DataMemberAttribute>();
            //    if ((dataMember != null) && hashValues.ContainsKey(dataMember.Name))
            //    {
            //        property.SetValue(obj, hashValues[dataMember.Name].ToString(), null);
            //    }
            //    else if (hashValues.ContainsKey(property.Name))
            //    {
            //        property.SetValue(obj, hashValues[property.Name].ToString(), null);
            //    }
            //}

            string elementName;
            object elementValue;
            IWddxElementDeserializer deserializer;
            while (input.Read() && (!(input.Name == "struct" && input.NodeType == XmlNodeType.EndElement)))
            {
                if (input.Name == "var" && input.NodeType != XmlNodeType.EndElement)
                {
                    elementName = input.GetAttribute("name");
                    input.Read();  // move to contents of <var>

                    if (propertyMap.ContainsKey(elementName))
                    {
                        deserializer = WddxElementDeserializerFactory.GetDeserializer(input.Name);
                        elementValue = deserializer.ParseElement(input);

                        propertyMap[elementName].SetValue(result, elementValue, null);
                    }
                }
            }
            input.ReadEndElement();

            return result;
        }
    }
}

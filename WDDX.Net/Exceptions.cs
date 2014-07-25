using System;

namespace Mueller.Wddx
{
	[Serializable]
	public class WddxException : ApplicationException
	{
		public WddxException() : base() {}
		public WddxException(string message) : base(message) {}
		public WddxException(string message, Exception inner) : base(message, inner) {}
	}

	/// <summary>
	///		The exception that is thrown when invalid WDDX is encountered.
	/// </summary>
	[Serializable]
	public class WddxValidationException : WddxException
	{
		private readonly string _validationError = "";

		public WddxValidationException() : base() {}
		public WddxValidationException(string message) : base(message) {}
		public WddxValidationException(string message, Exception inner) : base(message, inner) {}

		/// <summary>
		///		Initializes a new instance of the WddxValidationException class,
		///		specifying the validation error message.
		/// </summary>
		/// <param name="message">A message that describes the current exception.</param>
		/// <param name="validationError">A message that describes the specific validation failure.</param>
		public WddxValidationException(string message, string validationError) : base(message)
		{
			_validationError = validationError;
		}

        /// <summary>
        ///		Initializes a new instance of the WddxValidationException class,
        ///		specifying the validation error message and an inner exception.
        /// </summary>
        /// <param name="message">A message that describes the current exception.</param>
        /// <param name="validationError">A message that describes the specific validation failure.</param>
        /// <param name="inner">The exception to store within this one.</param>
        public WddxValidationException(string message, string validationError, Exception inner)
            : base(message, inner)
        {
            _validationError = validationError;
        }

		/// <summary>
		///		The error message returned by XSD validation.
		/// </summary>
		public string ValidationError
		{
			get { return _validationError; }
		}

		/// <summary>
		///		Returns a string that represents the current object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "WDDX validation error:\n" + _validationError + "\n\n" + base.ToString();
		}
	}
}

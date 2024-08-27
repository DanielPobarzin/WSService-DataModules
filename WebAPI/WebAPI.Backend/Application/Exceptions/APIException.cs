using System.Globalization;

namespace Application.Exceptions
{
	public class APIException : Exception
	{
		public APIException() : base()
		{
		}

		public APIException(string message) : base(message)
		{
		}

		public APIException(string message, params object[] args)
			: base(String.Format(CultureInfo.CurrentCulture, message, args))
		{
		}

		public APIException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

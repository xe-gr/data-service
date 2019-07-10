using System;

namespace xe.data.service.Exceptions
{
	public class ConfigurationNotFoundException : Exception
	{
		public ConfigurationNotFoundException(string message) : base(message)
		{

		}
	}
}

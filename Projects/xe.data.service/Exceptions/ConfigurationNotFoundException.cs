using System;

namespace xe.data.service.Exceptions
{
	public class ConfigurationNotFoundException(string message) : Exception(message);
}

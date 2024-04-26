using System;

namespace xe.data.service.Exceptions
{
	public class BadRequestException(string message) : Exception(message);
}
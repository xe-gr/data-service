using System.Collections.Generic;

namespace xe.data.service.Services.Interfaces
{
	public interface IDataService
	{
		List<dynamic> ExecuteRequest(string name, string parameters, string values);
	}
}
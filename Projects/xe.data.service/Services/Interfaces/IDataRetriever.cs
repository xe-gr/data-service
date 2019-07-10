using System.Data;

namespace xe.data.service.Services.Interfaces
{
	public interface IDataRetriever
	{
		DataSet RetrieveData(IDataCreator creator, string connectionString, string databaseType, string sql);
	}
}
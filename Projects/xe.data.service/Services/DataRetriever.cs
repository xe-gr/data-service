using System.Data;
using xe.data.service.Services.Interfaces;

namespace xe.data.service.Services
{
	public class DataRetriever : IDataRetriever
	{
		public DataSet RetrieveData(IDataCreator creator, string connectionString, string databaseType, string sql)
		{
			using (var con = creator.CreateConnection(connectionString, databaseType))
			using (var cmd = creator.CreateCommand(con, sql))
			using (var da = creator.CreateAdapter(cmd))
			{
				var ds = new DataSet();

				da.Fill(ds);

				return ds;
			}
		}
	}
}

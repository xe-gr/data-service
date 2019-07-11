using System.Data;
using xe.data.service.Extensions;
using xe.data.service.Services.Interfaces;

namespace xe.data.service.Services
{
	public class DataRetriever : IDataRetriever
	{
		public DataSet RetrieveData(IDataCreator creator, string connectionString, string databaseType, string sql)
		{
            var type = databaseType.ToDatabaseType();

			using (var con = creator.CreateConnection(connectionString, type))
			using (var cmd = creator.CreateCommand(con, type, sql))
			using (var da = creator.CreateAdapter(cmd, type))
			{
				var ds = new DataSet();

				da.Fill(ds);

				return ds;
			}
		}
	}
}
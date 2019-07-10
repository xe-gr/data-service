using System.Data.Common;
using System.Data.SqlClient;
using xe.data.service.Services.Interfaces;

namespace xe.data.service.Services
{
    public class DataCreator : IDataCreator
    {
        public DbDataAdapter CreateAdapter(DbCommand command)
        {
            return new SqlDataAdapter((SqlCommand)command);
        }

        public DbCommand CreateCommand(DbConnection connection, string sql)
        {
            return new SqlCommand(sql, (SqlConnection)connection);
        }

        public DbConnection CreateConnection(string connectionString, string databaseType)
        {
            return new SqlConnection(connectionString);
        }
    }
}
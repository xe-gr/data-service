using System.Data.Common;

namespace xe.data.service.Services.Interfaces
{
    public interface IDataCreator
    {
        DbConnection CreateConnection(string connectionString, string databaseType);
        DbCommand CreateCommand(DbConnection connection, string sql);
        DbDataAdapter CreateAdapter(DbCommand command);
    }
}
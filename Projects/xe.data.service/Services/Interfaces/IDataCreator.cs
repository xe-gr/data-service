using System.Data.Common;
using xe.data.service.Models;

namespace xe.data.service.Services.Interfaces
{
    public interface IDataCreator
    {
        DbConnection CreateConnection(string connectionString, DatabaseType databaseType);
        DbCommand CreateCommand(DbConnection connection, DatabaseType databaseType, string sql);
        DbDataAdapter CreateAdapter(DbCommand command, DatabaseType databaseType);
    }
}
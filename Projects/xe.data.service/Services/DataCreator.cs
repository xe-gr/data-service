using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using xe.data.service.Models;
using xe.data.service.Services.Interfaces;

namespace xe.data.service.Services
{
    public class DataCreator : IDataCreator
    {
        public DbDataAdapter CreateAdapter(DbCommand command, DatabaseType databaseType)
        {
            return databaseType switch
            {
                DatabaseType.SqlServer => new SqlDataAdapter((SqlCommand)command),
                DatabaseType.Oracle => new OracleDataAdapter((OracleCommand)command),
                DatabaseType.MySql => new MySqlDataAdapter((MySqlCommand)command),
                DatabaseType.Postgres => new NpgsqlDataAdapter((NpgsqlCommand)command),
                _ => throw new InvalidOperationException($"Invalid database type [{databaseType}]")
            };
        }

        public DbCommand CreateCommand(DbConnection connection, DatabaseType databaseType, string sql)
        {
            return databaseType switch
            {
                DatabaseType.SqlServer => new SqlCommand(sql, (SqlConnection)connection),
                DatabaseType.Oracle => new OracleCommand(sql, (OracleConnection)connection),
                DatabaseType.MySql => new MySqlCommand(sql, (MySqlConnection)connection),
                DatabaseType.Postgres => new NpgsqlCommand(sql, (NpgsqlConnection)connection),
                _ => throw new InvalidOperationException($"Invalid database type [{databaseType}]")
            };
        }

        public DbConnection CreateConnection(string connectionString, DatabaseType databaseType)
        {
            return databaseType switch
            {
                DatabaseType.SqlServer => new SqlConnection(connectionString),
                DatabaseType.Oracle => new OracleConnection(connectionString),
                DatabaseType.MySql => new MySqlConnection(connectionString),
                DatabaseType.Postgres => new NpgsqlConnection(connectionString),
                _ => throw new InvalidOperationException($"Invalid database type [{databaseType}]")
            };
        }
    }
}
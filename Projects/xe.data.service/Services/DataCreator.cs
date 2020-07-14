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
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    return new SqlDataAdapter((SqlCommand)command);
                case DatabaseType.Oracle:
                    return new OracleDataAdapter((OracleCommand)command);
                case DatabaseType.MySql:
                    return new MySqlDataAdapter((MySqlCommand) command);
                case DatabaseType.Postgres:
                    return new NpgsqlDataAdapter((NpgsqlCommand) command);
                default:
                    throw new InvalidOperationException($"Invalid database type [{databaseType}]");
            }
        }

        public DbCommand CreateCommand(DbConnection connection, DatabaseType databaseType, string sql)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    return new SqlCommand(sql, (SqlConnection)connection);
                case DatabaseType.Oracle:
                    return new OracleCommand(sql, (OracleConnection)connection);
                case DatabaseType.MySql:
                    return new MySqlCommand(sql, (MySqlConnection)connection);
                case DatabaseType.Postgres:
                    return new NpgsqlCommand(sql, (NpgsqlConnection) connection);
                default:
                    throw new InvalidOperationException($"Invalid database type [{databaseType}]");
            }
        }

        public DbConnection CreateConnection(string connectionString, DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    return new SqlConnection(connectionString);
                case DatabaseType.Oracle:
                    return new OracleConnection(connectionString);
                case DatabaseType.MySql:
                    return new MySqlConnection(connectionString);
                case DatabaseType.Postgres:
                    return new NpgsqlConnection(connectionString);
                default:
                    throw new InvalidOperationException($"Invalid database type [{databaseType}]");
            }
        }
    }
}
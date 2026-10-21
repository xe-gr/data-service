using System;
using xe.data.service.Models;

namespace xe.data.service.Extensions
{
    public static class DatabaseTypeExtensions
    {
        public static DatabaseType ToDatabaseType (this string databaseType)
        {
            return databaseType.ToLower() switch
            {
                "sqlserver" => DatabaseType.SqlServer,
                "oracle" => DatabaseType.Oracle,
                "mysql" => DatabaseType.MySql,
                "postgres" => DatabaseType.Postgres,
                _ => throw new InvalidOperationException($"Invalid database type [{databaseType}]")
            };
        }
    }
}
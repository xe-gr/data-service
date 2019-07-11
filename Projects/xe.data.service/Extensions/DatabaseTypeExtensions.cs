using System;
using xe.data.service.Models;

namespace xe.data.service.Extensions
{
    public static class DatabaseTypeExtensions
    {
        public static DatabaseType ToDatabaseType (this string databaseType)
        {
            switch (databaseType.ToLower())
            {
                case "sqlserver":
                    return DatabaseType.SqlServer;
                case "oracle":
                    return DatabaseType.Oracle;
                case "mysql":
                    return DatabaseType.MySql;
                default:
                    throw new InvalidOperationException($"Invalid database type [{databaseType}]");
            }
        }
    }
}
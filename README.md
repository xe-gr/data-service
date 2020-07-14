# data-service
The data service is a simple REST service that runs SQL statements on databases on behalf of a caller and returns the results back. It supports SQL Server, Postgres, Oracle and MySQL.

SQL statements are configured in file config.json. Here is an example:

```json
  {
    "DatabaseType": "SqlServer",
    "ConnectionString": "Server=localhost;Database=autocomplete;Uid=sa;Pwd=test",
    "SqlCommand": "SELECT TOP 10 * FROM Locations",
    "CommandTimeout": 30,
    "Name": "sample",
    "Parameters": ""
  }
```

If a caller then calls the service using [/api/data?name=sample](/api/data?name=sample), the service will connect to the SQL Server at localhost, execute the specified statement and return the results as a list of dynamic items automatically encoded to the JSON response.

Another example would be this configuration entry:
```json
  {
    "DatabaseType": "SqlServer",
    "ConnectionString": "Server=localhost;Database=autocomplete;Uid=sa;Pwd=test",
    "SqlCommand": "SELECT TOP $count$ * FROM Locations",
    "CommandTimeout": 30,
    "Name": "sample2",
    "Parameters": "count"
  }
```

This allows a caller to pass parameters. So calling the service using [/api/data?name=sample&parameters=count&values=10](/api/data?name=sample&parameters=count&values=10) would do the trick and instruct the service to prepare the SQL statement with count=10 then run the SQL statement and return the data back.

This service is best used within the confines of a secure environment and is intended to give internal services or people quick access to small queries without writing code but only via configuration.

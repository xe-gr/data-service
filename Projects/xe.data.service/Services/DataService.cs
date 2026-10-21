using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;
using xe.data.service.Exceptions;
using xe.data.service.Models;
using xe.data.service.Services.Interfaces;

namespace xe.data.service.Services
{
	public class DataService(
        IConfigurationReader configurationReader,
        IDataCreator dataCreator,
        IDataRetriever dataRetriever)
        : IDataService
    {
        public List<dynamic> ExecuteRequest(string name, string parameters, string values)
		{
			Validate(name, parameters, values,
				out var requestedParams,
				out var passedParams,
				out var passedValues,
				out var config);

			var sql = CreateSql(requestedParams, passedParams, passedValues, config);

			return Execute(config, sql);
		}

		private void Validate(string name,
			string parameters,
			string values,
			out List<string> requestedParams,
			out List<string> passedParams,
			out List<string> passedValues,
			out ConfigurationEntry config)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new BadRequestException("No config requested");
			}

			var configs = configurationReader.ReadConfiguration();
			config = configs.FirstOrDefault(x => string.Equals(x.Name.ToLower(), name.ToLower(), StringComparison.InvariantCulture));

			if (config == null)
			{
				throw new ConfigurationNotFoundException("Configuration not found");
			}

			requestedParams = Parse(config.Parameters);
			passedParams = Parse(parameters);
			passedValues = Parse(values);

			if (requestedParams.Count != passedParams.Count)
			{
				throw new BadRequestException("Parameters passed are incorrect");
			}

			if (passedParams.Count != passedValues.Count)
			{
				throw new BadRequestException("Passed values are incorrect");
			}
		}

		private static string CreateSql(List<string> requestedParams,
			IReadOnlyList<string> passedParams,
			IReadOnlyList<string> passedValues,
			ConfigurationEntry config)
		{
			var sql = config.SqlCommand;

			for (var i = 0; i < passedParams.Count; i++)
			{
				var p = passedParams[i];

				if (!requestedParams.Contains(p))
				{
					throw new BadRequestException($"Parameter {p} is unknown");

				}

				sql = sql.Replace($"${p}$", passedValues[i]);
			}

			return sql;
		}

		private List<dynamic> Execute(ConfigurationEntry config, string sql)
		{
			using (var ds = dataRetriever.RetrieveData(dataCreator, config.ConnectionString, config.DatabaseType, sql, config.CommandTimeout))
			{
				if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
				{
					return [];
				}

				var rows = new List<dynamic>();
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					var writer = new JTokenWriter();
					writer.WriteStartObject();

					foreach (DataColumn col in ds.Tables[0].Columns)
					{
						writer.WritePropertyName(col.ColumnName);
						writer.WriteValue(row[col.ColumnName]);
					}

					writer.WriteEndObject();

					rows.Add(writer.Token);
				}

				return rows;
			}
		}

		private static List<string> Parse(string commaSeparated)
		{
			return string.IsNullOrWhiteSpace(commaSeparated) ? [] : [.. commaSeparated.Split(',')];
		}
	}
}

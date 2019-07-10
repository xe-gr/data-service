using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using xe.data.service.Models;
using xe.data.service.Services.Interfaces;

namespace xe.data.service.Services
{
	public class ConfigurationReader : IConfigurationReader
	{
		public List<ConfigurationEntry> ReadConfiguration()
		{
			return JsonConvert.DeserializeObject<List<ConfigurationEntry>>(File.ReadAllText("config.json"));
		}
	}
}
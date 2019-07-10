using System.Collections.Generic;
using xe.data.service.Models;

namespace xe.data.service.Services.Interfaces
{
    public interface IConfigurationReader
    {
        List<ConfigurationEntry> ReadConfiguration();
    }
}
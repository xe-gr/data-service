using System;
using Microsoft.AspNetCore.Mvc;
using NLog;
using xe.data.service.Exceptions;
using xe.data.service.Services.Interfaces;

namespace xe.data.service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DataController(IDataService dataService) : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
		public IActionResult Get([FromQuery] string name, string parameters, string values)
		{
			Logger.Info($"Name [{name}], parameters [{parameters}], values [{values}]");

			try
			{
				return Ok(dataService.ExecuteRequest(name, parameters, values));
			}
			catch (ConfigurationNotFoundException e)
			{
				Logger.Warn(e.Message);
				return NotFound(e.Message);
			}
			catch (BadRequestException e)
			{
				Logger.Warn(e.Message);
				return BadRequest(e.Message);
			}
			catch (Exception e)
			{
				Logger.Error(e);
				throw;
			}
		}
	}
}
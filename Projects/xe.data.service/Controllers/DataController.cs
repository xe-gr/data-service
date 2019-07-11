using System;
using Microsoft.AspNetCore.Mvc;
using NLog;
using xe.data.service.Exceptions;
using xe.data.service.Services.Interfaces;

namespace xe.data.service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DataController : ControllerBase
	{
		private readonly IDataService _dataService;
		private readonly Logger _logger;

		public DataController(IDataService dataService)
		{
			_dataService = dataService;
			_logger = LogManager.GetCurrentClassLogger();
		}

		[HttpGet]
		public IActionResult Get([FromQuery] string name, string parameters, string values)
		{
			_logger.Info($"Name [{name}], parameters [{parameters}], values [{values}]");

			try
			{
				return Ok(_dataService.ExecuteRequest(name, parameters, values));
			}
			catch (ConfigurationNotFoundException e)
			{
				_logger.Warn(e.Message);
				return NotFound(e.Message);
			}
			catch (BadRequestException e)
			{
				_logger.Warn(e.Message);
				return BadRequest(e.Message);
			}
			catch (Exception e)
			{
				_logger.Error(e);
				throw;
			}
		}
	}
}
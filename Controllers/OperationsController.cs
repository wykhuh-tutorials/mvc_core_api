﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class OperationsController : Controller
    {
        private ILogger<OperationsController> _logger;
        private IConfigurationRoot _config;

        public OperationsController(ILogger<OperationsController> logger, IConfigurationRoot config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpOptions("reloadConfig")]
        public IActionResult ReloadingConfiguration()
        {
            try
            {
                _config.Reload();

                return Ok("Configureation reloaded.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"OperationsController exception: {ex}");
            }

            return BadRequest("Could not reload configuration");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCamp.Controllers
{
    // base route for all the actions in the controler
    [Route("api/[controller]")]
    public class CampsController : Controller
    {
        [HttpGet("")]
        // use IActionResult to return status code with the data 
        public IActionResult Get()
        {
            return Ok(new { Name = "Jane", Country = "England" });
        }
    }
}

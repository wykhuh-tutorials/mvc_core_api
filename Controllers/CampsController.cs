using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data;
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
        private ICampRepository _repo;

        // constructor inject to access respository
        public CampsController(ICampRepository repo)
        {
            // save copy of passed-in repo
            _repo = repo;
        }
        [HttpGet("")]
        // use IActionResult to return status code with the data 
        public IActionResult Get()
        {
            var camps = _repo.GetAllCamps();

            return Ok(camps);
        }
    }
}

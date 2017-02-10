using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
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

        [HttpGet("{id}")]
        // MVC assumes any pass in parameter listed in the url are query string parameters
        public IActionResult Get(int id, bool includeSpeakers = false)
        {
            try
            {
                Camp camp = null;

                if (includeSpeakers) camp = _repo.GetCampWithSpeakers(id);
                else camp = _repo.GetCamp(id);

                if (camp == null) return NotFound($"Camp {id} not found.");

                return Ok(camp);
            }
            catch
            {
                return BadRequest();
            }

        }
    }
}

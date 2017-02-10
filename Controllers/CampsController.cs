using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private ILogger _logger;

        // constructor inject to access dependencies
        public CampsController(ICampRepository repo, ILogger<CampsController> logger)
        {
            // save copy of passed-in repo
            _repo = repo;
            _logger = logger;
        }
        [HttpGet("")]
        // use IActionResult to return status code with the data 
        public IActionResult Get()
        {
            var camps = _repo.GetAllCamps();

            return Ok(camps);
        }

        [HttpGet("{id}", Name ="CampGet")]
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

        [HttpPost("")]
        // use [FromBody] if incoming data is json
        // make request async using async Task<> ... await
        public async Task<IActionResult> Post([FromBody]Camp model)
        {
            try
            {
                _logger.LogInformation("Creating new camp.");

                // save model to server;
                // the return model will have server generated data such as id
                _repo.Add(model);
                if (await _repo.SaveAllAsync())
                {
                    // pass in id to Url.Link via anonymous object
                    var newUri = Url.Link("CampGet", new { id = model.Id });

                    // use Created status for Post.
                    // Created needs new uri and record created.
                    // model will contain server generated data.
                    return Created(newUri, model);
                }
                else
                {
                    _logger.LogWarning("Could not save camp.");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Save camp exception: {ex}");
            }
            return BadRequest();
        }
    }
}

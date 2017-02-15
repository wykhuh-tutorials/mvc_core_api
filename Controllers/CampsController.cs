using AutoMapper;
using CodeCamp.Filters;
using CodeCamp.Models;
using Microsoft.AspNetCore.Cors;
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
    [EnableCors("anyGet")]
    [ValidateModel]
    public class CampsController : BaseController
    {
        private ICampRepository _repo;
        private ILogger _logger;
        private IMapper _mapper;

        // constructor inject to access dependencies
        public CampsController(ICampRepository repo, 
            ILogger<CampsController> logger,
            IMapper mapper)
        {
            // save copy of passed-in repo
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet("")]
        // use IActionResult to return status code with the data 
        public IActionResult Get()
        {
            var camps = _repo.GetAllCamps();

            return Ok(_mapper.Map<IEnumerable<CampModel>>(camps));
        }

        [HttpGet("{moniker}", Name ="CampGet")]
        // MVC assumes any pass in parameter listed in the url are query string parameters
        public IActionResult Get(string moniker, bool includeSpeakers = false)
        {
            try
            {
                Camp camp = null;

                if (includeSpeakers) camp = _repo.GetCampByMonikerWithSpeakers(moniker);
                else camp = _repo.GetCampByMoniker(moniker);

                if (camp == null) return NotFound($"Camp {moniker} not found.");

                // opt.Items passes a collection into the resolver.
                // we are passing down UrlHelper to the resolver
                return Ok(_mapper.Map<CampModel>(camp));
            }
            catch(Exception ex)
            {
                _logger.LogError($"get camp exception: {ex}");
            }
            return BadRequest();

        }

        [HttpPost("")]
        [EnableCors("demo")]
        // use [FromBody] if incoming data is json
        // make request async using async Task<> ... await
        public async Task<IActionResult> Post([FromBody]CampModel model)
        {
            try
            {
                _logger.LogInformation("Creating new camp.");

                // we are getting the model from request.body. need to convert model into entity.
                var camp = _mapper.Map<Camp>(model);

                // save camp entity to server;
                // the returned entity will have server generated data such as id
                _repo.Add(camp);
                if (await _repo.SaveAllAsync())
                {
                    // pass in id to Url.Link via anonymous object
                    var newUri = Url.Link("CampGet", new { moniker = camp.Moniker });

                    // use Created status for Post.
                    // Created needs new uri and record created.
                    // use mapper to turn entity into a model that has server generated data
                    return Created(newUri, _mapper.Map<CampModel>(camp));
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

        // accept both PUT and PATCH
        [HttpPut("{moniker}")]
        [HttpPatch("{moniker}")]
        public async Task<IActionResult> Put(string moniker, [FromBody] CampModel model)
        {
            try
            {
                var oldCamp = _repo.GetCampByMoniker(moniker);
                if (oldCamp == null) return NotFound($"Could not find camp moniker {moniker}");

                // modify the oldCamp
                // take the request.Body model, and override fields in oldCamp
                _mapper.Map(model, oldCamp);

                if (await _repo.SaveAllAsync())
                {
                    // oldCamp has been updated with changes
                    return Ok(_mapper.Map<CampModel>(oldCamp));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"update camp exception: {ex}");

            }
            return BadRequest("something blew up");

        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = _repo.GetCampByMoniker(moniker);
                if (oldCamp == null) return NotFound($"Could not find camp moniker {moniker}");

                // pass in whole camp instead of just id so we can examine the camp before deleting
                _repo.Delete(oldCamp);

                if (await _repo.SaveAllAsync())
                {
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"delete camp exception: {ex}");
            }
            return BadRequest();
        }

    }
}

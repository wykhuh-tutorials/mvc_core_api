using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCodeCamp.Data;
using Microsoft.AspNetCore.Mvc;
using CodeCamp.Models;
using MyCodeCamp.Data.Entities;

namespace CodeCamp.Controllers
{
    [Route("api/camps/{moniker}/speakers")]
    public class SpeakersController : BaseController
    {
        private ICampRepository _repository;
        private ILogger<SpeakersController> _logger;
        private IMapper _mapper;

        public SpeakersController(MyCodeCamp.Data.ICampRepository repository,
            ILogger<SpeakersController> logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(string moniker)
        {
            var speakers = _repository.GetSpeakersByMoniker(moniker);
            return Ok(_mapper.Map<IEnumerable<SpeakerModel>>(speakers));
        }

        [HttpGet("{id}", Name = "SpeakerGet")]
        public IActionResult Get(string moniker, int id)
        {
            var speaker = _repository.GetSpeaker(id);
            if (speaker == null) return NotFound();
            if (speaker.Camp.Moniker != moniker) return BadRequest("Speaker not in specified camp");

            return Ok(_mapper.Map<SpeakerModel>(speaker));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string moniker, [FromBody] SpeakerModel model)
        {
            try
            {
                var camp = _repository.GetCampByMoniker(moniker);
                if (camp == null) return BadRequest($"could not find camp moniker {moniker}");

                var speaker = _mapper.Map<Speaker>(model);
                speaker.Camp = camp;

                _repository.Add(speaker);

                if(await _repository.SaveAllAsync())
                {
                    var url = Url.Link("SpeakerGet", new { moniker = camp.Moniker, id = speaker.Id });
                    return Created(url, _mapper.Map<SpeakerModel>(speaker));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"create speaker exception: {ex}");
            }
            return BadRequest("create speaker error");
        }
    }
}

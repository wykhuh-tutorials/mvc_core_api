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
        private ICampRepository _respository;
        private ILogger<SpeakersController> _logger;
        private IMapper _mapper;

        public SpeakersController(MyCodeCamp.Data.ICampRepository repository,
            ILogger<SpeakersController> logger,
            IMapper mapper)
        {
            _respository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(string moniker)
        {
            var speakers = _respository.GetSpeakersByMoniker(moniker);
            return Ok(_mapper.Map<IEnumerable<SpeakerModel>>(speakers));
        }

        [HttpGet("{id}", Name = "SpeakerGet")]
        public IActionResult Get(string moniker, int id)
        {
            var speaker = _respository.GetSpeaker(id);
            if (speaker == null) return NotFound();
            if (speaker.Camp.Moniker != moniker) return BadRequest("Speaker not in specified camp");

            return Ok(_mapper.Map<SpeakerModel>(speaker));
        }
    }
}

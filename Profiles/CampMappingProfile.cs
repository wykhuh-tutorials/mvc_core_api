using AutoMapper;
using CodeCamp.Models;
using MyCodeCamp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCamp.Profiles
{
    // Profile class and CreateMap() comes from automapper
    public class CampMappingProfile : Profile
    {
        public CampMappingProfile()
        {
            // map Camp entity to CampModel
            CreateMap<Camp, CampModel>();
        }
    }
}

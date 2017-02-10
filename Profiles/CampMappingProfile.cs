using AutoMapper;
using CodeCamp.Models;
using Microsoft.AspNetCore.Mvc;
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
            CreateMap<Camp, CampModel>()
                // first lamda: new field to add to model
                // second lambda: how to calculate new field
                .ForMember(c => c.StartDate,
                    // MapFrom allows us to map from another entity field
                    opt => opt.MapFrom(camp => camp.EventDate))
                .ForMember(c => c.EndDate,
                    // ResolveUsing allows us to calculate the model field
                    opt => opt.ResolveUsing(camp => camp.EventDate.AddDays(camp.Length - 1)))
                 .ForMember(c => c.Url,
                    // instantiate CampUrlResolver using DI.
                    opt => opt.ResolveUsing<CampUrlResolver>())
                 // convert CampModel to Camp entity
                 .ReverseMap()
                 .ForMember(m => m.EventDate,
                    opt => opt.MapFrom(model => model.StartDate))
                 .ForMember(m => m.Length,
                    opt => opt.ResolveUsing(model => (model.EndDate - model.StartDate).Days + 1))
                 // convert flatten location to nested Location;
                 .ForMember(m => m.Location,
                        opt => opt.ResolveUsing(c => new Location()
                        {
                            Address1 = c.LocationAddress1,
                            Address2 = c.LocationAddress2,
                            Address3 = c.LocationAddress3,
                            CityTown = c.LocationCityTown,
                            StateProvince = c.LocationStateProvince,
                            PostalCode = c.LocationPostalCode,
                            Country = c.LocationCountry
                        }));

            CreateMap<Speaker, SpeakerModel>()
                .ForMember(c => c.Url,
                    opt => opt.ResolveUsing<SpeakerUrlResolver>())
                .ReverseMap();
        }
    }
}

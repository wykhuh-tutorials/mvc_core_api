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
                    // source, destination, unused, resolution context
                    opt => opt.ResolveUsing((camp, model, unused, ctx) =>
                    {
                        // we can get Items that we passed in from controller using ctx.Items.
                        // we are casting UrlHelper to IUrlHelper type
                        var url = (IUrlHelper)ctx.Items["UrlHelper"];
                        return url.Link("CampGet", new { id = camp.Id });
                    }));
        }
    }
}

﻿using AutoMapper;
using CodeCamp.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCamp.Models
{
    // source, destination, type of return value
    public class CampUrlResolver : IValueResolver<Camp, CampModel, string>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public CampUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(Camp source, CampModel destination, string destMember, ResolutionContext context)
        {
            var url = (IUrlHelper)_httpContextAccessor.HttpContext.Items[BaseController.URLHELPER];
            var b =  url.Link("CampGet", new { id = source.Id });
            return b;
        }
    }
}

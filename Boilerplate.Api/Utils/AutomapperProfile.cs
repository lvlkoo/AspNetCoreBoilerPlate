using System;
using System.Collections.Generic;
using AutoMapper;
using Boilerplate.DAL.Entities;
using Boilerplate.Models;

namespace Boilerplate.Api.Utils
{
    public class AutomapperProfile: Profile
    {
        public AutomapperProfile()
        {
            CreateMap<ApplicationRole, RoleModel>()
                .ForMember(dist => dist.Permissions, opt => opt.MapFrom(r => new List<string>(r.Permissions.Split(",", StringSplitOptions.None))));
        }
    }
}

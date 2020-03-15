using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Boilerplate.Commons.Exceptions;
using Boilerplate.EF;
using Boilerplate.Entities;
using Boilerplate.Models;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Services.Implementations
{
    public class DefaultRolesService : BaseDataService<ApplicationRole, RoleModel>, IRolesService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IPermissionsService _permissionsService;

        public DefaultRolesService(ApplicationDbContext context, IMapper mapper,
            RoleManager<ApplicationRole> roleManager, IPermissionsService permissionsService) : base(context, mapper)
        {
            _roleManager = roleManager;
            _permissionsService = permissionsService;
        }

        public async Task<IEnumerable<string>> GetPermissions()
        {
            return _permissionsService.GetAllPermissions();
        }

        public override async Task<IEnumerable<RoleModel>> Get()
        {
            return MapList<RoleModel>(await _roleManager.Roles.ToListAsync());
        }

        public override async Task<RoleModel> Get(Guid id)
        {
            var item = await _roleManager.Roles.FirstOrDefaultAsync(_ => _.Id == id);
            if (item == null)
                throw new EntityNotFoundException();

            return new RoleModel
            {
                Id = item.Id,
                Name = item.Name,
                Permissions = item.Permissions.Split(",").ToList()
            };
        }

        public override async Task<RoleModel> Create(RoleModel model)
        {
            var newRole = new ApplicationRole
            {
                Name = model.Name,
                Permissions = string.Join(',', model.Permissions)
            };

            await _roleManager.CreateAsync(newRole);
            return await Get(newRole.Id);
        }

        public override async Task<RoleModel> Update(Guid id, RoleModel model)
        {
            var item = await _roleManager.Roles.FirstOrDefaultAsync(_ => _.Id == model.Id);
            if (item == null)
                throw new EntityNotFoundException();

            var newRole = new ApplicationRole
            {
                Name = model.Name,
                Permissions = string.Join(',', model.Permissions)
            };

            await _roleManager.CreateAsync(newRole);
            return await Get(newRole.Id);
        }

        public override async Task Delete(Guid id)
        {
            var item = await _roleManager.Roles.FirstOrDefaultAsync(_ => _.Id == id);
            if (item == null)
                throw new EntityNotFoundException();

            await _roleManager.DeleteAsync(item);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Boilerplate.Models;

namespace Boilerplate.Services.Abstractions
{
    public interface IRolesService: ICrudService<RoleModel>
    {
        Task<IEnumerable<string>> GetPermissions();
    }
}

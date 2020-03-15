using System.Collections.Generic;

namespace Boilerplate.Services.Abstractions
{
    public interface IPermissionsService
    {
        IEnumerable<string> GetAllPermissions();
    }
}

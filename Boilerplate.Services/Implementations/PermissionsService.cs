using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Boilerplate.Commons.Attributes;
using Boilerplate.Commons.Static;
using Boilerplate.Services.Abstractions;

namespace Boilerplate.Services.Implementations
{
    public class PermissionsService: IPermissionsService
    {
        private static List<string> _loadedPermissions;

        public IEnumerable<string> GetAllPermissions()
        {
            if (_loadedPermissions == null)
            {
                _loadedPermissions = new List<string>();

                var permissions = typeof(Permissions).GetFields().Select(f => f.GetRawConstantValue().ToString()).ToList();
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(t => t.GetTypes())
                    .Where(t => t.IsClass && t.GetCustomAttributes<ValidatesPermissionsAttribute>().Any())
                    .ToList();

                foreach (var type in types)
                {
                    var rootName = type.Name.Split(new[] { "Controller" }, StringSplitOptions.RemoveEmptyEntries).First();

                    var attribute =
                        type.GetCustomAttribute(typeof(ValidatesPermissionsAttribute)) as ValidatesPermissionsAttribute;
                    if (!string.IsNullOrEmpty(attribute.RootName))
                        rootName = attribute.RootName;

                    _loadedPermissions.AddRange(permissions.Select(p => $"{rootName}/{p}"));
                }
            }

            return _loadedPermissions;
        }
    }
}

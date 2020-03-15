using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Boilerplate.Commons.Attributes;
using Boilerplate.Commons.Static;
using Boilerplate.Services.Abstractions;

namespace Boilerplate.IntegrationTests.Mocks
{
    public class MockedPermissionsService: IPermissionsService
    {
        public IEnumerable<string> GetAllPermissions()
        {
            var loadedPermissions = new List<string>();

            var permissions = typeof(Permissions).GetFields().Select(f => f.GetRawConstantValue().ToString()).ToList();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.Contains("Api"))
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

                loadedPermissions.AddRange(permissions.Select(p => $"{rootName}/{p}"));
            }

            return loadedPermissions;
        }
    }
}

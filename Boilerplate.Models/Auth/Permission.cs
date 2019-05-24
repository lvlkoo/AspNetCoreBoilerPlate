using System.Collections.Generic;
using System.Linq;

namespace Boilerplate.Models.Auth
{
    public static class Permission
    {
        public static IEnumerable<string> GetAllPermissions() =>
            typeof(Permission).GetNestedTypes()
                .SelectMany(t => t.GetFields().Select(f => f.GetRawConstantValue().ToString()));

        public static class UserPremission
        {
            public const string View = "user/view";
            public const string Edit = "user/edit";
            public const string Delete = "user/delete";
        }
    }
}

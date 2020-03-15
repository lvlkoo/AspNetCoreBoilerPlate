using System;

namespace Boilerplate.Commons.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidatesPermissionsAttribute: Attribute
    {
        public string RootName { get; set; }
    }
}

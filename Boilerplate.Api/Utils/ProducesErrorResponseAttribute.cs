using Boilerplate.Models;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Api.Utils
{
    public class ProducesErrorResponseAttribute : ProducesResponseTypeAttribute
    {
        public ProducesErrorResponseAttribute(int statusCode) : base(typeof(BaseResponse), statusCode)
        {
        }
    }
}

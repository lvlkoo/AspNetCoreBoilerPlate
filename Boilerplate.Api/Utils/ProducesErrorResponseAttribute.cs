using Boilerplate.Models;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Api.Utils
{
    /// <summary>
    /// Adds error responses metadata
    /// </summary>
    public class ProducesErrorResponseAttribute : ProducesResponseTypeAttribute
    {
        /// <inheritdoc />
        public ProducesErrorResponseAttribute(int statusCode) : base(typeof(BaseResponse), statusCode)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

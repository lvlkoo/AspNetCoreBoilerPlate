using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Boilerplate.Commons.Exceptions
{
    public class BadRequestException: BaseApiException
    {
        public BadRequestException() : base(400, "Bad request data")
        {
        }

        public BadRequestException(string message) : base(400, message)
        {
        }

        public BadRequestException(string message, IEnumerable<IdentityError> errors) : base(400,
            $"{message}. {string.Join(",", errors.Select(_ => $"{_.Code}: {_.Description}"))}")
        {
        }
    }
}

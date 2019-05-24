namespace Boilerplate.Models.Exceptions
{
    public class ForbiddenException : BaseApiException
    {
        public ForbiddenException(string message) : base(401, message)
        {
        }

        public ForbiddenException() : base(403, "You don't have access to this action")
        {
        }
    }
}

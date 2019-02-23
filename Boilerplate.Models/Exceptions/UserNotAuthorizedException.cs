namespace Boilerplate.Models.Exceptions
{
    public class UserNotAuthorizedException: BaseApiException
    {
        public UserNotAuthorizedException(string message) : base(401, message)
        {
        }

        public UserNotAuthorizedException() : base(401, "User not authorized")
        {
        }
    }
}

namespace Boilerplate.Models.Exceptions
{
    public class BadRequestException: BaseApiException
    {
        public BadRequestException(string message) : base(400, message)
        {
        }
    }
}

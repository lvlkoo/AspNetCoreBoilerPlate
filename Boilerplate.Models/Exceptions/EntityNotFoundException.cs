namespace Boilerplate.Models.Exceptions
{
    public class EntityNotFoundException: BaseApiException
    {
        public EntityNotFoundException() : base(404, "Entity with specified id not found")
        {
        }

        public EntityNotFoundException(string message) : base(404, message)
        {
        }
    }
}

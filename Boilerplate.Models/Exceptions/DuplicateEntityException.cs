namespace Boilerplate.Models.Exceptions
{
    public class DuplicateEntityException : BaseApiException
    {
        public DuplicateEntityException() : base(422, "Entity with specified unique fields is already exist")
        {
        }

        public DuplicateEntityException(params string[] fields) : base(422, $"Entity with specified {string.Join(",", fields)} is already exist")
        {
        }

        public DuplicateEntityException(string message) : base(422, message)
        {
        }
    }
}

using System;

namespace Boilerplate.Commons.Exceptions
{
    public class BaseApiException: Exception
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public BaseApiException(int statucCode, string message)
        {
            StatusCode = statucCode;
            StatusMessage = message;
        }
    }
}

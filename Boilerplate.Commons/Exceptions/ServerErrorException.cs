﻿namespace Boilerplate.Commons.Exceptions
{
    public class ServerErrorException: BaseApiException
    {
        public ServerErrorException(string message) : base(500, message)
        {
        }
    }
}

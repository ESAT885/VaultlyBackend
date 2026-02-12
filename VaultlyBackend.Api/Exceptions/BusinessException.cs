namespace VaultlyBackend.Api.Exceptions
{
    public class BusinessException : AppException
    {
        public BusinessException(string publicMessage, string logMessage) :
            base(publicMessage, logMessage, StatusCodes.Status400BadRequest)
        { }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string publicMessage, string logMessage)
            : base(publicMessage, logMessage, StatusCodes.Status404NotFound) { }
    }

    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string publicMessage, string logMessage)
            : base(publicMessage, logMessage, StatusCodes.Status401Unauthorized) { }
    }
}

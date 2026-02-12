namespace VaultlyBackend.Api.Exceptions
{
    public class AppException : Exception
    {
        private int status400BadRequest;

        public int StatusCode { get; }
        public string PublicMessage { get; }
        protected AppException(
            string publicMessage,
            string logMessage,
            int statusCode)
            : base(logMessage ?? publicMessage)
        {
            StatusCode = statusCode;
            PublicMessage = publicMessage;
        }


    }
}

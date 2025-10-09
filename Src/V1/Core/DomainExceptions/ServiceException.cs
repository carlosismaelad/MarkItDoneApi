
namespace MarkItDoneApi.Src.V1.Core.DomainExceptions
{
    public class ServiceException : Exception
    {
        public string Action { get; set; }
        public int StatusCode { get; set; }
        
        public ServiceException(string message) : this(message, null, 400, null)
        {
        }
        
        public ServiceException(string? message = null, string? action = null, int statusCode = 400, Exception? innerException = null) : base(message ?? "Um erro de validação ocorreu.", innerException)
        {
            Action = action ?? "Serviço indisponível no momento. Tente mais tarde.";
            StatusCode = statusCode;
        }

        public object ToJson()
        {
            return new
            {
                name = nameof(ServiceException),
                message = Message,
                action = Action,
                status_code = StatusCode
            };
        }
    }
}
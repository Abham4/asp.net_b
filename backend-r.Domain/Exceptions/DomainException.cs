using System.Collections;

namespace backend_r.Domain.Exceptions
{
    public class DomainException : Exception
    {
        private readonly string _message;
        
        public DomainException() {}
        
        public DomainException(string message, Exception exception)
        {
            throw new Exception(message, exception);
        }

        public DomainException(string message)
        {
            _message = message;
        }

        public override string Message => _message;
    }
}
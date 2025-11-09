using System;

namespace CitasMedicas.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public int StatusCode { get; set; } = 500;
        public string ErrorCode { get; set; }

        public BusinessException() { }

        public BusinessException(string message) : base(message) { }

        public BusinessException(string message, Exception innerException) : base(message, innerException) { }

        public BusinessException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public BusinessException(string message, string errorCode, int statusCode) : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}

using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CitasMedicas.Infrastructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var path = context.HttpContext.Request.Path;

            var statusCode = GetStatusCode(exception);
            var errorResponse = CreateErrorResponse(exception, path, statusCode);

            LogException(exception, statusCode, path);

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }

        private int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                BusinessException businessEx => businessEx.StatusCode,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ArgumentException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                NotImplementedException => StatusCodes.Status501NotImplemented,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private ErrorResponse CreateErrorResponse(Exception exception, string path, int statusCode)
        {
            var response = new ErrorResponse
            {
                Type = exception.GetType().Name,
                Message = GetFriendlyMessage(exception, statusCode),
                Path = path
            };

            if (exception is BusinessException businessEx && !string.IsNullOrEmpty(businessEx.ErrorCode))
                response.ErrorCode = businessEx.ErrorCode;

            return response;
        }

        private string GetFriendlyMessage(Exception exception, int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest => "Solicitud incorrecta. Verifique los datos enviados.",
                StatusCodes.Status401Unauthorized => "No autorizado. Debe autenticarse.",
                StatusCodes.Status403Forbidden => "Acceso denegado.",
                StatusCodes.Status404NotFound => "Recurso no encontrado.",
                StatusCodes.Status500InternalServerError => "Error interno del servidor.",
                _ => exception.Message
            };
        }

        private void LogException(Exception exception, int statusCode, string path)
        {
            var message = $"Error {statusCode} en {path}: {exception.Message}";
            if (statusCode >= 500)
                _logger.LogError(exception, message);
            else
                _logger.LogWarning(exception, message);
        }
    }
}

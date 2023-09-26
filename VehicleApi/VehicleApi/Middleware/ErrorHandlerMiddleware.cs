using System.Net;
using System.Text.Json;
using Vehicle.Aplication.CustomExceptions;

namespace VehicleApi.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private List<string> _errors;

        /// <summary>
        /// RequestDelegate: A delegate type method which handles the request.  
        ///                  Represents any Task returning method, which has a single parameter of type HttpContext.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        public ErrorHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ErrorHandlerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            const string SERVER_CAPTION = "Server Error";
            _errors = new List<string> { "UnexpectedError" };

            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                // determine what StatusCode you wish to return
                switch (error)
                {
                    case MyCustomException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest; //custom application error
                        _errors = e.Errors;
                        break;
                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound; // I want to deal with KeyNotFoundException specifically - not found error
                        break;
                    default:                         
                        response.StatusCode = (int)HttpStatusCode.InternalServerError; // catch all other errors - unhandled error
                        break;
                }

                _logger.LogError(MyLogEvents.TestItem, error, SERVER_CAPTION);

                // return error (message) to caller
                var result = JsonSerializer.Serialize(new { message = error?.Message, errors = _errors });
                await response.WriteAsync(result);
            }
        }
    }
}

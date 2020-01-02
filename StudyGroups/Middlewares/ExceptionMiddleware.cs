using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StudyGroups.Data.Repository;
using StudyGroups.WebAPI.Models;
using StudyGroups.WebAPI.Services.Exceptions;
using System;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace StudyGroups.WebAPI.WebSite.Middlewares
{
    /// <summary>
    /// Middleware class for handling Exceptions generally, and return proper HTTP responses.
    /// Registered in ApplicationExtensions during startup.
    /// With help of this class, there is no need try catch blocks in the code (just special cases).
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        /// <summary>
        /// Method what runs all of Http async Task invoke. 
        /// Wrap the call in a try catch block and call the original call.
        /// In the catch branch, there is a global logging step, and the call for handling the HTTP response returning.
        /// </summary>
        /// <param name="httpContext">Calling context object</param>
        /// <returns>Async Task</returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            // Wrap the original call into a try catch block
            try
            {
                // Call the original call.
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError($"Error on the service side - Message: {ex.Message} - Stack trace: {ex.StackTrace}");

                // Call response handling
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        /// <summary>
        /// Method for returning proper HTTP response
        /// </summary>
        /// <param name="httpContext">Calling context object</param>
        /// <param name="ex">Catched exception</param>
        /// <returns>Async Task</returns>
        private static Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            // Create Http 500 code for default
            var code = HttpStatusCode.InternalServerError;

            // If different Http error code needed, change it here
            if (ex is AuthenticationException) code = HttpStatusCode.Unauthorized;
            else if (ex is RegistrationException) code = HttpStatusCode.BadRequest;
            else if (ex is ParameterException) code = HttpStatusCode.BadRequest;
            else if (ex is NodeNotExistsException) code = HttpStatusCode.NoContent;
            // Create a Http response with the status code and the exception message
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)code;

            string message = "Internal server error, please contact administrator.";
            if ((int)code != 500)
                message = ex.Message;

            return httpContext.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = message
            }.ToString());
        }


    }
}

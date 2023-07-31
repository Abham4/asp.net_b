namespace backend_r.Api.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;
        private readonly IWebHostEnvironment _env;

        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger,IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }
        
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
            
            if (context.Exception.GetType() == typeof(DomainException))
            {
                var problem = new ValidationProblemDetails()
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to error property"
                };
                problem.Errors.Add("DomainExceptions", new string[] { context.Exception.Message });
                problem.Errors.Add("StackTrace", new string[] { context.Exception.StackTrace });
                context.Result = new BadRequestObjectResult(problem);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            else if (context.Exception.GetType() == typeof(ValidationException))
            {
                var problem = new ValidationProblemDetails()
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to error property"
                };
                ValidationException ex = context.Exception as ValidationException;
                problem.Errors.Add("ValidationException", new string[] { ex.Message });
                context.Result = new BadRequestObjectResult(problem);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            else
            {
                var json = new JsonErrorResponse { Messages = new[] { "An error occured. Try again" } };
                if (_env.IsDevelopment() || _env.IsProduction())
                {
                    json.DevelopMessage = context.Exception;
                }
                var objResult = new ObjectResult(json);
                objResult.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = objResult;

                context.HttpContext.Response.StatusCode = (int)(HttpStatusCode.BadRequest);
            }
            context.ExceptionHandled = true;
        }

        private class JsonErrorResponse 
        {
            public string[] Messages { get; set; }
            public object DevelopMessage { get; set; }
        }
    }
}
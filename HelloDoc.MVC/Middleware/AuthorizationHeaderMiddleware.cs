namespace HelloDoc.Middleware
{
    public class AuthorizationHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Session.GetString("token") != null)
            {
                string token = httpContext.Session.GetString("token");
                httpContext.Request.Headers["Authorization"] = "Bearer " + token;
            }
            await _next(httpContext);
        }
    }
}

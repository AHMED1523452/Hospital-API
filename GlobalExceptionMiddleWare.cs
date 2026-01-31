namespace HealthSync.MiddleWare
{
    public class GlobalExceptionMiddleWare
    {
        public GlobalExceptionMiddleWare(RequestDelegate _next)
        {
            Next = _next;
        }

        public RequestDelegate Next { get; }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //. make the request can arrive to the server or the controller or the action
                await Next(context);
            }
            catch(Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = ex.Message
                });
            }
        }
    }
}

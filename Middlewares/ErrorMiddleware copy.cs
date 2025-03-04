using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
namespace webApiProject.Middlewares;

public class ErrorMiddleware1
{
    private RequestDelegate next;

    public ErrorMiddleware1(RequestDelegate next)
    {
        this.next = next;
    }
    public async Task Invoke(HttpContext c)
    {
        c.Items["success"] = false;
        try
        {
            await next(c);
            c.Items["success"] = true;
        }
        catch (ApplicationException ex)
        {
            c.Response.StatusCode = 400;
            await c.Response.WriteAsync(ex.Message);
        }
        catch (Exception e)
        {
            c.Response.StatusCode = 500;
            await c.Response.WriteAsync($"{e}\n פנה לתמיכה תכנית");
        }
    }
   
}
public static class MiddlewareExtensions1
{
    public static WebApplication UseErrorMiddleware1(this WebApplication app)
    {
        app.UseMiddleware<ErrorMiddleware>();
        return app;
    }
}



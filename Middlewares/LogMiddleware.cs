using System.Diagnostics;
using System.Net.Mail;

namespace webApiProject.Middlewares;

public class LogMiddleware
{

    private RequestDelegate next;

    public LogMiddleware(RequestDelegate next)
    {
        this.next = next;
    }
    public async Task Invoke(HttpContext c)
    {
        await c.Response.WriteAsync($"in Log Middleware- strat\n");
        var timer = new Stopwatch();
        timer.Start();
        await next(c);
        Console.WriteLine($"{c.Request.Path}.{c.Request.Method} took {timer.ElapsedMilliseconds} ms."
            + $" Success: {c.Items["success"]}"
            + $" User: {c.User?.FindFirst("userId")?.Value ?? "unknown"}");
        await c.Response.WriteAsync("in Log Middleware- end\n");
    }

}
public static class LogMiddlewareHelper
{
    public static void UseLog(this IApplicationBuilder a)
    {
        a.UseMiddleware<LogMiddleware>();
    }
}
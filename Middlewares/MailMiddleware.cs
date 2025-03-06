using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace webApiProject.Middlewares;

public class MailMiddleware
{
    private readonly RequestDelegate next;

    public MailMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext c)
    {
        try
        {
            Console.WriteLine("before mail");
            SendMail();
            Console.WriteLine("after mail");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error sending mail: {ex.Message}");
            Console.WriteLine($"Error sending mail: {ex.Message}");
        }


        await next(c);
    }

    private void SendMail()
    {
        Console.WriteLine("in sentMail");
        var fromAddress = new MailAddress("a0548436799@gmail.com", "my mother");
        var toAddress = new MailAddress("g0583247266@gmail.com", "gila");
        const string fromPassword = "089742178";
        const string subject = "hiii";
        const string body = "whathap?";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        };
        smtp.Send(message);
    }
}

public static class MailMiddlewareHelper
{
    public static void UseMailMiddleware(this IApplicationBuilder a)
    {
        a.UseMiddleware<MailMiddleware>();
    }
}

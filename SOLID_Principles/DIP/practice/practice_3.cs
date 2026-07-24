using System;

public interface ISms
{
    void SendSmsText(string phone, string body);
}

public interface IAlert
{
    void DispatchAlert(string userId, string contactInfo, string message);
}
public class SmsGateway : ISms
{
    public void SendSmsText(string phone, string body)
    {
        Console.WriteLine($"Sending SMS to {phone}: {body}");
    }
}

public class NotificationDispatcher : IAlert
{
    private readonly ISms _smsGateway;

    public NotificationDispatcher(ISms sms)
    {
        _smsGateway = sms;
    }

    public void DispatchAlert(string userId, string contactInfo, string message)
    {
        Console.WriteLine($"Preparing system alert for user {userId}...");
        _smsGateway.SendSmsText(contactInfo, message);
    }
}

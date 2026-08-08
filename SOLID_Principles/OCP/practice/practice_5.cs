using System;

public class SystemAlert
{
    public string? Message { get; set; }
    public string? Severity { get; set; }
}

abstract class NotificationManager
{
    public abstract void SendAlertNotification(SystemAlert alert);
}
class Email : NotificationManager
{
    public override void SendAlertNotification(SystemAlert alert)
    {
        Console.WriteLine($"SMTP: Dispatching email alert -> {alert.Message}");
    }
}
class Sms : NotificationManager
{
    public override void SendAlertNotification(SystemAlert alert)
    {
        Console.WriteLine($"SMS Gateway: Sending text alert -> {alert.Message}");
    }
}
class PagerDuty : NotificationManager
{
    public override void SendAlertNotification(SystemAlert alert)
    {
        Console.WriteLine($"PagerDuty API: Triggering incident response -> {alert.Message}");
    }
}
public class AlertNotificationManager
{
    public void SendAlertNotification(SystemAlert alert, string channelType)
    {
        if (channelType == "Email")
        {
            Console.WriteLine($"SMTP: Dispatching email alert -> {alert.Message}");
        }
        else if (channelType == "Sms")
        {
            Console.WriteLine($"SMS Gateway: Sending text alert -> {alert.Message}");
        }
        else if (channelType == "PagerDuty")
        {
            Console.WriteLine($"PagerDuty API: Triggering incident response -> {alert.Message}");
        }
    }
}

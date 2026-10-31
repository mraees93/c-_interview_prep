using System;
using System.Collections.Generic;

/*
using System;
using System.Collections.Generic;

public class NoticeEnvelope { public string CaseId { get; set; } = string.Empty; public string AlertText { get; set; } = string.Empty; }

public class SendGridEmailGateway
{
    public void TransmitEmail(string caseId, string body) => Console.WriteLine($"[SENDGRID] Dispatched email package for Case {caseId}.");
}

public class NoticeDistributionEngine
{
    private readonly SendGridEmailGateway _mailGateway;

    public NoticeDistributionEngine()
    {
        _mailGateway = new SendGridEmailGateway();
    }

    public void ForwardLegalNotice(NoticeEnvelope envelope, string routingChannel)
    {
        Console.WriteLine($"Evaluating distribution topology for Case: {envelope.CaseId}...");
        
        string compiledMessage = string.Empty;

        if (routingChannel == "StandardEmail")
        {
            compiledMessage = $"[OFFICIAL DIGITAL NOTICE]\n{envelope.AlertText}";
        }
        else if (routingChannel == "MobileSms")
        {
            compiledMessage = $"[MOBILE COMPLIANCE ALERT]\n{envelope.AlertText}";
        }
        else if (routingChannel == "PhysicalSheriffDelivery")
        {
            throw new NotSupportedException("Physical sheriff deliveries require written handwritten signatures; digital routing is blocked!");
        }

        _mailGateway.TransmitEmail(envelope.CaseId, compiledMessage);
    }
}
*/

public class NoticeEnvelope { public string CaseId { get; set; } = string.Empty; public string AlertText { get; set; } = string.Empty; }

public class SendGridEmailGateway : INotificationSender
{
    public void TransmitEmail(string caseId, string body) => Console.WriteLine($"[SENDGRID] Dispatched email package for Case {caseId}.");
}

public interface INotificationSender
{
    void TransmitEmail(string caseId, string body);
}

public interface INotificationCompiler
{
    public string ForwardLegalNotice(NoticeEnvelope envelope);
}

public class StandardEmail : INotificationCompiler
{
    public string ForwardLegalNotice(NoticeEnvelope envelope)
    {
        return $"[OFFICIAL DIGITAL NOTICE]\n{envelope.AlertText}";
    }
}

public class MobileSms : INotificationCompiler
{
    public string ForwardLegalNotice(NoticeEnvelope envelope)
    {
        return $"[MOBILE COMPLIANCE ALERT]\n{envelope.AlertText}";
    }
}

public class NoticeDistributionEngine
{
    private readonly INotificationSender _notificationSender;    

    public NoticeDistributionEngine(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public void HandleLegalNotice(NoticeEnvelope envelope, INotificationCompiler notificationCompiler)
    {
        Console.WriteLine($"Evaluating distribution topology for Case: {envelope.CaseId}...");
        
        string compiledMessage = notificationCompiler.ForwardLegalNotice(envelope);

        _notificationSender.TransmitEmail(envelope.CaseId, compiledMessage);
    }
}

using System;

/*
using System;

public class NoticePayload
{
    public string CaseNumber { get; set; } = string.Empty;
    public string RecipientAddress { get; set; } = string.Empty;
}

public class LocalSmtpGateway
{
    public void DispatchEmail(string to, string body)
    {
        Console.WriteLine($"[SMTP] Notice sent directly via local mail server to: {to}");
    }
}

public class CourtNoticeEngine
{
    private readonly LocalSmtpGateway _smtpGateway;

    public CourtNoticeEngine()
    {
        _smtpGateway = new LocalSmtpGateway();
    }

    public void DistributeNotice(NoticePayload payload, string channelType)
    {
        Console.WriteLine($"Processing distribution for Case: {payload.CaseNumber}...");

        string alertMessage = $"Official Notice for Case {payload.CaseNumber}. Please review immediately.";

        if (channelType == "Email")
        {
            _smtpGateway.DispatchEmail(payload.RecipientAddress, alertMessage);
        }
        else if (channelType == "Sms")
        {
            Console.WriteLine($"[SMS] Text notice routed to mobile: {payload.RecipientAddress}");
        }
        else if (channelType == "PhysicalSheriff")
        {
            throw new NotSupportedException("Sheriff deliveries require hand-signed paper printouts; digital routing is blocked!");
        }
    }
}
*/
public class NoticePayload
{
    public string CaseNumber { get; set; } = string.Empty;
    public string RecipientAddress { get; set; } = string.Empty;
}

public class LocalSmtpGateway : INoticeSender
{
    public void DispatchEmail(string to, string body)
    {
        Console.WriteLine($"[SMTP] Notice sent directly via local mail server to: {to}");
    }
}
public interface INoticeSender
{
    void DispatchEmail(string to, string body);
}
public interface INoticeHandler
{
    void DistributeNotice(NoticePayload payload, INoticeSender noticeSender);
}

public class SmsNotice : INoticeHandler
{
    public void DistributeNotice(NoticePayload payload, INoticeSender noticeSender)
    {
        Console.WriteLine($"[SMS] Text notice routed to mobile: {payload.RecipientAddress}");
    }
}
public class EmailNotice : INoticeHandler
{
    public void DistributeNotice(NoticePayload payload, INoticeSender noticeSender)
    {
        string alertMessage = $"Official Notice for Case {payload.CaseNumber}. Please review immediately.";
        noticeSender.DispatchEmail(payload.RecipientAddress, alertMessage);
    }
}
public class CourtNoticeEngine
{
    private readonly INoticeSender _noticeSender;

    public CourtNoticeEngine(INoticeSender noticeSender)
    {
        _noticeSender = noticeSender;
    }

    public void HandleNotice(NoticePayload payload, INoticeHandler noticeHandler)
    {
        Console.WriteLine($"Processing distribution for Case: {payload.CaseNumber}...");

        noticeHandler.DistributeNotice(payload, _noticeSender);
    }
}

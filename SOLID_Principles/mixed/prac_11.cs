/*
using System;

public class SecurityPayload
{
    public string UserIp { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
}

public class LocalDataDogTelemetry
{
    public void PushSecurityLog(string accountId, string payload)
    {
        Console.WriteLine($"[TELEMETRY] Forwarded security payload hash for account {accountId} to monitoring stack.");
    }
}

public class UserAccessTrackingEngine
{
    private readonly LocalDataDogTelemetry _telemetry;

    public UserAccessTrackingEngine()
    {
        _telemetry = new LocalDataDogTelemetry();
    }

    public void ForwardSecurityAudit(SecurityPayload payload, string complianceModel)
    {
        Console.WriteLine($"Registering active auditing signature for Account: {payload.AccountId}...");

        string auditRecord = string.Empty;

        if (complianceModel == "StandardEmployeeAccess")
        {
            string systemStamp = $"[INTERNAL ACCESS VERIFICATION - TRACED AT {DateTime.UtcNow}]";
            auditRecord = $"{systemStamp}\nOrigin IP Reference Address: {payload.UserIp.Trim()}";
        }
        else if (complianceModel == "ThirdPartyVendorAccess")
        {
            string systemStamp = $"[EXTERNAL PARTNER ACCESS VERIFICATION - TRACED AT {DateTime.UtcNow}]";
            auditRecord = $"{systemStamp}\nOrigin IP Reference Address: {payload.UserIp.Trim()}";
        }
        else if (complianceModel == "SystemKernelBypass")
        {
            throw new NotSupportedException("Automated system-level kernel executions are stateless; writing to telemetry streams is blocked!");
        }

        _telemetry.PushSecurityLog(payload.AccountId, auditRecord);
    }
}
*/

public class SecurityPayload
{
    public string UserIp { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
}

public class LocalDataDogTelemetry : ITelemetry
{
    public void PushSecurityLog(string accountId, string payload)
    {
        Console.WriteLine($"[TELEMETRY] Forwarded security payload hash for account {accountId} to monitoring stack.");
    }
}

public interface ITelemetry
{
    void PushSecurityLog(string accountId, string payload);
}

public abstract class Security
{
    protected static readonly string StandardEmployeeSystemStamp = $"[INTERNAL ACCESS VERIFICATION - TRACED AT {DateTime.UtcNow}]";
    protected static readonly string ThirdPartyVendorSystemStamp = $"[EXTERNAL PARTNER ACCESS VERIFICATION - TRACED AT {DateTime.UtcNow}]";
    protected abstract string SystemStamp { get; }
    public string GetAuditRecord(SecurityPayload securityPayload)
    {
        return $"{SystemStamp}\nOrigin IP Reference Address: {securityPayload.UserIp.Trim()}";
    }
    public abstract string ForwardSecurityAudit(SecurityPayload securityPayload);
}

public class ThirdPartyVendorAccess : Security
{
    protected override string SystemStamp => ThirdPartyVendorSystemStamp;
    public override string ForwardSecurityAudit(SecurityPayload securityPayload)
    {
        return GetAuditRecord(securityPayload);
    }
}

public class StandardEmployeeAccess : Security
{
    protected override string SystemStamp => StandardEmployeeSystemStamp;
    public override string ForwardSecurityAudit(SecurityPayload securityPayload)
    {
        return GetAuditRecord(securityPayload);
    }
}

public class UserAccessTrackingEngine
{
    private readonly ITelemetry _telemetry;

    public UserAccessTrackingEngine(ITelemetry telemetry)
    {
        _telemetry = telemetry;
    }

    public void ProcessSecurityAudit(SecurityPayload payload, Security security)
    {
        Console.WriteLine($"Registering active auditing signature for Account: {payload.AccountId}...");

        string auditRecord = security.ForwardSecurityAudit(payload);

        _telemetry.PushSecurityLog(payload.AccountId, auditRecord);
    }
}

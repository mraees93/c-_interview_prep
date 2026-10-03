/*
using System;

public class OrderPayload
{
    public string CaseNumber { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
}

public class LocalNetworkPrinter
{
    public void PrintDocument(string caseNum, string formattedText)
    {
        Console.WriteLine($"[PRINTER] Printing final legal order for Case: {caseNum}");
    }
}

public class CourtOrderIssuanceEngine
{
    private readonly LocalNetworkPrinter _printer;

    public CourtOrderIssuanceEngine()
    {
        _printer = new LocalNetworkPrinter();
    }

    public void ProcessAndIssueOrder(OrderPayload payload, string orderType)
    {
        Console.WriteLine($"Registering issuance event for Case: {payload.CaseNumber}...");

        string finalOutput = string.Empty;

        if (orderType == "Interdict")
        {
            string stamp = $"[OFFICIAL INTERDICT STAMP - RUNNING AT {DateTime.UtcNow}]";
            finalOutput = $"{stamp}\n{payload.TextContent.ToUpper()}";
        }
        else if (orderType == "Subpoena")
        {
            string stamp = $"[OFFICIAL SUBPOENA STAMP - RUNNING AT {DateTime.UtcNow}]";
            finalOutput = $"{stamp}\n{payload.TextContent.ToUpper()}";
        }
        else if (orderType == "VerbalRemand")
        {
            throw new NotSupportedException("Verbal remands are spoken directly in the courtroom; printing physical copies is blocked!");
        }

        _printer.PrintDocument(payload.CaseNumber, finalOutput);
    }
}
*/

using System;

public class OrderPayload
{
    public string CaseNumber { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
}

public class LocalNetworkPrinter : INetworkProcessor
{
    public void PrintDocument(string caseNum, string formattedText)
    {
        Console.WriteLine($"[PRINTER] Printing final legal order for Case: {caseNum}");
    }
}

public interface INetworkProcessor
{
    void PrintDocument(string caseNum, string formattedText);
}

public abstract class Order2
{
    public string FinalOutput(string stamp, OrderPayload payload)
    {
        return $"{stamp}\n{payload.TextContent.ToUpper()}";
    }
    public abstract string ProcessAndIssueOrder(OrderPayload payload);
}

public class Interdict : Order2
{
    readonly string stamp = $"[OFFICIAL INTERDICT STAMP - RUNNING AT {DateTime.UtcNow}]";
    public override string ProcessAndIssueOrder(OrderPayload payload)
    {
        return FinalOutput(stamp, payload);
    }
}

public class Subpoena : Order2
{
    readonly string stamp = $"[OFFICIAL SUBPOENA STAMP - RUNNING AT {DateTime.UtcNow}]";

    public override string ProcessAndIssueOrder(OrderPayload payload)
    {
        return FinalOutput(stamp, payload);
    }
}

public class CourtOrderIssuanceEngine
{
    private readonly INetworkProcessor _networkProcessor;

    public CourtOrderIssuanceEngine(INetworkProcessor networkProcessor)
    {
        _networkProcessor = networkProcessor;
    }

    public void HandleProcessAndIssueOrder(OrderPayload payload, Order2 order)
    {
        Console.WriteLine($"Registering issuance event for Case: {payload.CaseNumber}...");

        string finalOutput = order.ProcessAndIssueOrder(payload);

        _networkProcessor.PrintDocument(payload.CaseNumber, finalOutput);
    }
}

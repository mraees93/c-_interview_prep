using System;
/*
using System;

public class TransactionPayload { public string ReferenceId { get; set; } = string.Empty; public decimal GrossAmount { get; set; } }

public class LocalOracleDatabaseGateway
{
    public void SaveCalculatedFee(string refId, decimal dynamicFee)
    {
        Console.WriteLine($"[ORACLE DATABASE] Committed transactional fee of ZAR {dynamicFee} for ref {refId}");
    }
}

public class TransactionFeeEngine
{
    private readonly LocalOracleDatabaseGateway _oracleGateway;

    public TransactionFeeEngine()
    {
        _oracleGateway = new LocalOracleDatabaseGateway();
    }

    public void CalculateAndCommitFee(TransactionPayload payload, string calculationTier)
    {
        Console.WriteLine($"Processing backend pricing schema variables for: {payload.ReferenceId}...");
        
        decimal finalFee = 0.00m;

        if (calculationTier == "CorporateBulk")
        {
            decimal baselineDeduction = payload.GrossAmount * 0.02m;
            finalFee = Math.Round(baselineDeduction, 2);
        }
        else if (calculationTier == "RetailSme")
        {
            decimal baselineDeduction = payload.GrossAmount * 0.05m;
            finalFee = Math.Round(baselineDeduction, 2);
        }
        else if (calculationTier == "ProBonoExempt")
        {
            throw new NotSupportedException("Pro-Bono litigation files are entirely tax and fee exempt; processing deductions is blocked!");
        }

        _oracleGateway.SaveCalculatedFee(payload.ReferenceId, finalFee);
    }
}
*/

public class TransactionPayload { public string ReferenceId { get; set; } = string.Empty; public decimal GrossAmount { get; set; } }

public class LocalOracleDatabaseGateway : IDocumentRepository
{
    public void SaveCalculatedFee(string refId, decimal dynamicFee)
    {
        Console.WriteLine($"[ORACLE DATABASE] Committed transactional fee of ZAR {dynamicFee} for ref {refId}");
    }
}
public interface IDocumentRepository
{
    void SaveCalculatedFee(string refId, decimal dynamicFee);
}
public abstract class DiscountBase
{
    protected abstract decimal Multiplier { get; }
    public decimal GetFinalFee(TransactionPayload payload)
    {
        decimal baselineDeduction = payload.GrossAmount * Multiplier;
        return Math.Round(baselineDeduction, 2);
    }
}
public class CorporateBulkDiscount : DiscountBase
{
    protected override decimal Multiplier => 0.02m;
}
public class RetailSmeDiscount : DiscountBase
{
    protected override decimal Multiplier => 0.05m;
}
public class TransactionFeeEngine
{
    private readonly IDocumentRepository _documentRepository;

    public TransactionFeeEngine(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public void ProcessTransactionFee(TransactionPayload payload, DiscountBase discount)
    {
        Console.WriteLine($"Processing backend pricing schema variables for: {payload.ReferenceId}...");
        
        decimal finalFee = discount.GetFinalFee(payload);
        _documentRepository.SaveCalculatedFee(payload.ReferenceId, finalFee);
    }
}

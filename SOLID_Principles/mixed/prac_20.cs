using System;

/*
using System;

public class BillingStatement
{
    public string AccountId { get; set; } = string.Empty;
    public decimal BaseFee { get; set; }
    public decimal TaxRate { get; set; } = 0.15m;
}

public class LocalLedgerDatabase
{
    public void RecordFinalCharge(string accountId, decimal totalAmount)
    {
        Console.WriteLine($"[LEDGER] Account {accountId} billed ZAR {totalAmount}");
    }
}

public class BillingCalculationEngine
{
    private readonly LocalLedgerDatabase _ledgerDb;

    public BillingCalculationEngine()
    {
        _ledgerDb = new LocalLedgerDatabase();
    }

    public void ProcessStatement(BillingStatement statement, string clientTier)
    {
        decimal discountedBase = statement.BaseFee;

        if (clientTier == "EnterpriseRetainer")
        {
            discountedBase = Math.Round(statement.BaseFee * 0.85m, 2);
        }
        else if (clientTier == "StandardFirm")
        {
            discountedBase = Math.Round(statement.BaseFee * 0.95m, 2);
        }
        else if (clientTier == "GovernmentGrant")
        {
            statement.TaxRate = 0.00m;
            discountedBase = Math.Round(statement.BaseFee * 0.90m, 2);
        }

        decimal finalTotal = Math.Round(discountedBase * (1 + statement.TaxRate), 2);
        _ledgerDb.RecordFinalCharge(statement.AccountId, finalTotal);
    }
}
*/

public class BillingStatement
{
    public string AccountId { get; set; } = string.Empty;
    public decimal BaseFee { get; set; }
    public decimal TaxRate { get; set; } = 0.15m;
}

public class LocalLedgerDatabase : ILedgerRepository
{
    public void RecordFinalCharge(string accountId, decimal totalAmount)
    {
        Console.WriteLine($"[LEDGER] Account {accountId} billed ZAR {totalAmount}");
    }
}

public interface ILedgerRepository
{
    void RecordFinalCharge(string accountId, decimal totalAmount);
}

public abstract class BillingBase
{
    protected abstract decimal Multiplier { get; }
    protected virtual decimal GetTaxRate(BillingStatement statement) => statement.TaxRate;
    public decimal GetFinalTotal(BillingStatement statement)
    {
        decimal discountedBase = Math.Round(statement.BaseFee * Multiplier, 2);
        return Math.Round(discountedBase * (1 + GetTaxRate(statement)), 2);
    }
}

public class EnterpriseRetainerBilling : BillingBase
{
    protected override decimal Multiplier => 0.85m;
}

public class StandardFirmBilling : BillingBase
{
    protected override decimal Multiplier => 0.95m;
}

public class GovernmentGrantBilling : BillingBase
{
    protected override decimal Multiplier => 0.90m;
    protected override decimal GetTaxRate(BillingStatement statement) => 0.00m;
}

public class BillingCalculationEngine
{
    private readonly ILedgerRepository _ledgerRepository;

    public BillingCalculationEngine(ILedgerRepository ledgerRepository)
    {
        _ledgerRepository = ledgerRepository;
    }

    public void ProcessBillingStatement(BillingStatement statement, BillingBase billing)
    {
        decimal finalTotal = billing.GetFinalTotal(statement);
        _ledgerRepository.RecordFinalCharge(statement.AccountId, finalTotal);
    }
}

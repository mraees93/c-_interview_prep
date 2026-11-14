// using System;

// public class BillingStatement
// {
//     public string AccountId { get; set; } = string.Empty;
//     public decimal BaseFee { get; set; }
//     public decimal TaxRate { get; set; } = 0.15m;
// }

// public class LocalLedgerDatabase : ILedgerRepository
// {
//     public void RecordFinalCharge(string accountId, decimal totalAmount)
//     {
//         Console.WriteLine($"[LEDGER] Account {accountId} billed ZAR {totalAmount}");
//     }
// }

// public interface ILedgerRepository
// {
//     void RecordFinalCharge(string accountId, decimal totalAmount);
// }

// public abstract class BillingBase : BillingStatement
// {
//     protected int baseFee
//     protected int discountedBase = Math.Round(BaseFee)
// }

// public class BillingCalculationEngine
// {
//     private readonly ILedgerRepository _ledgerRepository;

//     public BillingCalculationEngine(ILedgerRepository ledgerRepository)
//     {
//         _ledgerRepository = ledgerRepository;
//     }

//     public void ProcessStatement(BillingStatement statement, string clientTier)
//     {
//         decimal discountedBase = statement.BaseFee;

//         if (clientTier == "EnterpriseRetainer")
//         {
//             discountedBase = Math.Round(statement.BaseFee * 0.85m, 2);
//         }
//         else if (clientTier == "StandardFirm")
//         {
//             discountedBase = Math.Round(statement.BaseFee * 0.95m, 2);
//         }
//         else if (clientTier == "GovernmentGrant")
//         {
//             statement.TaxRate = 0.00m;
//             discountedBase = Math.Round(statement.BaseFee * 0.90m, 2);
//         }

//         decimal finalTotal = Math.Round(discountedBase * (1 + statement.TaxRate), 2);
//         _ledgerRepository.RecordFinalCharge(statement.AccountId, finalTotal);
//     }
// }

// using System;

// public class TransactionPayload { public string ReferenceId { get; set; } = string.Empty; public decimal GrossAmount { get; set; } }

// public class LocalOracleDatabaseGateway : IDocumentRepository
// {
//     public void SaveCalculatedFee(string refId, decimal dynamicFee)
//     {
//         Console.WriteLine($"[ORACLE DATABASE] Committed transactional fee of ZAR {dynamicFee} for ref {refId}");
//     }
// }
// public interface IDocumentRepository
// {
//     void SaveCalculatedFee(string refId, decimal dynamicFee);
// }
// public abstract class Discount
// {
//     protected readonly TransactionPayload _payload;

//     public Discount(TransactionPayload payload)
//     {
//         _payload = payload;
//     }
//     public string ReferenceId => _payload.ReferenceId;
//     protected abstract decimal baselineDeduction { get; }
//     protected abstract decimal CalculateFee();
//     public decimal GetFinalFee()
//     {
//         return Math.Round(CalculateFee(), 2);
//     }
// }
// public class CorporateBulk : Discount
// {
//     public CorporateBulk(TransactionPayload payload) : base(payload) {}
//     protected override decimal baselineDeduction => _payload.GrossAmount * 0.02m;
//     protected override decimal CalculateFee()
//     {
//         return baselineDeduction;
//     }
// }
// public class RetailSme : Discount
// {
//     public RetailSme(TransactionPayload payload) : base(payload) {}

//     protected override decimal baselineDeduction => _payload.GrossAmount * 0.05m;
//     protected override decimal CalculateFee()
//     {
//         return baselineDeduction;
//     }
// }
// public class TransactionFeeEngine
// {
//     private readonly IDocumentRepository _documentRepository;

//     public TransactionFeeEngine(IDocumentRepository documentRepository)
//     {
//         _documentRepository = documentRepository;
//     }

//     public void ProcessTransactionFee(Discount discount)
//     {
//         Console.WriteLine($"Processing backend pricing schema variables for: {discount.ReferenceId}...");
        
//         decimal finalFee = discount.GetFinalFee();
//         _documentRepository.SaveCalculatedFee(discount.ReferenceId, finalFee);
//     }
// }

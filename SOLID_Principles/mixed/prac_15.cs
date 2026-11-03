using System;
using System.Collections.Generic;

public class FeeInvoice { public string InvoiceId { get; set; } = string.Empty; public decimal Amount { get; set; } }

public interface IInvoiceModifier
{
    void ModifyInvoiceAmount(FeeInvoice invoice, decimal structuralAdjustment);
}
public interface ILegalBillingPipeline
{
    void CalculateTax(FeeInvoice invoice);
    void SyncToLedgerCache(FeeInvoice invoice);
}

public abstract class Billing
{
    protected string OperatorId { get; set; } = "Billing_Bot";
    protected List<string> AuditedInvoices { get; set; } = new List<string>();
    public void TrackAuditTelemetry(string step, string invId) => Console.WriteLine($"[TELEMETRY] Operator {OperatorId} executed {step} on Invoice {invId}");
}

public class StandardCorporateBilling : Billing, ILegalBillingPipeline, IInvoiceModifier
{
    public StandardCorporateBilling()
    {
        OperatorId = "Billing_Bot_01";
    }

    public void CalculateTax(FeeInvoice invoice)
    {
        TrackAuditTelemetry("Tax_Calculation", invoice.InvoiceId);
        AuditedInvoices.Add(invoice.InvoiceId);
    }

    public void ModifyInvoiceAmount(FeeInvoice invoice, decimal structuralAdjustment)
    {
        TrackAuditTelemetry("Amount_Adjustment", invoice.InvoiceId);
        invoice.Amount += structuralAdjustment;
    }

    public void SyncToLedgerCache(FeeInvoice invoice) => Console.WriteLine("Syncing finalized transaction payload directly to ledger node cache.");
}

public class ReadOnlyProBonoBilling : Billing, ILegalBillingPipeline
{
    public ReadOnlyProBonoBilling()
    {
        OperatorId = "ProBono_Bot_99";
    }

    public void CalculateTax(FeeInvoice invoice)
    {
        TrackAuditTelemetry("Tax_Calculation", invoice.InvoiceId);
        AuditedInvoices.Add(invoice.InvoiceId);
    }
    public void SyncToLedgerCache(FeeInvoice invoice) => Console.WriteLine("Syncing finalized transaction payload directly to ledger node cache.");
}

public class BillingEngine
{
    protected void ProcessInvoiceBilling(FeeInvoice feeInvoice, StandardCorporateBilling corporateBilling)
    {
        corporateBilling.ModifyInvoiceAmount(feeInvoice, 235);
        corporateBilling.CalculateTax(feeInvoice);
        corporateBilling.SyncToLedgerCache(feeInvoice);
    }
}
using System;

public class LegalCase
{
    public decimal ClaimAmount { get; set; }
    public int PageCount { get; set; }
}

public abstract class FillingFee
{
    public abstract decimal CalculateFilingFee(LegalCase legalCase);
}

public class Civil : FillingFee
{
    public override decimal CalculateFilingFee(LegalCase legalCase)
    {
        return 500.00m + (legalCase.PageCount * 2.50m);
    }
}

public class Criminal : FillingFee
{
    public override decimal CalculateFilingFee(LegalCase legalCase)
    {
        return 0.00m;
    }
}

public class Commercial : FillingFee
{
    public override decimal CalculateFilingFee(LegalCase legalCase)
    {
        return 1500.00m + (legalCase.ClaimAmount * 0.01m);
    }
}

public class CourtFeeCalculator
{
    public decimal CalculateFilingFee(LegalCase legalCase, string caseCategory)
    {
        if (caseCategory == "Civil")
        {
            return 500.00m + (legalCase.PageCount * 2.50m);
        }
        else if (caseCategory == "Criminal")
        {
            return 0.00m;
        }
        else if (caseCategory == "Commercial")
        {
            return 1500.00m + (legalCase.ClaimAmount * 0.01m);
        }

        throw new ArgumentException("Unsupported legal case category");
    }
}

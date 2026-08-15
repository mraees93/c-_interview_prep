// public class LegalFeeCalculator
// {
//     public decimal CalculateTotalFee(string clientType, decimal baseAmount)
//     {
//         if (clientType == "Corporate")
//         {
//             return baseAmount * 1.2m;
//         }
//         else if (clientType == "ProBono")
//         {
//             return 0;
//         }
//         return baseAmount;
//     }
// }

abstract class LegalFeeCalculator
{
    public abstract decimal CalculateTotalFee(decimal baseAmount);
}

class Corporate : LegalFeeCalculator
{
    public override decimal CalculateTotalFee(decimal baseAmount)
    {
        return baseAmount * 1.2m;
    }
}

class ProBono : LegalFeeCalculator
{
    public override decimal CalculateTotalFee(decimal baseAmount)
    {
        return 0;
    }
}
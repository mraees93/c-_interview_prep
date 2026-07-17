public class AssetDepreciationCalculator
{
    public decimal Calculate(string assetType, decimal cost, int lifespanYears)
    {
        if (assetType == "Vehicle")
        {
            return cost / lifespanYears;
        }
        else if (assetType == "Software")
        {
            return cost / (lifespanYears * 0.5m);
        }
        return 0;
    }
}

public interface IAsset
{
    public decimal Calculate(decimal cost, int lifespanYears);
}

public class Vehicle : IAsset
{
    public decimal Calculate(decimal cost, int lifespanYears) => cost / lifespanYears;
}

public class Software : IAsset
{
    public decimal Calculate(decimal cost, int lifespanYears) => cost / (lifespanYears * 0.5m);
}
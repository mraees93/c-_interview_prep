using System;

public class Order1
{
    public decimal TotalWeight { get; set; }
    public decimal TotalAmount { get; set; }
}

public class Shipping
{
    public virtual decimal CalculateShipping(Order1 order)
    {
        return order.TotalWeight * 0.5m;
    }
}

public class StandardShipping : Shipping
{
    public override decimal CalculateShipping(Order1 order)
    {
        return base.CalculateShipping(order);
    }
}

public class NextDayShipping : Shipping
{
    public override decimal CalculateShipping(Order1 order)
    {
        return (order.TotalWeight * 1.5m) + 10.00m;
    }
}

public class FreeShipping : Shipping
{
    public override decimal CalculateShipping(Order1 order)
    {
        if (order.TotalAmount > 50.00m)
            {
                return 0.00m;
            }
        else  {
            return base.CalculateShipping(order);
        }
    }
}


public class ShippingCostCalculator
{
    public decimal CalculateShipping(Order1 order, string shippingType)
    {
        if (shippingType == "Standard")
        {
            return order.TotalWeight * 0.5m;
        }
        else if (shippingType == "NextDay")
        {
            return (order.TotalWeight * 1.5m) + 10.00m;
        }
        else if (shippingType == "FreeShipping")
        {
            if (order.TotalAmount > 50.00m)
            {
                return 0.00m;
            }
            return order.TotalWeight * 0.5m;
        }

        throw new ArgumentException("Unsupported shipping method");
    }
}

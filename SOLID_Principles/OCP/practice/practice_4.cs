using System;

public class CustomerOrder
{
    public decimal Subtotal { get; set; }
}
class Discount
{
    public virtual decimal ApplyDiscount(CustomerOrder order)
    {
        return order.Subtotal;
    }
}
class FlatTenPercent : Discount
{
    public override decimal ApplyDiscount(CustomerOrder order)
    {
        return order.Subtotal * 0.90m;
    }
}
class VipFixedAmount : Discount
{
    public override decimal ApplyDiscount(CustomerOrder order)
    {
        return order.Subtotal - 25.00m;
    }
}
class ClearanceHalfPrice : Discount
{
    public override decimal ApplyDiscount(CustomerOrder order)
    {
        return order.Subtotal * 0.50m;
    }
}
public class DiscountManager
{
    public decimal ApplyDiscount(CustomerOrder order, string discountType)
    {
        if (discountType == "FlatTenPercent")
        {
            return order.Subtotal * 0.90m;
        }
        else if (discountType == "VipFixedAmount")
        {
            return order.Subtotal - 25.00m;
        }
        else if (discountType == "ClearanceHalfPrice")
        {
            return order.Subtotal * 0.50m;
        }

        return order.Subtotal;
    }
}

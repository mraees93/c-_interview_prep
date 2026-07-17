public class OrderValidator
{
    public bool Validate(string orderType, object order)
    {
        if (orderType == "Digital")
        {
            return true; // Digital validation rules
        }
        else if (orderType == "Physical")
        {
            return true; // Physical validation rules
        }
        return false;
    }
}

public abstract class Order
{
    public abstract void ValidateOrderNumber(object orderNumber);
}

public class Digital : Order
{
    public override void ValidateOrderNumber(object orderNumber)
    {
        System.Console.WriteLine($"Digital order no. : {orderNumber}");
    }
}

public class Physical : Order
{
    public override void ValidateOrderNumber(object orderNumber)
    {
        System.Console.WriteLine($"Physical order no. : {orderNumber}");
    }
}
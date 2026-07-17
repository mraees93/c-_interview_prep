public interface IPaymentCard
{
    public void ProcessOnlineTransaction(decimal amount);
}

public interface GiftCard
{
    public void HandToWorker(int barcode);
}

public class PaymentCard : IPaymentCard
{
    public void ProcessOnlineTransaction(decimal amount)
    {
        System.Console.WriteLine($"Payment is {amount}");
    }
}

public class AtmosphericGiftCard : GiftCard
{
    public void HandToWorker(int barcode)
    {
        System.Console.WriteLine($"Handing worker the card and scanning {barcode}");
    }
}



using System;
using System.Threading;
using System.Threading.Tasks;

public class PaymentRequest
{
    public string UserId { get; set; }
    public decimal Amount { get; set; }
}

public class SubscriptionApiController
{
    public async Task ProcessPayment(PaymentRequest request)
    {
        Console.WriteLine($"API POST: Received payment request for user {request.UserId}");

        await ChargeGatewayAsync(request.Amount);

        await LogTransactionToCloud(request.UserId);

        Console.WriteLine("API POST: Payment transaction finalized successfully.");
    }

    private async Task ChargeGatewayAsync(decimal amount)
    {
        await Task.Delay(2000);
    }

    private async Task LogTransactionToCloud(string userId)
    {
        await Task.Delay(1000);
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class SyncKitchen
{
    public void PrepareOrder()
    {
        CookCake();
        WashDishes();
    }

    private void CookCake()
    {
        Thread.Sleep(3000);
    }

    private void WashDishes()
    {
        Thread.Sleep(1000);
    }
}

//CONCURRENT PROGRAMMING - 2 chefs prepping 3 dishes so never stays idle(ASYNC) OR 3 chefs prepping 1 big dish to complete it faster(PARALLEL)
public class AsyncKitchen
{
    public async Task PrepareOrderAsync()
    {
        Task cakeTask = CookCakeAsync();
        Task sauceTask = SimmerSauceAsync();

        await Task.WhenAll(cakeTask, sauceTask);
    }

    private async Task CookCakeAsync()
    {
        await Task.Delay(3000);
    }

    private async Task SimmerSauceAsync()
    {
        await Task.Delay(1000);
    }
}

public class ParallelKitchen
{
    public void ChopBulkOnions()
    {
        List<int> onionPile = IEnumerable.Range(1, 1000).ToList();

        Parallel.ForEach(onionPile, onion =>
        {
            ProcessOnion(onion);
        });
    }

    private void ProcessOnion(int onionId)
    {
        double result = 0;
        for(int i = 0; i < 100_000; i++)
        {
            result += Math.Sqrt(i);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FitnessClass
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class BookingController
{
    public async Task<List<FitnessClass>> GetAvailableClassesAsync()
    {
        Console.WriteLine("Fetching schedules from regional servers...");

        var localTask = FetchLocalClassesAsync();
        var nationalTask = FetchNationalClassesAsync();

        await Task.WhenAll(localTask, nationalTask);

        var allClasses = new List<FitnessClass>();
        allClasses.AddRange(localTask.Result);
        allClasses.AddRange(nationalTask.Result);

        return allClasses;
    }

    public async Task ConfirmBookingAsync(int classId, string userEmail)
    {
        Console.WriteLine($"Validating payment status for class {classId}...");

        await VerifyPaymentAsync(userEmail);

        Console.WriteLine("Booking confirmed and slot reserved.");
    }

    public async Task<bool> IsGymOpenAsync()
    {
        var task = await Task.FromResult(true);

        return task;
    }

    private async Task<List<FitnessClass>> FetchLocalClassesAsync()
    {
        await Task.Delay(1000);
        return new List<FitnessClass> { new FitnessClass { Id = 1, Name = "Yoga" } };
    }

    private async Task<List<FitnessClass>> FetchNationalClassesAsync()
    {
        await Task.Delay(1500);
        return new List<FitnessClass> { new FitnessClass { Id = 2, Name = "Pilates" } };
    }

    private async Task VerifyPaymentAsync(string email)
    {
        await Task.Delay(1000);
    }
}

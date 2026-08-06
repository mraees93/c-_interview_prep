public class FleetCar
{
    // INSTANCE FIELD: Every separate car object tracks its own unique color state
    public string Color { get; set; } = "White";

    // STATIC FIELD: There is only ONE counter in memory shared across ALL cars globally
    public static int TotalCarsCreated { get; private set; }

    public FleetCar()
    {
        TotalCarsCreated++; // Increments the single global shared counter
    }
}
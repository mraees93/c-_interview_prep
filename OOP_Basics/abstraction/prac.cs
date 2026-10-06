//Auto-Implemented Properties (The Standard Door)
public class Chef
{
    public string Name { get; set; }

    public int Age { get; private set; }

    // BEST PRACTICE: Use a Constructor to safely set private properties at birth
    public Chef(string name, int initialAge)
    {
        Name = name;
        // Validation check right at the source guardrail
        if (initialAge < 18) throw new ArgumentException("Chefs must be adults!");
        Age = initialAge;
    }

    //BEST PRACTICE: Use a descriptive method to change state later
    public void CelebrateBirthDay()
    {
        Age++; //Completely legal because we are INSIDE the class
    }
}

public class Restaurant
{
    public void OpenKitchen()
    {
        // 1. Creation via the constructor gateway
        Chef headChef = new Chef("Gordon", 50);
        // 2. Getting data (Allowed everywhere)
        System.Console.WriteLine($"{headChef.Name} is {headChef.Age} years old.");
        // 3. Setting data via a structured event
        headChef.CelebrateBirthDay();

        //headChef.Age = 51; error
    }
}

//Full Properties with Backing Fields (The Guarded Gate)
public class Order
{
    private decimal _price; // Private Backing Field

    public decimal Price
    {
        get => _price;
        set
        {
            // Business rule validation constraint
            if (value < 0) throw new ArgumentException("Price cannot be negative!");
            _price = value;
        }
    }
}

// Computed Properties (The On-the-Fly Chef)

public class Rectangle
{
    public double Width { get; set; }
    public double Height { get; set; }

    public double Area => Width * Height; //computed property using lambda syntax

    /*
    The thread looks at the warehouse floor, reads the numbers currently inside Width and Height, and brings copies of those numbers down onto its private Desk Clipboard (the Stack) just for a split second
    */
}

//Init-Only Properties (The Immutable Setup - C# 9+)

public class Product
{
    public string Sku { get; init; } // init  makes the property immutable
}


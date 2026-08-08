Software entities (classes, modules, functions, etc.) should be open for extension but closed for modification.

This principle promotes the idea that existing code should be able to be extended with new functionality without modifying its source code. It encourages the use of abstraction and polymorphism to achieve this goal, allowing for code to be easily extended through inheritance or composition.

extending the codebase with more classes, implementing the interface and overriding its method is called the **strategy pattern**

# Interface vs. Abstract Class in the Strategy Pattern

In the **Strategy Pattern**, deciding whether to use an interface or an abstract class depends entirely on whether your strategies need to **share common code** or if they are **completely independent behaviors**.

---

### 🟢 Use an Interface When: Strategies Share Zero Code
Interfaces are the default choice for the Strategy Pattern. Use an interface if each strategy calculates or performs its action in a completely unique way.

* **No Code Duplication:** There is no shared layout, math, or properties between the strategies.
* **Keeps Inheritance Free:** In C#, a class can only inherit from *one* class but can implement *multiple* interfaces. Using an interface keeps your class's single inheritance slot open.

```csharp
public interface INotificationStrategy
{
    void SendAlert(string message);
}

public class EmailStrategy : INotificationStrategy
{
    public void SendAlert(string message) => Console.WriteLine(\$"Email: {message}");
}

public class SlackStrategy : INotificationStrategy
{
    public void SendAlert(string message) => Console.WriteLine(\$"Slack: {message}");
}
```

---

### 🔵 Use an Abstract Class When: Strategies Share Common Logic or Fields
Use an abstract class if your strategies have **identical helper methods, shared properties, or baseline settings** that you want to write once and reuse via inheritance.

* **DRY Principle (Don't Repeat Yourself):** It stops you from copy-pasting the exact same fields or methods into five different strategy classes.
* **Provides Default Behavior:** You can mark a method as `virtual` to give it a default behavior that child classes can optionally `override`.

```csharp
public abstract class TaxStrategy
{
    protected decimal BaseTaxRate { get; set; } = 0.15m;

    protected decimal CalculateBaseLevy(decimal amount) => amount * 0.02m;

    public abstract decimal CalculateTotalTax(decimal amount);
}

public class LocalTaxStrategy : TaxStrategy
{
    public override decimal CalculateTotalTax(decimal amount)
    {
        return (amount * BaseTaxRate) + CalculateBaseLevy(amount);
    }
}
```

---

### ⚡ Summary Decision Matrix

| Requirement | Interface | Abstract Class |
| :--- | :--- | :--- |
| **Shared fields/properties needed** | ❌ No | 🟢 Yes |
| **Shared helper methods needed** | ❌ No | 🟢 Yes |
| **Leaves C# class inheritance slot open** | 🟢 Yes | ❌ No (Uses the 1 slot) |
| **Can be used on C# struct types** | 🟢 Yes | ❌ No (Classes only) |

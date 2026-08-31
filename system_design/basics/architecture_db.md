# 🗄️ Database Architecture: Traditional Monolith vs. Modular Monolith

---

## 📊 Structural Comparison Matrix

| Architectural Attribute | Traditional Monolith (Single Chaos Kitchen) | Modular Monolith (Compartmentalised Kitchen Lot) |
| :--- | :--- | :--- |
| **Database Schema** | **Unified / Shared** (Everything dumped into the default `dbo` space). | **Strictly Isolated Schemas** (Dedicated boundaries like `identity.` and `billing.`). |
| **Cross-Domain Data Access** | **Direct Database Joins**. Any query can touch any table across the entire system. | **Forbidden at DB Level**. Cross-schema querying is blocked by architecture or DB permissions. |
| **Communication Mechanism** | Direct SQL `INNER JOIN` across unrelated domain tables. | **Public Interfaces / Service Windows** or in-memory domain events. |
| **The Production Trap** | **The Big Ball of Mud:** Changing a column in one table breaks multiple unrelated business logic modules. | **Boundary Leaks:** Over-exposing data models through public interfaces instead of clean DTOs. |

---

## 🚨 Code & Query Examples

### 1. Traditional Monolith (The Direct Join Trap)
In a traditional monolith, the billing logic reaches straight across the communal counter to grab user data. The C# database context contains all tables, allowing tight, dangerous coupling.

#### ❌ The Cross-Domain SQL Query:
```sql
-- The billing engine directly reads the identity tables. 
-- If the Identity team alters the User table structure, the Billing engine crashes.
SELECT 
    i.InvoiceId, 
    i.Amount, 
    u.EmailAddress, 
    u.CellNumber 
FROM dbo.Invoices i
INNER JOIN dbo.Users u ON i.UserId = u.UserId;
```

---

### 2. Modular Monolith (The Service Window Pattern)
In a modular monolith, the billing module code **cannot** see the `identity` tables. It must cleanly request data through an in-memory C# communication contract (the service window).

#### 🛡️ The Database Layout:
The tables are split into rigid, walled territories within the same database engine:
*   `identity.Users`
*   `billing.Invoices`

#### 🔌 The C# Service Window Contract (Public Interface):
```csharp
namespace LexisNexisWorkspace.Modules.Identity.Public;

public interface IIdentityModuleServiceWindow
{
    // The billing module is only allowed to get data through this contract
    Task<UserContactDetailsDto> GetContactDetailsAsync(Guid userId);
}
```

#### ⚙️ The Safe Modular Implementation:
```csharp
namespace LexisNexisWorkspace.Modules.Billing.Engine;

public class BillingProcessor
{
    private readonly IBillingRepository _billingRepository;
    private readonly IIdentityModuleServiceWindow _identityServiceWindow; // The Service Window

    public BillingProcessor(
        IBillingRepository billingRepository, 
        IIdentityModuleServiceWindow identityServiceWindow)
    {
        _billingRepository = billingRepository;
        _identityServiceWindow = identityServiceWindow;
    }

    public async Task ProcessInvoiceAsync(Guid invoiceId)
    {
        // 1. Fetch invoice data cleanly from its own isolated billing schema table
        var invoice = await _billingRepository.GetByIdAsync(invoiceId);

        // 2. Step up to the service window to request identity details safely
        var clientDetails = await _identityServiceWindow.GetContactDetailsAsync(invoice.UserId);

        // 3. Execute billing notification logic safely
        SendInvoiceEmail(clientDetails.EmailAddress, invoice.Amount);
    }
}
```

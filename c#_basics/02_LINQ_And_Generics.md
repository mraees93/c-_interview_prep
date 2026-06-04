# ⚡ LINQ Querying Pipelines & Generic Architectures
*LexisNexis Cape Town Interview Preparation - Module 2*

## ⚙️ LINQ Collections and Core Execution Methods

### Concept Summary
**Language Integrated Query (LINQ)** provides declarative data querying capabilities on collections implementing `IEnumerable<T>`. 

*   **The Crucial Interview Concept: Deferred (Lazy) Execution.** Methods returning `IEnumerable<T>` (e.g., `Where`, `Select`, `Take`) do not filter or transform items instantly. They store a query execution plan. 
*   The query executes **only** when iterated over (via `foreach` or calling immediate evaluation methods like `ToList()`, `ToArray()`, `First()`, `Count()`).
*   **JS/TS Comparison:** JS array prototypes (`.map()`, `.filter()`) execute **immediately** and produce a new array in memory right away. LINQ delays execution until the data is explicitly demanded.

### 🚨 Common Problem: The Double-Evaluation Trap
```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class Employee
{
    public string Name { get; set; }
    public string Department { get; set; }
    public double Salary { get; set; }
}

public class LinqEvaluationDemo
{
    public static void ProcessEmployees(List<Employee> staff)
    {
        // Lazy execution blueprint setup. Data is NOT filtered yet!
        var highEarnersQuery = staff.Where(e => e.Salary > 65000);

        // ❌ INTERVIEW TRAP: Evaluates the predicate logic across the entire list the 1st time
        int totalCount = highEarnersQuery.Count(); 

        // ❌ INTERVIEW TRAP: Evaluates the entire filter logic across the collection a 2nd time!
        foreach (var employee in highEarnersQuery) 
        {
            Console.WriteLine(employee.Name);
        }

        //  CORRECT INTERMEDIATE APPROACH: Cache results to memory via immediate execution
        List<Employee> concreteList = staff.Where(e => e.Salary > 65000).ToList();
        
        int optimizedCount = concreteList.Count; // O(1) property read from list metadata
        // Subsequent iterations now loop over the pre-cached subset memory block.
    }
}
```

### Complete LINQ Method Reference Library
```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class LinqLibrary 
{
    public static void ExecuteAllLinqMethods() 
    {
        var rawNumbers = new List<int> { 1, 2, 3, 4, 5, 5, 6, 7, 8, 9, 10 };
        var mixedWords = new List<string> { "table", "chair", "desk", "laptop" };

        // 1. Where (JS equivalent: .filter())
        IEnumerable<int> evens = rawNumbers.Where(n => n % 2 == 0);

        // 2. Select (JS equivalent: .map())
        IEnumerable<string> mappedStrings = rawNumbers.Select(n => \$"Num: {n}");

        // 3. SelectMany (JS equivalent: .flatMap())
        var nestedList = new List<List<int>> { new List<int>{1,2}, new List<int>{3,4} };
        IEnumerable<int> flattened = nestedList.SelectMany(list => list);

        // 4. First / FirstOrDefault (JS equivalent: .find())
        int firstMatch = rawNumbers.FirstOrDefault(n => n > 7);

        // 5. Single / SingleOrDefault
        int singleMatch = rawNumbers.SingleOrDefault(n => n == 9);

        // 6. Any (JS equivalent: .some())
        bool hasLargeNum = rawNumbers.Any(n => n > 100);

        // 7. All (JS equivalent: .every())
        bool allPositive = rawNumbers.All(n => n > 0);

        // 8. Distinct (Deduplication)
        IEnumerable<int> uniqueNumbers = rawNumbers.Distinct();

        // 9. OrderBy / OrderByDescending (JS equivalent: .sort())
        var sortedWords = mixedWords.OrderBy(word => word.Length);

        // 10. GroupBy
        var groupedByParity = rawNumbers.GroupBy(n => n % 2 == 0 ? "Even" : "Odd");

        // 11. ToDictionary
        Dictionary<string, string> wordMap = mixedWords.ToDictionary(w => w.ToUpper(), w => w);

        // 12. Skip & Take (Essential for Database Server-Side Pagination pipelines)
        var paginatedResult = rawNumbers.Skip(5).Take(3).ToList(); 

        // 13. Aggregate (JS equivalent: .reduce())
        int sumTotal = rawNumbers.Aggregate((runningTotal, nextValue) => runningTotal + nextValue);
    }
}
```

---

## 🧬 Generics: C# vs. TypeScript

Generics enforce type reusability while eliminating runtime type-casting overheads.

### Crucial Architectural Difference
*   **TypeScript Generics exist purely at Compile-Time.** They undergo type erasure when compiled. JavaScript runs the output code completely blind to types at runtime.
*   **C# Generics are Reified at Runtime.** The Common Value Runtime (CLR) retains full knowledge of exact type arguments. Passing a value type (like `int`) triggers the runtime to generate a unique, highly optimized machine-code path matching memory configurations.

```csharp
// C# Generic Repository with constraints
public class Repository<TEntity> where TEntity : class, new()
{
    private readonly List<TEntity> _dataStore = new List<TEntity>();

    public void AddRecord(TEntity entity) => _dataStore.Add(entity);

    public TEntity CreateBlankInstance() => new TEntity(); // Allowed by new() constraint
}
```

```typescript
// TypeScript Compile-Time Generic equivalent
class Repository<TEntity extends object> {
    private dataStore: TEntity[] = [];

    public addRecord(entity: TEntity): void {
        this.dataStore.push(entity);
    }
}
```

```java

    List<Integer> points = new ArrayList<>();
    points.add(10);
    points.add(20);
    points.add(30);

    Stream<Integer> query = points.stream().filter(p -> p > 15);

    List<Integer> materialize = query.toList();

    points.add(40);

    System.out.println(materialize.size());
    System.out.println(points.size());

```

```java

    List<Integer> transactions = new ArrayList<>();

    int result = transactions.stream()
                             .filter(t -> t > 100)
                             .findFirst() // returns an optional Integer
                             .orElse(0); // if empty, returns default zero

    System.out.println(result);


    /* Performance Note: Primitive Specialisation

    If you are processing highly intensive or massive mathematical arrays, you can use an IntStream in Java. This avoids the object overhead of Integer wrapping on the heap and works directly with primitive memory layouts: */

    int result = transactions.stream()
                             .mapToInt(Integer::intValue) // converts to primitive IntStream
                             .filter(t -> t > 100)
                             .findFirst() // Returns an optional Integer
                             .orElse(0);

```

```java

    record User(int id, String role) {} // lightweight immutable record

    User[] users = {
        new User(1, "Admin"),
        new User(2, "User"),
        new User(3, "Guest")
    }

    List<User> target = List.of(users).stream()
        .filter(u -> u.id() == 2 || "Admin".equals(u.Role()))
        .toList();

    target.forEach(System.out::println);
```

```java
    Map<String, Integer> scores = new HashMap<>();

    scores.put("TeamA", 10);
    scores.put("teama", 20);

    System.out.println(scores.size());
```

### Base Collection Declarations Used in Loops

Before running the pipeline loops below, the source collections are declared and initialized across the three languages as follows:

#### TypeScript (TS)
```typescript
const rawNumbers: number[] = [1, 2, 3, 4, 5, 5, 6, 7, 8, 9, 10];
const mixedWords: string[] = ["table", "chair", "desk", "laptop"];
const nestedList: number[][] = [[1, 2], [3, 4]];
```

#### Java (JVM)
```java
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

List<Integer> rawNumbers = new ArrayList<>(Arrays.asList(1, 2, 3, 4, 5, 5, 6, 7, 8, 9, 10));
List<String> mixedWords = new ArrayList<>(Arrays.asList("table", "chair", "desk", "laptop"));
List<List<Integer>> nestedList = new ArrayList<>(Arrays.asList(
    new ArrayList<>(Arrays.asList(1, 2)),
    new ArrayList<>(Arrays.asList(3, 4))
));
```

#### C# (.NET)
```csharp
using System.Collections.Generic;

var rawNumbers = new List<int> { 1, 2, 3, 4, 5, 5, 6, 7, 8, 9, 10 };
var mixedWords = new List<string> { "table", "chair", "desk", "laptop" };
var nestedList = new List<List<int>> { new List<int>{1, 2}, new List<int>{3, 4} };
```

---

### Complete Method Reference Library

### 1. Where 
* **TypeScript:** `.filter(n => n % 2 === 0)`
* **C# LINQ:** `.Where(n => n % 2 == 0)`
* **Java Stream:** 
```java
List<Integer> evens = rawNumbers.stream()
                                .filter(n -> n % 2 == 0)
                                .toList();
```

---

### 2. Select
* **TypeScript:** `.map(n => `Num: ${n}`)`
* **C# LINQ:** `.Select(n => $"Num: {n}")`
* **Java Stream:** 
```java
List<String> mappedStrings = rawNumbers.stream()
                                       .map(n -> "Num: " + n)
                                       .toList();
```

---

### 3. SelectMany
* **TypeScript:** `.flatMap(list => list)`
* **C# LINQ:** `.SelectMany(list => list)`
* **Java Stream:** 
```java
List<Integer> flattened = nestedList.stream()
                                    .flatMap(List::stream) // Flattens inner lists into a single stream
                                    .toList();
```

---

### 4. First / FirstOrDefault
* **TypeScript:** `.find(n => n > 7) ?? 0`
* **C# LINQ:** `.FirstOrDefault(n => n > 7)`
* **Java Stream:** 
```java
int firstMatch = rawNumbers.stream()
                           .filter(n -> n > 7)
                           .findFirst()
                           .orElse(0); // Explicitly specify the fallback default value
```

---

### 5. Single / SingleOrDefault
* **TypeScript:** *No direct native method (requires filtering + checking length)*
* **C# LINQ:** `.SingleOrDefault(n => n == 9)`
* **Java Stream:** *(Streams do not have a built-in "throw if more than one" method; you filter and get the first, or handle manual validation)*
```java
int singleMatch = rawNumbers.stream()
                            .filter(n -> n == 9)
                            .findFirst()
                            .orElse(0); 
```

---

### 6. Any
* **TypeScript:** `.some(n => n > 100)`
* **C# LINQ:** `.Any(n => n > 100)`
* **Java Stream:** 
```java
boolean hasLargeNum = rawNumbers.stream()
                                .anyMatch(n -> n > 100); // Terminal short-circuiting operation
```

---

### 7. All
* **TypeScript:** `.every(n => n > 0)`
* **C# LINQ:** `.All(n => n > 0)`
* **Java Stream:** 
```java
boolean allPositive = rawNumbers.stream()
                                .allMatch(n -> n > 0); // Terminal short-circuiting operation
```

---

### 8. Distinct
* **TypeScript:** `[...new Set(rawNumbers)]`
* **C# LINQ:** `.Distinct()`
* **Java Stream:** 
```java
List<Integer> uniqueNumbers = rawNumbers.stream()
                                        .distinct()
                                        .toList();
```

---

### 9. OrderBy / OrderByDescending
* **TypeScript:** `.sort((a, b) => a.length - b.length)`
* **C# LINQ:** `.OrderBy(word => word.Length)`
* **Java Stream:** 
```java
// Ascending order
List<String> sortedWords = mixedWords.stream()
                                     .sorted(Comparator.comparingInt(String::length))
                                     .toList();

// Descending order equivalent to .OrderByDescending()
List<String> sortedDesc = mixedWords.stream()
                                    .sorted(Comparator.comparingInt(String::length).reversed())
                                    .toList();
```

---

### 10. GroupBy
* **TypeScript:** `Object.groupBy(rawNumbers, n => ...)` *(Modern TS / ES2024)*
* **C# LINQ:** `.GroupBy(n => n % 2 == 0 ? "Even" : "Odd")`
* **Java Stream:** 
```java
import java.util.Map;
import java.util.stream.Collectors;

Map<String, List<Integer>> groupedByParity = rawNumbers.stream()
    .collect(Collectors.groupingBy(n -> n % 2 == 0 ? "Even" : "Odd"));
```

---

### 11. ToDictionary
* **TypeScript:** `Object.fromEntries(mixedWords.map(w => [w.toUpperCase(), w]))`
* **C# LINQ:** `.ToDictionary(w => w.ToUpper(), w => w)`
* **Java Stream:** 
```java
import java.util.Map;
import java.util.stream.Collectors;

Map<String, String> wordMap = mixedWords.stream()
    .collect(Collectors.toMap(
        w -> w.toUpperCase(), // Key Mapper
        w -> w                // Value Mapper
    ));
```

---

### 12. Skip & Take
* **TypeScript:** `.slice(5, 5 + 3)`
* **C# LINQ:** `.Skip(5).Take(3)`
* **Java Stream:** 
```java
List<Integer> paginatedResult = rawNumbers.stream()
                                          .skip(5)
                                          .limit(3) // 'limit' is Java's version of 'Take'
                                          .toList();
```

---

### 13. Aggregate
* **TypeScript:** `.reduce((runningTotal, nextValue) => runningTotal + nextValue)`
* **C# LINQ:** `.Aggregate((runningTotal, nextValue) => runningTotal + nextValue)`
* **Java Stream:** 
```java
int sumTotal = rawNumbers.stream()
                         .reduce((runningTotal, nextValue) -> runningTotal + nextValue)
                         .orElse(0); // Reduce returns an Optional if the list could be empty
```

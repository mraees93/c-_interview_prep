# 🧪 TDD, Mocking & Code Coverage Cheatsheet

## 1. What is Test-Driven Development (TDD)?

TDD is a development discipline where you write your **automated unit tests *before* you write the actual production code**. It follows the **Red-Green-Refactor** loop:

1. **🔴 Red (Write a Failing Test):** Write a unit test for a new feature or business rule. Run it—it fails because the feature doesn't exist yet.
2. **🟢 Green (Make It Pass):** Write the bare minimum, simplest production code required to make that specific test pass.
3. **🔵 Refactor (Clean Up):** Clean up your code (improve naming, remove duplication, apply SOLID principles) while re-running the test to guarantee you didn't break anything.

---

## 2. The AAA Pattern (Structuring Every Unit Test)

Every clean unit test in C# (xUnit/NUnit) and Java (JUnit) follows the **AAA** structure:

1. **Arrange:** Set up your variables, mock objects, and expected inputs on the clipboard.
2. **Act:** Call the single method you are actually testing.
3. **Assert:** Verify that the output matches what you expected.

---

## 3. Why Interfaces (The Gate Intercom) Make Mocking Possible

> **The Kitchen Analogy:** **The Cardboard Stove.** If you want to test whether a chef knows how to flip a pancake properly, you don't want to fire up a massive, expensive commercial gas range or wait for a real delivery truck from the database. Instead, you hand the chef a **Fake Cardboard Stove (a Mock)** that fits the exact same kitchen hookup (**the Public Interface**).

* **Tightly Coupled (Untestable ❌):** If `CaseService` runs `new SqlCaseRepository()` inside itself, your unit test is forced to hit a real SQL database. That is slow, fragile, and breaks if the network drops.
* **Decoupled via Interface (Testable ✅):** Because `CaseService` depends on the **`ICaseRepository` Gate Intercom**, your unit test can plug in a **Fake Mock** (using **Moq** in C# or **Mockito** in Java) that instantly returns dummy data in 1 millisecond.

---

## 4. Code Comparison: C# (xUnit + Moq) vs. Java (JUnit + Mockito)

### C# Example
```csharp
[Fact]
public async Task GetActiveCount_ReturnsCorrectCount()
{
    // 1. ARRANGE: Create a fake Gate Intercom using Moq
    var mockRepo = new Mock<ICaseRepository>();
    mockRepo.Setup(r => r.GetActiveCaseTitlesAsync())
            .ReturnsAsync(new List<string> { "S v Zuma", "State v Maharaj" });

    var service = new CaseService(mockRepo.Object); // Inject the fake!

    // 2. ACT: Run the actual business logic
    var result = await service.GetActiveCountAsync();

    // 3. ASSERT: Verify the outcome
    Assert.Equal(2, result);
}
```

### Java Example
```java
@Test
void getActiveCount_ReturnsCorrectCount() {
    // 1. ARRANGE: Create a fake Gate Intercom using Mockito
    ICaseRepository mockRepo = Mockito.mock(ICaseRepository.class);
    Mockito.when(mockRepo.getActiveCaseTitles())
           .thenReturn(List.of("S v Zuma", "State v Maharaj"));

    CaseService service = new CaseService(mockRepo); // Inject the fake!

    // 2. ACT: Run the actual business logic
    int result = service.getActiveCount();

    // 3. ASSERT: Verify the outcome
    Assertions.assertEquals(2, result);
}
```

---

## 5. Unit Tests vs. Integration Tests vs. Code Coverage

| Test Type | What It Tests | Speed & Dependencies |
| :--- | :--- | :--- |
| **Unit Test** | Tests **one single class/method** in complete isolation. | **Millisecond fast.** Uses Mocks for all databases and external APIs. |
| **Integration Test** | Tests how **multiple layers work together** (e.g., API Controller ➔ Service ➔ Real Test Database). | **Slower.** Spins up an in-memory database or Docker container to verify real SQL queries and wiring. |

### 🎯 The Code Coverage Trap (Interview Pro-Tip)
* **What it is:** The percentage of your codebase executed by automated tests (e.g., 80% coverage).
* **The Panel Answer:** *"High code coverage is a great safety net, but 100% coverage doesn't guarantee bug-free software if your assertions are weak. I focus on testing **critical business rules, edge cases, and failure paths** (like null inputs or thrown exceptions) rather than writing pointless tests just to inflate coverage metrics on simple getters and setters."*

---

---

## 💬 Intermediate Panel Q&A: TDD & Mocking

### Q1: What is the difference between a Mock and a Stub?
* **Stub (The Dummy Prop):** A simple fake object that just returns hardcoded data so your test can run without crashing (e.g., returning a static list of 2 cases). You don't test *how* the stub was used; you just assert on the final output of your service.
* **Mock (The Security Camera):** A smarter fake object that **records interactions**. You use a Mock when you need to assert behavior—for example, verifying that `mockEmailService.SendEmail()` was called **exactly once** when a case was closed.

### Q2: What happens if a class is hard to unit test? What is that telling you about your architecture?
* **Answer:** Difficulty writing a unit test is an immediate architectural red flag—it usually means the class is **tightly coupled** and violating the Single Responsibility or Dependency Inversion principles. If I have to spin up a real database or mock 10 different dependencies just to test one method, it tells me I need to break that class into smaller pieces and hide external dependencies behind clean interfaces (the **Gate Intercom** pattern).

### Q3: Should you ever mock private methods or static classes?
* **Answer:** **No.** Mocking private methods violates **Encapsulation**—unit tests should only test the public behavior (the *What*), never the internal implementation details (the *How*). If a private method is so complex that it needs its own unit tests, that is a sign it should be extracted into its own separate class with a public interface. Similarly, static classes cannot be mocked easily because they are bolted to the building, so I wrap static calls (like `DateTime.Now` or file I/O) inside an injectable interface.

### Q4: When do you choose a Unit Test over an Integration Test?
* **Answer:** I follow the **Testing Pyramid**. The vast majority of my tests are fast **Unit Tests** using mocks to verify business logic, edge cases, and validation rules in milliseconds. I write fewer, targeted **Integration Tests** (without mocks) specifically at the boundaries—such as verifying that my EF Core / Hibernate queries actually generate valid SQL against a real database or that my API controller routing and security filters work end-to-end.

### Q5: How do you practice TDD when requirements are unclear or changing quickly in an Agile sprint?
* **Answer:** I start by writing tests for the core, non-negotiable business rules and happy-path contracts first, using interfaces to keep the implementation flexible. If requirements shift mid-sprint, those existing tests act as a living safety net—they immediately tell me which old assumptions broke so I can update the tests first and refactor the code with zero fear of regression.

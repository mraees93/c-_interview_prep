# C# Clean Naming Conventions

### 🟢 Interfaces (Role-Based)
Name interfaces after their **abstract role or capability**, never after a specific technology, database, or medium.
* **The Formula:** `I` + [Data being touched] + [The Job/Action]
* **Good:** `INoticeSender`, `IDocumentRepository`, `IPriceCalculator`
* **Bad:** `IEmailSender`, `ISqlDatabase`, `IExcelWriter`

### 🔵 Concrete & Abstract Classes (The Standard Extension Convention)
Re-use the abstract base name as the suffix for your concrete classes to keep your architecture uniform and easy to read.

* **The Abstract Class:** Name it using the pure, high-level noun representing the base concept.
  * *Examples:* `Discount`, `Document`, `Notice`
* **The Concrete Class:** Simply extend that exact same name by adding a descriptive prefix to show *how* it implements the rule.
  * *Examples:* `FlatTenPercentDiscount`, `SignedStatuteDocument`, `SmtpNotice`

### ⚡ Summary Reference Matrix

| Architectural Slot | Naming Pattern | Example |
| :--- | :--- | :--- |
| **Interface** | `I` + Role Name | `IAuditLogWriter` |
| **Abstract Class** | Generic Noun | `LogWriter` |
| **Class (File Implementation)** | Technology + Abstract Suffix | `LocalFileLogWriter` |
| **Class (Cloud Implementation)** | Cloud Provider + Abstract Suffix | `AzureBlobLogWriter` |
| **Class (Mock/Test Implementation)** | `Mock` + Abstract Suffix | `MockLogWriter` |

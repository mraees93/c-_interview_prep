# C# Clean Naming Conventions

### 🟢 Interfaces (Role-Based)
Name interfaces after their **abstract role or capability**, never after a specific technology, database, or medium.
* **The Formula:** `I` + [Data being touched] + [The Job/Action]
* **Good:** `INoticeSender`, `IDocumentRepository`, `IPriceCalculator`
* **Bad:** `IEmailSender`, `ISqlDatabase`, `IExcelWriter`

### 🔵 Abstract & Concrete Classes (Choose ONE Style)

#### Style A: The Pure Noun Suffix Pattern (Clean Semantics)
* **Abstract Parent:** Pure, high-level generic noun. 
  * *Examples:* `Discount`, `Document`, `Notice`
* **Concrete Child:** [Descriptive Prefix] + [Parent Noun Suffix]
  * *Examples:* `FlatTenPercentDiscount`, `SignedStatuteDocument`, `SmtpNotice`

#### Style B: The Suffix Anchor Pattern (Enterprise Visual Scan)
* **Abstract Parent:** Generic Noun + `Base` suffix.
  * *Examples:* `DiscountBase`, `DocumentBase`, `NoticeBase`
* **Concrete Child:** [Descriptive Prefix] + [Generic Noun Suffix]
  * *Examples:* `FlatTenPercentDiscount`, `SignedStatuteDocument`, `SmtpNotice`

### ⚡ Summary Reference Matrix (Style A vs Style B)

| Architectural Slot | Style A (Pure Noun) | Style B (Suffix Anchor) |
| :--- | :--- | :--- |
| **Interface** | `IAuditLogWriter` | `IAuditLogWriter` |
| **Abstract Class** | `LogWriter` | `LogWriterBase` |
| **Concrete Implementation** | `LocalFileLogWriter` | `LocalFileLogWriter` |
| **Cloud Implementation** | `AzureBlobLogWriter` | `AzureBlobLogWriter` |
| **Mock / Test Class** | `MockLogWriter` | `MockLogWriter` |

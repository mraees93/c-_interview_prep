# SQL Schema Design Scenarios - Interview Preparation

This module tracks relational database design patterns, normalization rules, indexing strategies, and transactional consistency constraints evaluated by technical panels.

---

## 1. Many-to-Many Modeling & Entity Resolution

### The Panel Scenario
LexisNexis needs to track legal **Matters** (court cases) and **Lawyers**. 
* A Matter can have multiple Lawyers working on it over time.
* A Lawyer can be assigned to multiple distinct Matters simultaneously.
* We must track the precise date a lawyer was assigned to a matter and their specific role on that case (e.g., "Lead Counsel", "Researcher").

### The Solution Blueprint (The Junction Table Pattern)
To resolve a many-to-many relationship in a relational database, you must introduce a third table called a **Junction (or Bridge) Table**. This moves the architecture into **two distinct one-to-many relationships**, allowing you to store payload data specific to the relationship itself.

```sql
-- 1. Strong Entity Table
CREATE TABLE Lawyers (
    LawyerId INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(150) NOT NULL,
    Department VARCHAR(100) NOT NULL
);

-- 2. Strong Entity Table
CREATE TABLE Matters (
    MatterId INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(250) NOT NULL,
    DateOpened DATE NOT NULL,
    Status VARCHAR(20) DEFAULT 'Active'
);

-- 3. Junction Table (Resolves Many-to-Many and stores metadata)
CREATE TABLE MatterAssignments (
    MatterId INT NOT NULL,
    LawyerId INT NOT NULL,
    AssignmentDate DATE NOT NULL DEFAULT GETDATE(),
    Role VARCHAR(50) NOT NULL,
    
    -- Composite Primary Key prevents duplicate active mappings
    CONSTRAINT PK_MatterAssignments PRIMARY KEY (MatterId, LawyerId),
    
    -- Foreign Keys guarantee Referential Integrity
    CONSTRAINT FK_Assignments_Matters FOREIGN KEY (MatterId) REFERENCES Matters(MatterId) ON DELETE CASCADE,
    CONSTRAINT FK_Assignments_Lawyers FOREIGN KEY (LawyerId) REFERENCES Lawyers(LawyerId) ON DELETE CASCADE
);
```

### Panel Talking Points
*   **Composite Primary Key:** Point out that `PRIMARY KEY (MatterId, LawyerId)` is a deliberate choice. It naturally blocks a user from accidentally assigning the exact same lawyer to the exact same case twice, maintaining strict transactional data hygiene at the engine level.
*   **Index Optimization:** Explain that SQL Server automatically builds a clustered index on the primary key, optimized for queries filtering by `MatterId`. However, to optimize queries looking up *what cases a specific lawyer is holding*, you should append an explicit non-clustered index on the reverse column sequence:
    `CREATE NONCLUSTERED INDEX IX_MatterAssignments_LawyerId ON MatterAssignments(LawyerId);`

---

## 2. Self-Referencing Hierarchies (Adjacency List Pattern)

### The Panel Scenario
You are designing a database structure to model corporate law firm organizational structures. A **Firm Department** can contain multiple child sub-departments (e.g., "Litigation" contains "Commercial Litigation" and "Personal Injury Litigation"). The nesting hierarchy can go deep.

### The Solution Blueprint (Adjacency List Pattern)
Instead of creating separate tables for different levels of departments (which breaks flexibility), use a **Self-Referencing Foreign Key** where a row points directly back to another row within its own table.

```sql
CREATE TABLE Departments (
    DepartmentId INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName VARCHAR(100) NOT NULL,
    
    -- Nullable field: Top-level divisions will have a NULL ParentId
    ParentDepartmentId INT NULL,
    
    CONSTRAINT FK_Departments_Self FOREIGN KEY (ParentDepartmentId) 
        REFERENCES Departments(DepartmentId)
);
```

### Panel Talking Points
*   **Recursive Querying (CTEs):** Explain that navigating this self-referencing schema efficiently requires writing a **Recursive Common Table Expression (CTE)** to walk up or down the structural tree inside a single database round-trip:
```sql
WITH RecursiveDepartments AS (
    -- Anchor Member: Find the top-level parent
    SELECT DepartmentId, DepartmentName, ParentDepartmentId, 1 AS HierarchyLevel
    FROM Departments
    WHERE ParentDepartmentId IS NULL
    
    UNION ALL
    
    -- Recursive Member: Join the child nodes back to the anchor
    SELECT d.DepartmentId, d.DepartmentName, d.ParentDepartmentId, r.HierarchyLevel + 1
    FROM Departments d
    INNER JOIN RecursiveDepartments r ON d.ParentDepartmentId = r.DepartmentId
)
SELECT * FROM RecursiveDepartments;
```

---

## 3. Historical Data Tracking & Audit Auditing

### The Panel Scenario
Legal statutes and compliance documents change frequently. LexisNexis requires a database schema that does not overwrite old versions of laws when updates occur. We must be able to view what a specific document looked like at any exact point in history.

### The Solution Blueprint (System-Versioned Temporal Tables)
While you can design custom audit trail tables manually with triggers, modern MS SQL Server (the primary engine requested in the job description) provides a built-in engine called **System-Versioned Temporal Tables**.

```sql
CREATE TABLE LegalDocuments (
    DocId INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(250) NOT NULL,
    ContentText NVARCHAR(MAX) NOT NULL,
    
    -- Mandatory temporal tracking datetime columns
    ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
    ValidTo DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
    
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LegalDocumentsHistory));
```

### Panel Talking Points
*   **Native Engine Optimization:** Explain that by declaring `SYSTEM_VERSIONING = ON`, the SQL engine automatically moves modified or deleted records out of the primary table into a dedicated, read-only shadow history table (`LegalDocumentsHistory`).
*   **Time-Travel Queries:** Show that developers can execute "time-travel" queries instantly using native T-SQL syntax without needing complex, performance-heavy custom timestamp tracking filters:
    `SELECT * FROM LegalDocuments FOR SYSTEM_TIME AS OF '2025-06-01 12:00:00' WHERE DocId = 101;`

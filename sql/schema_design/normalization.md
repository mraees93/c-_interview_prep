# Normalization Explained

Normalization is the process of organizing data in a db to eliminate redundancy. Think of breaking down a large complex table into smaller tables while maintaining data relationships. It enhances data integrity which ensures that data remains consistent and accurate.

### Normal Forms

There are six primary normal forms, but in practice, most systems aim for the third normal form (3NF).

If the interviewer asks about normalization, they are usually testing if you know where to stop.

"We usually normalize to 3NF to ensure data integrity and avoid redundancy. Going further into 4NF or 5NF often makes the schema too complex and slows down performance due to the high number of joins required."

| Normal Form | Main Rule Enforced | Problem Solved |
| :--- | :--- | :--- |
| **1NF (First)** | **Atomicity:** Each cell must contain a single, indivisible value; no repeating groups. | Eliminates multi-valued attributes and repeating columns. |
| **2NF (Second)** | **Full Functional Dependency:** Every non-key column must depend on the entire primary key. | Removes partial dependencies (where data depends on only part of a composite key). |
| **3NF (Third)** | **No Transitive Dependency:** Non-key columns must depend only on the primary key, not on other non-key columns. | Eliminates indirect relationships that cause redundant updates. |
| **BCNF (3.5NF)** | **Determinant is Superkey:** Every determinant in a functional dependency must be a candidate key. | Fixes anomalies in tables with multiple overlapping candidate keys. |


# Relational Database Relationships: Analogies & Structures

This guide details how One-to-Many and Many-to-Many relationships are structured in a production relational database, using clear analogies, table mappings, and SQL definitions.

---

## 1. The One-to-Many (1:M) Relationship

### The Analogy: The Court & Case Files
Think of a **High Court Room** and **Legal Case Files**. 
* A specific Court Room can host **many** different Case Files over a month.
* However, a single, specific Case File can only belong to **one** primary Court Room at any given time to avoid jurisdictional conflicts.

### The Database Structure
In a One-to-Many relationship, the unique primary key of the "One" side is placed as a **Foreign Key** inside the "Many" side table.

```sql
-- The "One" Side Table
CREATE TABLE CourtRooms (
    CourtRoomId INT IDENTITY(1,1) PRIMARY KEY, -- Primary Key
    RoomNumber VARCHAR(10) NOT NULL,
    FloorNumber INT NOT NULL
);

-- The "Many" Side Table
CREATE TABLE CaseFiles (
    CaseId INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(250) NOT NULL,
    DateFiled DATE NOT NULL,
    
    -- FOREIGN KEY POINTING TO THE "ONE" SIDE
    -- Every Case File belongs to exactly ONE Court Room.
    -- Multiple Case Files can reference the exact same CourtRoomId.
    CourtRoomId INT NOT NULL, 
    
    CONSTRAINT FK_CaseFiles_CourtRooms FOREIGN KEY (CourtRoomId) 
        REFERENCES CourtRooms(CourtRoomId)
);
```

---

## 2. The Many-to-Many (M:N) Relationship

### The Analogy: Judges & Legal Statutes
Think of **Judges** and **Legal Statutes** (Acts of Parliament).
* A single Judge will cite and interpret **many** different Statutes throughout their career.
* A single Legal Statute can be cited and interpreted by **many** different Judges across separate legal judgments.
* *The Data Intersection:* We cannot place a foreign key directly in either table without breaking normalization rules (1NF atomicity). We also want to track *why* a specific judge used a specific statute (e.g., the date of citation).

### The Database Structure
To resolve a Many-to-Many relationship, you must implement a **Junction (Bridge) Table** that breaks the architecture down into two clean, back-to-back One-to-Many relationships.

```sql
-- Entity Table 1 (The first "Many" target)
CREATE TABLE Judges (
    JudgeId INT IDENTITY(1,1) PRIMARY KEY, -- Primary Key
    FullName VARCHAR(150) NOT NULL,
    AppointmentYear INT NOT NULL
);

-- Entity Table 2 (The second "Many" target)
CREATE TABLE Statutes (
    StatuteId INT IDENTITY(1,1) PRIMARY KEY, -- Primary Key
    StatuteCode VARCHAR(50) NOT NULL, -- e.g., "Act 108 of 1996"
    OfficialTitle VARCHAR(250) NOT NULL
);

-- The Junction Table (Resolves the Many-to-Many architecture)
CREATE TABLE JudgeStatuteCitations (
    JudgeId INT NOT NULL,
    StatuteId INT NOT NULL,
    CitationDate DATETIME NOT NULL DEFAULT GETDATE(), -- Relationship Metadata
    ParagraphReference VARCHAR(50) NOT NULL,          -- Relationship Metadata
    
    -- COMPOSITE PRIMARY KEY
    -- Enforces that a specific Judge can map to a specific Statute, 
    -- but blocks identical duplicate active mappings to maintain 2NF integrity.
    CONSTRAINT PK_JudgeStatuteCitations PRIMARY KEY (JudgeId, StatuteId),
    
    -- TWO FOREIGN KEYS POINTING BACK TO THE ENTITY TABLES
    -- This turns the M:N layout into two clean 1:M relationships under the hood.
    CONSTRAINT FK_Citations_Judges FOREIGN KEY (JudgeId) REFERENCES Judges(JudgeId),
    CONSTRAINT FK_Citations_Statutes FOREIGN KEY (StatuteId) REFERENCES Statutes(StatuteId)
);
```

---

### 🚀 When to Use Denormalization

* **Read-Heavy Architectures:** Use when read volumes vastly outnumber write operations (e.g., 95:5 read-to-write ratio).
* **High-Scale NoSQL Systems:** Mandatory in Document or Key-Value stores to avoid complex, distributed runtime application joins.
* **Heavy Aggregations:** Use when queries constantly compute expensive runtime metrics like `SUM()`, `AVG()`, or `COUNT()` over millions of rows.
* **Reporting & OLAP Warehouses:** Standard practice in data warehouses (Star/Snowflake schemas) to build rapid-fire dashboard reporting layers.
* **Complex Multi-Table Joins:** Apply when a critical user path requires joining 5+ large tables, crippling application response times.
* **Historical Snapshots:** Use to lock in point-in-time facts (e.g., saving the exact price and address on an `Invoice` table, even if the product price or user profile changes later).


### ⚠️ Production Traps & Trade-offs of Denormalization

* **Write Amplification:** A single user update (e.g., changing a username) forces the system to fire hundreds of background writes to sync duplicate data across other tables.
* **Eventual Consistency Lag:** Because data isn't updated in one central spot, different screens in your application will show mismatched data until background sync jobs catch up.
* **Data Corruption & Drift:** If a worker thread fails or an update query crashes halfway through, your duplicated records will become permanently out of sync.
* **Massive Storage Bloat:** Storing text strings like `CustomerName` or `ItemTitle` thousands of times instead of a simple 4-byte `INT` foreign key causes database file sizes to skyrocket.
* **Loss of Single Source of Truth:** Debugging data anomalies becomes incredibly difficult because you can no longer tell which record holds the definitive, accurate value.
* **Code Complexity Overhead:** Your application code becomes highly complex, requiring transactional hooks, message queues, or event-driven pipelines just to keep disparate tables mirrored.

# SQL Schema Design & Normalization - Folder Guide

This document defines the core focus areas of the `sql/schema_design/` subfolder, provides technical interview strategy tips, and confirms how the existing scenarios map to database normalization theory.

---

## 1. Purpose of This Folder

The `sql/schema_design/` directory consolidates critical relational database blueprints needed to pass intermediate-to-senior technical screens. It focuses entirely on data layout safety, performance boundaries, and storage layer structural choices before any C# code or ORM engine interacts with the database.

### Core Theoretical Alignment: Normalization
All architectural scenarios detailed in `interview_q.md` fall directly under the discipline of **Database Normalization** (specifically 1NF, 2NF, and 3NF compliance):
*   **First Normal Form (1NF):** Enforces atomic columns and prevents repeating groups or native array stores inside single table attributes.
*   **Second Normal Form (2NF):** Resolves partial key dependencies. The **Junction Table Pattern** used in Scenario 1 establishes explicit key boundaries, ensuring non-key attributes depend on the complete composite primary key.
*   **Third Normal Form (3NF):** Eliminates transitive fields, ensuring attributes depend strictly on the key, the whole key, and nothing but the key. Adhering to 3NF blocks data duplication and prevents update/delete anomalies.

---

## 2. Structural Content Blueprint

The accompanying `interview_q.md` file isolates three highly tested structural patterns required for processing multi-million row transactional frameworks:

1.  **Many-to-Many Modeling & Entity Resolution:** Utilizes an optimized bridge table configuration with explicit index optimizations to resolve complex intersections without data duplication.
2.  **Self-Referencing Hierarchies (Adjacency List Pattern):** Implements localized relational tree pointers, leveraging Recursive Common Table Expressions (CTEs) to navigate nested department graphs within a single server round-trip.
3.  **Historical Data Tracking & Audit Pipelines:** Replaces legacy, performance-heavy custom database triggers with native, system-versioned MS SQL temporal history partitions.

---

## 3. High-Yield Interview Execution Tips

When a technical panel hands you an open-ended database design challenge, execute these defensive communication steps:

*   **Confirm Data Scale First:** Before sketching any table fields, ask: *"Are we designing this schema to handle a high-frequency transactional data stream scaling to millions of entries rapidly, or is this a lower-frequency read-heavy reporting configuration?"* This immediately highlights your architectural scope.
*   **Enforce Engine-Level Contraints:** Never assume the application server layer or an ORM like Entity Framework Core will keep the data clean. Explicitly write out `NOT NULL`, `FOREIGN KEY`, and `UNIQUE` constraints to prove you build foolproof storage boundaries.
*   **Defend Composite Indexes Intellectually:** If you implement a composite key inside a junction table, note that the column sequence matters. Explain that SQL Server optimizes the clustered index for the *first* column listed. Explicitly tell the panel you are adding a secondary non-clustered index on the reverse column order to preserve $O(\log N)$ index seek speeds for dual-directional queries.
*   **Treat Denormalization as a Cost-Based Trade-off:** If the panel asks when you would purposefully break 3NF boundaries, answer using performance metrics: *"I only introduce controlled redundancy if we hit a severe read bottleneck where deep multi-table JOIN operations are slowing down our indexing engine, and the cost of write-overhead sync is lower than the read penalty."*

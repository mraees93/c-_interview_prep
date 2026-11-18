### Relational Data Modeling: Active Recall Notes

#### 1. Schema Design & Relationships
* **1: Suppose we need to model users, roles, and permissions where a user can have multiple roles and a role can have multiple permissions. How would you design the tables?**
  * **A:** Create junction (bridge) tables (`UserRoles`, `RolePermissions`) with composite primary keys and foreign key constraints to cleanly resolve Many-to-Many (`M:N`) relationships.
  * **A2:** create bridge/junction tables with a composite primary key(to avoid duplicates and maintain 2NF integrity). Add foreign key constraints. This way we create 2 clean back-to-back 1 to many relationships to cleanly resolve the many-to-many relationships.

THREE Core Entity Tables: Users, Roles, and Permissions

• TWO Junction/Bridge Tables:
1. UserRoles (UserID, RoleID) — resolves the User-to-Role many-to-many.
2. RolePermissions (RoleID, PermissionID) — resolves the Role-to-Permission many-to-many.

*Note:* Each junction table decomposes a Many-to-Many relationship into two 1-to-Many relationships—so across UserRoles and RolePermissions, we end up with four clean 1-to-Many links connecting our three core entity tables.

* **1.1: What else might you add to those junction tables in a real enterprise system?**
* **A:** Audit Metadata (keyword: "relationship metadata" (or "relationship attributes")): Adding columns like AssignedDate or AssignedByUserID directly onto the UserRoles junction table so compliance teams can track when and by whom a user was granted specific access rights.

---

* **Q: How do you handle soft deletes in a relational database, and what edge cases do they create with unique constraints?**
  * **A:** Use an `IsDeleted` boolean or `DeletedAt` timestamp column instead of `DELETE`. A standard `UNIQUE` constraint on an email column will block a soft-deleted user from re-registering unless you use a filtered/partial unique index (`WHERE DeletedAt IS NULL`).

*Bonus points:*
1. Why you prefer DeletedAt over IsDeleted: Mention that you prefer DeletedAt because a nullable timestamp gives you both the active/deleted state and an audit trail of when the deletion happened in a single column.
2. Application-layer awareness: Mention that in Java/Spring (or any backend), you enforce this globally using an ORM filter (like Hibernate's @SQLRestriction("deleted_at IS NULL")) so developers don't accidentally forget WHERE DeletedAt IS NULL in manual queries.

---

* **3: When would you choose a UUID/GUID over an auto-incrementing integer (`INT`/`BIGINT`) for a primary key?**

  * **A:** UUIDs prevent ID enumeration in public APIs and allow distributed services to generate IDs without database collisions, while auto-incrementing integers keep clustered indexes smaller, sequential, and faster for heavy joins.

* **3.1 Why do random UUIDs hurt clustered indexes?**

* **Page Splits & Fragmentation (`UUIDv4` vs. `UUIDv7`):** Random `UUIDv4` inserts out of order, causing B-Tree page splits and bloating non-clustered indexes; time-ordered **`UUIDv7`** (or `NEWSEQUENTIALID()`) keeps inserts sequential.
* **The Hybrid Pattern:** Use an internal **`BIGINT` clustered PK** for fast joins and expose a separate **`UUID`** for public REST APIs.

---

#### 2. Normalization vs. Denormalization
* **Q: Walk me through 1NF, 2NF, and 3NF using a simple example like an e-commerce `Orders` table.**
  * **A:** Split repeating item groups into `OrderItems` (**1NF**), move product names/descriptions into a `Products` table so they don't depend on only part of a composite key (**2NF**), and move customer address details into a `Customers` table rather than storing them redundantly on every order (**3NF**).







  

TODO:

* **Q: Can you give an example of when you would intentionally denormalize a table?**
  * **A:** Store a snapshot of `UnitPriceAtPurchase` directly on the `OrderItems` row so historical invoices don't change when a product's current price updates, or pre-calculate summary totals for heavy reporting dashboards.
  * **Why:** Normalization prioritizes write consistency, whereas intentional denormalization protects historical immutability (freezing values at the time of a transaction) and pre-calculates aggregates—trading extra write-maintenance complexity for faster, join-free reads on heavy dashboards.

#### 3. Indexing & Query Performance
* **Q: A query filtering by `Status` and `CreatedDate` is taking 4 seconds on a table with 10 million rows. How do you fix it, and does column order in a composite index matter?**
  * **A:** Add a composite non-clustered index, and **yes, column order matters**—put the equality filter (`Status`) first and the range filter (`CreatedDate`) second so the database engine can seek directly into the matching status slice before scanning the date range.
* **Leftmost Prefix Rule & "Equality First, Range Second":**
  * Composite indexes `(A, B, C)` are sorted strictly left-to-right—the engine can seek on `(A)`, `(A, B)`, or `(A, B, C)`, but cannot seek if a leading column is skipped (e.g., querying only `B` or `C`).
  * **Equality First, Range Second:** A range filter (`>`, `<`, `BETWEEN`) halts B-Tree seeking for every column to its right, so always place `=` columns before range columns.

* **Q: Why wouldn't we just put an index on every column in a table?**
  * **A:** Every index consumes disk/memory and slows down `INSERT`, `UPDATE`, and `DELETE` operations because the database must update every B-Tree structure whenever data changes.

#### 4. Data Integrity & Concurrency
* **Q: What is the difference between an `INNER JOIN` and a `LEFT JOIN`, and how can a `WHERE` clause accidentally turn a `LEFT JOIN` into an `INNER JOIN`?**
  * **A:** `INNER JOIN` returns only matching rows across both tables, while `LEFT JOIN` returns all rows from the left table plus matches from the right (or `NULL` if none exist). Filtering on a column from the right-hand table inside the `WHERE` clause (e.g., `WHERE right_table.status = 'Active'`) eliminates all `NULL` rows—that filter belongs in the `ON` clause instead.

* **Q: Two users try to book the last available seat at the exact same millisecond. How do you prevent double-booking at the database level?**
  * **A:** Wrap the check-and-update inside an **ACID transaction** using either **optimistic locking** (a `RowVersion` column that fails if modified concurrently) or **pessimistic locking** (`SELECT ... FOR UPDATE`).
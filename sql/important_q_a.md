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







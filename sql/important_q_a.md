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
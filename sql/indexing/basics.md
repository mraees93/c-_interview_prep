### 🗄️ Why a Table Can Only Have One Clustered Index?

When you create a Clustered Index, the database engine takes the raw, unordered rows (a "Heap") and physically organizes them into a balanced tree (B-Tree) structure. The leaf nodes (the bottom layer) of a Clustered Index B-Tree do not contain pointers to the data—they contain the actual, full-width data rows of the table.
Because a table cannot exist in two separate physical places or sequences at once, you are structurally capped at one Clustered Index.

* **The Index IS the Table:** A clustered index does not point to data; its leaf nodes **are** the physical data rows on disk.
* **Physical Storage Limitation:** Data blocks on a physical hard drive or SSD can only be arranged in **one order** at any given moment. 
* **The Sort Restriction:** You cannot physically sort the exact same rows simultaneously by `LastName` and by `EmployeeID` on the storage hardware.
* **The Book Analogy:** A phone book can only be bound in one physical order (alphabetically by name). To search by phone number instead, you need a separate, external index leaflet (a Non-Clustered index) pointing back to the main pages.

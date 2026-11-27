### ⚡ Complete Indexing Cheatsheet, Analogies & Code Fixes

#### 1. Clustered vs. Non-Clustered (The Book Analogy)
* **Clustered Index = The Actual Book:** The pages are physically sorted in order on disk (e.g., by `ID`). When you find the page, you are already holding the entire row of data. You can only sort a physical book one way, so **max 1 Clustered Index per table**.
* **Non-Clustered Index = The Index at the Back of the Book:** A separate lookup list sorted by another column (e.g., `Email`). It gives you the page number (pointer), forcing the engine to flip back to the main table (**Key Lookup**) if extra columns are needed.

#### 2. Seek vs. Scan (Finding a Word in the Book)
* **Seek (Fast):** Jumping straight to the `"S"` section of the index via the B-Tree to find `"Smith"`.
* **Scan (Slow):** Reading every single page from start to finish because a function or leading wildcard hid the sorted order.
* **❌ Slow (Index Scan):**
  ```sql
  SELECT * FROM LegalDocuments WHERE YEAR(CreatedDate) = 2026;
  SELECT * FROM Lawyers WHERE FullName LIKE '%Smith';
  ```
* **✅ Fast (Index Seek):**
  ```sql
  SELECT * FROM LegalDocuments WHERE CreatedDate >= '2026-01-01' AND CreatedDate < '2027-01-01';
  SELECT * FROM Lawyers WHERE FullName LIKE 'Smith%';
  ```

#### 3. Covering Index (`INCLUDE` — Stopping the Extra Trip)
* **Analogy:** Instead of making you flip from the index at the back of the book to the main chapter just to check a phone number, you write the phone number right next to the name in the index itself.
* **Mechanic:** Appends extra `SELECT` columns onto the non-clustered index leaf nodes, eliminating the extra trip (**Key Lookup**) back to the main table.
* **❌ Slow (Seek + Key Lookup):**
  ```sql
  CREATE INDEX IX_Lawyers_DeptID ON Lawyers(DeptID);
  SELECT FullName, BillingRateZAR FROM Lawyers WHERE DeptID = 5;
  ```
* **✅ Fast (Pure Index Seek):**
  ```sql
  CREATE INDEX IX_Lawyers_DeptID_Covering ON Lawyers(DeptID) INCLUDE (FullName, BillingRateZAR);
  ```

#### 4. Composite Index Rules `(A, B, C)` (The Phonebook Analogy)
* **Analogy:** A phonebook sorted by `(LastName, FirstName)` is globally sorted by `LastName` first. All `"John"` entries are only alphabetized *inside* `"Smith"`—they are scattered across the rest of the book.
* **Leftmost Prefix Rule:** Sorted strictly left-to-right—can seek on `(A)`, `(A, B)`, or `(A, B, C)`, never if a leading column is skipped (e.g., querying only `B` or `C`).
* **Equality First, Range Second:** Put `=` filters before range filters (`>`, `<`, `BETWEEN`), because a range filter stops B-Tree seeking for every column to its right.
* **❌ Slow (Partial Seek — Range First):**
  ```sql
  CREATE INDEX IX_Docs_Bad ON LegalDocuments(CreatedDate, DocType);
  SELECT DocID FROM LegalDocuments WHERE DocType = 'Contract' AND CreatedDate >= '2026-01-01';
  ```
* **✅ Fast (Full Seek — Equality First):**
  ```sql
  CREATE INDEX IX_Docs_Good ON LegalDocuments(DocType, CreatedDate);
  ```

#### 5. Filtered Index & The Write Trade-Off
* **The Write Trade-Off:** Every index speeds up `SELECT` reads, but slows down `INSERT`, `UPDATE`, and `DELETE` writes because every index must be updated too.
* **Filtered Index Fix:** Keep indexes tiny and fast on writes by indexing only the active subset your query cares about.
* **❌ Slow / Bloated:**
  ```sql
  CREATE INDEX IX_Logs_Status ON DocumentAccessLogs(Status);
  ```
* **✅ Fast / Tiny:**
  ```sql
  CREATE INDEX IX_Logs_Pending ON DocumentAccessLogs(AccessTimestamp) WHERE Status = 'Pending';
  ```

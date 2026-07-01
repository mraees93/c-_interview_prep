### 🏡 Lexis Convey Schema

#### 1. `DeedsOffices` Table

| Column Name | Data Type | Key Type | Description |
| :--- | :--- | :--- | :--- |
| `OfficeID` | `INT` | Primary Key | Unique ID for the deeds office. |
| `OfficeName` | `VARCHAR(50)` | - | Regional office name. |
| `Province` | `VARCHAR(50)` | - | Province location. |

**Sample Data:**

| OfficeID | OfficeName | Province |
| :--- | :--- | :--- |
| 1 | Cape Town | Western Cape |
| 2 | Johannesburg | Gauteng |
| 3 | Pietermaritzburg | KwaZulu-Natal |

#### 2. `PropertyTransfers` Table

| Column Name | Data Type | Key Type | Description |
| :--- | :--- | :--- | :--- |
| `TransferID` | `INT` | Primary Key | Unique ID for the transfer file. |
| `OfficeID` | `INT` | Foreign Key | References `DeedsOffices(OfficeID)`. |
| `PurchasePrice` | `DECIMAL(12,2)` | - | The sale price of the property in ZAR. |
| `Status` | `VARCHAR(30)` | - | Current stage: 'Lodged', 'Registered', 'Rejected'. |

**Sample Data:**

| TransferID | OfficeID | PurchasePrice | Status |
| :--- | :--- | :--- | :--- |
| 8001 | 1 | 2500000.00 | Registered |
| 8002 | 1 | 1850000.00 | Lodged |
| 8003 | 2 | 4200000.00 | Registered |

#### 3. `TransferDocuments` Table

| Column Name | Data Type | Key Type | Description |
| :--- | :--- | :--- | :--- |
| `DocumentID` | `INT` | Primary Key | Unique ID for the document. |
| `TransferID` | `INT` | Foreign Key | References `PropertyTransfers(TransferID)`. |
| `DocType` | `VARCHAR(50)` | - | Type of legal document. |
| `IsApproved` | `BOOLEAN` | - | True if verified by the conveyancer. |

**Sample Data:**

| DocumentID | TransferID | DocType | IsApproved |
| :--- | :--- | :--- | :--- |
| 9501 | 8001 | Power of Attorney | TRUE |
| 9502 | 8001 | Rates Clearance | TRUE |
| 9503 | 8002 | Title Deed Draft | FALSE |

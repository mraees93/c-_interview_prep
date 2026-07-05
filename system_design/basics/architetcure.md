# LexisNexis Interview Study Guide: Software Architecture Matrix

## Part 1: Layered Architecture vs. Onion Architecture

### Architectural Comparison

| Evaluation Metric | Traditional Layered Architecture | Onion Architecture |
| :--- | :--- | :--- |
| **Core Architecture Principle** | Database-centric / N-Tier pipeline execution | Domain-driven / Dependency Inversion execution |
| **Dependency Vector** | Top-down (Presentation → Business → Data Access) | Inward (Outer rings depend exclusively on inner rings) |
| **Database Coupling Status** | Foundation layer; business rules explicitly depend on it | Infrastructure detail; exists on the outermost ring |
| **Code Abstraction Level** | Low; high coupling to object-relational mappers (ORMs) | High; domain layer contains zero framework code |
| **Unit Testing Strategy** | Complex; requires heavy database mocking/stubbing | Simple; fast unit testing of pure domain logic |
| **Component Swappability** | Rigid; UI or DB modifications trigger cascading breaks | Fluid; adapters change without impacting the core domain |

---

### Core Structural Layouts

#### Traditional Layered Flow
```mermaid
graph LR
    Pres[Presentation Layer] --> Biz[Business Logic Layer]
    Biz --> Data[Data Access Layer]
    Data --> DB[(Database)]
    
    style Pres fill:#f9f,stroke:#333,stroke-width:2px,color:#000
    style Biz fill:#bbf,stroke:#333,stroke-width:2px,color:#000
    style Data fill:#ddf,stroke:#333,stroke-width:2px,color:#000
    style DB fill:#fff,stroke:#333,stroke-width:2px,color:#000
```

#### Onion Architecture (Concentric Dependencies)
```mermaid
graph RL
    Infra[Infrastructure / UI Layer] --> App[Application Services]
    App --> DomServ[Domain Services]
    DomServ --> Core((Domain Model Core))
    
    style Infra fill:#fff,stroke:#333,stroke-width:2px,color:#000
    style App fill:#fdd,stroke:#333,stroke-width:2px,color:#000
    style DomServ fill:#fbb,stroke:#333,stroke-width:2px,color:#000
    style Core fill:#f99,stroke:#333,stroke-width:4px,color:#000
```

---

### 💡 High-Yield Interview Notes (Layered vs. Onion)

* **Dependency Inversion Principle (DIP):** Onion architecture is the enterprise implementation of DIP. The core domain layer declares the abstractions (interfaces), while the outer infrastructure layer implements those interfaces.
* **LexisNexis Application Context:** LexisNexis applications manage extensive text indexes, public records, and legal graphs. Onion architecture allows you to change the storage implementation (e.g., migrating from an on-premise relational database to an AWS cloud-managed NoSQL store) by updating only the infrastructure layer, leaving complex legal evaluation logic untouched.
* **Database-First vs. Domain-First:** Traditional layered systems require designing database tables first. Onion architecture forces a Domain-First approach where business entities and rules are modelled first, entirely decoupled from data storage considerations.

---

## Part 2: Monolithic vs. Modular Monolithic vs. Microservices

### Ultimate Architectural Comparison Matrix

| Evaluation Metric | Monolithic | Modular Monolithic | Microservices |
| :--- | :--- | :--- | :--- |
| **Deployment Unit** | Single application artifact | Single application artifact | Multiple independent network artifacts |
| **Runtime Isolation** | None; all logic runs in one process | None; modules share one process | Complete; services run in distinct processes |
| **Code Base Structure** | Unified single codebase repository | Managed modules inside one repository | Isolated individual service repositories |
| **Data Architecture** | Single shared database | Shared database with isolated schemas | Strict database-per-service isolation |
| **Inter-Component Calls** | In-memory stack execution | In-memory via public interfaces | Over-the-network (REST, gRPC, MQ) |
| **Transaction Strategy** | Local ACID transactions (Easy) | Local ACID transactions (Easy) | Distributed transactions / Saga pattern (Hard) |
| **Operational Overhead** | Low; simple CI/CD pipelines | Low; standard deployment configurations | Very high; requires service mesh and k8s |
| **Blast Radius (Failure)** | System-wide (Single fault crashes app) | System-wide (Single fault crashes app) | Isolated (Graceful degradation handles faults) |
| **Horizontal Scaling** | Uniform scaling of entire codebase | Uniform scaling of entire codebase | Granular scaling of specific bottlenecks |

---

### Deep-Dive Architectural Profiles

#### 1. Monolithic Architecture
A unified software system where all components—user authentication, payment processing, report generation, and data access—are compiled and packaged as a single deployment artifact.

* **Primary Benefits:**
  * Simple to develop, step-debug, profile, and test early in the lifecycle.
  * Zero network latency or serialization overhead between internal modules.
  * Straightforward deployments without complex orchestrators.
* **Core Disadvantages:**
  * Codebase naturally degrades into a "Big Ball of Mud" as features grow.
  * Long build times and slow deployment pipelines limit team velocity.
  * Inefficient scaling forces you to duplicate the entire app footprint to scale one bottleneck.
* **When to Deploy:**
  * Small development groups (1 to 2 agile product teams).
  * Proof of concepts (PoCs) or early validation MVPs.
  * Systems with straightforward business domains and low raw throughput.

#### 2. Modular Monolithic Architecture
A single deployment unit containing explicitly isolated logical modules. Modules enforce compile-time or package boundaries and interface strictly through clean public APIs.

* **Primary Benefits:**
  * Provides structural separation of concerns without introducing network latency.
  * Fast local developer execution loops and simple integration testing.
  * Establishes a highly structured migration path to microservices if needed later.
* **Core Disadvantages:**
  * Demands continuous code discipline and architectural linter checks to stop boundary leaks.
  * Runtime crashes still impact the whole system due to the single process model.
  * Independent module scaling remains impossible under heavy traffic loads.
* **When to Deploy:**
  * Mid-sized engineering structures (3 to 5 separate product teams).
  * Highly complex business domains that need to avoid distributed systems overhead.
  * Applications requiring clear code ownership without microservice deployment friction.

#### 3. Microservices Architecture
An architectural pattern that breaks an application into a network of autonomous, loosely coupled services. Each service owns its runtime process, data schemas, and deployment lifecycle.

* **Primary Benefits:**
  * High deployment velocity; teams deploy updates without syncing with other teams.
  * Precision scalability lets you resource-allocate specific bottlenecks independently.
  * Polyglot freedom allows picking different languages, frameworks, or databases per service.
* **Core Disadvantages:**
  * Severe operational tax spanning distributed tracing, log aggregation, and mesh routing.
  * Complex data management challenges including eventual consistency, data joins, and syncs.
  * Hard-to-trace partial failure modes caused by network partitions and timeouts.
* **When to Deploy:**
  * Large-scale software engineering organisations (dozens of distributed teams).
  * High-throughput applications with vastly different infrastructure requirements per feature.
  * Domains requiring high-velocity deployment cycles across independent teams.

---

### 💡 High-Yield Interview Notes (Monolith vs. Microservices)

* **Conway's Law:** "Organizations design systems which mirror their communication structures." Highlight that microservices are as much an **organisational solution** for scaling engineering teams as they are a technical solution for performance.
* **The Fallacy of Distributed Computing:** Avoid assuming networks are fast, reliable, or secure. Moving from a monolith to microservices trades local CPU speed for network hops, which adds latency and introduces partial network failure states.
* **LexisNexis Application Context:** For core search and indexing tools handling billions of document requests, microservices fit well (e.g., separating document parsing, search queries, and payment processing). However, for analytical internal workflows, using a modular monolith avoids complex distributed transaction overhead.

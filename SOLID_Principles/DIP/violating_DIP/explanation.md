- The Car class directly creates an instance of the Engine class, leading to a tight coupling between Car and Engine.

- If the Engine class changes, it may affect the Car class, violating the Dependency Inversion Principle.

- High-Level Class: The high-level class is typically the one that represents the main functionality or business logic of the application. It orchestrates the interaction between various components and is often more abstract in nature.

- Low-Level Class: The low-level class is usually one that provides specific functionality or services that are used by the high-level class. It typically deals with implementation details and is more concrete in nature.
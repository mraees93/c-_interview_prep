In this refactored code, we define an abstract Shape class with an abstract CalculateArea() method. Concrete shape classes (Circle and Rectangle) inherit from the Shape class and provide their own implementations of CalculateArea().

Adding a new shape, such as a triangle, would involve creating a new class – extending the codebase – that inherits from Shape and implements CalculateArea(), without modifying existing code. This adheres to the OCP by allowing for extension without modification.

Being able to add functionality without modifying existing code means that we don’t have to worry as much about breaking existing working code and introducing bugs.

Following the OCP encourages us to design our software so that we add new features only by adding new code. This helps us to build loosely-coupled, maintainable software.
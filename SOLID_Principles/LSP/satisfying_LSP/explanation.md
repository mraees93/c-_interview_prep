Shape rectangle = new Rectangle { Width = 5, Height = 4 };

Console.WriteLine($"Area of the rectangle: {rectangle.Area}");

Shape square = new Square { SideLength = 5 };

Console.WriteLine($"Area of the square: {square.Area}");

In this corrected example, we redesign the Square class to directly set the side length. Now, a Square is correctly modeled as a subclass of Shape, and it adheres to the Liskov Substitution Principle.

How does this satisfy LSP? Well, we have a superclass, Shape, and subclasses Rectangle and Square. Both Rectangle and Square maintain the correct expected behavior of a Shape — we can substitute a square for a rectangle and the area will still be calculated correctly.



on the vid there is another example of the human class, and its sub classes parent and child
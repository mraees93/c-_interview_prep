Now, let’s test out if Rectangle calculates its area correctly:

var rect = new Rectangle();
rect.Height = 10;
rect.Width = 5;

System.Console.WriteLine("Expected area = 10 * 5 = 50.");

System.Console.WriteLine("Calculated area = " + rect.Area);

output:

Expected area = 10 * 5 = 50.

Calculated area = 50

perfect!

Now, in our program, the Square class inherits from, or extends, the Rectangle class, because, mathematically, a square is just a special type of rectangle, where its height equals its width. Because of this, we decided that Square should extend Rectangle – it’s like saying “a square is a (special type of) rectangle”.

But look what happens if we substitute the Rectangle class for the Square class:

var rect = new Square();
rect.Height = 10;
rect.Width = 5;

System.Console.WriteLine("Expected area = 10 * 5 = 50.");

System.Console.WriteLine("Calculated area = " + rect.Area);

Expected area = 10 * 5 = 50.

Calculated area = 25

DAMN


Oh dear, LSP has been violated: we replaced the object of a superclass (Rectangle) with an object of its subclass (Square), and it affected the correctness of our program. By modeling Square as a subclass of Rectangle, and allowing width and height to be independently set, we violate the LSP. When setting the width and height of a Square, it should retain its squareness, but our implementation allows for inconsistency.
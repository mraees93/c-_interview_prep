In this example, we have an IShape interface representing both 2D and 3D shapes. However, the Volume() method is problematic for 2D shapes, like Circle and Rectangle, because they don't have volume. This violates the ISP because clients (classes using the IShape interface) may be forced to depend on methods they do not need.



var circle = new Circle();
circle.Radius = 10;

System.Console.WriteLine(circle.Area());
System.Console.WriteLine(circle.Volume()); // My text editor doesn't flag a problem...

var sphere = new Sphere();
sphere.Radius = 10;

System.Console.WriteLine(sphere.Area());
System.Console.WriteLine(sphere.Volume());


Usually, if I try to call a method on an object that doesn’t exist, VS Code will tell me that I’m making a mistake. But above, when I call circle.Volume(), VS code is like “no problem”. And VS code is correct, because the IShape interface forces Circle to implement a Volume() method, even though circles don’t have volume.

It’s easy to see how violating ISP can introduce bugs into a program – above, everything looks fine, until we run the program and an exception gets thrown.
To adhere to the Dependency Inversion Principle, we introduce an abstraction (interface) between Car and Engine, allowing Car to depend on an abstraction instead of a concrete implementation.

We can now inject any type of engine into Car implementations:

var engine = new Engine(); // concrete implementation to be "injected" into the car

var car = new Car(engine);

car.StartCar();
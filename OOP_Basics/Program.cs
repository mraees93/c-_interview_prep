// See https://aka.ms/new-console-template for more information
// using OOP_Basics.abstraction;
//  Vehicle myCar = new Car();
//         myCar.Start();
//         myCar.Stop();

//         IVehicle myBike = new Bike();
//         myBike.Start();
//         myBike.Stop();

// using OOP_Basics.encapsulation;
// BankAccount myAccount = new BankAccount(1000);
// myAccount.Balance = 450;
// Console.WriteLine(myAccount.Balance);
// myAccount.Deposit(500);
// myAccount.Withdraw(200);
// Console.WriteLine($"Final Balance: {myAccount.GetBalance()}");

// using OOP_Basics.polymorphism;
// Animal myAnimal = new Animal(); 
// Animal myCat = new Cat();  
// Animal myDog = new Dog(); 
//all 3 outputs: "The animal makes a sound" IF NOT using virtual on parent method and override on children methods
// because the base class method overrides the derived class method, when they share the same name.
// myAnimal.animalSound();
// myCat.animalSound();
// myDog.animalSound();

using OOP_Basics.practice;
//practice_1
// ParentDocument doc = new LegalBrief();
// System.Console.WriteLine(doc);

//practice_2
LegalEngine engine = new SpecializedSearchEngine();
engine.Process();
engine.Run();

//practice_4
// FleetCar car = new FleetCar {Color="Green"};
// FleetCar car1 = new FleetCar();
// car1.Color = "Blue";
// car1.Color = "Red";
// System.Console.WriteLine(car.Color);
// System.Console.WriteLine(FleetCar.TotalCarsCreated);

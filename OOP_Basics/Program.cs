// See https://aka.ms/new-console-template for more information
using OOP_Basics.abstraction;
 IVehicle myCar = new Car();
        myCar.Start();
        myCar.Stop();

        IVehicle myBike = new Bike();
        myBike.Start();
        myBike.Stop();

// using OOP_Basics.encapsulation;
// BankAccount myAccount = new BankAccount(1000);
//         myAccount.Deposit(500);
//         myAccount.Withdraw(200);
//         Console.WriteLine($"Final Balance: {myAccount.GetBalance()}");
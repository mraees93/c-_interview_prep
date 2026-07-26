using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOP_Basics.encapsulation
{
    public class BankAccount
    {
        private double _balance; // internal data is hidden

        public double Balance { get; set; }
        // {
        //     get { return name; }   // get method
        //     set { name = value; }  // set method
        // }
        public BankAccount (double initialBalance) //constructor
        {
            _balance = initialBalance;
        }

        public void Deposit(double amount)
        {
            if(amount > 0)
            {
                _balance += amount;
                Console.WriteLine($"Deposited: {amount}, New Balance: {_balance}");
            } else
            {
                Console.WriteLine("Deposit amount must be positive.");
            }
        }

        public void Withdraw(double amount)
        {
            if (amount > 0 && amount <= _balance)
            {
                _balance -= amount;
                Console.WriteLine($"Withdrawn: {amount}, Remaining Balance: {_balance}");
            }
            else
            {
                Console.WriteLine("Invalid withdrawal amount.");
            }
        }

        public double GetBalance() //controlled access
        {
            return _balance;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOP_Basics.encapsulation
{
    public class BankAccount
    {
        private double balance; // internal data is hidden
        public BankAccount (double initialBalance)
        {
            balance = initialBalance;
        }

        public void Deposit(double amount)
        {
            if(amount > 0)
            {
                balance += amount;
                Console.WriteLine($"Deposited: {amount}, New Balance: {balance}");
            } else
            {
                Console.WriteLine("Deposit amount must be positive.");
            }
        }

        public void Withdraw(double amount)
        {
            if (amount > 0 && amount <= balance)
            {
                balance -= amount;
                Console.WriteLine($"Withdrawn: {amount}, Remaining Balance: {balance}");
            }
            else
            {
                Console.WriteLine("Invalid withdrawal amount.");
            }
        }

        public double GetBalance() //controlled access
        {
            return balance;
        }
    }
}
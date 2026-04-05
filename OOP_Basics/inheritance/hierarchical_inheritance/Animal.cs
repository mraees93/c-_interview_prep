using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//Multiple child classes inherit from a single parent class.
//A Dog and a Cat are both Animals, but they have different behaviors.

namespace OOP_Basics.inheritance.hierarchical_inheritance
{
    public class Animal
    {
        public void Eat() => Console.WriteLine("Animal is eating...");
    }

    class Dog : Animal
    {
        public void Bark() => Console.WriteLine("Dog is barking...");
    }

    class Cat : Animal
    {
        public void Meow() => Console.WriteLine("Cat is meowing...");
    }

    class Program
    {
        static void Main()
        {
            Dog myDog = new Dog();
            myDog.Eat();
            myDog.Bark();

            Cat myCat = new Cat();
            myCat.Eat();
            myCat.Meow();
        }
    }
}
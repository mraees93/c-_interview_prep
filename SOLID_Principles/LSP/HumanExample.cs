using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.LSP
{
    public class HumanExample
    {
        // --- THE BASE CONTRACT ---
        // LSP Rule: The Parent class defines a "contract" of behaviors that 
        // ALL children are guaranteed to perform.
        public abstract class Human
        {
            public virtual void Eat() => Console.WriteLine("Human is eating...");
            public virtual void Sleep() => Console.WriteLine("Human is sleeping...");
        }

        // --- SPECIALIZED INTERFACES (Role-based) ---
        // We keep these separate so we don't force the Baby to "Work".
        // This prevents LSP violations because we aren't "lying" about Human capabilities.
        public interface IAdultActions
        {
            void Work();
            void MakeDinner();
        }

        public interface IBabyActions
        {
            void Play();
        }

        // --- CHILD CLASSES ---

        public class Parent : Human, IAdultActions
        {
            // Parent satisfies LSP by successfully implementing Human behaviors
            public override void Eat() => Console.WriteLine("Parent is eating a sandwich.");

            public void Work() => Console.WriteLine("Parent is working at the office.");
            public void MakeDinner() => Console.WriteLine("Parent is cooking pasta.");
        }

        public class Baby : Human, IBabyActions
        {
            // Baby satisfies LSP because it CAN Eat and Sleep, 
            // even if it does so differently (drinking milk).
            public override void Eat() => Console.WriteLine("Baby is drinking milk.");

            public void Play() => Console.WriteLine("Baby is playing with blocks.");
        }

        class Program
        {
            static void Main(string[] args)
            {
                // 1. SUBSTITUTION IN COLLECTIONS
                // Even though they are different types, we can treat them as 'Human'.
                // This is LSP in action: the List doesn't care about the specific child type.
                List<Human> family = new List<Human>
            {
                new Parent(),
                new Baby()
            };

                Console.WriteLine("--- Daily Routine (LSP Substitution) ---");
                foreach (var person in family)
                {
                    // We call methods defined in the Parent (Human) class.
                    // Because both classes follow LSP, this code never crashes, 
                    // regardless of whether 'person' is a Baby or a Parent.
                    ExecuteHumanBasics(person);
                }
            }

            // 2. SUBSTITUTION IN METHOD PARAMETERS
            // This method accepts a 'Human'. Because of LSP, we can pass a Parent 
            // or a Baby without this method needing to know which is which.
            static void ExecuteHumanBasics(Human human)
            {
                // LSP guarantees that any 'Human' passed here will have these methods.
                human.Eat();
                human.Sleep();
                Console.WriteLine("---------------------------------");
            }
        }
    }
}
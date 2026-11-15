/*
for inheritance what is best practice to change a parents field value in the child component, by 
changing the field value in the child constructor?

Passing the value up to the parent constructor via : base(...) is the best practice, rather than 
overwriting a field inside the child constructor body.


Constructor Chaining

Keep the state encapsulated in the base class and require derived classes to supply it upon creation:
*/
public class Animal0
{
    private readonly string _sound;

    protected Animal0(string sound)
    {
        _sound = sound;
    }

    public void Speak() => Console.WriteLine(_sound);
}

public class Dog0 : Animal0
{
    // Pass the value up instead of mutating a protected field
    public Dog0() : base("Woof") 
    { 
    }
}

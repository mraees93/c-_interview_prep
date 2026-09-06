using System;
/*
Fix this snippet so that comparing empA and empB correctly prints "Gotcha 2: Match found!" because their internal properties match.
*/
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class InterviewGotchaEngine
{
    public void GotchaTwo_ObjectComparison()
    {
        Employee empA = new Employee { Id = 2, Name = "Chantel" };
        Employee empB = new Employee { Id = 2, Name = "Chantel" };

        if (empA.Id == empB.Id && empA.Name == empB.Name) // if(empA == empB), Employee is a reference type so it will only compare memory addresses
        {
            Console.WriteLine("Gotcha 2: Match found!");
        }
        else
        {
            Console.WriteLine("Gotcha 2: No match!");
        }
    }
}

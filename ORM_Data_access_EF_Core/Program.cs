using System.Collections.Generic;
using System.Linq;

namespace ORM_Data_access_EF_Core;

class Program
{
    static void Main(string[] args)
    {
        // List<string> apps = new List<string> { "Loan", "Credit", "Savings" };
        // var query = apps.Where(a => a.StartsWith("C")); //compiler doesnt actually run the filtering logic,creates a blueprint
        // apps.Add("Crease"); //step 1
        // int count = query.Count(); //step 2 - only executes when forced to materialize data with .Count()
        // Console.WriteLine(count);

        // List<int> points = new List<int> { 10, 20, 30 };
        // var query = points.Where(p => p > 15).ToList(); //breaks deferred(delayed) execution,forces Immediate Execution.
        // points.Add(40);
        // Console.WriteLine(query.Count);
        // Console.WriteLine(points.Count);
        // points.ForEach(num => Console.WriteLine(num));

        // List<int> transactions = new List<int>();
        // int result = transactions.First(t => t > 100); //throws InvalidOperationException runtime error
        //use .FirstOrDefault() => returns default value of underlying data type. Since int is a value type, it would return 0

        // var users = new[]
        // {
        //     new { ID = 1, Role = "Admin" },
        //     new { ID = 2, Role = "User" },
        //     new { ID = 3, Role = "Guest" }
        // };
        // var target = users.Where(u => u.ID == 2 || u.Role == "Admin");
        // target.ToList().ForEach(user => Console.WriteLine(user));

        // Dictionary<string, int> scores = new Dictionary<string, int>();
        // scores.Add("TeamA", 10);
        // scores.Add("teama", 20);
        // Console.WriteLine(scores.Count);
        //Executes successfully because string keys are completely case-sensitive by default.

        // int[] salaries = { 40000, 45000, 50000 };
        // var res1 = from s in salaries where s >= 45000 select s;
        // var res2 = salaries.Where(s => s >= 45000);
        //2 statements compile down to the same method invocation path + yield identical runtime results.

        // string[] names = { "John", null, "Alex", "David" };
        // var filtered = names.Where(n => n.Length > 3);
        // Console.WriteLine(filtered.Count()); //throws a NullReferenceException error

        // List<int> ranks = new List<int> { 5, 3, 9, 1 };
        // var outcome = ranks.OrderBy(r => r).OrderByDescending(r => r);
        // Console.WriteLine(outcome.First()); // => 9

        // List<int> inventory = new List<int> { 10, 20, 30 };
        // foreach (int item in inventory)
        // {
        //     if (item == 20)
        //     {
        //         inventory.Remove(item);
        //     }
        // }
        //throws an InvalidOperationException because a collection cannot be modified while an enumerator is reading it.

        // The Standard Workaround: If a developer actually needs to filter items safely out of a list, they should use a 
        // standard for loop counting backward, or utilize a clean LINQ filtering method like .RemoveAll(item => item == 20);

        // .ToList() transforms the blueprint into an optimized query then the filtered data only crosses the network boundary into c# app memory
        // IQueryable<CaseDocument> casesA = _context.Cases;
        // var resultA = casesA.Where(c => c.Category == "Criminal").Take(10).ToList(); 

        // // IEnumerable acts as an in-memory collection pointer, the ORM translates _context.Cases to a "SELECT * FROM Cases;", then only filters that data which results in a heap OutOfMemoryException crash
        // IEnumerable<CaseDocument> casesB = _context.Cases;
        // var resultB = casesB.Where(c => c.Category == "Criminal").Take(10).ToList();

        // //MODIFYING DB DATA - Avoiding N+1 trap
        // //Avoid N+1 trap with eager loading using the .Include() extension method
        // var casesWithAttachments = _context.Cases.Include(c => c.Attachements).ToList();
        // foreach (var courtCase in casesWithAttachments)
        // {
        //     ProcessAttachements(courtCase.Attachments);
        // }

        // //READ-ONLY DB DATA - Avoiding N+1 trap
        // // Project the query into a DTO, this disables EF Core's change tracker and forces the DB to only transmit the needed columns over the network
        // public record CaseSummaryDto(string CaseId, string Title, List<string> AttachmentNames);
        // var summaryReport = _context.Cases
        //     .Select(c => new CaseSummaryDto(
        //         c.Id,
        //         c.Title,
        //         c.Attachments.Select(a => a.FileName).ToList()
        //     ))
        //     .ToList();
    }
}

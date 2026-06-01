using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexisNexisPrep.DataProjections
{
    // Mock Models for relational data structures
    public class LegalCase { public int Id { get; set; } public string Title { get; set; } public int Year { get; set; } public int JudgeId { get; set; } public Judge Judge { get; set; } }
    public class Judge { public int Id { get; set; } public string Name { get; set; } }
    
    // Data Transfer Object optimized for the client API payload
    public class CaseDto { public int Id { get; set; } public string Title { get; set; } public string JudgeName { get; set; } }
    public class AppDbContext : DbContext { public DbSet<LegalCase> LegalCases { get; set; } public DbSet<Judge> Judges { get; set; } }

    /// <summary>
    /// ANALOGY FOR FOLDER 2:
    /// ❌ N+1 Trap (Bad) = Opening a book, finding a list of 50 citations, and standing up to walk 
    ///    to the library bookshelf 50 separate times to check who wrote each individual citation.
    ///    
    ///  Data Projection (Good) = Handing a single request sheet to a copy machine clerk that says: 
    ///    "Print out a custom single-page sheet that pairs each citation name with its author side-by-side."
    /// </summary>
    public class DataProjectionService
    {
        private readonly AppDbContext _context;
        public DataProjectionService(AppDbContext context) => _context = context;

        /// <summary>
        /// PERFORMANCE FAILURE PATTERN (The Chatty Network Trap)
        /// Interview Scenario: "What is the N+1 problem, and how do you spot it in code review?"
        /// </summary>
        public async Task<List<LegalCase>> GetCasesWithNPlusOne(int filterYear)
        {
            // 1. INITIAL QUERY (The "1" in N+1): Pulls down the basic parent rows.
            // If 500 cases match this year criteria, 'cases' will contain 500 entities.
            var cases = await _context.LegalCases.Where(c => c.Year == filterYear).ToListAsync();

            // 2. THE CHATTY LOOP (The "N" in N+1): 
            // You are looping over every single parent case row in memory.
            foreach (var item in cases)
            {
                // CRITICAL STRUCTURAL ERROR: Inside the loop, you fire an isolated database ping 
                // to get the relational Judge record. If you have 500 cases, your API forces 
                // the database engine to open, parse, and execute 500 individual standalone queries.
                // This causes database connection pool starvation and high request latencies.
                item.Judge = await _context.Judges.FindAsync(item.JudgeId);
            }
            return cases;
        }

        /// <summary>
        /// ENTERPRISE PRODUCTION PATTERN (Clean Joins via DTO Projection)
        /// Interview Scenario: "How do you refactor an N+1 looping lookup to maximize performance?"
        /// </summary>
        public async Task<List<CaseDto>> GetCasesEnterpriseWay(int filterYear)
        {
            // 1. SOLVES N+1 EFFICIENCY: Utilizing .Select() instructs EF Core's translation engine
            // to generate a single, high-performance SQL "LEFT OUTER JOIN" statement under the hood.
            // 
            // 2. PREVENTS 'SELECT *': Instead of grabbing every column from the table, SQL server only 
            // extracts and yields the exact fields mapped inside the custom DTO instantiator.
            //
            // 3. IMPLICIT ASNOTRACKING: Because you are projecting rows directly into a custom DTO class 
            // instead of a tracked entity model, EF Core knows it is impossible to run an update on it.
            // It automatically disables the Change Tracker audit engines completely. It behaves 
            // exactly as if you manually typed out .AsNoTracking() yourself.
            return await _context.LegalCases
                .Where(c => c.Year == filterYear)
                .Select(c => new CaseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    JudgeName = c.Judge.Name // Resolved cleanly in a single combined database query execution pass.
                })
                .ToListAsync();
        } //LINQ Projection
    } 
}
//LINQ creates an Expression Tree which the EF Provider compiles into SQL at the ToListAsync materialisation boundary
/*
"By projecting directly into a custom DTO using .Select(), I force Entity Framework to compile a single SQL statement utilizing an explicit database join. 
This resolves the N+1 problem in a single network round-trip, eliminates SELECT * payload bloat by only fetching the columns I need, and implicitly bypasses 
EF Core's Change Tracker memory snapshots since DTOs are inherently read-only."
*/

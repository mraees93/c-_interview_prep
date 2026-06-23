using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexisNexisPrep.QueryMechanics
{
    // Mock Domain Model representing a table with millions of rows
    public class LegalCase { public int Id { get; set; } public string Title { get; set; } public int Year { get; set; } }
    public class AppDbContext : DbContext { public DbSet<LegalCase> LegalCases { get; set; } }

    /// <summary>
    /// ANALOGY FOR FOLDER 1:
    /// ❌ IEnumerable (Bad) = Driving a massive truck to a warehouse, loading MILLIONS of raw files 
    ///    into the back, driving them to your small home office, and sorting through them on your desk.
    ///    
    ///  IQueryable (Good) = Texting a highly efficient warehouse librarian a precise query. 
    ///    The librarian finds the exact 5 files you need and sends ONLY those files over to your office.
    /// </summary>
    public class QueryExecutionService
    {
        private readonly AppDbContext _context;
        public QueryExecutionService(AppDbContext context) => _context = context;

        /// <summary>
        /// PERFORMANCE FAILURE PATTERN (In-Memory Processing)
        /// Interview Scenario: "What happens if we query a database using IEnumerable?"
        /// </summary>
        public List<LegalCase> GetCasesInApplicationMemory(int filterYear)
        {
            // 1. THE TRAP: Calling .AsEnumerable() breaks the query building process immediately.
            // EF Core instantly runs "SELECT * FROM LegalCases" against SQL Server.
            // Millions of historical records stream across the network wire.
            //
            // 2. NOTE ON TRACKING: Even though .AsNoTracking() is used to stop EF Core from taking 
            // reference memory snapshots, it CANNOT fix the massive network saturation and RAM bloat 
            // caused by pulling down the entire database table.
            IEnumerable<LegalCase> rawTableStream = _context.LegalCases.AsNoTracking().AsEnumerable();

            // 3. EXECUTION OUTCOME: The SQL server does zero heavy lifting. 
            // Your web application server's CPU must loop through every single object in memory
            // to run this .Where filter. Under LexisNexis data loads, this triggers an OutOfMemoryException.
            return rawTableStream.Where(c => c.Year == filterYear).ToList();
        }

        /// <summary>
        /// ENTERPRISE PRODUCTION PATTERN (Database-Side Processing)
        /// Interview Scenario: "How do you optimize a read-only search engine query in .NET?"
        /// </summary>
        public async Task<List<LegalCase>> GetCasesOnDatabaseServer(int filterYear)
        {
            // 1. THE OPERATION: .AsQueryable() defines an expression blueprint (Abstract Syntax Tree).
            // Absolutely NO SQL is executed at this stage.
            IQueryable<LegalCase> queryBlueprint = _context.LegalCases.AsQueryable();

            // 2. THE OPTIMIZATION: .AsNoTracking() tells EF Core to bypass the Change Tracker completely.
            // It skips saving baseline copies of properties and resolving identity references.
            // This drops web server RAM usage by 40% to 50% for this entire request lifecycle.
            var optimalQuery = queryBlueprint
                .AsNoTracking()
                .Where(c => c.Year == filterYear); // Appends the WHERE clause directly into the SQL blueprint.

            // 3. EXECUTION BOUNDARY: The query travels to SQL server ONLY when an async materializer 
            // method like ToListAsync() is reached.
            // The database handles the indexing search and executes: "SELECT * FROM LegalCases WHERE Year = @p0"
            // Only the tiny, relevant filtered matching result set travels back across the network.
            return await optimalQuery.ToListAsync();
        }
    }
}
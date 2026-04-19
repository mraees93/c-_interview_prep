using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.LSP.violating_LSP_2
{
    public class Violating_LSP
    {
        public class LegalDocument
        {
            public virtual void SaveToArchive() => Console.WriteLine("Document archived.");
        }

        // VIOLATION: This subclass "lies" about its capabilities.
        public class ReadOnlyDocument : LegalDocument
        {
            public override void SaveToArchive()
            {
                // This breaks the program's expectation that any LegalDocument can be saved.
                throw new InvalidOperationException("Cannot save a read-only document!");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.LSP.satisfying_LSP_2
{
    public class Satisfying_LSP
    {
        public interface IDocument
        {
            string GetContent();
        }

        public interface ISaveableDocument : IDocument
        {
            void SaveToArchive();
        }

        // Satisfies LSP: No one is forced to implement a method they can't use.
        public class ArchiveDocument : ISaveableDocument
        {
            public string GetContent() => "Legal content...";
            public void SaveToArchive() => Console.WriteLine("Archived.");
        }

        public class ReadOnlyDocument : IDocument
        {
            public string GetContent() => "View only content.";
        }
    }
}
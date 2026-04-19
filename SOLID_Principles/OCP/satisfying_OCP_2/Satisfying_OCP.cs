using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.OCP.satisfying_OCP_2
{
    public class Satisfying_OCP
    {
        // The Extension Point
        public interface IExportStrategy
        {
            void Export(string content);
        }

        // Concrete Extensions
        public class PdfExportStrategy : IExportStrategy
        {
            public void Export(string content) => Console.WriteLine("PDF Export logic.");
        }

        public class WordExportStrategy : IExportStrategy
        {
            public void Export(string content) => Console.WriteLine("Word Export logic.");
        }

        // SATISFACTION: This class is now "Closed". 
        // It works with ANY strategy you give it without needing updates.
        public class DocumentExporter
        {
            public void Export(string content, IExportStrategy strategy)
            {
                strategy.Export(content);
            }
        }

    }
}
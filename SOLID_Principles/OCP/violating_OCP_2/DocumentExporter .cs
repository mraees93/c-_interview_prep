using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.OCP.violating_OCP_2
{
    public enum DocType { Pdf, Word }
    public class DocumentExporter
    {
        public void Export(string content, DocType type)
        {
            if (type == DocType.Pdf)
            {
                Console.WriteLine("Exporting to PDF...");
            }
            else if (type == DocType.Word)
            {
                Console.WriteLine("Exporting to Word...");
            }
            // VIOLATION: To add 'Excel', I must change THIS code. And the class should be 
            // closed tp modification
        }
    }
}
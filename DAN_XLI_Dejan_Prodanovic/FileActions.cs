using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAN_XLI_Dejan_Prodanovic
{
    class FileActions
    {
        public static void WriteToFile(string fileName, string textToWriteToFile)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                
                    sw.WriteLine(textToWriteToFile);
                 
            }

        }
    }
}

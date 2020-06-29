using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAN_XLI_Dejan_Prodanovic.Validations
{
    class ValidationClass
    {
        public static bool NumberOfCoppiesIsValid(string NumberOfCoppies)
        {
            bool succes;

            int numOfCoppiesInt;
            succes = Int32.TryParse(NumberOfCoppies, out numOfCoppiesInt);

            return succes;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXSA_PE_05.Tools
{
    public class Round
    {
        CultureInfo Cultura = CultureInfo.GetCultureInfo("es-MX");

        public decimal truncatetwo(decimal value)
        {
            decimal roundvalue = 0.00M;

            roundvalue = ((Math.Truncate(value * 100)) / 100);

            return roundvalue;
        }

        public decimal roundthree(decimal value)
        {
            decimal roundvalue = 0.00M;

            roundvalue = ((Math.Truncate(value * 1000)) / 1000);
            roundvalue = Math.Round(roundvalue, 2);

            return roundvalue;
        }
    }
}

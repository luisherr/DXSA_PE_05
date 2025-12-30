using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXSA_PE_05.Classes
{
    public class DXSA_PE_05proceso
    {
        public Boolean CrearConexion()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

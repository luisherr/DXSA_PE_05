using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXSA_PE_05.Tools
{
    public class Base64 
    {
        public String EncodeStringToBase64(string valor)
        {
            String cadena = String.Empty;
            if (!String.IsNullOrEmpty(valor))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(valor);
                cadena = Convert.ToBase64String(byteArray);
            }
            return cadena;
        }
        public String EncodeBytesToBase64(Byte[] valor)
        {
            String cadena = String.Empty;
            if (valor != null)
            {
                cadena = Convert.ToBase64String(valor);
            }
            return cadena;
        }

        public String DecodeBase64ToString(string valor)
        {
            String cadena = String.Empty;
            if (!String.IsNullOrEmpty(valor))
            {
                byte[] myBase64ret = Convert.FromBase64String(valor);
                cadena = Encoding.UTF8.GetString(myBase64ret);
            }
            return cadena;
        }
        public String DecodeBase64ToStringFile(string valor)
        {
            String cadena = String.Empty;
            if (!String.IsNullOrEmpty(valor))
            {
                byte[] myBase64ret = Convert.FromBase64String(valor);
                cadena = Encoding.GetEncoding(1252).GetString(myBase64ret);
            }
            return cadena;
        }

        public MemoryStream DecodeBase64ToFileStream(string valor)
        {
            MemoryStream stream = new MemoryStream();
            if (!String.IsNullOrEmpty(valor))
            {
                byte[] myBase64ret = Convert.FromBase64String(valor);
                stream = new MemoryStream(myBase64ret);
            }
            return stream;
        }
    }

    public class Bytes  
    {

    }

    public class Stream 
    {
        public MemoryStream EncodeStringToStream(string valor)
        {
            MemoryStream cadena = new MemoryStream();
            if (!String.IsNullOrEmpty(valor))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(valor);
                cadena = new MemoryStream(byteArray);
            }
            return cadena;
        }
        public MemoryStream DecodeBase64ToFileStream(string valor)
        {
            MemoryStream stream = new MemoryStream();
            if (!String.IsNullOrEmpty(valor))
            {
                byte[] myBase64ret = Convert.FromBase64String(valor);
                stream = new MemoryStream(myBase64ret);
            }
            return stream;
        }
    }
}

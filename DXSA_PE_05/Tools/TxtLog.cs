using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXSA_PE_05.Tools
{
    public class TxtLog
    {
        public void RegistrarEnArchivoLog(string cliente, string mensaje)
        {
            try
            {
                #region Fecha Hoy
                DateTime fechaActual = DateTime.Now;
                string fecha = fechaActual.ToString("yyyy-MM-dd", CultureInfo.GetCultureInfo("es-MX"));
                #endregion Fecha Hoy

                string Directorio = AppDomain.CurrentDomain.BaseDirectory + "LOGS SISTEMA";

                #region crear Directorio
                if (!Directory.Exists(Directorio))
                {
                    // Crea el directorio si no existe.
                    Directory.CreateDirectory(Directorio);
                }
                #endregion crear Directorio

                string RutaArchivo = $"{Directorio}\\{cliente}_{fecha}.txt";

                using (StreamWriter writer = new StreamWriter(RutaArchivo, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {mensaje}");
                }
            }
            catch (Exception ex)
            {
                // Manejar errores al escribir en el archivo de registro
                Console.WriteLine("Error al escribir en el archivo de registro: " + ex.Message);
            }
        }
    }
}

using DXSA_PE_05.Classes;
using System;
using System.ServiceProcess;

namespace DXSA_PE_05
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// Si se ejecuta en modo interactivo (Visual Studio/consola), ejecuta en modo prueba.
        /// Si se ejecuta como servicio, inicia el Windows Service.
        /// </summary>
        static void Main()
        {
            // Detectar si está en modo interactivo (Visual Studio, consola) o como servicio
            if (Environment.UserInteractive)
            {
                // Modo consola para pruebas (F5 en Visual Studio o doble clic)
                Console.WriteLine("===========================================");
                Console.WriteLine("  DXSA_PE_05 - Modo Consola (Pruebas)");
                Console.WriteLine("===========================================\n");
                Console.WriteLine("Ejecutando proceso de descarga de XML desde SFTP...\n");

                var proceso = new DXSA_PE_05proceso();
                bool resultado = proceso.CrearConexion();

                Console.WriteLine();
                if (resultado)
                {
                    Console.WriteLine("[OK] Proceso completado exitosamente");
                }
                else
                {
                    Console.WriteLine("[ERROR] El proceso terminó con errores");
                }

                Console.WriteLine("\nRevisa la carpeta 'LOGS SISTEMA' para más detalles.");
                Console.WriteLine("\nPresione cualquier tecla para salir...");
                Console.ReadKey();
            }
            else
            {
                // Modo Windows Service (cuando está instalado y se inicia como servicio)
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new DXSA_PE_05Control()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}

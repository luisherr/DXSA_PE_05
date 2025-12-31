using DXSA_PE_05.Classes;
using System;
using System.ServiceProcess;

namespace DXSA_PE_05
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// Uso: DXSA_PE_05.exe [--console | --test]
        /// --console: Ejecuta el proceso una vez en modo consola
        /// --test: Solo prueba la conexión SFTP sin descargar archivos
        /// Sin argumentos: Inicia como Windows Service
        /// </summary>
        static void Main(string[] args)
        {
            // Modo consola para pruebas
            if (args.Length > 0)
            {
                string modo = args[0].ToLower();

                if (modo == "--console" || modo == "-c")
                {
                    Console.WriteLine("=== DXSA_PE_05 - Modo Consola ===");
                    Console.WriteLine("Iniciando proceso de descarga de XML desde SFTP...\n");

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

                    Console.WriteLine("\nPresione cualquier tecla para salir...");
                    Console.ReadKey();
                    return;
                }
                else if (modo == "--test" || modo == "-t")
                {
                    Console.WriteLine("=== DXSA_PE_05 - Prueba de Conexión ===");
                    Console.WriteLine("Probando conexión al servidor SFTP...\n");

                    var proceso = new DXSA_PE_05proceso();
                    bool conectado = proceso.ProbarConexion();

                    Console.WriteLine();
                    if (conectado)
                    {
                        Console.WriteLine("[OK] Conexión SFTP exitosa");
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] No se pudo conectar al servidor SFTP");
                    }

                    Console.WriteLine("\nPresione cualquier tecla para salir...");
                    Console.ReadKey();
                    return;
                }
                else if (modo == "--help" || modo == "-h")
                {
                    MostrarAyuda();
                    return;
                }
            }

            // Modo Windows Service (por defecto)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new DXSA_PE_05Control()
            };
            ServiceBase.Run(ServicesToRun);
        }

        static void MostrarAyuda()
        {
            Console.WriteLine("=== DXSA_PE_05 - Descarga XML desde SFTP ===\n");
            Console.WriteLine("Uso: DXSA_PE_05.exe [opciones]\n");
            Console.WriteLine("Opciones:");
            Console.WriteLine("  --console, -c    Ejecuta el proceso de descarga en modo consola");
            Console.WriteLine("  --test, -t       Prueba la conexión SFTP sin descargar archivos");
            Console.WriteLine("  --help, -h       Muestra esta ayuda");
            Console.WriteLine("  (sin argumentos) Inicia como Windows Service\n");
            Console.WriteLine("Configuración en App.config:");
            Console.WriteLine("  SftpHost         Servidor SFTP (ej: localhost, 10.255.31.55)");
            Console.WriteLine("  SftpPuerto       Puerto SFTP (ej: 22, 2222)");
            Console.WriteLine("  SftpUser         Usuario SFTP");
            Console.WriteLine("  SftpPass         Contraseña SFTP");
            Console.WriteLine("  SftpRutaRemota   Ruta en el servidor (ej: /home/sftp/alicorp)");
            Console.WriteLine("  RutaAlmacenar    Carpeta local para guardar los XML");
        }
    }
}

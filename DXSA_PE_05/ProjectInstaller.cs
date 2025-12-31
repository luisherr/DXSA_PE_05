using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace DXSA_PE_05
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            // Configurar el proceso del servicio
            serviceProcessInstaller = new ServiceProcessInstaller();
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem; // Ejecutar como SYSTEM

            // Configurar el servicio
            serviceInstaller = new ServiceInstaller();
            serviceInstaller.ServiceName = "DXSA_PE_05";
            serviceInstaller.DisplayName = "DXSA PE 05 - Descarga XML SFTP";
            serviceInstaller.Description = "Servicio para descargar archivos XML desde SFTP de Alicorp (SOVOS)";
            serviceInstaller.StartType = ServiceStartMode.Automatic; // Iniciar automáticamente con Windows

            // Agregar los instaladores
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}

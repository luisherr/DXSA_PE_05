using DXSA_PE_05.Services;
using DXSA_PE_05.Tools;
using System;

namespace DXSA_PE_05.Classes
{
    /// <summary>
    /// Clase principal del proceso de descarga de archivos XML desde SFTP
    /// Estructura de carpetas: RUC EMPRESA >> PERIODO (MM-YYYY) >> DIA (YYYYMMDD) >> XML
    /// </summary>
    public class DXSA_PE_05proceso
    {
        private readonly SftpService _sftpService;
        private readonly TxtLog _log;

        public DXSA_PE_05proceso()
        {
            _sftpService = new SftpService();
            _log = new TxtLog();
        }

        /// <summary>
        /// Ejecuta el proceso de conexión y descarga de archivos XML desde el SFTP de Alicorp
        /// </summary>
        /// <returns>True si el proceso se ejecutó correctamente</returns>
        public Boolean CrearConexion()
        {
            try
            {
                _log.RegistrarEnArchivoLog("Proceso", "Iniciando proceso de descarga de XML");

                // Ejecutar descarga de archivos XML
                var resultado = _sftpService.DescargarArchivosXml();

                if (resultado.Exitoso)
                {
                    _log.RegistrarEnArchivoLog("Proceso",
                        $"Proceso finalizado exitosamente. " +
                        $"Descargados: {resultado.ArchivosDescargados}, " +
                        $"Omitidos: {resultado.ArchivosOmitidos}, " +
                        $"Errores: {resultado.ArchivosConError}");
                    return true;
                }
                else
                {
                    _log.RegistrarEnArchivoLog("Proceso", $"Proceso finalizado con errores: {resultado.MensajeError}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.RegistrarEnArchivoLog("Proceso", $"Error crítico en proceso: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Prueba la conexión al servidor SFTP sin descargar archivos
        /// </summary>
        /// <returns>True si la conexión es exitosa</returns>
        public Boolean ProbarConexion()
        {
            try
            {
                _log.RegistrarEnArchivoLog("Proceso", "Probando conexión SFTP...");
                bool conectado = _sftpService.ProbarConexion();

                if (conectado)
                {
                    _log.RegistrarEnArchivoLog("Proceso", "Conexión SFTP exitosa");
                }
                else
                {
                    _log.RegistrarEnArchivoLog("Proceso", "No se pudo conectar al servidor SFTP");
                }

                return conectado;
            }
            catch (Exception ex)
            {
                _log.RegistrarEnArchivoLog("Proceso", $"Error al probar conexión: {ex.Message}");
                return false;
            }
        }
    }
}

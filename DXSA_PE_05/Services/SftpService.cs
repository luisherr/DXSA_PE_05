using DXSA_PE_05.Tools;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DXSA_PE_05.Services
{
    /// <summary>
    /// Servicio para gestionar la conexión SFTP y descarga de archivos XML
    /// Estructura de carpetas en SFTP: RUC EMPRESA >> PERIODO (MM-YYYY) >> DIA (YYYYMMDD) >> XML
    /// Formato de archivo: RUC-TIPO_DOC-SERIE-CORRELATIVO.XML
    /// </summary>
    public class SftpService
    {
        private readonly string _host;
        private readonly int _puerto;
        private readonly string _usuario;
        private readonly string _password;
        private readonly string _rutaRemota;
        private readonly string _rutaLocal;
        private readonly string _rucFiltro;
        private readonly TxtLog _log;

        // Patrones regex para validar estructura
        private readonly Regex _regexRuc = new Regex(@"^\d{11}$", RegexOptions.Compiled);
        private readonly Regex _regexPeriodo = new Regex(@"^\d{2}-\d{4}$", RegexOptions.Compiled); // MM-YYYY
        private readonly Regex _regexDia = new Regex(@"^\d{8}$", RegexOptions.Compiled); // YYYYMMDD
        private readonly Regex _regexArchivoXml = new Regex(@"^\d{11}-\d{2}-[A-Za-z0-9]+-\d+\.xml$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public SftpService()
        {
            _host = ConfigurationManager.AppSettings["SftpHost"];
            _puerto = int.Parse(ConfigurationManager.AppSettings["SftpPuerto"]);
            _usuario = ConfigurationManager.AppSettings["SftpUser"];
            _password = ConfigurationManager.AppSettings["SftpPass"];
            _rutaRemota = ConfigurationManager.AppSettings["SftpRutaRemota"];
            _rutaLocal = ConfigurationManager.AppSettings["RutaAlmacenar"];
            _rucFiltro = ConfigurationManager.AppSettings["RucEmpresa"];
            _log = new TxtLog();
        }

        /// <summary>
        /// Ejecuta el proceso completo de descarga de archivos XML desde el SFTP
        /// </summary>
        /// <returns>Resultado de la operación con estadísticas</returns>
        public SftpResult DescargarArchivosXml()
        {
            var resultado = new SftpResult();

            try
            {
                _log.RegistrarEnArchivoLog("SFTP", $"Iniciando conexión a {_host}:{_puerto}");

                using (var sftp = new SftpClient(_host, _puerto, _usuario, _password))
                {
                    sftp.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30);
                    sftp.Connect();

                    if (!sftp.IsConnected)
                    {
                        resultado.Exitoso = false;
                        resultado.MensajeError = "No se pudo establecer conexión con el servidor SFTP";
                        _log.RegistrarEnArchivoLog("SFTP", resultado.MensajeError);
                        return resultado;
                    }

                    _log.RegistrarEnArchivoLog("SFTP", "Conexión establecida exitosamente");

                    // Verificar que existe la ruta remota
                    if (!sftp.Exists(_rutaRemota))
                    {
                        resultado.Exitoso = false;
                        resultado.MensajeError = $"La ruta remota no existe: {_rutaRemota}";
                        _log.RegistrarEnArchivoLog("SFTP", resultado.MensajeError);
                        return resultado;
                    }

                    // Crear directorio local si no existe
                    if (!Directory.Exists(_rutaLocal))
                    {
                        Directory.CreateDirectory(_rutaLocal);
                        _log.RegistrarEnArchivoLog("SFTP", $"Directorio local creado: {_rutaLocal}");
                    }

                    // Procesar estructura de carpetas
                    ProcesarCarpetasRuc(sftp, resultado);

                    sftp.Disconnect();
                }

                resultado.Exitoso = true;
                _log.RegistrarEnArchivoLog("SFTP", $"Proceso completado. Archivos descargados: {resultado.ArchivosDescargados}, Errores: {resultado.ArchivosConError}");
            }
            catch (Exception ex)
            {
                resultado.Exitoso = false;
                resultado.MensajeError = $"Error en proceso SFTP: {ex.Message}";
                _log.RegistrarEnArchivoLog("SFTP", $"ERROR: {ex.Message}");
            }

            return resultado;
        }

        /// <summary>
        /// Procesa las carpetas de RUC en el primer nivel
        /// </summary>
        private void ProcesarCarpetasRuc(SftpClient sftp, SftpResult resultado)
        {
            var carpetasRuc = sftp.ListDirectory(_rutaRemota)
                .Where(f => f.IsDirectory && !f.Name.StartsWith(".") && _regexRuc.IsMatch(f.Name))
                .ToList();

            _log.RegistrarEnArchivoLog("SFTP", $"Carpetas RUC encontradas: {carpetasRuc.Count}");

            foreach (var carpetaRuc in carpetasRuc)
            {
                // Si hay filtro de RUC, solo procesar ese RUC
                if (!string.IsNullOrEmpty(_rucFiltro) && carpetaRuc.Name != _rucFiltro)
                {
                    continue;
                }

                _log.RegistrarEnArchivoLog("SFTP", $"Procesando RUC: {carpetaRuc.Name}");
                ProcesarCarpetasPeriodo(sftp, carpetaRuc.FullName, carpetaRuc.Name, resultado);
            }
        }

        /// <summary>
        /// Procesa las carpetas de periodo (MM-YYYY) dentro de un RUC
        /// </summary>
        private void ProcesarCarpetasPeriodo(SftpClient sftp, string rutaRuc, string ruc, SftpResult resultado)
        {
            try
            {
                var carpetasPeriodo = sftp.ListDirectory(rutaRuc)
                    .Where(f => f.IsDirectory && !f.Name.StartsWith(".") && _regexPeriodo.IsMatch(f.Name))
                    .ToList();

                foreach (var carpetaPeriodo in carpetasPeriodo)
                {
                    _log.RegistrarEnArchivoLog("SFTP", $"Procesando Periodo: {carpetaPeriodo.Name}");
                    ProcesarCarpetasDia(sftp, carpetaPeriodo.FullName, ruc, carpetaPeriodo.Name, resultado);
                }
            }
            catch (Exception ex)
            {
                _log.RegistrarEnArchivoLog("SFTP", $"Error al procesar periodos de RUC {ruc}: {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa las carpetas de día (YYYYMMDD) dentro de un periodo
        /// </summary>
        private void ProcesarCarpetasDia(SftpClient sftp, string rutaPeriodo, string ruc, string periodo, SftpResult resultado)
        {
            try
            {
                var carpetasDia = sftp.ListDirectory(rutaPeriodo)
                    .Where(f => f.IsDirectory && !f.Name.StartsWith(".") && _regexDia.IsMatch(f.Name))
                    .ToList();

                foreach (var carpetaDia in carpetasDia)
                {
                    _log.RegistrarEnArchivoLog("SFTP", $"Procesando Día: {carpetaDia.Name}");
                    DescargarArchivosXmlDeCarpeta(sftp, carpetaDia.FullName, ruc, periodo, carpetaDia.Name, resultado);
                }
            }
            catch (Exception ex)
            {
                _log.RegistrarEnArchivoLog("SFTP", $"Error al procesar días de periodo {periodo}: {ex.Message}");
            }
        }

        /// <summary>
        /// Descarga los archivos XML de una carpeta de día específica
        /// Mantiene la misma estructura de carpetas localmente
        /// </summary>
        private void DescargarArchivosXmlDeCarpeta(SftpClient sftp, string rutaDia, string ruc, string periodo, string dia, SftpResult resultado)
        {
            try
            {
                // Crear estructura de carpetas local: RUC/PERIODO/DIA
                string rutaLocalCompleta = Path.Combine(_rutaLocal, ruc, periodo, dia);
                if (!Directory.Exists(rutaLocalCompleta))
                {
                    Directory.CreateDirectory(rutaLocalCompleta);
                }

                var archivosXml = sftp.ListDirectory(rutaDia)
                    .Where(f => !f.IsDirectory && f.Name.ToLower().EndsWith(".xml"))
                    .ToList();

                foreach (var archivo in archivosXml)
                {
                    try
                    {
                        string rutaArchivoLocal = Path.Combine(rutaLocalCompleta, archivo.Name);

                        // Verificar si el archivo ya existe y tiene el mismo tamaño
                        if (File.Exists(rutaArchivoLocal))
                        {
                            var infoLocal = new FileInfo(rutaArchivoLocal);
                            if (infoLocal.Length == archivo.Length)
                            {
                                resultado.ArchivosOmitidos++;
                                continue; // Ya existe, omitir
                            }
                        }

                        // Descargar archivo
                        using (var fileStream = File.Create(rutaArchivoLocal))
                        {
                            sftp.DownloadFile(archivo.FullName, fileStream);
                        }

                        resultado.ArchivosDescargados++;
                        resultado.ArchivosDescargadosLista.Add(new ArchivoDescargado
                        {
                            NombreArchivo = archivo.Name,
                            RutaLocal = rutaArchivoLocal,
                            RutaRemota = archivo.FullName,
                            Ruc = ruc,
                            Periodo = periodo,
                            Dia = dia,
                            TamanioBytes = archivo.Length
                        });

                        _log.RegistrarEnArchivoLog("SFTP", $"Archivo descargado: {archivo.Name}");
                    }
                    catch (Exception ex)
                    {
                        resultado.ArchivosConError++;
                        _log.RegistrarEnArchivoLog("SFTP", $"Error al descargar {archivo.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.RegistrarEnArchivoLog("SFTP", $"Error al listar archivos en {rutaDia}: {ex.Message}");
            }
        }

        /// <summary>
        /// Prueba la conexión al servidor SFTP
        /// </summary>
        /// <returns>True si la conexión es exitosa</returns>
        public bool ProbarConexion()
        {
            try
            {
                using (var sftp = new SftpClient(_host, _puerto, _usuario, _password))
                {
                    sftp.ConnectionInfo.Timeout = TimeSpan.FromSeconds(10);
                    sftp.Connect();
                    bool conectado = sftp.IsConnected;
                    sftp.Disconnect();
                    return conectado;
                }
            }
            catch (Exception ex)
            {
                _log.RegistrarEnArchivoLog("SFTP", $"Error al probar conexión: {ex.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// Resultado de la operación de descarga SFTP
    /// </summary>
    public class SftpResult
    {
        public bool Exitoso { get; set; }
        public string MensajeError { get; set; }
        public int ArchivosDescargados { get; set; }
        public int ArchivosConError { get; set; }
        public int ArchivosOmitidos { get; set; }
        public List<ArchivoDescargado> ArchivosDescargadosLista { get; set; } = new List<ArchivoDescargado>();
    }

    /// <summary>
    /// Información de un archivo XML descargado
    /// </summary>
    public class ArchivoDescargado
    {
        public string NombreArchivo { get; set; }
        public string RutaLocal { get; set; }
        public string RutaRemota { get; set; }
        public string Ruc { get; set; }
        public string Periodo { get; set; }
        public string Dia { get; set; }
        public long TamanioBytes { get; set; }
    }
}

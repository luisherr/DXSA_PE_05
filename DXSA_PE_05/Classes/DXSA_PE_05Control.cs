using DXSA_PE_05.Classes;
using DXSA_PE_05.Tools;
using System;
using System.Configuration;
using System.ServiceProcess;

namespace DXSA_PE_05
{
    public partial class DXSA_PE_05Control : ServiceBase
    {
        Boolean ProcessIndicator = false;
        TxtLog LogSistema = new TxtLog();
        public DXSA_PE_05Control()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        private void TimerControl_Elapsed(object sender, EventArgs e)
        {
            #region Validar Proceso Activo
            if (ProcessIndicator)
            {
                LogSistema.RegistrarEnArchivoLog("ProcesoPrincipal", $"Proceso Ejecutandose");
                return;
            }
            ProcessIndicator = true;
            #endregion Validar Proceso Activo

            #region Validar Red activa
            bool RedActiva = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            if (!RedActiva)
            {
                ProcessIndicator = false;
                LogSistema.RegistrarEnArchivoLog("ProcesoPrincipal", $"Sin red activa");
                return;
            }
            #endregion Validar Red Activa

            #region Modificar Intervalo Tiempo
            if (this.TimerControl.Interval != Convert.ToInt32(ConfigurationManager.AppSettings["IntervaloTiempo"]))
            {
                this.TimerControl.Stop();
                this.TimerControl.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["IntervaloTiempo"]);
                this.TimerControl.Start();

                LogSistema.RegistrarEnArchivoLog("ProcesoPrincipal", $"Intervalo de ejecución configurado");
            }
            #endregion Modificar Intervalo Tiempo


            try
            {
                DXSA_PE_05proceso oDXSA_PE_05 = new DXSA_PE_05proceso();

                oDXSA_PE_05.CrearConexion();
            }
            catch 
            {
                ProcessIndicator = false;
            }
            ProcessIndicator = false;
        }
    }
}

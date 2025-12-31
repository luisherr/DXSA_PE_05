using System.Timers;

namespace DXSA_PE_05
{
    partial class DXSA_PE_05Control
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            //this.TimerControl_Elapsed(null, null);
            this.TimerControl = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.TimerControl)).BeginInit();
            //
            // TimerControl
            //
            this.TimerControl.Enabled = false; // Se inicia en OnStart
            this.TimerControl.Interval = 6000D;
            this.TimerControl.Elapsed += new System.Timers.ElapsedEventHandler(this.TimerControl_Elapsed);
            //
            // Service1
            //
            this.ServiceName = "DXSA_PE_05";
            ((System.ComponentModel.ISupportInitialize)(this.TimerControl)).EndInit();
        }

        #endregion

        private Timer TimerControl;
    }
}

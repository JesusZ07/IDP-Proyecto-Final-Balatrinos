namespace CapaPresentacion
{
    partial class Form1
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

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelIzquierdo = new System.Windows.Forms.Panel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.panelDerecho = new System.Windows.Forms.Panel();
            this.btn_GestionHuespedes = new System.Windows.Forms.Button();
            this.btn_GestionReservaciones = new System.Windows.Forms.Button();
            this.btn_GestionHabitaciones = new System.Windows.Forms.Button();
            this.panelIzquierdo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.panelDerecho.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelIzquierdo
            // 
            this.panelIzquierdo.BackColor = System.Drawing.Color.Black;
            this.panelIzquierdo.Controls.Add(this.picLogo);
            this.panelIzquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelIzquierdo.Location = new System.Drawing.Point(0, 0);
            this.panelIzquierdo.Name = "panelIzquierdo";
            this.panelIzquierdo.Padding = new System.Windows.Forms.Padding(24, 28, 24, 28);
            this.panelIzquierdo.Size = new System.Drawing.Size(450, 540);
            this.panelIzquierdo.TabIndex = 0;
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.Black;
            this.picLogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picLogo.Location = new System.Drawing.Point(0, 0);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(470, 540);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            // 
            // panelDerecho
            // 
            this.panelDerecho.BackColor = System.Drawing.Color.Black;
            this.panelDerecho.Controls.Add(this.btn_GestionHuespedes);
            this.panelDerecho.Controls.Add(this.btn_GestionReservaciones);
            this.panelDerecho.Controls.Add(this.btn_GestionHabitaciones);
            this.panelDerecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDerecho.Location = new System.Drawing.Point(450, 0);
            this.panelDerecho.Name = "panelDerecho";
            this.panelDerecho.Size = new System.Drawing.Size(450, 540);
            this.panelDerecho.TabIndex = 1;
            // 
            // btn_GestionHuespedes
            // 
            this.btn_GestionHuespedes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(171)))), ((int)(((byte)(77)))));
            this.btn_GestionHuespedes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_GestionHuespedes.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_GestionHuespedes.Location = new System.Drawing.Point(55, 329);
            this.btn_GestionHuespedes.Name = "btn_GestionHuespedes";
            this.btn_GestionHuespedes.Size = new System.Drawing.Size(340, 72);
            this.btn_GestionHuespedes.TabIndex = 5;
            this.btn_GestionHuespedes.Text = "Gestion de huespedes";
            this.btn_GestionHuespedes.UseVisualStyleBackColor = false;
            // 
            // btn_GestionReservaciones
            // 
            this.btn_GestionReservaciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(171)))), ((int)(((byte)(77)))));
            this.btn_GestionReservaciones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_GestionReservaciones.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_GestionReservaciones.Location = new System.Drawing.Point(55, 234);
            this.btn_GestionReservaciones.Name = "btn_GestionReservaciones";
            this.btn_GestionReservaciones.Size = new System.Drawing.Size(340, 72);
            this.btn_GestionReservaciones.TabIndex = 4;
            this.btn_GestionReservaciones.Text = "Gestion de reservaciones";
            this.btn_GestionReservaciones.UseVisualStyleBackColor = false;
            // 
            // btn_GestionHabitaciones
            // 
            this.btn_GestionHabitaciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.btn_GestionHabitaciones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_GestionHabitaciones.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_GestionHabitaciones.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(171)))), ((int)(((byte)(77)))));
            this.btn_GestionHabitaciones.Location = new System.Drawing.Point(55, 139);
            this.btn_GestionHabitaciones.Name = "btn_GestionHabitaciones";
            this.btn_GestionHabitaciones.Size = new System.Drawing.Size(340, 72);
            this.btn_GestionHabitaciones.TabIndex = 3;
            this.btn_GestionHabitaciones.Text = "Gestion de habitaciones";
            this.btn_GestionHabitaciones.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(245)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(900, 540);
            this.Controls.Add(this.panelDerecho);
            this.Controls.Add(this.panelIzquierdo);
            this.Name = "Form1";
            this.Text = "Panel principal";
            this.panelIzquierdo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.panelDerecho.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelIzquierdo;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Panel panelDerecho;
        private System.Windows.Forms.Button btn_GestionHuespedes;
        private System.Windows.Forms.Button btn_GestionReservaciones;
        private System.Windows.Forms.Button btn_GestionHabitaciones;
    }
}


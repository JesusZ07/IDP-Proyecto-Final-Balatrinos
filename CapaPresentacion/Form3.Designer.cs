namespace CapaPresentacion
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Regresar = new System.Windows.Forms.Button();
            this.dgv_Reservaciones = new System.Windows.Forms.DataGridView();
            this.lblTotalRegistros = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dtp_FechaSalida = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtp_FechaEntrada = new System.Windows.Forms.DateTimePicker();
            this.txt_NumeroPersonas = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_Limpiar = new System.Windows.Forms.Button();
            this.btn_Editar = new System.Windows.Forms.Button();
            this.btn_Eliminar = new System.Windows.Forms.Button();
            this.btn_Guardar = new System.Windows.Forms.Button();
            this.txt_ReservacionID = new System.Windows.Forms.TextBox();
            this.btn_Actualizar = new System.Windows.Forms.Button();
            this.btn_Agregar = new System.Windows.Forms.Button();
            this.txt_Estatus = new System.Windows.Forms.TextBox();
            this.txt_NombreHuesped = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Reservaciones)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Regresar
            // 
            this.btn_Regresar.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_Regresar.FlatAppearance.BorderSize = 0;
            this.btn_Regresar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Regresar.Font = new System.Drawing.Font("Impact", 9F);
            this.btn_Regresar.Location = new System.Drawing.Point(700, 235);
            this.btn_Regresar.Name = "btn_Regresar";
            this.btn_Regresar.Size = new System.Drawing.Size(74, 25);
            this.btn_Regresar.TabIndex = 29;
            this.btn_Regresar.Text = "Cerrar";
            this.btn_Regresar.UseVisualStyleBackColor = false;
            // 
            // dgv_Reservaciones
            // 
            this.dgv_Reservaciones.AllowUserToAddRows = false;
            this.dgv_Reservaciones.AllowUserToDeleteRows = false;
            this.dgv_Reservaciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Reservaciones.Location = new System.Drawing.Point(27, 273);
            this.dgv_Reservaciones.Name = "dgv_Reservaciones";
            this.dgv_Reservaciones.Size = new System.Drawing.Size(747, 279);
            this.dgv_Reservaciones.TabIndex = 32;
            // 
            // lblTotalRegistros
            // 
            this.lblTotalRegistros.AutoSize = true;
            this.lblTotalRegistros.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalRegistros.Font = new System.Drawing.Font("Impact", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalRegistros.Location = new System.Drawing.Point(28, 240);
            this.lblTotalRegistros.Name = "lblTotalRegistros";
            this.lblTotalRegistros.Size = new System.Drawing.Size(179, 20);
            this.lblTotalRegistros.TabIndex = 31;
            this.lblTotalRegistros.Text = "Reservaciones existentes";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.dtp_FechaSalida);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtp_FechaEntrada);
            this.groupBox1.Controls.Add(this.txt_NumeroPersonas);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btn_Limpiar);
            this.groupBox1.Controls.Add(this.btn_Editar);
            this.groupBox1.Controls.Add(this.btn_Eliminar);
            this.groupBox1.Controls.Add(this.btn_Guardar);
            this.groupBox1.Controls.Add(this.txt_ReservacionID);
            this.groupBox1.Controls.Add(this.btn_Actualizar);
            this.groupBox1.Controls.Add(this.btn_Agregar);
            this.groupBox1.Controls.Add(this.txt_Estatus);
            this.groupBox1.Controls.Add(this.txt_NombreHuesped);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Font = new System.Drawing.Font("Impact", 10F);
            this.groupBox1.Location = new System.Drawing.Point(29, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(745, 208);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Información de la reservación";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Impact", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(344, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 15);
            this.label6.TabIndex = 34;
            this.label6.Text = "Fecha de salida";
            // 
            // dtp_FechaSalida
            // 
            this.dtp_FechaSalida.Font = new System.Drawing.Font("Arial", 10F);
            this.dtp_FechaSalida.Location = new System.Drawing.Point(346, 101);
            this.dtp_FechaSalida.Name = "dtp_FechaSalida";
            this.dtp_FechaSalida.Size = new System.Drawing.Size(238, 23);
            this.dtp_FechaSalida.TabIndex = 33;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Impact", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 15);
            this.label2.TabIndex = 32;
            this.label2.Text = "Fecha de entrada";
            // 
            // dtp_FechaEntrada
            // 
            this.dtp_FechaEntrada.Font = new System.Drawing.Font("Arial", 10F);
            this.dtp_FechaEntrada.Location = new System.Drawing.Point(14, 101);
            this.dtp_FechaEntrada.Name = "dtp_FechaEntrada";
            this.dtp_FechaEntrada.Size = new System.Drawing.Size(238, 23);
            this.dtp_FechaEntrada.TabIndex = 31;
            // 
            // txt_NumeroPersonas
            // 
            this.txt_NumeroPersonas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_NumeroPersonas.Location = new System.Drawing.Point(346, 38);
            this.txt_NumeroPersonas.Name = "txt_NumeroPersonas";
            this.txt_NumeroPersonas.Size = new System.Drawing.Size(131, 20);
            this.txt_NumeroPersonas.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Impact", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(343, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 15);
            this.label5.TabIndex = 29;
            this.label5.Text = "Número de personas";
            // 
            // btn_Limpiar
            // 
            this.btn_Limpiar.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_Limpiar.FlatAppearance.BorderSize = 0;
            this.btn_Limpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Limpiar.Font = new System.Drawing.Font("Impact", 9F);
            this.btn_Limpiar.Location = new System.Drawing.Point(96, 163);
            this.btn_Limpiar.Name = "btn_Limpiar";
            this.btn_Limpiar.Size = new System.Drawing.Size(75, 22);
            this.btn_Limpiar.TabIndex = 12;
            this.btn_Limpiar.Text = "Limpiar";
            this.btn_Limpiar.UseVisualStyleBackColor = false;
            // 
            // btn_Editar
            // 
            this.btn_Editar.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_Editar.FlatAppearance.BorderSize = 0;
            this.btn_Editar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Editar.Font = new System.Drawing.Font("Impact", 9F);
            this.btn_Editar.Location = new System.Drawing.Point(439, 162);
            this.btn_Editar.Name = "btn_Editar";
            this.btn_Editar.Size = new System.Drawing.Size(75, 23);
            this.btn_Editar.TabIndex = 28;
            this.btn_Editar.Text = "Editar";
            this.btn_Editar.UseVisualStyleBackColor = false;
            // 
            // btn_Eliminar
            // 
            this.btn_Eliminar.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_Eliminar.FlatAppearance.BorderSize = 0;
            this.btn_Eliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Eliminar.Font = new System.Drawing.Font("Impact", 9F);
            this.btn_Eliminar.Location = new System.Drawing.Point(177, 163);
            this.btn_Eliminar.Name = "btn_Eliminar";
            this.btn_Eliminar.Size = new System.Drawing.Size(75, 22);
            this.btn_Eliminar.TabIndex = 27;
            this.btn_Eliminar.Text = "Eliminar";
            this.btn_Eliminar.UseVisualStyleBackColor = false;
            // 
            // btn_Guardar
            // 
            this.btn_Guardar.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_Guardar.FlatAppearance.BorderSize = 0;
            this.btn_Guardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Guardar.Font = new System.Drawing.Font("Impact", 9F);
            this.btn_Guardar.Location = new System.Drawing.Point(15, 163);
            this.btn_Guardar.Name = "btn_Guardar";
            this.btn_Guardar.Size = new System.Drawing.Size(75, 23);
            this.btn_Guardar.TabIndex = 13;
            this.btn_Guardar.Text = "Guardar";
            this.btn_Guardar.UseVisualStyleBackColor = false;
            // 
            // txt_ReservacionID
            // 
            this.txt_ReservacionID.Enabled = false;
            this.txt_ReservacionID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_ReservacionID.Location = new System.Drawing.Point(190, 38);
            this.txt_ReservacionID.Name = "txt_ReservacionID";
            this.txt_ReservacionID.Size = new System.Drawing.Size(131, 20);
            this.txt_ReservacionID.TabIndex = 11;
            this.txt_ReservacionID.Text = "0";
            // 
            // btn_Actualizar
            // 
            this.btn_Actualizar.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_Actualizar.FlatAppearance.BorderSize = 0;
            this.btn_Actualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Actualizar.Font = new System.Drawing.Font("Impact", 9F);
            this.btn_Actualizar.Location = new System.Drawing.Point(358, 162);
            this.btn_Actualizar.Name = "btn_Actualizar";
            this.btn_Actualizar.Size = new System.Drawing.Size(75, 23);
            this.btn_Actualizar.TabIndex = 25;
            this.btn_Actualizar.Text = "Actualizar";
            this.btn_Actualizar.UseVisualStyleBackColor = false;
            // 
            // btn_Agregar
            // 
            this.btn_Agregar.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_Agregar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Agregar.FlatAppearance.BorderSize = 0;
            this.btn_Agregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Agregar.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Agregar.Location = new System.Drawing.Point(277, 162);
            this.btn_Agregar.Name = "btn_Agregar";
            this.btn_Agregar.Size = new System.Drawing.Size(75, 23);
            this.btn_Agregar.TabIndex = 26;
            this.btn_Agregar.Text = "Nuevo";
            this.btn_Agregar.UseVisualStyleBackColor = false;
            // 
            // txt_Estatus
            // 
            this.txt_Estatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Estatus.Location = new System.Drawing.Point(503, 38);
            this.txt_Estatus.Name = "txt_Estatus";
            this.txt_Estatus.Size = new System.Drawing.Size(131, 20);
            this.txt_Estatus.TabIndex = 8;
            // 
            // txt_NombreHuesped
            // 
            this.txt_NombreHuesped.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_NombreHuesped.Location = new System.Drawing.Point(14, 38);
            this.txt_NombreHuesped.Name = "txt_NombreHuesped";
            this.txt_NombreHuesped.Size = new System.Drawing.Size(151, 20);
            this.txt_NombreHuesped.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Impact", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(500, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Estatus";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Impact", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(187, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Impact", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Nombre del huésped";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 572);
            this.Controls.Add(this.btn_Regresar);
            this.Controls.Add(this.dgv_Reservaciones);
            this.Controls.Add(this.lblTotalRegistros);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form3";
            this.Text = "Gestión de reservaciones";
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Reservaciones)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Regresar;
        private System.Windows.Forms.DataGridView dgv_Reservaciones;
        private System.Windows.Forms.Label lblTotalRegistros;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtp_FechaSalida;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtp_FechaEntrada;
        private System.Windows.Forms.TextBox txt_NumeroPersonas;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_Limpiar;
        private System.Windows.Forms.Button btn_Editar;
        private System.Windows.Forms.Button btn_Eliminar;
        private System.Windows.Forms.Button btn_Guardar;
        private System.Windows.Forms.TextBox txt_ReservacionID;
        private System.Windows.Forms.Button btn_Actualizar;
        private System.Windows.Forms.Button btn_Agregar;
        private System.Windows.Forms.TextBox txt_Estatus;
        private System.Windows.Forms.TextBox txt_NombreHuesped;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
    }
}
namespace YOUTUBE
{
    partial class Form3
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.PictureBox picMiniatura;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblCalidad;
        private System.Windows.Forms.Label lblRuta;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.Label lblTamano;
        private System.Windows.Forms.TextBox txtRuta;
        private System.Windows.Forms.Button btnCarpeta;
        private System.Windows.Forms.Button btnAbrirArchivo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.picMiniatura = new System.Windows.Forms.PictureBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblCalidad = new System.Windows.Forms.Label();
            this.lblRuta = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblTamano = new System.Windows.Forms.Label();
            this.txtRuta = new System.Windows.Forms.TextBox();
            this.btnCarpeta = new System.Windows.Forms.Button();
            this.btnAbrirArchivo = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picMiniatura)).BeginInit();
            this.SuspendLayout();
            // 
            // picMiniatura
            // 
            this.picMiniatura.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.picMiniatura.Location = new System.Drawing.Point(14, 14);
            this.picMiniatura.Name = "picMiniatura";
            this.picMiniatura.Size = new System.Drawing.Size(220, 143);
            this.picMiniatura.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMiniatura.TabIndex = 0;
            this.picMiniatura.TabStop = false;
            // 
            // lblTitulo
            // 
            this.lblTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitulo.AutoEllipsis = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.lblTitulo.Location = new System.Drawing.Point(248, 14);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(377, 65);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Título del video";
            // 
            // lblCalidad
            // 
            this.lblCalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCalidad.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblCalidad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblCalidad.Location = new System.Drawing.Point(619, 14);
            this.lblCalidad.Name = "lblCalidad";
            this.lblCalidad.Size = new System.Drawing.Size(80, 18);
            this.lblCalidad.TabIndex = 2;
            this.lblCalidad.Text = "Completado";
            this.lblCalidad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRuta
            // 
            this.lblRuta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRuta.AutoEllipsis = true;
            this.lblRuta.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblRuta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblRuta.Location = new System.Drawing.Point(248, 42);
            this.lblRuta.Name = "lblRuta";
            this.lblRuta.Size = new System.Drawing.Size(351, 37);
            this.lblRuta.TabIndex = 4;
            this.lblRuta.Text = "Ruta del archivo";
            this.lblRuta.Visible = false;
            // 
            // lblFecha
            // 
            this.lblFecha.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFecha.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblFecha.Location = new System.Drawing.Point(244, 91);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(300, 18);
            this.lblFecha.TabIndex = 5;
            this.lblFecha.Text = "Completado: --";
            // 
            // lblTamano
            // 
            this.lblTamano.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblTamano.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblTamano.Location = new System.Drawing.Point(550, 91);
            this.lblTamano.Name = "lblTamano";
            this.lblTamano.Size = new System.Drawing.Size(180, 18);
            this.lblTamano.TabIndex = 6;
            this.lblTamano.Text = "0.00 MB";
            // 
            // txtRuta
            // 
            this.txtRuta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRuta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtRuta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRuta.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.txtRuta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.txtRuta.Location = new System.Drawing.Point(244, 115);
            this.txtRuta.Name = "txtRuta";
            this.txtRuta.ReadOnly = true;
            this.txtRuta.Size = new System.Drawing.Size(455, 22);
            this.txtRuta.TabIndex = 7;
            // 
            // btnCarpeta
            // 
            this.btnCarpeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCarpeta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
            this.btnCarpeta.FlatAppearance.BorderSize = 0;
            this.btnCarpeta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCarpeta.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCarpeta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.btnCarpeta.Location = new System.Drawing.Point(705, 111);
            this.btnCarpeta.Name = "btnCarpeta";
            this.btnCarpeta.Size = new System.Drawing.Size(35, 25);
            this.btnCarpeta.TabIndex = 8;
            this.btnCarpeta.Text = "📁";
            this.btnCarpeta.UseVisualStyleBackColor = false;
            // 
            // btnAbrirArchivo
            // 
            this.btnAbrirArchivo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAbrirArchivo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(110)))), ((int)(((byte)(70)))));
            this.btnAbrirArchivo.FlatAppearance.BorderSize = 0;
            this.btnAbrirArchivo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbrirArchivo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAbrirArchivo.ForeColor = System.Drawing.Color.White;
            this.btnAbrirArchivo.Location = new System.Drawing.Point(248, 140);
            this.btnAbrirArchivo.Name = "btnAbrirArchivo";
            this.btnAbrirArchivo.Size = new System.Drawing.Size(150, 37);
            this.btnAbrirArchivo.TabIndex = 9;
            this.btnAbrirArchivo.Text = "▶ Abrir archivo";
            this.btnAbrirArchivo.UseVisualStyleBackColor = false;
            // 
            // btnEliminar
            // 
            this.btnEliminar.AutoSize = true;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminar.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.btnEliminar.Location = new System.Drawing.Point(715, 3);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(35, 37);
            this.btnEliminar.TabIndex = 10;
            this.btnEliminar.Text = "X";
            this.btnEliminar.MouseEnter += new System.EventHandler(this.btnEliminar_MouseEnter);
            this.btnEliminar.MouseLeave += new System.EventHandler(this.btnEliminar_MouseLeave);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.picMiniatura);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblCalidad);
            this.Controls.Add(this.lblRuta);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.lblTamano);
            this.Controls.Add(this.txtRuta);
            this.Controls.Add(this.btnCarpeta);
            this.Controls.Add(this.btnAbrirArchivo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.Name = "Form3";
            this.Size = new System.Drawing.Size(754, 185);
            ((System.ComponentModel.ISupportInitialize)(this.picMiniatura)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label btnEliminar;
    }
}
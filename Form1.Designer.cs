namespace YOUTUBE
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnPegar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnDescargados;
        private System.Windows.Forms.Button btnConfiguracion;
        private System.Windows.Forms.Button btnResetearPaths;
        private System.Windows.Forms.Button btnBorrarRegistro;

        private System.Windows.Forms.FlowLayoutPanel flowLista;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label lblEstadoGlobal;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnPegar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnDescargados = new System.Windows.Forms.Button();
            this.btnConfiguracion = new System.Windows.Forms.Button();
            this.btnResetearPaths = new System.Windows.Forms.Button();
            this.btnBorrarRegistro = new System.Windows.Forms.Button();
            this.flowLista = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblEstadoGlobal = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.flowLista.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(56)))));
            this.panelTop.Controls.Add(this.button2);
            this.panelTop.Controls.Add(this.button1);
            this.panelTop.Controls.Add(this.lblTitulo);
            this.panelTop.Controls.Add(this.btnPegar);
            this.panelTop.Controls.Add(this.btnLimpiar);
            this.panelTop.Controls.Add(this.btnDescargados);
            this.panelTop.Controls.Add(this.btnConfiguracion);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 60);
            this.panelTop.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Red;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(790, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 30);
            this.button1.TabIndex = 7;
            this.button1.Text = "SISTEMAS";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.lblTitulo.Location = new System.Drawing.Point(-615, -170);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(0, 25);
            this.lblTitulo.TabIndex = 0;
            // 
            // btnPegar
            // 
            this.btnPegar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnPegar.FlatAppearance.BorderSize = 0;
            this.btnPegar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPegar.ForeColor = System.Drawing.Color.White;
            this.btnPegar.Location = new System.Drawing.Point(16, 16);
            this.btnPegar.Name = "btnPegar";
            this.btnPegar.Size = new System.Drawing.Size(110, 30);
            this.btnPegar.TabIndex = 1;
            this.btnPegar.Text = "📋 Pegar URL";
            this.btnPegar.UseVisualStyleBackColor = false;
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
            this.btnLimpiar.FlatAppearance.BorderSize = 0;
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.btnLimpiar.Location = new System.Drawing.Point(136, 16);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(110, 30);
            this.btnLimpiar.TabIndex = 2;
            this.btnLimpiar.Text = "🧹 Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            // 
            // btnDescargados
            // 
            this.btnDescargados.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(110)))), ((int)(((byte)(70)))));
            this.btnDescargados.FlatAppearance.BorderSize = 0;
            this.btnDescargados.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDescargados.ForeColor = System.Drawing.Color.White;
            this.btnDescargados.Location = new System.Drawing.Point(256, 16);
            this.btnDescargados.Name = "btnDescargados";
            this.btnDescargados.Size = new System.Drawing.Size(130, 30);
            this.btnDescargados.TabIndex = 3;
            this.btnDescargados.Text = "✅ Descargados";
            this.btnDescargados.UseVisualStyleBackColor = false;
            // 
            // btnConfiguracion
            // 
            this.btnConfiguracion.BackColor = System.Drawing.Color.Transparent;
            this.btnConfiguracion.FlatAppearance.BorderSize = 0;
            this.btnConfiguracion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfiguracion.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfiguracion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.btnConfiguracion.Location = new System.Drawing.Point(400, 6);
            this.btnConfiguracion.Name = "btnConfiguracion";
            this.btnConfiguracion.Size = new System.Drawing.Size(54, 45);
            this.btnConfiguracion.TabIndex = 6;
            this.btnConfiguracion.Text = "⚙";
            this.btnConfiguracion.UseVisualStyleBackColor = false;
            // 
            // btnResetearPaths
            // 
            this.btnResetearPaths.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
            this.btnResetearPaths.FlatAppearance.BorderSize = 0;
            this.btnResetearPaths.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetearPaths.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.btnResetearPaths.Location = new System.Drawing.Point(14, 20);
            this.btnResetearPaths.Name = "btnResetearPaths";
            this.btnResetearPaths.Size = new System.Drawing.Size(130, 30);
            this.btnResetearPaths.TabIndex = 4;
            this.btnResetearPaths.Text = "🔄 Reset rutas";
            this.btnResetearPaths.UseVisualStyleBackColor = false;
            // 
            // btnBorrarRegistro
            // 
            this.btnBorrarRegistro.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
            this.btnBorrarRegistro.FlatAppearance.BorderSize = 0;
            this.btnBorrarRegistro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBorrarRegistro.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.btnBorrarRegistro.Location = new System.Drawing.Point(14, 56);
            this.btnBorrarRegistro.Name = "btnBorrarRegistro";
            this.btnBorrarRegistro.Size = new System.Drawing.Size(130, 30);
            this.btnBorrarRegistro.TabIndex = 5;
            this.btnBorrarRegistro.Text = "🗑 Borrar registro";
            this.btnBorrarRegistro.UseVisualStyleBackColor = false;
            // 
            // flowLista
            // 
            this.flowLista.AutoScroll = true;
            this.flowLista.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.flowLista.Controls.Add(this.panel1);
            this.flowLista.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLista.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLista.Location = new System.Drawing.Point(0, 60);
            this.flowLista.Name = "flowLista";
            this.flowLista.Padding = new System.Windows.Forms.Padding(10);
            this.flowLista.Size = new System.Drawing.Size(900, 500);
            this.flowLista.TabIndex = 1;
            this.flowLista.WrapContents = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.Controls.Add(this.btnBorrarRegistro);
            this.panel1.Controls.Add(this.btnResetearPaths);
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(863, 115);
            this.panel1.TabIndex = 0;
            this.panel1.Visible = false;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(56)))));
            this.panelBottom.Controls.Add(this.lblEstadoGlobal);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 560);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(900, 40);
            this.panelBottom.TabIndex = 2;
            // 
            // lblEstadoGlobal
            // 
            this.lblEstadoGlobal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEstadoGlobal.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEstadoGlobal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblEstadoGlobal.Location = new System.Drawing.Point(0, 0);
            this.lblEstadoGlobal.Name = "lblEstadoGlobal";
            this.lblEstadoGlobal.Size = new System.Drawing.Size(900, 40);
            this.lblEstadoGlobal.TabIndex = 0;
            this.lblEstadoGlobal.Text = "  Listo.";
            this.lblEstadoGlobal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Red;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(790, 16);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 30);
            this.button2.TabIndex = 8;
            this.button2.Text = "SISTEMAS";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.flowLista);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "YOUTUBE Downloader";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.flowLista.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
    }
}
namespace YOUTUBE
{
    partial class Form2
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.PictureBox picMiniatura;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblCalidad;
        private System.Windows.Forms.Label lblUrl;
        private ProgressBarEx progress;
        private System.Windows.Forms.Label lblMb;
        private System.Windows.Forms.Label lblPorcentaje;
        private System.Windows.Forms.Label lblVel;
        private System.Windows.Forms.Label lblEta;
        private System.Windows.Forms.TextBox txtDestino;
        private System.Windows.Forms.Button btnCarpeta;
        private System.Windows.Forms.CheckBox chkSub;
        private System.Windows.Forms.Button btnPausa;
        private System.Windows.Forms.Button btnDescargar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.picMiniatura = new System.Windows.Forms.PictureBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblCalidad = new System.Windows.Forms.Label();
            this.lblUrl = new System.Windows.Forms.Label();
            this.lblMb = new System.Windows.Forms.Label();
            this.lblPorcentaje = new System.Windows.Forms.Label();
            this.lblVel = new System.Windows.Forms.Label();
            this.lblEta = new System.Windows.Forms.Label();
            this.txtDestino = new System.Windows.Forms.TextBox();
            this.btnCarpeta = new System.Windows.Forms.Button();
            this.chkSub = new System.Windows.Forms.CheckBox();
            this.btnPausa = new System.Windows.Forms.Button();
            this.btnDescargar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
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
            this.lblTitulo.Size = new System.Drawing.Size(461, 25);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Título del video";
            // 
            // lblCalidad
            // 
            this.lblCalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCalidad.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblCalidad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblCalidad.Location = new System.Drawing.Point(670, 42);
            this.lblCalidad.Name = "lblCalidad";
            this.lblCalidad.Size = new System.Drawing.Size(80, 18);
            this.lblCalidad.TabIndex = 2;
            this.lblCalidad.Text = "HD 1080p";
            this.lblCalidad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblUrl
            // 
            this.lblUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUrl.AutoEllipsis = true;
            this.lblUrl.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblUrl.Location = new System.Drawing.Point(248, 42);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(444, 53);
            this.lblUrl.TabIndex = 4;
            this.lblUrl.Text = "https://...";
            // 
            // lblMb
            // 
            this.lblMb.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblMb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(210)))));
            this.lblMb.Location = new System.Drawing.Point(248, 96);
            this.lblMb.Name = "lblMb";
            this.lblMb.Size = new System.Drawing.Size(180, 18);
            this.lblMb.TabIndex = 6;
            this.lblMb.Text = "0.00 MB / -- MB";
            // 
            // lblPorcentaje
            // 
            this.lblPorcentaje.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPorcentaje.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.lblPorcentaje.Location = new System.Drawing.Point(432, 95);
            this.lblPorcentaje.Name = "lblPorcentaje";
            this.lblPorcentaje.Size = new System.Drawing.Size(70, 18);
            this.lblPorcentaje.TabIndex = 7;
            this.lblPorcentaje.Text = "0.0%";
            // 
            // lblVel
            // 
            this.lblVel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblVel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblVel.Location = new System.Drawing.Point(508, 96);
            this.lblVel.Name = "lblVel";
            this.lblVel.Size = new System.Drawing.Size(100, 18);
            this.lblVel.TabIndex = 8;
            this.lblVel.Text = "-- /s";
            // 
            // lblEta
            // 
            this.lblEta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEta.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblEta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblEta.Location = new System.Drawing.Point(618, 96);
            this.lblEta.Name = "lblEta";
            this.lblEta.Size = new System.Drawing.Size(120, 18);
            this.lblEta.TabIndex = 9;
            this.lblEta.Text = "Restante -- min";
            this.lblEta.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDestino
            // 
            this.txtDestino.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestino.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtDestino.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDestino.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.txtDestino.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.txtDestino.Location = new System.Drawing.Point(248, 122);
            this.txtDestino.Name = "txtDestino";
            this.txtDestino.Size = new System.Drawing.Size(492, 22);
            this.txtDestino.TabIndex = 10;
            // 
            // btnCarpeta
            // 
            this.btnCarpeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCarpeta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
            this.btnCarpeta.FlatAppearance.BorderSize = 0;
            this.btnCarpeta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCarpeta.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCarpeta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.btnCarpeta.Location = new System.Drawing.Point(795, 121);
            this.btnCarpeta.Name = "btnCarpeta";
            this.btnCarpeta.Size = new System.Drawing.Size(35, 25);
            this.btnCarpeta.TabIndex = 11;
            this.btnCarpeta.Text = "📁";
            this.btnCarpeta.UseVisualStyleBackColor = false;
            this.btnCarpeta.Visible = false;
            // 
            // chkSub
            // 
            this.chkSub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSub.AutoSize = true;
            this.chkSub.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.chkSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.chkSub.Location = new System.Drawing.Point(807, 122);
            this.chkSub.Name = "chkSub";
            this.chkSub.Size = new System.Drawing.Size(84, 17);
            this.chkSub.TabIndex = 12;
            this.chkSub.Text = "Subcarpeta";
            this.chkSub.Visible = false;
            // 
            // btnPausa
            // 
            this.btnPausa.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPausa.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
            this.btnPausa.FlatAppearance.BorderSize = 0;
            this.btnPausa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPausa.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnPausa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.btnPausa.Location = new System.Drawing.Point(248, 152);
            this.btnPausa.Name = "btnPausa";
            this.btnPausa.Size = new System.Drawing.Size(110, 37);
            this.btnPausa.TabIndex = 13;
            this.btnPausa.Text = "⏸ Pausar";
            this.btnPausa.UseVisualStyleBackColor = false;
            // 
            // btnDescargar
            // 
            this.btnDescargar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDescargar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(110)))), ((int)(((byte)(70)))));
            this.btnDescargar.FlatAppearance.BorderSize = 0;
            this.btnDescargar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDescargar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDescargar.ForeColor = System.Drawing.Color.White;
            this.btnDescargar.Location = new System.Drawing.Point(565, 152);
            this.btnDescargar.Name = "btnDescargar";
            this.btnDescargar.Size = new System.Drawing.Size(175, 37);
            this.btnDescargar.TabIndex = 14;
            this.btnDescargar.Text = "⬇ Descargar";
            this.btnDescargar.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label1.Location = new System.Drawing.Point(715, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 37);
            this.label1.TabIndex = 15;
            this.label1.Text = "X";
            this.label1.MouseEnter += new System.EventHandler(this.label1_MouseEnter);
            this.label1.MouseLeave += new System.EventHandler(this.label1_MouseLeave);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picMiniatura);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblCalidad);
            this.Controls.Add(this.lblUrl);
            this.Controls.Add(this.lblMb);
            this.Controls.Add(this.lblPorcentaje);
            this.Controls.Add(this.lblVel);
            this.Controls.Add(this.lblEta);
            this.Controls.Add(this.txtDestino);
            this.Controls.Add(this.btnCarpeta);
            this.Controls.Add(this.chkSub);
            this.Controls.Add(this.btnPausa);
            this.Controls.Add(this.btnDescargar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.Name = "Form2";
            this.Size = new System.Drawing.Size(754, 195);
            ((System.ComponentModel.ISupportInitialize)(this.picMiniatura)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label1;
    }
}
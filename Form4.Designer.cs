namespace YOUTUBE
{
    partial class Form4
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelContenido;
        private System.Windows.Forms.Label lblDescargas;
        private System.Windows.Forms.TextBox txtDescargas;
        private System.Windows.Forms.Button btnExaminarDescargas;
        private System.Windows.Forms.Label lblHerramientas;
        private System.Windows.Forms.TextBox txtHerramientas;
        private System.Windows.Forms.Button btnExaminarHerramientas;
        private System.Windows.Forms.Label lblHerramientasAviso;
        private System.Windows.Forms.GroupBox grpShutdown;
        private System.Windows.Forms.RadioButton rdoSinShutdown;
        private System.Windows.Forms.RadioButton rdoHoraFija;
        private System.Windows.Forms.RadioButton rdoAlTerminarLista;
        private System.Windows.Forms.NumericUpDown numHoras;
        private System.Windows.Forms.NumericUpDown numMinutos;
        private System.Windows.Forms.NumericUpDown numSegundos;
        private System.Windows.Forms.Label lblH;
        private System.Windows.Forms.Label lblM;
        private System.Windows.Forms.Label lblS;
        private System.Windows.Forms.GroupBox grpMySQL;
        private System.Windows.Forms.CheckBox chkMySQL;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label lblTipoServidor;
        private System.Windows.Forms.Button btnProbarMySql;
        private System.Windows.Forms.CheckBox chkMantenerPngs;
        private System.Windows.Forms.Button btnCerrar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelContenido = new System.Windows.Forms.Panel();
            this.lblDescargas = new System.Windows.Forms.Label();
            this.txtDescargas = new System.Windows.Forms.TextBox();
            this.btnExaminarDescargas = new System.Windows.Forms.Button();
            this.lblHerramientas = new System.Windows.Forms.Label();
            this.txtHerramientas = new System.Windows.Forms.TextBox();
            this.btnExaminarHerramientas = new System.Windows.Forms.Button();
            this.lblHerramientasAviso = new System.Windows.Forms.Label();
            this.grpShutdown = new System.Windows.Forms.GroupBox();
            this.rdoSinShutdown = new System.Windows.Forms.RadioButton();
            this.rdoHoraFija = new System.Windows.Forms.RadioButton();
            this.rdoAlTerminarLista = new System.Windows.Forms.RadioButton();
            this.numHoras = new System.Windows.Forms.NumericUpDown();
            this.numMinutos = new System.Windows.Forms.NumericUpDown();
            this.numSegundos = new System.Windows.Forms.NumericUpDown();
            this.lblH = new System.Windows.Forms.Label();
            this.lblM = new System.Windows.Forms.Label();
            this.lblS = new System.Windows.Forms.Label();
            this.grpMySQL = new System.Windows.Forms.GroupBox();
            this.chkMySQL = new System.Windows.Forms.CheckBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.lblTipoServidor = new System.Windows.Forms.Label();
            this.btnProbarMySql = new System.Windows.Forms.Button();
            this.chkMantenerPngs = new System.Windows.Forms.CheckBox();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.panelContenido.SuspendLayout();
            this.grpShutdown.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHoras)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinutos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSegundos)).BeginInit();
            this.grpMySQL.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelContenido
            // 
            this.panelContenido.AutoScroll = true;
            this.panelContenido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(56)))));
            this.panelContenido.Controls.Add(this.lblDescargas);
            this.panelContenido.Controls.Add(this.txtDescargas);
            this.panelContenido.Controls.Add(this.btnExaminarDescargas);
            this.panelContenido.Controls.Add(this.lblHerramientas);
            this.panelContenido.Controls.Add(this.txtHerramientas);
            this.panelContenido.Controls.Add(this.btnExaminarHerramientas);
            this.panelContenido.Controls.Add(this.lblHerramientasAviso);
            this.panelContenido.Controls.Add(this.grpShutdown);
            this.panelContenido.Controls.Add(this.grpMySQL);
            this.panelContenido.Controls.Add(this.btnCerrar);
            this.panelContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenido.Location = new System.Drawing.Point(0, 0);
            this.panelContenido.Name = "panelContenido";
            this.panelContenido.Padding = new System.Windows.Forms.Padding(20);
            this.panelContenido.Size = new System.Drawing.Size(513, 535);
            this.panelContenido.TabIndex = 0;
            // 
            // lblDescargas
            // 
            this.lblDescargas.AutoSize = true;
            this.lblDescargas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.lblDescargas.Location = new System.Drawing.Point(12, 10);
            this.lblDescargas.Name = "lblDescargas";
            this.lblDescargas.Size = new System.Drawing.Size(122, 15);
            this.lblDescargas.TabIndex = 0;
            this.lblDescargas.Text = "Carpeta de descargas:";
            // 
            // txtDescargas
            // 
            this.txtDescargas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtDescargas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDescargas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.txtDescargas.Location = new System.Drawing.Point(12, 32);
            this.txtDescargas.Name = "txtDescargas";
            this.txtDescargas.Size = new System.Drawing.Size(395, 23);
            this.txtDescargas.TabIndex = 1;
            // 
            // btnExaminarDescargas
            // 
            this.btnExaminarDescargas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
            this.btnExaminarDescargas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExaminarDescargas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.btnExaminarDescargas.Location = new System.Drawing.Point(417, 31);
            this.btnExaminarDescargas.Name = "btnExaminarDescargas";
            this.btnExaminarDescargas.Size = new System.Drawing.Size(80, 25);
            this.btnExaminarDescargas.TabIndex = 2;
            this.btnExaminarDescargas.Text = "Examinar";
            this.btnExaminarDescargas.UseVisualStyleBackColor = false;
            // 
            // lblHerramientas
            // 
            this.lblHerramientas.AutoSize = true;
            this.lblHerramientas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.lblHerramientas.Location = new System.Drawing.Point(12, 70);
            this.lblHerramientas.Name = "lblHerramientas";
            this.lblHerramientas.Size = new System.Drawing.Size(139, 15);
            this.lblHerramientas.TabIndex = 3;
            this.lblHerramientas.Text = "Carpeta de herramientas:";
            // 
            // txtHerramientas
            // 
            this.txtHerramientas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtHerramientas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHerramientas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.txtHerramientas.Location = new System.Drawing.Point(12, 92);
            this.txtHerramientas.Name = "txtHerramientas";
            this.txtHerramientas.Size = new System.Drawing.Size(395, 23);
            this.txtHerramientas.TabIndex = 4;
            // 
            // btnExaminarHerramientas
            // 
            this.btnExaminarHerramientas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
            this.btnExaminarHerramientas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExaminarHerramientas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.btnExaminarHerramientas.Location = new System.Drawing.Point(417, 91);
            this.btnExaminarHerramientas.Name = "btnExaminarHerramientas";
            this.btnExaminarHerramientas.Size = new System.Drawing.Size(80, 25);
            this.btnExaminarHerramientas.TabIndex = 5;
            this.btnExaminarHerramientas.Text = "Examinar";
            this.btnExaminarHerramientas.UseVisualStyleBackColor = false;
            // 
            // lblHerramientasAviso
            // 
            this.lblHerramientasAviso.AutoSize = true;
            this.lblHerramientasAviso.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(170)))), ((int)(((byte)(60)))));
            this.lblHerramientasAviso.Location = new System.Drawing.Point(12, 120);
            this.lblHerramientasAviso.Name = "lblHerramientasAviso";
            this.lblHerramientasAviso.Size = new System.Drawing.Size(240, 15);
            this.lblHerramientasAviso.TabIndex = 6;
            this.lblHerramientasAviso.Text = "⚠ No se encontró yt-dlp.exe en esa carpeta.";
            this.lblHerramientasAviso.Visible = false;
            // 
            // grpShutdown
            // 
            this.grpShutdown.Controls.Add(this.rdoSinShutdown);
            this.grpShutdown.Controls.Add(this.rdoHoraFija);
            this.grpShutdown.Controls.Add(this.rdoAlTerminarLista);
            this.grpShutdown.Controls.Add(this.numHoras);
            this.grpShutdown.Controls.Add(this.numMinutos);
            this.grpShutdown.Controls.Add(this.numSegundos);
            this.grpShutdown.Controls.Add(this.lblH);
            this.grpShutdown.Controls.Add(this.lblM);
            this.grpShutdown.Controls.Add(this.lblS);
            this.grpShutdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.grpShutdown.Location = new System.Drawing.Point(12, 145);
            this.grpShutdown.Name = "grpShutdown";
            this.grpShutdown.Size = new System.Drawing.Size(490, 120);
            this.grpShutdown.TabIndex = 7;
            this.grpShutdown.TabStop = false;
            this.grpShutdown.Text = "Cierre automático del equipo";
            // 
            // rdoSinShutdown
            // 
            this.rdoSinShutdown.AutoSize = true;
            this.rdoSinShutdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.rdoSinShutdown.Location = new System.Drawing.Point(15, 22);
            this.rdoSinShutdown.Name = "rdoSinShutdown";
            this.rdoSinShutdown.Size = new System.Drawing.Size(73, 19);
            this.rdoSinShutdown.TabIndex = 0;
            this.rdoSinShutdown.Text = "Sin cierre";
            // 
            // rdoHoraFija
            // 
            this.rdoHoraFija.AutoSize = true;
            this.rdoHoraFija.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.rdoHoraFija.Location = new System.Drawing.Point(15, 50);
            this.rdoHoraFija.Name = "rdoHoraFija";
            this.rdoHoraFija.Size = new System.Drawing.Size(82, 19);
            this.rdoHoraFija.TabIndex = 1;
            this.rdoHoraFija.Text = "En tiempo:";
            // 
            // rdoAlTerminarLista
            // 
            this.rdoAlTerminarLista.AutoSize = true;
            this.rdoAlTerminarLista.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.rdoAlTerminarLista.Location = new System.Drawing.Point(15, 85);
            this.rdoAlTerminarLista.Name = "rdoAlTerminarLista";
            this.rdoAlTerminarLista.Size = new System.Drawing.Size(191, 19);
            this.rdoAlTerminarLista.TabIndex = 2;
            this.rdoAlTerminarLista.Text = "Al terminar la lista de descargas";
            // 
            // numHoras
            // 
            this.numHoras.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.numHoras.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.numHoras.Location = new System.Drawing.Point(115, 48);
            this.numHoras.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.numHoras.Name = "numHoras";
            this.numHoras.Size = new System.Drawing.Size(55, 23);
            this.numHoras.TabIndex = 3;
            // 
            // numMinutos
            // 
            this.numMinutos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.numMinutos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.numMinutos.Location = new System.Drawing.Point(195, 48);
            this.numMinutos.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numMinutos.Name = "numMinutos";
            this.numMinutos.Size = new System.Drawing.Size(55, 23);
            this.numMinutos.TabIndex = 4;
            // 
            // numSegundos
            // 
            this.numSegundos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.numSegundos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.numSegundos.Location = new System.Drawing.Point(275, 48);
            this.numSegundos.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numSegundos.Name = "numSegundos";
            this.numSegundos.Size = new System.Drawing.Size(55, 23);
            this.numSegundos.TabIndex = 5;
            // 
            // lblH
            // 
            this.lblH.AutoSize = true;
            this.lblH.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblH.Location = new System.Drawing.Point(176, 52);
            this.lblH.Name = "lblH";
            this.lblH.Size = new System.Drawing.Size(14, 15);
            this.lblH.TabIndex = 6;
            this.lblH.Text = "h";
            // 
            // lblM
            // 
            this.lblM.AutoSize = true;
            this.lblM.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblM.Location = new System.Drawing.Point(256, 52);
            this.lblM.Name = "lblM";
            this.lblM.Size = new System.Drawing.Size(18, 15);
            this.lblM.TabIndex = 7;
            this.lblM.Text = "m";
            // 
            // lblS
            // 
            this.lblS.AutoSize = true;
            this.lblS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(170)))), ((int)(((byte)(185)))));
            this.lblS.Location = new System.Drawing.Point(336, 52);
            this.lblS.Name = "lblS";
            this.lblS.Size = new System.Drawing.Size(12, 15);
            this.lblS.TabIndex = 8;
            this.lblS.Text = "s";
            // 
            // grpMySQL
            // 
            this.grpMySQL.Controls.Add(this.chkMySQL);
            this.grpMySQL.Controls.Add(this.lblHost);
            this.grpMySQL.Controls.Add(this.txtHost);
            this.grpMySQL.Controls.Add(this.lblPort);
            this.grpMySQL.Controls.Add(this.txtPort);
            this.grpMySQL.Controls.Add(this.lblUser);
            this.grpMySQL.Controls.Add(this.txtUser);
            this.grpMySQL.Controls.Add(this.lblPass);
            this.grpMySQL.Controls.Add(this.txtPass);
            this.grpMySQL.Controls.Add(this.lblTipoServidor);
            this.grpMySQL.Controls.Add(this.btnProbarMySql);
            this.grpMySQL.Controls.Add(this.chkMantenerPngs);
            this.grpMySQL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.grpMySQL.Location = new System.Drawing.Point(12, 275);
            this.grpMySQL.Name = "grpMySQL";
            this.grpMySQL.Size = new System.Drawing.Size(490, 175);
            this.grpMySQL.TabIndex = 8;
            this.grpMySQL.TabStop = false;
            this.grpMySQL.Text = "MySQL (opcional)";
            // 
            // chkMySQL
            // 
            this.chkMySQL.AutoSize = true;
            this.chkMySQL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.chkMySQL.Location = new System.Drawing.Point(15, 22);
            this.chkMySQL.Name = "chkMySQL";
            this.chkMySQL.Size = new System.Drawing.Size(112, 19);
            this.chkMySQL.TabIndex = 0;
            this.chkMySQL.Text = "Habilitar MySQL";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.lblHost.Location = new System.Drawing.Point(15, 52);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(35, 15);
            this.lblHost.TabIndex = 1;
            this.lblHost.Text = "Host:";
            // 
            // txtHost
            // 
            this.txtHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtHost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.txtHost.Location = new System.Drawing.Point(70, 48);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(200, 23);
            this.txtHost.TabIndex = 2;
            this.txtHost.Text = "localhost";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.lblPort.Location = new System.Drawing.Point(285, 52);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(45, 15);
            this.lblPort.TabIndex = 3;
            this.lblPort.Text = "Puerto:";
            // 
            // txtPort
            // 
            this.txtPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.txtPort.Location = new System.Drawing.Point(335, 48);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(80, 23);
            this.txtPort.TabIndex = 4;
            this.txtPort.Text = "3306";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.lblUser.Location = new System.Drawing.Point(15, 82);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(50, 15);
            this.lblUser.TabIndex = 5;
            this.lblUser.Text = "Usuario:";
            // 
            // txtUser
            // 
            this.txtUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.txtUser.Location = new System.Drawing.Point(70, 78);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(200, 23);
            this.txtUser.TabIndex = 6;
            this.txtUser.Text = "root";
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.lblPass.Location = new System.Drawing.Point(15, 112);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(60, 15);
            this.lblPass.TabIndex = 7;
            this.lblPass.Text = "Password:";
            // 
            // txtPass
            // 
            this.txtPass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.txtPass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.txtPass.Location = new System.Drawing.Point(70, 108);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(200, 23);
            this.txtPass.TabIndex = 8;
            this.txtPass.UseSystemPasswordChar = true;
            // 
            // lblTipoServidor
            // 
            this.lblTipoServidor.AutoSize = true;
            this.lblTipoServidor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(200)))), ((int)(((byte)(140)))));
            this.lblTipoServidor.Location = new System.Drawing.Point(150, 24);
            this.lblTipoServidor.Name = "lblTipoServidor";
            this.lblTipoServidor.Size = new System.Drawing.Size(104, 15);
            this.lblTipoServidor.TabIndex = 9;
            this.lblTipoServidor.Text = "🏠 Servidor LOCAL";
            // 
            // btnProbarMySql
            // 
            this.btnProbarMySql.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
            this.btnProbarMySql.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProbarMySql.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.btnProbarMySql.Location = new System.Drawing.Point(285, 78);
            this.btnProbarMySql.Name = "btnProbarMySql";
            this.btnProbarMySql.Size = new System.Drawing.Size(130, 53);
            this.btnProbarMySql.TabIndex = 10;
            this.btnProbarMySql.Text = "Probar conexión";
            this.btnProbarMySql.UseVisualStyleBackColor = false;
            // 
            // chkMantenerPngs
            // 
            this.chkMantenerPngs.AutoSize = true;
            this.chkMantenerPngs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.chkMantenerPngs.Location = new System.Drawing.Point(15, 145);
            this.chkMantenerPngs.Name = "chkMantenerPngs";
            this.chkMantenerPngs.Size = new System.Drawing.Size(235, 19);
            this.chkMantenerPngs.TabIndex = 11;
            this.chkMantenerPngs.Text = "Mantener PNGs (no borrar carpeta png)";
            // 
            // btnCerrar
            // 
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(110)))), ((int)(((byte)(70)))));
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(12, 465);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(490, 40);
            this.btnCerrar.TabIndex = 9;
            this.btnCerrar.Text = "💾 Guardar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(56)))));
            this.ClientSize = new System.Drawing.Size(513, 535);
            this.Controls.Add(this.panelContenido);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "Form4";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuración";
            this.panelContenido.ResumeLayout(false);
            this.panelContenido.PerformLayout();
            this.grpShutdown.ResumeLayout(false);
            this.grpShutdown.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHoras)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinutos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSegundos)).EndInit();
            this.grpMySQL.ResumeLayout(false);
            this.grpMySQL.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
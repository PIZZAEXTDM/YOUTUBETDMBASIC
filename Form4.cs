using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace YOUTUBE
{
    public partial class Form4 : Form
    {
        public string CarpetaDescargas { get; private set; }
        public string CarpetaHerramientas { get; private set; }

        public enum ShutdownModo { Ninguno, Tiempo, AlTerminarLista }
        public ShutdownModo ShutdownElegido { get; private set; } = ShutdownModo.Ninguno;
        public TimeSpan ShutdownTiempo { get; private set; } = TimeSpan.Zero;

        private MySqlConfiguracion _mySqlCfg = new MySqlConfiguracion();

        public Form4()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(42, 45, 56);
            this.ForeColor = Color.FromArgb(235, 235, 240);

            txtDescargas.Text = PathManager.CarpetaDescargas ?? "";
            txtHerramientas.Text = PathManager.CarpetaHerramientas ?? "";

            ConfigurarDrop(txtDescargas, esHerramientas: false);
            ConfigurarDrop(txtHerramientas, esHerramientas: true);

            btnExaminarDescargas.Click += (s, e) =>
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    if (Directory.Exists(txtDescargas.Text)) fbd.SelectedPath = txtDescargas.Text;
                    if (fbd.ShowDialog() == DialogResult.OK) txtDescargas.Text = fbd.SelectedPath;
                }
            };

            btnExaminarHerramientas.Click += (s, e) =>
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    if (Directory.Exists(txtHerramientas.Text)) fbd.SelectedPath = txtHerramientas.Text;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        txtHerramientas.Text = fbd.SelectedPath;
                        ValidarHerramientas();
                    }
                }
            };

            txtHerramientas.Leave += (s, e) => ValidarHerramientas();
            btnCerrar.Click += (s, e) => CerrarYGuardar();

            rdoHoraFija.CheckedChanged += (s, e) => ActualizarEstadoShutdown();
            rdoAlTerminarLista.CheckedChanged += (s, e) => ActualizarEstadoShutdown();
            rdoSinShutdown.CheckedChanged += (s, e) => ActualizarEstadoShutdown();
            ActualizarEstadoShutdown();

            ValidarHerramientas();

            _mySqlCfg = MySqlConfiguracion.Cargar();
            chkMySQL.Checked = _mySqlCfg.Habilitado;
            txtHost.Text = _mySqlCfg.Host;
            txtUser.Text = _mySqlCfg.Usuario;
            txtPass.Text = _mySqlCfg.Password;
            txtPort.Text = _mySqlCfg.Puerto.ToString();
            chkMantenerPngs.Checked = _mySqlCfg.MantenerPngs;

            chkMySQL.CheckedChanged += (s, e) => MostrarOcultarBloqueMySql();
            txtHost.TextChanged += (s, e) => RefrescarTipoServidor();
            txtPort.TextChanged += (s, e) => RefrescarTipoServidor();
            btnProbarMySql.Click += (s, e) => ProbarMySql();

            MostrarOcultarBloqueMySql();
            RefrescarTipoServidor();
        }

        private void ConfigurarDrop(TextBox txt, bool esHerramientas)
        {
            txt.AllowDrop = true;
            txt.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
                else if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.UnicodeText))
                    e.Effect = DragDropEffects.Copy;
                else e.Effect = DragDropEffects.None;
            };
            txt.DragDrop += (s, e) =>
            {
                string ruta = ObtenerRutaDeDrop(e);
                if (string.IsNullOrEmpty(ruta)) return;
                txt.Text = ruta;
                if (esHerramientas) ValidarHerramientas();
            };
        }

        private static string ObtenerRutaDeDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var items = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (items != null && items.Length > 0)
                {
                    string p = items[0];
                    if (Directory.Exists(p)) return p;
                    if (File.Exists(p)) return Path.GetDirectoryName(p);
                }
            }
            string txt = null;
            if (e.Data.GetDataPresent(DataFormats.UnicodeText))
                txt = e.Data.GetData(DataFormats.UnicodeText) as string;
            else if (e.Data.GetDataPresent(DataFormats.Text))
                txt = e.Data.GetData(DataFormats.Text) as string;
            if (!string.IsNullOrWhiteSpace(txt))
            {
                string p = txt.Trim().Trim('"');
                if (Directory.Exists(p)) return p;
                if (File.Exists(p)) return Path.GetDirectoryName(p);
            }
            return null;
        }

        private void ActualizarEstadoShutdown()
        {
            bool tiempoActivo = rdoHoraFija.Checked;
            numHoras.Enabled = tiempoActivo;
            numMinutos.Enabled = tiempoActivo;
            numSegundos.Enabled = tiempoActivo;
        }

        private void ValidarHerramientas()
        {
            string ruta = txtHerramientas.Text?.Trim();
            bool ok = !string.IsNullOrWhiteSpace(ruta)
                   && Directory.Exists(ruta)
                   && File.Exists(Path.Combine(ruta, "yt-dlp.exe"));
            lblHerramientasAviso.Visible = !ok;
            lblHerramientasAviso.Text = ok ? ""
                : "⚠ No se encontró yt-dlp.exe en esa carpeta.";
        }

        private void MostrarOcultarBloqueMySql()
        {
            bool on = chkMySQL.Checked;
            txtHost.Visible = on;
            txtUser.Visible = on;
            txtPass.Visible = on;
            txtPort.Visible = on;
            lblHost.Visible = on;
            lblUser.Visible = on;
            lblPass.Visible = on;
            lblPort.Visible = on;
            lblTipoServidor.Visible = on;
            btnProbarMySql.Visible = on;
            chkMantenerPngs.Visible = on;
            chkMantenerPngs.Enabled = on;
        }

        private void RefrescarTipoServidor()
        {
            var tmp = new MySqlConfiguracion { Host = txtHost.Text?.Trim() ?? "" };
            bool online = tmp.Tipo == MySqlConfiguracion.TipoServidor.Online;
            lblTipoServidor.Text = online ? "🌐 Servidor ONLINE" : "🏠 Servidor LOCAL";
            lblTipoServidor.ForeColor = online
                ? Color.FromArgb(220, 170, 60)
                : Color.FromArgb(120, 200, 140);
        }

        private void ProbarMySql()
        {
            var cfg = new MySqlConfiguracion
            {
                Habilitado = chkMySQL.Checked,
                Host = txtHost.Text?.Trim() ?? "",
                Usuario = txtUser.Text?.Trim() ?? "",
                Password = txtPass.Text ?? "",
                Puerto = int.TryParse(txtPort.Text, out var p) ? p : 3306
            };

            if (!cfg.Validar(out string err))
            {
                MessageBox.Show(this, "Configuración inválida:\n" + err, "MySQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cs = $"Server={cfg.Host};Port={cfg.Puerto};User ID={cfg.Usuario};Password={cfg.Password};SslMode=None;Connection Timeout=5;";
            try
            {
                using (var conn = new MySqlConnection(cs))
                {
                    conn.Open();
                    MessageBox.Show(this, "✅ Conexión OK.\nServidor: " + conn.ServerVersion,
                        "MySQL", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "❌ No se pudo conectar:\n" + ex.Message, "MySQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void PrecargarShutdown(ShutdownModo modo, DateTime timestamp)
        {
            if (modo == ShutdownModo.Tiempo && timestamp > DateTime.Now)
            {
                var restante = timestamp - DateTime.Now;
                rdoHoraFija.Checked = true;
                numHoras.Value = Math.Max(0, Math.Min(24, (int)restante.TotalHours));
                numMinutos.Value = Math.Max(0, Math.Min(59, restante.Minutes));
                numSegundos.Value = Math.Max(0, Math.Min(59, restante.Seconds));
            }
            else if (modo == ShutdownModo.AlTerminarLista) rdoAlTerminarLista.Checked = true;
            else rdoSinShutdown.Checked = true;
            ActualizarEstadoShutdown();
        }

        private void CerrarYGuardar()
        {
            CarpetaDescargas = txtDescargas.Text?.Trim();
            CarpetaHerramientas = txtHerramientas.Text?.Trim();

            if (rdoHoraFija.Checked)
            {
                ShutdownElegido = ShutdownModo.Tiempo;
                int h = (int)numHoras.Value, m = (int)numMinutos.Value, sg = (int)numSegundos.Value;
                ShutdownTiempo = new TimeSpan(h, m, sg);
                if (ShutdownTiempo.TotalSeconds < 5) ShutdownTiempo = TimeSpan.FromSeconds(5);
            }
            else if (rdoAlTerminarLista.Checked)
            { ShutdownElegido = ShutdownModo.AlTerminarLista; ShutdownTiempo = TimeSpan.Zero; }
            else { ShutdownElegido = ShutdownModo.Ninguno; ShutdownTiempo = TimeSpan.Zero; }

            if (chkMySQL.Checked)
            {
                _mySqlCfg = new MySqlConfiguracion
                {
                    Habilitado = true,
                    Host = txtHost.Text?.Trim() ?? "",
                    Usuario = txtUser.Text?.Trim() ?? "",
                    Password = txtPass.Text ?? "",
                    Puerto = int.TryParse(txtPort.Text, out var pp) ? pp : 3306,
                    MantenerPngs = chkMantenerPngs.Checked
                };
                if (!_mySqlCfg.Validar(out string errMySql))
                {
                    MessageBox.Show(this, "Configuración MySQL inválida:\n" + errMySql,
                        "MySQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _mySqlCfg.Guardar();
            }
            else
            {
                _mySqlCfg = new MySqlConfiguracion { Habilitado = false, MantenerPngs = false };
                _mySqlCfg.Guardar();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
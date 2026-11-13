using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YOUTUBE
{
    public partial class Form1 : Form
    {
        private static readonly Color FgPrincipal = Color.FromArgb(235, 235, 240);
        private static readonly Color FgSec = Color.FromArgb(165, 170, 185);
        private static readonly Color Accent = Color.FromArgb(220, 40, 40);
        private static readonly Color BgButton = Color.FromArgb(55, 58, 72);
        private static readonly Color BgVerde = Color.FromArgb(60, 110, 70);

        private const int INSTAGRAM_MAX_POR_DIA = 4;
        private const string REG_KEY = @"Software\YOUTUBEDownloader";
        private const string REG_DATE = "InstagramDate";
        private const string REG_COUNT = "InstagramCount";
        private const string REG_HIST_KEY = @"Software\YOUTUBEDownloader\Historial";
        private const string REG_PROCESO = @"Software\YOUTUBEDownloader\EnProceso";
        private const string REG_PARCIALES = @"Software\YOUTUBEDownloader\Parciales";
        private const string REG_SHUT = @"Software\YOUTUBEDownloader\Shutdown";

        private string _carpetaDestino;
        private string _carpetaPngBase;

        private bool _modoHistorial = false;
        private bool _usuarioCambioModoManual = false;
        private bool _huboDescargasEnSesion = false;
        private bool _mantenerPngs = false;
        private bool _mantenerPngsLeido = false;

        private readonly List<Form3> _historial = new List<Form3>();
        private readonly Dictionary<DescargaItem, Form2> _enProceso = new Dictionary<DescargaItem, Form2>();

        private readonly Dictionary<string, string> _urlANombreBase =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private Form4.ShutdownModo _shutdownModo = Form4.ShutdownModo.Ninguno;
        private DateTime _shutdownTimestamp = DateTime.MinValue;
        private System.Windows.Forms.Timer _shutdownTimer;
        private System.Windows.Forms.Timer _timerReintentoYouTube;
        private System.Windows.Forms.Timer _timerIncrustarPendientes;
        private System.Windows.Forms.Timer _timerMySqlPoll;
        private System.Windows.Forms.Timer _timerMySqlPendientes;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
                                                       int nRightRect, int nBottomRect,
                                                       int nWidthEllipse, int nHeightEllipse);

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private static extern void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private static extern void SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        public Form1()
        {
            InitializeComponent();

            // ═══ Extraer recursos embebidos si faltan ═══
            try
            {
                string carpetaExe = Path.GetDirectoryName(Application.ExecutablePath);
                string marker = Path.Combine(carpetaExe, "_extraido.marker");
                string ytDlp = Path.Combine(carpetaExe, "yt-dlp.exe");
                string ffmpeg = Path.Combine(carpetaExe, "ffmpeg.exe");
                string deno = Path.Combine(carpetaExe, "deno.exe");
                string webp = Path.Combine(carpetaExe, "libwebp_x64.dll");

                bool todosPresentes = File.Exists(ytDlp) && File.Exists(ffmpeg) && File.Exists(deno) && File.Exists(webp);
                bool extraer = false;

                if (!todosPresentes) extraer = true;
                else if (File.Exists(marker))
                {
                    try
                    {
                        for (int a = 0; a < ListaRecursos.Archivos.Length; a++)
                        {
                            var arch = ListaRecursos.Archivos[a];
                            string p = Path.Combine(carpetaExe, arch.RutaRelativa);
                            if (!File.Exists(p) || new FileInfo(p).Length != arch.TamanoOriginal) { extraer = true; break; }
                        }
                    }
                    catch { extraer = true; }
                }
                else extraer = true;

                if (extraer && ListaRecursos.Archivos != null && ListaRecursos.Archivos.Length > 0)
                {
                    for (int a = 0; a < ListaRecursos.Archivos.Length; a++)
                    {
                        var arch = ListaRecursos.Archivos[a];
                        string rutaFinal = Path.Combine(carpetaExe, arch.RutaRelativa);
                        string dir = Path.GetDirectoryName(rutaFinal);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

                        using (var fs = new FileStream(rutaFinal, FileMode.Create, FileAccess.Write))
                        {
                            for (int f = 0; f < arch.NumeroFragmentos; f++)
                            {
                                Type tipo = ListaRecursos.Fragmentos[arch.IndicePrimerFragmento + f];
                                var campo = tipo.GetField("Base64Data");
                                if (campo == null) continue;
                                string frag = campo.GetValue(null) as string;
                                if (string.IsNullOrEmpty(frag)) continue;
                                int usable = frag.Length - (frag.Length % 4);
                                if (usable <= 0) continue;
                                byte[] bloque = Convert.FromBase64String(frag.Substring(0, usable));
                                fs.Write(bloque, 0, bloque.Length);
                            }
                        }
                    }
                    try { File.WriteAllText(marker, DateTime.Now.ToString("o")); } catch { }
                }

                try { PathManager.ConfigurarCarpetaHerramientas(carpetaExe); } catch { }
                try { string diag = WebP.Diagnose(); Debug.WriteLine("[WebP-Diag] " + diag); } catch { }
            }
            catch (Exception exInit) { Debug.WriteLine("[Init] EX: " + exInit); }

            var ctx = new ContextMenuStrip();
            ctx.Items.Add("📋 Pegar URL", null, (s, e) => PegarUrlDelPortapapeles());
            ctx.Items.Add("🧹 Limpiar todo", null, (s, e) => LimpiarTodo());
            flowLista.ContextMenuStrip = ctx;
            this.ContextMenuStrip = ctx;

            PathManager.Cargar();
            _carpetaDestino = Path.Combine(PathManager.CarpetaDescargas, "Descargas");
            _carpetaPngBase = Path.Combine(PathManager.CarpetaDescargas, "png");
            Directory.CreateDirectory(_carpetaDestino);
            Directory.CreateDirectory(_carpetaPngBase);

            MySqlSync.Equipo = Environment.MachineName;
            try { MySqlSync.RegistrarEquipo(); } catch { }

            if (btnPegar != null) btnPegar.Click += (s, e) => PegarUrlDelPortapapeles();
            if (btnLimpiar != null) btnLimpiar.Click += (s, e) => LimpiarTodo();

            btnDescargados.Click += (s, e) =>
            {
                _usuarioCambioModoManual = true;
                _modoHistorial = !_modoHistorial;
                ActualizarVisibilidadPaneles();
                lblEstadoGlobal.Text = _modoHistorial
                    ? $"  ✅ Mostrando {_historial.Count} descarga(s) completada(s)."
                    : "  ⬇ Mostrando descargas en proceso.";
                AjustarAnchoTarjetas();
            };

            btnConfiguracion.Click += (s, e) => AbrirConfiguracion();

            if (btnBorrarRegistro != null)
                btnBorrarRegistro.Click += (s, e) => BorrarTodoElRegistro();

            if (btnResetearPaths != null)
                btnResetearPaths.Click += (s, e) => ResetearPathsYRefrescar();

            flowLista.Resize += (s, e) => AjustarAnchoTarjetas();
            flowLista.ControlAdded += (s, e) => AjustarAnchoTarjetas();
            AjustarAnchoTarjetas();

            this.AllowDrop = true;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;
            flowLista.AllowDrop = true;
            flowLista.DragEnter += Form1_DragEnter;
            flowLista.DragDrop += Form1_DragDrop;

            var timerValidador = new System.Windows.Forms.Timer { Interval = 5000 };
            timerValidador.Tick += (s, e) => ValidarHistorial();
            timerValidador.Start();

            _shutdownTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _shutdownTimer.Tick += (s, e) => ComprobarShutdown();

            _timerReintentoYouTube = new System.Windows.Forms.Timer { Interval = 15000 };
            _timerReintentoYouTube.Tick += (s, e) => ReintentarYouTubeFallidos();
            _timerReintentoYouTube.Start();

            _timerIncrustarPendientes = new System.Windows.Forms.Timer { Interval = 10000 };
            _timerIncrustarPendientes.Tick += (s, e) => ReintentarIncrustacionesPendientes();
            _timerIncrustarPendientes.Start();

            _timerMySqlPoll = new System.Windows.Forms.Timer { Interval = 3000 };
            _timerMySqlPoll.Tick += async (s, e) =>
            {
                try { await MySqlSync.PollComandosAsync(this); } catch { }
            };
            _timerMySqlPoll.Start();

            _timerMySqlPendientes = new System.Windows.Forms.Timer { Interval = 5000 };
            _timerMySqlPendientes.Tick += (s, e) => ProcesarPendientesMySql();
            _timerMySqlPendientes.Start();

            CargarHistorialDesdeDisco();
            CargarProcesosDesdeDisco();
            CargarParcialesDesdeDisco();
            DetectarParcialesHuerfanos();
            CargarShutdownGuardado();

            try { lblEstadoGlobal.Text = "  " + WebP.Diagnose(); }
            catch (Exception ex) { lblEstadoGlobal.Text = "  ❌ Diagnóstico WebP: " + ex.Message; }

            if (!PathManager.HerramientasValidas())
                lblEstadoGlobal.Text = "  ⚠ No se encontró yt-dlp.exe. Abre ⚙ Configuración o arrastra los EXEs.";
        }

        private bool MantenerPngs
        {
            get
            {
                if (!_mantenerPngsLeido)
                {
                    try { _mantenerPngs = MySqlConfiguracion.Cargar().MantenerPngs; }
                    catch { _mantenerPngs = false; }
                    _mantenerPngsLeido = true;
                }
                return _mantenerPngs;
            }
        }

        // ══════════════════════════════════════════════════════════════
        // MySQL
        // ══════════════════════════════════════════════════════════════
        private void ProcesarPendientesMySql()
        {
            if (!MySqlSync.Habilitado()) return;
            if (!PathManager.HerramientasValidas()) return;

            List<string> pendientes;
            try { pendientes = MySqlSync.ObtenerPendientes(); }
            catch { return; }

            if (pendientes == null || pendientes.Count == 0) return;

            foreach (var url in pendientes)
            {
                bool yaEsta = false;
                foreach (var kv in _enProceso)
                {
                    if (string.Equals(kv.Key.Url, url, StringComparison.OrdinalIgnoreCase))
                    { yaEsta = true; break; }
                }
                if (yaEsta) continue;
                if (!EsUrlSoportada(url)) continue;

                _usuarioCambioModoManual = false;
                if (_modoHistorial) { _modoHistorial = false; ActualizarVisibilidadPaneles(); }
                CrearPanelDescarga(url);
                lblEstadoGlobal.Text = "  📥 URL agregada desde panel web.";
            }
        }

        public void CancelarApagadoRemoto()
        {
            _shutdownModo = Form4.ShutdownModo.Ninguno;
            _shutdownTimestamp = DateTime.MinValue;
            try { _shutdownTimer?.Stop(); } catch { }
            GuardarShutdown();
            try { MySqlSync.MarcarEstadoEquipo("activo"); } catch { }
            lblEstadoGlobal.Text = "  🔕 Apagado remoto cancelado.";
        }

        public void EjecutarApagadoRemoto(string tipo, int valorSegundos)
        {
            try { MySqlSync.MarcarEstadoEquipo("apagando"); } catch { }

            if (tipo == "tiempo" && valorSegundos > 0)
            {
                _shutdownModo = Form4.ShutdownModo.Tiempo;
                _shutdownTimestamp = DateTime.Now.AddSeconds(valorSegundos);
                _shutdownTimer.Start();
                lblEstadoGlobal.Text = $"  ⏱ Apagado remoto en {valorSegundos}s.";
            }
            else if (tipo == "al_terminar")
            {
                _shutdownModo = Form4.ShutdownModo.AlTerminarLista;
                _shutdownTimestamp = DateTime.MinValue;
                _shutdownTimer.Stop();
                lblEstadoGlobal.Text = "  ⏱ Apagado remoto al terminar la lista.";
            }
            else
            {
                lblEstadoGlobal.Text = "  🔴 Apagado remoto inmediato.";
                ApagarSistema();
            }
        }

        // ══════════════════════════════════════════════════════════════
        // Reintentos
        // ══════════════════════════════════════════════════════════════
        private void ReintentarYouTubeFallidos()
        {
            if (!PathManager.HerramientasValidas()) return;

            int activas = 0;
            Form2 pendiente = null;

            foreach (Control c in flowLista.Controls)
            {
                if (c is Form2 f2 && !f2.IsDisposed)
                {
                    var desc = f2.Tag as DescargaItem;
                    if (desc == null) continue;

                    if (desc.Iniciada && !desc.EnPausa) activas++;
                    else if (f2.BtnDescargar.Text.Contains("Reintentar")
                             && f2.BtnDescargar.Enabled
                             && EsUrlYouTubeValida(desc.Url))
                    {
                        if (pendiente == null) pendiente = f2;
                    }
                }
            }

            if (activas > 0 || pendiente == null) return;

            var descarga = pendiente.Tag as DescargaItem;
            if (descarga == null) return;

            try
            {
                pendiente.SetEstadoOcupado();
                lblEstadoGlobal.Text = "  🔄 Reintentando YouTube automáticamente…";

                _ = descarga.IniciarAsync(pendiente.TxtDestino.Text, pendiente.ChkSub.Checked)
                    .ContinueWith(t =>
                    {
                        try
                        {
                            if (t.Result)
                                pendiente.BeginInvoke(new Action(() => pendiente.SetEstadoListo()));
                            else if (!descarga.EnPausa)
                                pendiente.BeginInvoke(new Action(() => pendiente.SetEstadoReintentar()));
                        }
                        catch { }
                    });
            }
            catch (Exception ex) { Debug.WriteLine("[AutoRetryYouTube] " + ex.Message); }
        }

        private void ReintentarIncrustacionesPendientes()
        {
            if (!PathManager.HerramientasValidas()) return;

            foreach (Control c in flowLista.Controls)
            {
                if (!(c is Form2 f2) || f2.IsDisposed) continue;
                var desc = f2.Tag as DescargaItem;
                if (desc == null) continue;
                if (!desc.HayDescargaCompletaPendienteDeCaratula) continue;
                if (desc.Iniciada) continue;
                if (!f2.BtnDescargar.Enabled) continue;
                if (!f2.BtnDescargar.Text.Contains("Reintentar")) continue;

                try
                {
                    lblEstadoGlobal.Text = "  🔄 Reintentando incrustar carátula automáticamente…";
                    f2.SetEstadoOcupado();
                    _ = desc.ReintentarIncrustacionAsync(f2.TxtDestino.Text)
                        .ContinueWith(t =>
                        {
                            try
                            {
                                if (t.Result)
                                    f2.BeginInvoke(new Action(() => f2.SetEstadoListo()));
                                else if (!desc.EnPausa)
                                    f2.BeginInvoke(new Action(() => f2.SetEstadoReintentar()));
                            }
                            catch { }
                        });
                    break;
                }
                catch (Exception ex) { Debug.WriteLine("[ReintentarIncrustacion] " + ex.Message); }
            }
        }

      protected override void OnFormClosing(FormClosingEventArgs e)
{
    try { GuardarParcialesEnDisco(); } catch { }
    try { GuardarProcesosEnDisco(); } catch { }
    try { GuardarHistorialEnDisco(); } catch { }
    try { GuardarShutdown(); } catch { }
    try { MySqlSync.MarcarEstadoEquipo("apagado"); } catch { }

    // Matar procesos yt-dlp/ffmpeg hijos para liberar .part
    try
    {
        foreach (var kv in _enProceso.ToList())
        {
            try { kv.Key.Cancelar(); } catch { }
        }
    }
    catch { }

    base.OnFormClosing(e);
}

        private void LimpiarPngsTemporales()
        {
            if (MantenerPngs) return;
            try
            {
                if (!Directory.Exists(_carpetaPngBase)) return;
                foreach (var f in Directory.GetFiles(_carpetaPngBase))
                { try { File.Delete(f); } catch { } }
                foreach (var d in Directory.GetDirectories(_carpetaPngBase))
                { try { Directory.Delete(d, true); } catch { } }
            }
            catch (Exception ex) { Debug.WriteLine("[LimpiarPngs] " + ex.Message); }
        }

        private void ResetearPathsYRefrescar()
        {
            var dr = MessageBox.Show(
                "¿Restablecer las carpetas a los valores por defecto?\n\n" +
                "· Descargas → %USERPROFILE%\\Downloads\\YOUTUBEDownloader\n" +
                "· Herramientas → carpeta del ejecutable\n\n" +
                "Los archivos descargados NO se borran.",
                "Resetear rutas", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr != DialogResult.Yes) return;

            PathManager.Reset();

            _carpetaDestino = Path.Combine(PathManager.CarpetaDescargas, "Descargas");
            _carpetaPngBase = Path.Combine(PathManager.CarpetaDescargas, "png");
            try { Directory.CreateDirectory(_carpetaDestino); } catch { }
            try { Directory.CreateDirectory(_carpetaPngBase); } catch { }

            foreach (var kv in _enProceso)
            {
                if (kv.Value != null && !kv.Value.IsDisposed)
                    kv.Value.SetDestino(_carpetaDestino);
            }

            lblEstadoGlobal.Text = "  🔄 Rutas restablecidas a los valores por defecto.";
        }

        // ══════════════════════════════════════════════════════════════
        // PNG helpers
        // ══════════════════════════════════════════════════════════════
        private Image CargarPngConFallback(string pngPathGuardado, string nombreBase)
        {
            if (!string.IsNullOrWhiteSpace(pngPathGuardado) && File.Exists(pngPathGuardado))
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(pngPathGuardado);
                    using (var ms = new MemoryStream(bytes))
                        return Image.FromStream(ms);
                }
                catch { }
            }

            if (!string.IsNullOrWhiteSpace(nombreBase) && Directory.Exists(_carpetaPngBase))
            {
                try
                {
                    string cand = Path.Combine(_carpetaPngBase, nombreBase + ".png");
                    if (File.Exists(cand))
                    {
                        byte[] bytes = File.ReadAllBytes(cand);
                        using (var ms = new MemoryStream(bytes))
                            return Image.FromStream(ms);
                    }

                    var encontrados = Directory.GetFiles(_carpetaPngBase, nombreBase + "*.png", SearchOption.TopDirectoryOnly);
                    if (encontrados.Length > 0)
                    {
                        byte[] bytes = File.ReadAllBytes(encontrados[0]);
                        using (var ms = new MemoryStream(bytes))
                            return Image.FromStream(ms);
                    }
                }
                catch { }
            }

            if (!string.IsNullOrWhiteSpace(pngPathGuardado))
            {
                try
                {
                    string dir = Path.GetDirectoryName(pngPathGuardado);
                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir) && !string.IsNullOrWhiteSpace(nombreBase))
                    {
                        string cand = Path.Combine(dir, nombreBase + ".png");
                        if (File.Exists(cand))
                        {
                            byte[] bytes = File.ReadAllBytes(cand);
                            using (var ms = new MemoryStream(bytes))
                                return Image.FromStream(ms);
                        }
                    }
                }
                catch { }
            }
            return null;
        }

        private void ActualizarPngDeProceso(DescargaItem descarga, string pngPath)
        {
            if (descarga == null || string.IsNullOrWhiteSpace(pngPath)) return;
            if (_enProceso.TryGetValue(descarga, out var card) && card != null && !card.IsDisposed)
            {
                card.PngPathActual = pngPath;
                GuardarProcesosEnDisco();
            }
        }

        private static string SanitizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return "video";
            var sb = new StringBuilder(nombre.Length);
            char[] invalid = Path.GetInvalidFileNameChars();
            foreach (char c in nombre)
                if (Array.IndexOf(invalid, c) < 0) sb.Append(c);
                else sb.Append('_');
            string s = sb.ToString().Trim();
            if (s.Length > 180) s = s.Substring(0, 180).Trim();
            return s.Length == 0 ? "video" : s;
        }

        private void AbrirConfiguracion()
        {
            using (var f4 = new Form4())
            {
                f4.PrecargarShutdown(_shutdownModo, _shutdownTimestamp);
                var dr = f4.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    PathManager.Guardar(f4.CarpetaDescargas, f4.CarpetaHerramientas);

                    _carpetaDestino = Path.Combine(PathManager.CarpetaDescargas, "Descargas");
                    _carpetaPngBase = Path.Combine(PathManager.CarpetaDescargas, "png");
                    Directory.CreateDirectory(_carpetaDestino);
                    Directory.CreateDirectory(_carpetaPngBase);

                    _mantenerPngsLeido = false;

                    _shutdownModo = f4.ShutdownElegido;
                    if (_shutdownModo == Form4.ShutdownModo.Tiempo)
                    {
                        _shutdownTimestamp = DateTime.Now + f4.ShutdownTiempo;
                        _shutdownTimer.Start();
                        lblEstadoGlobal.Text = $"  ⏱ Cierre programado en {f4.ShutdownTiempo:hh\\:mm\\:ss}.";
                    }
                    else if (_shutdownModo == Form4.ShutdownModo.AlTerminarLista)
                    {
                        _shutdownTimestamp = DateTime.MinValue;
                        _shutdownTimer.Stop();
                        lblEstadoGlobal.Text = "  ⏱ Cierre al terminar la lista activo.";
                    }
                    else
                    {
                        _shutdownTimestamp = DateTime.MinValue;
                        _shutdownTimer.Stop();
                        lblEstadoGlobal.Text = "  🔕 Cierre automático desactivado.";
                    }

                    GuardarShutdown();
                    try { MySqlSync.Equipo = Environment.MachineName; MySqlSync.RegistrarEquipo(); } catch { }

                    if (!PathManager.HerramientasValidas())
                        lblEstadoGlobal.Text = "  ⚠ Configuración guardada pero yt-dlp.exe no se encontró.";
                }
            }
        }

        private void CargarShutdownGuardado()
        {
            _shutdownModo = Form4.ShutdownModo.Ninguno;
            _shutdownTimestamp = DateTime.MinValue;
            try { _shutdownTimer?.Stop(); } catch { }
        }

        private void GuardarShutdown()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(REG_SHUT))
                {
                    if (key == null) return;
                    string modo = _shutdownModo == Form4.ShutdownModo.Tiempo ? "Tiempo"
                               : _shutdownModo == Form4.ShutdownModo.AlTerminarLista ? "AlTerminarLista"
                               : "Ninguno";
                    key.SetValue("Modo", modo);
                    long unix = _shutdownTimestamp == DateTime.MinValue
                        ? 0 : new DateTimeOffset(_shutdownTimestamp).ToUnixTimeSeconds();
                    key.SetValue("Timestamp", unix);
                }
            }
            catch { }
        }

        private void ComprobarShutdown()
        {
            if (_shutdownModo != Form4.ShutdownModo.Tiempo) return;
            if (_shutdownTimestamp == DateTime.MinValue) return;
            if (DateTime.Now >= _shutdownTimestamp)
            {
                _shutdownTimer.Stop();
                ApagarSistema();
            }
        }

        private void IntentarShutdownAlTerminarLista()
        {
            if (_shutdownModo != Form4.ShutdownModo.AlTerminarLista) return;
            if (!_huboDescargasEnSesion) return;
            if (_enProceso.Count > 0) return;
            if (_historial.Count == 0) return;

            foreach (Control c in flowLista.Controls)
            {
                if (c is Form2 f2 && !f2.IsDisposed && f2.Visible
                    && f2.BtnDescargar.Enabled && f2.BtnDescargar.Text.Contains("Reintentar"))
                    return;
            }

            _shutdownTimer.Stop();
            ApagarSistema();
        }

        private void ApagarSistema()
        {
            try { MySqlSync.MarcarEstadoEquipo("apagado"); } catch { }
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "shutdown.exe",
                    Arguments = "/s /t 0 /f",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            catch (Exception ex) { Debug.WriteLine("[Shutdown] Error: " + ex.Message); }

            Application.Exit();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var items = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (items != null && items.Length > 0)
                {
                    foreach (var path in items)
                    {
                        if (Directory.Exists(path) && PathManager.IntentarDescubrirHerramientas(path))
                        { lblEstadoGlobal.Text = "  ✅ Carpeta de herramientas actualizada: " + path; return; }
                    }
                    foreach (var path in items)
                    {
                        if (File.Exists(path))
                        {
                            string dir = Path.GetDirectoryName(path);
                            if (PathManager.IntentarDescubrirHerramientas(dir))
                            { lblEstadoGlobal.Text = "  ✅ Carpeta de herramientas actualizada: " + dir; return; }
                        }
                    }
                    lblEstadoGlobal.Text = "  ⚠ No se detectó yt-dlp.exe en lo arrastrado.";
                }
                return;
            }

            string txt = null;
            if (e.Data.GetDataPresent(DataFormats.UnicodeText))
                txt = e.Data.GetData(DataFormats.UnicodeText) as string;
            else if (e.Data.GetDataPresent(DataFormats.Text))
                txt = e.Data.GetData(DataFormats.Text) as string;

            if (string.IsNullOrWhiteSpace(txt)) return;
            txt = txt.Trim();

            foreach (var linea in txt.Split('\n'))
            {
                string cand = linea.Trim();
                if (EsUrlSoportada(cand)) { ProcesarUrlSoltada(cand); return; }
            }
            if (EsUrlSoportada(txt)) { ProcesarUrlSoltada(txt); return; }
            lblEstadoGlobal.Text = "  ❌ Texto soltado no es una URL soportada.";
        }

        private void ProcesarUrlSoltada(string url)
        {
            if (!PathManager.HerramientasValidas())
            { lblEstadoGlobal.Text = "  ⚠ No se encontró yt-dlp.exe. Configura la carpeta de herramientas."; return; }
            if (EsUrlInstagram(url))
            {
                int usadas;
                if (!PuedeDescargarInstagram(out usadas))
                { lblEstadoGlobal.Text = $"  ⛔ Límite diario de Instagram alcanzado ({usadas}/{INSTAGRAM_MAX_POR_DIA})."; return; }
            }
            _usuarioCambioModoManual = false;
            if (_modoHistorial) { _modoHistorial = false; ActualizarVisibilidadPaneles(); }
            CrearPanelDescarga(url);
            lblEstadoGlobal.Text = "  ⏳ URL soltada. Iniciando descarga...";
        }

        private void AjustarAnchoTarjetas()
        {
            if (flowLista == null || flowLista.IsDisposed) return;

            int scrollBar = SystemInformation.VerticalScrollBarWidth;
            int padding = flowLista.Padding.Left + flowLista.Padding.Right;
            int ancho = flowLista.ClientSize.Width - padding - scrollBar - 4;
            if (ancho < 720) ancho = 720;

            foreach (Control c in flowLista.Controls)
            {
                if ((c is Form2 || c is Form3) && c.Width != ancho) c.Width = ancho;
            }
        }

        private void ActualizarVisibilidadPaneles()
        {
            flowLista.SuspendLayout();
            foreach (Control c in flowLista.Controls)
            {
                if (c is Form2 f2) f2.Visible = !_modoHistorial;
                else if (c is Form3 f3) f3.Visible = _modoHistorial;
            }
            flowLista.ResumeLayout();
            if (btnDescargados != null)
                btnDescargados.Text = _modoHistorial ? "⬇ En proceso" : "✅ Descargados";
        }

        private void ComprobarAutoRedirect()
        {
            int activos = 0;
            foreach (Control c in flowLista.Controls)
                if (c is Form2 f2 && !f2.IsDisposed) activos++;

            if (activos == 0 && !_usuarioCambioModoManual && !_modoHistorial && _historial.Count > 0)
            {
                _modoHistorial = true;
                ActualizarVisibilidadPaneles();
                AjustarAnchoTarjetas();
                lblEstadoGlobal.Text = $"  ✅ Lista completada. Mostrando {_historial.Count} descarga(s) completada(s).";
            }

            IntentarAutoReintentarUltimo();
            IntentarShutdownAlTerminarLista();
        }

        private void IntentarAutoReintentarUltimo()
        {
            if (_enProceso.Count == 0) return;
            if (!PathManager.HerramientasValidas()) return;

            Form2 ultimoEnReintentar = null;
            DescargaItem ultimaDescarga = null;
            int totalProcesando = 0;

            var controles = new List<Form2>();
            foreach (Control c in flowLista.Controls)
                if (c is Form2 f2 && !f2.IsDisposed) controles.Add(f2);

            for (int i = controles.Count - 1; i >= 0; i--)
            {
                var f2 = controles[i];
                var desc = f2.Tag as DescargaItem;
                if (desc == null) continue;

                bool activa = desc.Iniciada && !desc.EnPausa;
                if (activa) totalProcesando++;

                if (!activa && f2.Visible
                    && f2.BtnDescargar.Text.Contains("Reintentar")
                    && f2.BtnDescargar.Enabled)
                {
                    if (ultimoEnReintentar == null)
                    { ultimoEnReintentar = f2; ultimaDescarga = desc; }
                }
            }

            if (totalProcesando > 0) return;
            if (ultimoEnReintentar == null || ultimaDescarga == null) return;

            try
            {
                if (!ultimaDescarga.Iniciada && ultimaDescarga.HayDescargaCompletaPendienteDeCaratula)
                {
                    _ = ultimaDescarga.ReintentarIncrustacionAsync(ultimoEnReintentar.TxtDestino.Text);
                }
                else
                {
                    ultimoEnReintentar.SetEstadoOcupado();
                    _ = ultimaDescarga.IniciarAsync(ultimoEnReintentar.TxtDestino.Text, ultimoEnReintentar.ChkSub.Checked)
                        .ContinueWith(t =>
                        {
                            try
                            {
                                if (t.Result)
                                    ultimoEnReintentar.BeginInvoke(new Action(() => ultimoEnReintentar.SetEstadoListo()));
                                else if (!ultimaDescarga.EnPausa)
                                    ultimoEnReintentar.BeginInvoke(new Action(() => ultimoEnReintentar.SetEstadoReintentar()));
                            }
                            catch { }
                        });
                }
                lblEstadoGlobal.Text = "  🔄 Reintentando automáticamente el último pendiente…";
            }
            catch (Exception ex) { Debug.WriteLine("[AutoRetry] " + ex.Message); }
        }

        private void ValidarHistorial()
        {
            bool cambio = false;
            for (int i = _historial.Count - 1; i >= 0; i--)
            {
                var f3 = _historial[i];
                if (f3 == null || f3.IsDisposed) { _historial.RemoveAt(i); cambio = true; continue; }
                if (!f3.ExisteArchivo())
                {
                    try { flowLista.Controls.Remove(f3); } catch { }
                    try { f3.PicMiniatura.Image?.Dispose(); } catch { }
                    f3.Dispose();
                    _historial.RemoveAt(i);
                    cambio = true;
                }
            }
            if (cambio)
            {
                GuardarHistorialEnDisco();
                AjustarAnchoTarjetas();
                if (_modoHistorial)
                    lblEstadoGlobal.Text = $"  ✅ Mostrando {_historial.Count} descarga(s) completada(s).";
            }
        }

        // ══════════════════════════════════════════════════════════════
        // Persistencia
        // ══════════════════════════════════════════════════════════════
        private void GuardarHistorialEnDisco()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(REG_HIST_KEY))
                {
                    if (key == null) return;
                    foreach (var name in key.GetValueNames())
                        try { key.DeleteValue(name, false); } catch { }
                    foreach (var sub in key.GetSubKeyNames())
                        try { key.DeleteSubKeyTree(sub, false); } catch { }

                    int i = 0;
                    foreach (var f3 in _historial)
                    {
                        if (f3 == null || f3.IsDisposed || !f3.ExisteArchivo()) continue;
                        using (var sub = key.CreateSubKey(i.ToString()))
                        {
                            if (sub == null) continue;
                            sub.SetValue("ArchivoPath", f3.ArchivoPath ?? "");
                            sub.SetValue("CarpetaDestino", f3.CarpetaDestino ?? "");
                            sub.SetValue("NombreBase", f3.NombreBase ?? "");
                            sub.SetValue("Titulo", f3.LblTitulo.Text ?? "");
                            sub.SetValue("Fecha", f3.LblFecha.Text ?? "");
                            sub.SetValue("Tamano", f3.LblTamano.Text ?? "");
                            sub.SetValue("PngPath", f3.PngPathGuardado ?? "");
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("[Historial] Error guardando: " + ex.Message); }
        }

        private void CargarHistorialDesdeDisco()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REG_HIST_KEY))
                {
                    if (key == null) return;
                    foreach (var subName in key.GetSubKeyNames())
                    {
                        using (var sub = key.OpenSubKey(subName))
                        {
                            if (sub == null) continue;
                            string archivoPath = sub.GetValue("ArchivoPath", "")?.ToString() ?? "";
                            string carpeta = sub.GetValue("CarpetaDestino", "")?.ToString() ?? "";
                            string nombreBase = sub.GetValue("NombreBase", "")?.ToString() ?? "";
                            string titulo = sub.GetValue("Titulo", "")?.ToString() ?? "";
                            string fecha = sub.GetValue("Fecha", "")?.ToString() ?? "";
                            string tamano = sub.GetValue("Tamano", "")?.ToString() ?? "";
                            string pngPath = sub.GetValue("PngPath", "")?.ToString() ?? "";

                            string archivoReal = archivoPath;
                            bool existe = !string.IsNullOrEmpty(archivoReal) && File.Exists(archivoReal);

                            if (!existe && !string.IsNullOrEmpty(carpeta) && Directory.Exists(carpeta) && !string.IsNullOrEmpty(nombreBase))
                            {
                                try
                                {
                                    string[] exts = { ".mp4", ".mkv", ".webm", ".mov" };
                                    foreach (var ext in exts)
                                    {
                                        string cand = Path.Combine(carpeta, nombreBase + ext);
                                        if (File.Exists(cand)) { archivoReal = cand; existe = true; break; }
                                    }
                                    if (!existe)
                                    {
                                        var encontrados = Directory.GetFiles(carpeta, nombreBase + ".*", SearchOption.AllDirectories);
                                        foreach (var f in encontrados)
                                        {
                                            string ex = Path.GetExtension(f).ToLowerInvariant();
                                            if (ex == ".mp4" || ex == ".mkv" || ex == ".webm" || ex == ".mov")
                                            { archivoReal = f; existe = true; break; }
                                        }
                                    }
                                }
                                catch { }
                            }
                            if (!existe) continue;

                            AgregarForm3AlHistorial(archivoReal, carpeta,
                                string.IsNullOrEmpty(nombreBase) ? Path.GetFileNameWithoutExtension(archivoReal) : nombreBase,
                                string.IsNullOrEmpty(titulo) ? Path.GetFileNameWithoutExtension(archivoReal) : titulo,
                                string.IsNullOrEmpty(fecha) ? "Completado: --" : fecha,
                                tamano, pngPath, persistir: false);
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("[Historial] Error cargando: " + ex.Message); }
        }

        private void GuardarProcesosEnDisco()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(REG_PROCESO))
                {
                    if (key == null) return;
                    foreach (var name in key.GetValueNames())
                        try { key.DeleteValue(name, false); } catch { }
                    foreach (var sub in key.GetSubKeyNames())
                        try { key.DeleteSubKeyTree(sub, false); } catch { }

                    int i = 0;
                    foreach (var kv in _enProceso)
                    {
                        var desc = kv.Key;
                        var card = kv.Value;
                        if (desc == null || card == null || card.IsDisposed) continue;
                        using (var sub = key.CreateSubKey(i.ToString()))
                        {
                            if (sub == null) continue;
                            sub.SetValue("Url", desc.Url ?? "");
                            sub.SetValue("Destino", card.TxtDestino?.Text ?? "");
                            sub.SetValue("Titulo", card.LblTitulo?.Text ?? "");
                            sub.SetValue("PngPath", card.PngPathActual ?? "");
                            sub.SetValue("Calidad", card.LblCalidad?.Text ?? "");
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("[Procesos] Error guardando: " + ex.Message); }
        }

        private void CargarProcesosDesdeDisco()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REG_PROCESO))
                {
                    if (key == null) return;
                    foreach (var subName in key.GetSubKeyNames())
                    {
                        using (var sub = key.OpenSubKey(subName))
                        {
                            if (sub == null) continue;
                            string url = sub.GetValue("Url", "")?.ToString() ?? "";
                            string destino = sub.GetValue("Destino", "")?.ToString() ?? "";
                            string titulo = sub.GetValue("Titulo", "")?.ToString() ?? "";
                            string png = sub.GetValue("PngPath", "")?.ToString() ?? "";
                            string calidad = sub.GetValue("Calidad", "")?.ToString() ?? "";
                            if (string.IsNullOrWhiteSpace(url)) continue;
                            RecrearForm2DesdeDisco(url, destino, titulo, png, calidad);
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("[Procesos] Error cargando: " + ex.Message); }
        }

        private void RecrearForm2DesdeDisco(string url, string destino, string titulo, string pngPath, string calidad)
        {
            var card = new Form2 { Width = 730, Height = 195, Margin = new Padding(0, 0, 0, 10) };
            card.SetUrl(url);
            card.SetDestino(string.IsNullOrWhiteSpace(destino) ? _carpetaDestino : destino);
            card.SetTitulo(string.IsNullOrWhiteSpace(titulo) ? "Video" : titulo);
            card.SetCalidad(string.IsNullOrWhiteSpace(calidad) ? "Restaurado" : calidad);

            var descarga = new DescargaItem(url, card);
            card.Tag = descarga;
            ConectarEventosForm2(card, descarga);
            _enProceso[descarga] = card;

            flowLista.SuspendLayout();
            flowLista.Controls.Add(card);
            flowLista.Controls.SetChildIndex(card, 0);
            flowLista.ResumeLayout();
            AjustarAnchoTarjetas();

            string nombreBase = !string.IsNullOrWhiteSpace(pngPath)
                ? Path.GetFileNameWithoutExtension(pngPath)
                : (!string.IsNullOrWhiteSpace(titulo) ? SanitizarNombre(titulo) : null);

            Image img = CargarPngConFallback(pngPath, nombreBase);
            if (img != null)
            {
                try
                {
                    card.PicMiniatura.Image?.Dispose();
                    card.PicMiniatura.Image = img;
                    card.PicMiniatura.SizeMode = PictureBoxSizeMode.Zoom;
                    card.PicMiniatura.Refresh();
                    card.PngPathActual = pngPath;
                    if (string.IsNullOrWhiteSpace(card.PngPathActual) && !string.IsNullOrWhiteSpace(nombreBase))
                    {
                        string cand = Path.Combine(_carpetaPngBase, nombreBase + ".png");
                        if (File.Exists(cand)) card.PngPathActual = cand;
                    }
                }
                catch { }
            }

            card.SetEstadoReintentar();
        }

        private void GuardarParcialesEnDisco()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(REG_PARCIALES))
                {
                    if (key == null) return;
                    foreach (var name in key.GetValueNames())
                        try { key.DeleteValue(name, false); } catch { }

                    int i = 0;
                    foreach (var kv in _urlANombreBase)
                    {
                        if (string.IsNullOrEmpty(kv.Key) || string.IsNullOrEmpty(kv.Value)) continue;
                        key.SetValue(i.ToString(), kv.Key + "|||" + kv.Value);
                        i++;
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("[Parciales] Error guardando: " + ex.Message); }
        }

        private void CargarParcialesDesdeDisco()
        {
            _urlANombreBase.Clear();
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REG_PARCIALES))
                {
                    if (key == null) return;
                    foreach (var name in key.GetValueNames())
                    {
                        string v = key.GetValue(name, "")?.ToString() ?? "";
                        int sep = v.IndexOf("|||", StringComparison.Ordinal);
                        if (sep <= 0) continue;
                        string url = v.Substring(0, sep);
                        string nom = v.Substring(sep + 3);
                        if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(nom))
                            _urlANombreBase[url] = nom;
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("[Parciales] Error cargando: " + ex.Message); }
        }

        public void RegistrarParcial(string url, string nombreBase)
{
    if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(nombreBase)) return;
    _urlANombreBase[url] = nombreBase;
    if (!_urlANombreBase.ContainsKey(url + "::alias"))
    {
        _urlANombreBase[url + "::alias"] = nombreBase;
    }

    GuardarParcialesEnDisco();
}

        public void OlvidarParcial(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            if (_urlANombreBase.Remove(url))
                GuardarParcialesEnDisco();
        }

      private void DetectarParcialesHuerfanos()
{
    try
    {
        if (string.IsNullOrWhiteSpace(_carpetaDestino) || !Directory.Exists(_carpetaDestino))
            return;

        // Buscar .part Y .ytdl Y .temp (todos son restos de descargas incompletas)
        var restos = new List<string>();
        foreach (var patron in new[] { "*.part", "*.ytdl", "*.temp" })
        {
            try { restos.AddRange(Directory.GetFiles(_carpetaDestino, patron, SearchOption.AllDirectories)); }
            catch { }
        }

        if (restos.Count == 0) return;

        // Ya hay paneles en proceso? Si sí, no duplicar
        var urlsEnProceso = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in _enProceso)
            urlsEnProceso.Add(kv.Key.Url);

        foreach (var restoPath in restos)
        {
            string nombreArchivo = Path.GetFileName(restoPath);

            // Quitar ".part", ".ytdl", ".temp" del final
            string baseSinExt = nombreArchivo;
            foreach (var ext in new[] { ".part", ".ytdl", ".temp" })
            {
                if (baseSinExt.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                {
                    baseSinExt = baseSinExt.Substring(0, baseSinExt.Length - ext.Length);
                    break;
                }
            }

            // Quitar CUALQUIER extensión al final: .mp4, .mkv, .webm, .mov, .ts,
            // .fXXX (formato de yt-dlp), .fvXXX, .faXXX, .audio, .video, .m4a, .m4v, etc.
            string nombreBaseLimpio = baseSinExt;
            while (true)
            {
                string ext = Path.GetExtension(nombreBaseLimpio);
                if (string.IsNullOrEmpty(ext)) break;

                string extLower = ext.ToLowerInvariant();
                bool esExtDeMedia = extLower == ".mp4" || extLower == ".mkv" || extLower == ".webm"
                                 || extLower == ".mov" || extLower == ".ts" || extLower == ".m4a"
                                 || extLower == ".m4v" || extLower == ".audio" || extLower == ".video"
                                 || extLower == ".flv" || extLower == ".3gp";
                bool esExtFormatoYtDlp = extLower.StartsWith(".f") && extLower.Length >= 3
                                     && int.TryParse(extLower.Substring(2), out _);
                bool esExtFragmento = extLower.StartsWith(".part-frag") || extLower.StartsWith(".temp-");

                if (esExtDeMedia || esExtFormatoYtDlp || esExtFragmento)
                    nombreBaseLimpio = nombreBaseLimpio.Substring(0, nombreBaseLimpio.Length - ext.Length);
                else
                    break;
            }

            // ── Buscar URL por 3 estrategias ──
            string urlEncontrada = null;

            // 1) Coincidencia exacta con _urlANombreBase
            foreach (var kv in _urlANombreBase)
            {
                if (string.Equals(kv.Value, nombreBaseLimpio, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(kv.Value, baseSinExt, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(kv.Value, nombreArchivo, StringComparison.OrdinalIgnoreCase))
                {
                    urlEncontrada = kv.Key;
                    break;
                }
            }

            // 2) Coincidencia por "empieza por" (por si yt-dlp añade sufijos)
            if (string.IsNullOrEmpty(urlEncontrada))
            {
                foreach (var kv in _urlANombreBase)
                {
                    if (string.IsNullOrEmpty(kv.Value)) continue;
                    if (nombreBaseLimpio.StartsWith(kv.Value, StringComparison.OrdinalIgnoreCase)
                        || kv.Value.StartsWith(nombreBaseLimpio, StringComparison.OrdinalIgnoreCase))
                    {
                        urlEncontrada = kv.Key;
                        break;
                    }
                }
            }

            // 3) Si no se encontró URL, es huérfano real: mostrarlo como panel
            //    con URL placeholder para que el usuario lo vea, pero NO reanudar
            //    automáticamente (no sabemos la URL).
            if (string.IsNullOrEmpty(urlEncontrada))
            {
                Debug.WriteLine("[Parciales] .part sin URL asociada: " + restoPath
                    + " · nombreBaseLimpio=" + nombreBaseLimpio
                    + " · baseSinExt=" + baseSinExt);
                continue;
            }

            // Ya hay un panel con esa URL?
            if (urlsEnProceso.Contains(urlEncontrada))
                continue;

            // Crear panel y reanudar
            RecrearForm2DesdeParcial(urlEncontrada, nombreBaseLimpio, restoPath);
            urlsEnProceso.Add(urlEncontrada);
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine("[Parciales] Detectar: " + ex.Message);
    }
}

        private void RecrearForm2DesdeParcial(string url, string nombreBase, string partPath)
        {
            var card = new Form2 { Width = 730, Height = 195, Margin = new Padding(0, 0, 0, 10) };
            card.SetUrl(url);
            card.SetDestino(_carpetaDestino);
            card.SetTitulo(nombreBase);
            card.SetCalidad("Parcial (reanudar)");

            var descarga = new DescargaItem(url, card);
            card.Tag = descarga;
            ConectarEventosForm2(card, descarga);
            _enProceso[descarga] = card;

            flowLista.SuspendLayout();
            flowLista.Controls.Add(card);
            flowLista.Controls.SetChildIndex(card, 0);
            flowLista.ResumeLayout();
            AjustarAnchoTarjetas();

            Image img = CargarPngConFallback(null, nombreBase);
            if (img != null)
            {
                try
                {
                    card.PicMiniatura.Image?.Dispose();
                    card.PicMiniatura.Image = img;
                    card.PicMiniatura.SizeMode = PictureBoxSizeMode.Zoom;
                    card.PicMiniatura.Refresh();
                    string cand = Path.Combine(_carpetaPngBase, nombreBase + ".png");
                    if (File.Exists(cand)) card.PngPathActual = cand;
                }
                catch { }
            }

            card.SetEstadoReintentar();
            lblEstadoGlobal.Text = $"  🔄 Parcial detectado: {nombreBase}. Reanudando…";

            // ══════════════════════════════════════════════════════════════
            // AUTO-REANUDAR: arranca la descarga desde el .part sin que el
            // usuario pulse Reintentar. La lambda captura 'card', 'descarga'
            // y 'formPadre' como variables locales → no hay CS0103/CS0136.
            // ══════════════════════════════════════════════════════════════
            var formPadre = this;

            _ = Task.Run(async () =>
            {
                await Task.Delay(800);

                if (card.IsDisposed) return;

                try
                {
                    card.BeginInvoke(new Action(async () =>
                    {
                        try
                        {
                            if (card.IsDisposed) return;

                            card.SetEstadoOcupado();

                            string destinoLocal = card.TxtDestino.Text?.Trim();
                            if (string.IsNullOrWhiteSpace(destinoLocal))
                                destinoLocal = formPadre._carpetaDestino;

                            bool ok = await descarga.IniciarAsync(destinoLocal, card.ChkSub.Checked);

                            if (card.IsDisposed) return;

                            if (ok) card.SetEstadoListo();
                            else if (!descarga.EnPausa) card.SetEstadoReintentar();

                            formPadre.lblEstadoGlobal.Text = ok
                                ? "  ✅ Parcial reanudado automáticamente."
                                : "  ⚠ No se pudo reanudar el parcial. Pulsa Reintentar.";
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("[AutoReanudar] " + ex.Message);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[AutoReanudar] " + ex.Message);
                }
            });
        }

        // ══════════════════════════════════════════════════════════════
        // Eventos Form2
        // ══════════════════════════════════════════════════════════════
        private void ConectarEventosForm2(Form2 card, DescargaItem descarga)
        {
            string url = descarga.Url;

            card.UrlDropped += (s, ev) => ProcesarUrlSoltada(ev.Url);

            card.BotonCarpetaClick += (s, e) =>
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    if (Directory.Exists(card.TxtDestino.Text)) fbd.SelectedPath = card.TxtDestino.Text;
                    if (fbd.ShowDialog() == DialogResult.OK) card.TxtDestino.Text = fbd.SelectedPath;
                }
            };

            card.AbrirUrlEnNavegador += (s, e) =>
            {
                try { Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true }); }
                catch (Exception ex) { lblEstadoGlobal.Text = "  ❌ No se pudo abrir el navegador: " + ex.Message; }
            };

            card.AbrirCarpetaDescargador += (s, e) =>
            {
                try
                {
                    string destino = card.TxtDestino.Text?.Trim();
                    if (string.IsNullOrWhiteSpace(destino) || !Directory.Exists(destino))
                    { destino = _carpetaDestino; Directory.CreateDirectory(destino); }
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"\"{destino}\"",
                        UseShellExecute = true
                    });
                    lblEstadoGlobal.Text = "  📂 Carpeta abierta: " + destino;
                }
                catch (Exception ex) { lblEstadoGlobal.Text = "  ❌ No se pudo abrir la carpeta: " + ex.Message; }
            };

            card.BotonEliminarClick += (s, e) =>
            {
                descarga.Cancelar();
                descarga.Dispose();
                if (card.PicMiniatura.Image != null) { card.PicMiniatura.Image.Dispose(); card.PicMiniatura.Image = null; }
                _enProceso.Remove(descarga);
                flowLista.Controls.Remove(card);
                card.Dispose();
                OlvidarParcial(descarga.Url);
                GuardarProcesosEnDisco();
                lblEstadoGlobal.Text = "  🗑 Panel eliminado.";
                ComprobarAutoRedirect();
            };

            card.BotonPausaClick += async (s, e) =>
            {
                if (descarga.EnPausa)
                {
                    descarga.EnPausa = false;
                    card.BtnPausa.Text = "⏸ Pausar";
                    card.BtnPausa.BackColor = BgButton;
                    card.SetEstadoOcupado();
                    await descarga.ReanudarAsync(card.TxtDestino.Text, card.ChkSub.Checked);
                    if (!descarga.EnPausa && !descarga.Iniciada) card.SetEstadoListo();
                    else card.SetEstadoReintentar();
                    return;
                }

                card.BtnPausa.Text = "⏳ Pausando...";
                card.BtnPausa.Enabled = false;
                await descarga.PausarAsync();
                card.BtnPausa.Text = "▶ Continuar";
                card.BtnPausa.BackColor = BgVerde;
                card.BtnPausa.Enabled = true;
                card.BtnDescargar.Text = "▶ Reanudar";
                card.BtnDescargar.BackColor = BgVerde;
                card.BtnDescargar.Enabled = true;
                card.SetEstadoDescargando();
                lblEstadoGlobal.Text = "  ⏸ Descarga pausada. Los .part se conservan.";
            };

            card.BotonDescargarClick += async (s, e) => { await EjecutarBotonDescargar(descarga, card); };
        }

        private static bool PuedeDescargarInstagram(out int usadasHoy)
        {
            usadasHoy = 0;
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(REG_KEY))
                {
                    if (key == null) return true;
                    string fechaGuardada = key.GetValue(REG_DATE, "").ToString();
                    string hoy = DateTime.Now.ToString("yyyyMMdd");
                    if (fechaGuardada != hoy)
                    {
                        key.SetValue(REG_DATE, hoy);
                        key.SetValue(REG_COUNT, 0);
                        usadasHoy = 0;
                        return true;
                    }
                    int count = 0;
                    int.TryParse(key.GetValue(REG_COUNT, "0").ToString(), out count);
                    usadasHoy = count;
                    return count < INSTAGRAM_MAX_POR_DIA;
                }
            }
            catch { return true; }
        }

        private static void IncrementarContadorInstagram()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(REG_KEY))
                {
                    if (key == null) return;
                    string hoy = DateTime.Now.ToString("yyyyMMdd");
                    string fechaGuardada = key.GetValue(REG_DATE, "").ToString();
                    int count = 0;
                    if (fechaGuardada == hoy)
                        int.TryParse(key.GetValue(REG_COUNT, "0").ToString(), out count);
                    key.SetValue(REG_DATE, hoy);
                    key.SetValue(REG_COUNT, count + 1);
                }
            }
            catch { }
        }

        private void PegarUrlDelPortapapeles()
        {
            if (!Clipboard.ContainsText())
            { lblEstadoGlobal.Text = "  ❌ El portapapeles no contiene texto."; return; }
            string url = Clipboard.GetText().Trim();
            if (!EsUrlSoportada(url))
            { lblEstadoGlobal.Text = "  ❌ URL no soportada."; return; }
            if (!PathManager.HerramientasValidas())
            { lblEstadoGlobal.Text = "  ⚠ No se encontró yt-dlp.exe. Configura la carpeta de herramientas."; return; }
            if (EsUrlInstagram(url))
            {
                int usadas;
                if (!PuedeDescargarInstagram(out usadas))
                { lblEstadoGlobal.Text = $"  ⛔ Límite diario de Instagram alcanzado ({usadas}/{INSTAGRAM_MAX_POR_DIA}). Intenta mañana."; return; }
            }
            _usuarioCambioModoManual = false;
            if (_modoHistorial) { _modoHistorial = false; ActualizarVisibilidadPaneles(); }
            CrearPanelDescarga(url);
            lblEstadoGlobal.Text = "  ⏳ Panel creado. Iniciando descarga automática...";
        }

        private static string DetectarPlataforma(string url)
        {
            if (EsUrlYouTubeValida(url)) return "YouTube";
            if (EsUrlFacebook(url)) return "Facebook";
            if (EsUrlInstagram(url)) return "Instagram";
            if (EsUrlThreads(url)) return "Threads";
            if (EsUrlOkRu(url)) return "OK.ru";
            if (EsUrlTwitch(url)) return "Twitch";
            return "desconocida";
        }

        private static bool EsUrlYouTubeValida(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;
            return url.IndexOf("youtube.com", StringComparison.OrdinalIgnoreCase) >= 0
                || url.IndexOf("youtu.be", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool EsUrlFacebook(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;
            return url.IndexOf("facebook.com", StringComparison.OrdinalIgnoreCase) >= 0
                || url.IndexOf("fb.watch", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool EsUrlInstagram(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;
            return url.IndexOf("instagram.com", StringComparison.OrdinalIgnoreCase) >= 0
                || url.IndexOf("ig.me", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool EsUrlThreads(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;
            return url.IndexOf("threads.net", StringComparison.OrdinalIgnoreCase) >= 0
                || url.IndexOf("threads.com", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool EsUrlOkRu(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;
            return url.IndexOf("ok.ru", StringComparison.OrdinalIgnoreCase) >= 0
                || url.IndexOf("odnoklassniki.ru", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool EsUrlTwitch(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;
            return url.IndexOf("twitch.tv", StringComparison.OrdinalIgnoreCase) >= 0
                || url.IndexOf("clips.twitch.tv", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool EsUrlSoportada(string url)
        {
            return EsUrlYouTubeValida(url) || EsUrlFacebook(url) || EsUrlInstagram(url)
                || EsUrlThreads(url) || EsUrlOkRu(url) || EsUrlTwitch(url);
        }

        private static bool RequiereH264Forzado(string url)
        {
            return EsUrlYouTubeValida(url) || EsUrlFacebook(url);
        }

        private void LimpiarTodo()
        {
            var paneles = new List<Control>();
            foreach (Control c in flowLista.Controls)
                if (c is Form2) paneles.Add(c);

            foreach (var c in paneles)
            {
                try { flowLista.Controls.Remove(c); } catch { }
                c.Dispose();
            }
            _enProceso.Clear();
            GuardarProcesosEnDisco();
            lblEstadoGlobal.Text = "  🧹 Lista de descargas en proceso limpiada.";
            ComprobarAutoRedirect();
        }

        private void BorrarTodoElRegistro()
        {
            var dr = MessageBox.Show(
                "¿Borrar TODA la configuración guardada?\n\n" +
                "· Historial de descargas\n· Descargas en proceso\n· Parciales (.part) recordados\n" +
                "· Cierre automático programado\n· Contador de Instagram\n· Carpetas configuradas\n\n" +
                "Los archivos descargados NO se borran, solo el registro de la app.\n\n¿Continuar?",
                "Borrar registro", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dr != DialogResult.Yes) return;

            _shutdownModo = Form4.ShutdownModo.Ninguno;
            _shutdownTimestamp = DateTime.MinValue;
            try { _shutdownTimer?.Stop(); } catch { }

            try { Registry.CurrentUser.DeleteSubKeyTree(@"Software\YOUTUBEDownloader", false); }
            catch (Exception ex) { Debug.WriteLine("[Registro] Error borrando: " + ex.Message); }

            _huboDescargasEnSesion = false;
            _usuarioCambioModoManual = false;
            _modoHistorial = false;

            try
            {
                flowLista.SuspendLayout();
                var copia = new List<Control>();
                foreach (Control c in flowLista.Controls) copia.Add(c);
                foreach (var c in copia)
                { try { flowLista.Controls.Remove(c); } catch { } try { c.Dispose(); } catch { } }
                flowLista.ResumeLayout();
            }
            catch { }

            _historial.Clear();
            _enProceso.Clear();
            _urlANombreBase.Clear();

            try
            {
                PathManager.Cargar();
                _carpetaDestino = Path.Combine(PathManager.CarpetaDescargas, "Descargas");
                _carpetaPngBase = Path.Combine(PathManager.CarpetaDescargas, "png");
                Directory.CreateDirectory(_carpetaDestino);
                Directory.CreateDirectory(_carpetaPngBase);
            }
            catch { }

            ActualizarVisibilidadPaneles();
            AjustarAnchoTarjetas();
            lblEstadoGlobal.Text = "  🧹 Registro borrado. Se volverá a guardar automáticamente al usar la app.";
        }

        private void CrearPanelDescarga(string url)
        {
            _huboDescargasEnSesion = true;

            var card = new Form2 { Width = 730, Height = 195, Margin = new Padding(0, 0, 0, 10) };
            card.SetUrl(url);
            card.SetDestino(_carpetaDestino);

            string etiquetaCalidad = "▶️ YouTube · HD 1080p";
            if (EsUrlFacebook(url)) etiquetaCalidad = "📘 Facebook · Reel";
            else if (EsUrlInstagram(url)) etiquetaCalidad = "📸 Instagram · Post";
            else if (EsUrlThreads(url)) etiquetaCalidad = "🧵 Threads";
            else if (EsUrlOkRu(url)) etiquetaCalidad = "🟠 OK.ru";
            else if (EsUrlTwitch(url)) etiquetaCalidad = "🟣 Twitch";
            card.SetCalidad(etiquetaCalidad);

            var descarga = new DescargaItem(url, card);
            card.Tag = descarga;
            ConectarEventosForm2(card, descarga);
            _enProceso[descarga] = card;

            flowLista.SuspendLayout();
            flowLista.Controls.Add(card);
            flowLista.Controls.SetChildIndex(card, 0);
            flowLista.ResumeLayout();
            flowLista.ScrollControlIntoView(card);
            AjustarAnchoTarjetas();

            try { MySqlSync.PublicarDescarga(url, null, DetectarPlataforma(url), "pendiente", 0); } catch { }

            _ = descarga.CargarMetadatosAsync();
            _ = AutoIniciarDescargaAsync(descarga, card);
        }

        private async Task EjecutarBotonDescargar(DescargaItem descarga, Form2 card)
        {
            if (descarga.Iniciada && !descarga.EnPausa && !descarga.HayDescargaCompletaPendienteDeCaratula)
            {
                descarga.EnPausa = true;
                card.BtnDescargar.Text = "⏳...";
                card.BtnDescargar.Enabled = false;
                await descarga.PausarAsync();
                card.BtnDescargar.Text = "▶ Reanudar";
                card.BtnDescargar.BackColor = BgVerde;
                card.BtnDescargar.Enabled = true;
                card.BtnPausa.Text = "▶ Continuar";
                card.BtnPausa.BackColor = BgVerde;
                return;
            }

            if (descarga.EnPausa)
            {
                descarga.EnPausa = false;
                card.SetEstadoOcupado();
                await descarga.ReanudarAsync(card.TxtDestino.Text, card.ChkSub.Checked);
                if (!descarga.EnPausa && !descarga.Iniciada) card.SetEstadoListo();
                else card.SetEstadoReintentar();
                return;
            }

            string destino = card.TxtDestino.Text?.Trim();
            if (string.IsNullOrWhiteSpace(destino))
            { lblEstadoGlobal.Text = "  ❌ Carpeta destino inválida."; return; }
            try { Directory.CreateDirectory(destino); }
            catch { lblEstadoGlobal.Text = "  ❌ No se pudo crear la carpeta."; return; }

            if (EsUrlInstagram(descarga.Url) && !descarga.Iniciada && !descarga.HayDescargaCompletaPendienteDeCaratula)
            {
                int usadas;
                if (!PuedeDescargarInstagram(out usadas))
                { lblEstadoGlobal.Text = $"  ⛔ Límite diario de Instagram alcanzado ({usadas}/{INSTAGRAM_MAX_POR_DIA})."; return; }
            }

            if (!descarga.Iniciada && descarga.HayDescargaCompletaPendienteDeCaratula)
            {
                card.SetEstadoOcupado();
                bool okRetry = await descarga.ReintentarIncrustacionAsync(destino);
                if (okRetry) card.SetEstadoListo();
                else card.SetEstadoReintentar();
                return;
            }

            if (descarga.Iniciada) { lblEstadoGlobal.Text = "  ℹ️ La descarga ya está en curso."; return; }

            card.SetEstadoOcupado();
            bool ok = await descarga.IniciarAsync(destino, card.ChkSub.Checked);
            if (ok) card.SetEstadoListo();
            else if (!descarga.EnPausa) card.SetEstadoReintentar();

            if (ok && EsUrlInstagram(descarga.Url)) IncrementarContadorInstagram();
        }

        private async Task AutoIniciarDescargaAsync(DescargaItem descarga, Form2 card)
        {
            await Task.Delay(200);
            if (descarga.Iniciada) return;
            string destino = card.TxtDestino.Text?.Trim();
            if (string.IsNullOrWhiteSpace(destino))
            { lblEstadoGlobal.Text = "  ❌ Carpeta destino inválida."; return; }
            try { Directory.CreateDirectory(destino); }
            catch { lblEstadoGlobal.Text = "  ❌ No se pudo crear la carpeta."; return; }

            card.SetEstadoOcupado();
            bool ok = await descarga.IniciarAsync(destino, card.ChkSub.Checked);
            if (ok) card.SetEstadoListo();
            else if (!descarga.EnPausa) card.SetEstadoReintentar();

            if (ok && EsUrlInstagram(descarga.Url)) IncrementarContadorInstagram();
        }

        private void MoverAHistorial(DescargaItem descarga, string mp4Path, string pngPath)
        {
            if (descarga == null) return;
            if (!_enProceso.TryGetValue(descarga, out var card2)) return;

            flowLista.SuspendLayout();
            flowLista.Controls.Remove(card2);
            flowLista.ResumeLayout();
            _enProceso.Remove(descarga);
            if (card2.PicMiniatura.Image != null) { card2.PicMiniatura.Image.Dispose(); card2.PicMiniatura.Image = null; }
            card2.Dispose();

            GuardarProcesosEnDisco();
            OlvidarParcial(descarga.Url);

            string carpeta = Path.GetDirectoryName(mp4Path);
            if (string.IsNullOrEmpty(carpeta)) carpeta = _carpetaDestino;
            string nombreBase = Path.GetFileNameWithoutExtension(mp4Path);

            string titulo = nombreBase;
            string fecha = "Completado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            string tamano = null;
            double tamMb = 0;
            try
            {
                if (File.Exists(mp4Path))
                {
                    var fi = new FileInfo(mp4Path);
                    tamano = (fi.Length / (1024.0 * 1024.0)).ToString("0.00") + " MB";
                    tamMb = fi.Length / 1048576.0;
                }
            }
            catch { }

            try { MySqlSync.PublicarDescarga(descarga.Url, nombreBase, DetectarPlataforma(descarga.Url), "completado", 100, tamMb, mp4Path); } catch { }

            if (!MantenerPngs)
            {
                try { if (!string.IsNullOrEmpty(pngPath) && File.Exists(pngPath)) File.Delete(pngPath); } catch { }
                LimpiarPngsTemporales();
            }

            string plataformaEtiqueta = DetectarPlataforma(descarga.Url);
            AgregarForm3AlHistorial(mp4Path, carpeta, nombreBase, titulo, fecha, tamano,
                pngPath, persistir: true, plataforma: plataformaEtiqueta);

            lblEstadoGlobal.Text = "  ✅ Descarga completada y movida al historial.";
            ComprobarAutoRedirect();
        }

        private void AgregarForm3AlHistorial(
            string archivoPath, string carpetaDestino, string nombreBase,
            string titulo, string fecha, string tamano,
            string pngPath = null, bool persistir = true,
            string plataforma = null)
        {
            var f3 = new Form3
            {
                Width = 730,
                Height = 195,
                Margin = new Padding(0, 0, 0, 10),
                ArchivoPath = archivoPath,
                CarpetaDestino = carpetaDestino,
                NombreBase = nombreBase,
                PngPathGuardado = pngPath,
                Visible = _modoHistorial
            };

            f3.LblTitulo.Text = titulo ?? nombreBase;
            f3.LblRuta.Text = archivoPath;
            f3.LblFecha.Text = fecha ?? "Completado: --";
            f3.TxtRuta.Text = archivoPath;
            if (!string.IsNullOrEmpty(tamano)) f3.LblTamano.Text = tamano;

            f3.LblCalidad.Text = string.IsNullOrWhiteSpace(plataforma)
                ? "✅ Completado"
                : $"✅ {plataforma} · Completado";

            string nombreBasePng = !string.IsNullOrWhiteSpace(nombreBase)
                ? nombreBase
                : Path.GetFileNameWithoutExtension(archivoPath);

            Image imgCargada = CargarPngConFallback(pngPath, nombreBasePng);

            if (imgCargada == null && !string.IsNullOrWhiteSpace(nombreBasePng)
                && !string.IsNullOrWhiteSpace(carpetaDestino) && Directory.Exists(carpetaDestino))
            {
                try
                {
                    var encontrados = Directory.GetFiles(carpetaDestino, nombreBasePng + "*.png", SearchOption.AllDirectories);
                    if (encontrados.Length > 0)
                    {
                        byte[] bytes = File.ReadAllBytes(encontrados[0]);
                        using (var ms = new MemoryStream(bytes))
                            imgCargada = Image.FromStream(ms);
                    }
                }
                catch { }
            }

            f3.BotonAbrirCarpetaClick += (s, e) =>
            {
                try
                {
                    string dir = Path.GetDirectoryName(f3.ArchivoPath);
                    if (!Directory.Exists(dir)) dir = f3.CarpetaDestino;
                    if (Directory.Exists(dir))
                        Process.Start(new ProcessStartInfo
                        { FileName = "explorer.exe", Arguments = $"\"{dir}\"", UseShellExecute = true });
                }
                catch { }
            };

            f3.BotonAbrirArchivoClick += (s, e) =>
            {
                try
                {
                    if (File.Exists(f3.ArchivoPath))
                        Process.Start(new ProcessStartInfo { FileName = f3.ArchivoPath, UseShellExecute = true });
                    else lblEstadoGlobal.Text = "  ❌ El archivo ya no existe.";
                }
                catch (Exception ex) { lblEstadoGlobal.Text = "  ❌ " + ex.Message; }
            };

            f3.BotonEliminarClick += (s, e) =>
            {
                _historial.Remove(f3);
                flowLista.Controls.Remove(f3);
                f3.Dispose();
                GuardarHistorialEnDisco();
                AjustarAnchoTarjetas();
            };

            f3.UrlDropped += (s, ev) => ProcesarUrlSoltada(ev.Url);

            _historial.Add(f3);
            flowLista.SuspendLayout();
            flowLista.Controls.Add(f3);
            flowLista.Controls.SetChildIndex(f3, 0);
            flowLista.ResumeLayout();
            AjustarAnchoTarjetas();

            if (imgCargada != null)
            {
                try
                {
                    f3.PicMiniatura.Image = imgCargada;
                    f3.PicMiniatura.SizeMode = PictureBoxSizeMode.Zoom;
                    f3.PicMiniatura.Refresh();
                }
                catch { }
            }

            if (persistir) GuardarHistorialEnDisco();
        }

        // ══════════════════════════════════════════════════════════════
        // DescargaItem
        // ══════════════════════════════════════════════════════════════
        private class DescargaItem : IDisposable
        {
            public string Url { get; }
            public Form2 Card { get; }
            public bool EnPausa { get; set; }
            public bool Iniciada { get; private set; }
            public bool HayDescargaCompletaPendienteDeCaratula { get; private set; }

            private Process _proceso;
            private CancellationTokenSource _cts;

            private double _totalMb = -1;
            private double _descargadoMb = 0;
            private double _ultimaVelocidad = 0;
            private int _segundosObservados = 0;
            private bool _canceladoPorUsuario = false;

            private string _videoId = null;
            private string _titulo = null;
            private string _webpPath = null;
            private string _pngPath = null;
            private string _mp4PathFinal = null;
            private string _carpetaPngActual = null;

            private int _intentosDescarga = 0;
            private const int MAX_REINTENTOS_DESCARGA = 10;

            public DescargaItem(string url, Form2 card)
            {
                Url = url;
                Card = card;
            }

            private static string ObtenerArgsCookies(string carpetaHerr)
            {
                string cookiesPath = Path.Combine(carpetaHerr, "cookies.txt");
                if (File.Exists(cookiesPath)) return $"--cookies \"{cookiesPath}\" ";
                return "";
            }

            private string NombreBaseArchivo()
            {
                if ((EsUrlFacebook(Url) || EsUrlInstagram(Url) || EsUrlTwitch(Url))
                    && !string.IsNullOrEmpty(_videoId) && !_videoId.StartsWith("fb_"))
                    return _videoId;
                return SanitizarNombre(_titulo ?? _videoId ?? "video");
            }

            public async Task CargarMetadatosAsync()
            {
                try
                {
                    string herr = PathManager.CarpetaHerramientas;
                    string ytDlp = Path.Combine(herr, "yt-dlp.exe");
                    if (!File.Exists(ytDlp)) { SetEstadoGlobal("❌ Falta yt-dlp.exe en " + herr); return; }

                    string destino = (Card.TxtDestino?.Text ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(destino))
                        destino = Path.Combine(PathManager.CarpetaDescargas, "Descargas");
                    Directory.CreateDirectory(destino);

                    _carpetaPngActual = Path.Combine(PathManager.CarpetaDescargas, "png");
                    Directory.CreateDirectory(_carpetaPngActual);

                    bool esFacebook = EsUrlFacebook(Url);
                    bool esInstagram = EsUrlInstagram(Url);
                    bool esThreads = EsUrlThreads(Url);
                    bool esTwitch = EsUrlTwitch(Url);
                    bool necesitaCookies = esFacebook || esInstagram || esThreads || esTwitch;

                    string preArgs = necesitaCookies ? ObtenerArgsCookies(herr) + "--impersonate Chrome " : "";

                    var psiId = CrearPsi(ytDlp, preArgs + $"--no-playlist --no-warnings --skip-download --print \"%(id)s\" \"{Url}\"");
                    string idRaw = await LeerSalidaAsync(psiId);
                    _videoId = (idRaw ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(_videoId) || _videoId.StartsWith("ERROR"))
                        _videoId = "vid_" + Math.Abs(Url.GetHashCode()).ToString("X8");

                    var psiTitulo = CrearPsi(ytDlp, preArgs + $"--no-playlist --no-warnings --skip-download --print \"%(title)s\" \"{Url}\"");
                    string tituloRaw = await LeerSalidaAsync(psiTitulo);
                    _titulo = (tituloRaw ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(_titulo) || _titulo.StartsWith("ERROR"))
                        _titulo = esFacebook ? "Facebook Video " + _videoId
                               : esInstagram ? "Instagram Video " + _videoId
                               : esThreads ? "Threads Video " + _videoId
                               : esTwitch ? "Twitch Video " + _videoId
                               : _videoId;

                    if (Card.LblTitulo != null && !Card.LblTitulo.IsDisposed)
                    {
                        void SetTitulo() { if (!Card.LblTitulo.IsDisposed) Card.LblTitulo.Text = _titulo; }
                        if (Card.LblTitulo.InvokeRequired) Card.LblTitulo.BeginInvoke((Action)SetTitulo);
                        else SetTitulo();
                    }

                    string[] candidatos = Directory.GetFiles(_carpetaPngActual, $"thumb_{_videoId}.*", SearchOption.TopDirectoryOnly);

                    if (candidatos.Length == 0)
                    {
                        var psiThumb = CrearPsi(ytDlp,
                            preArgs + $"--no-playlist --no-warnings --skip-download --write-thumbnail " +
                            (necesitaCookies ? "" : "--convert-thumbnails webp ") +
                            $"-o \"{Path.Combine(_carpetaPngActual, "thumb_%(id)s")}.%(ext)s\" \"{Url}\"");
                        await EjecutarSinSalidaAsync(psiThumb);
                        candidatos = Directory.GetFiles(_carpetaPngActual, $"thumb_{_videoId}.*", SearchOption.TopDirectoryOnly);
                    }

                    string nombreBase = NombreBaseArchivo();
                    bool miniaturaOk = false;
                    if (candidatos.Length > 0)
                    {
                        _webpPath = candidatos.OrderByDescending(f => File.GetLastWriteTimeUtc(f)).First();
                        try
                        {
                            _pngPath = ImageLoader.PrepararMiniatura(_webpPath, _carpetaPngActual, nombreBase);
                            miniaturaOk = !string.IsNullOrEmpty(_pngPath) && File.Exists(_pngPath);
                            if (miniaturaOk)
                            {
                                try
                                {
                                    Card.BeginInvoke(new Action(() =>
                                    { if (!Card.IsDisposed) Card.PngPathActual = _pngPath; }));
                                }
                                catch { }

                                try
                                {
                                    var form = Card?.FindForm() as Form1;
                                    if (form != null && !form.IsDisposed)
                                    {
                                        string pngLocal = _pngPath;
                                        form.BeginInvoke(new Action(() => form.ActualizarPngDeProceso(this, pngLocal)));
                                    }
                                }
                                catch { }
                            }
                        }
                        catch (Exception ex)
                        { Debug.WriteLine("[Metadatos] PrepararMiniatura EX: " + ex.ToString()); }
                    }

                    if (miniaturaOk)
                    {
                        PintarMiniatura();
                        SetEstadoGlobal("🖼 Miniatura lista: " + _pngPath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[Metadatos] EX: " + ex.ToString());
                    SetEstadoGlobal("❌ Metadatos: " + ex.Message);
                }
            }

            private void PintarMiniatura()
            {
                if (string.IsNullOrEmpty(_pngPath) || !File.Exists(_pngPath)) return;
                if (Card.PicMiniatura == null || Card.PicMiniatura.IsDisposed) return;
                Image img;
                try
                {
                    byte[] bytes = File.ReadAllBytes(_pngPath);
                    using (var ms = new MemoryStream(bytes))
                        img = Image.FromStream(ms);
                }
                catch { return; }

                void Pintar()
                {
                    if (Card.PicMiniatura.IsDisposed) { img.Dispose(); return; }
                    Card.PicMiniatura.Image?.Dispose();
                    Card.PicMiniatura.Image = img;
                    Card.PicMiniatura.SizeMode = PictureBoxSizeMode.Zoom;
                    Card.PicMiniatura.Refresh();
                }
                if (Card.PicMiniatura.InvokeRequired) Card.PicMiniatura.BeginInvoke((Action)Pintar);
                else Pintar();
            }

            private static async Task<bool> EsperarArchivoLibreAsync(string ruta, int intentos = 10, int msEntreIntentos = 300)
            {
                if (string.IsNullOrEmpty(ruta)) return false;
                for (int i = 0; i < intentos; i++)
                {
                    try
                    {
                        if (!File.Exists(ruta)) { await Task.Delay(msEntreIntentos); continue; }
                        using (var fs = new FileStream(ruta, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                            return true;
                    }
                    catch { await Task.Delay(msEntreIntentos); }
                }
                return false;
            }

            public async Task<bool> IniciarAsync(string destino, bool usarSubcarpeta)
            {
                Iniciada = true;
                _canceladoPorUsuario = false;
                _cts = new CancellationTokenSource();
                EnPausa = false;
                _mp4PathFinal = null;

                _descargadoMb = 0.01;
                _totalMb = -1;
                _ultimaVelocidad = 0;

                ActualizarUISincrono(() =>
                {
                    if (Card.LblMb != null && !Card.LblMb.IsDisposed) { Card.LblMb.Text = "0.01 MB / -- MB"; Card.LblMb.Refresh(); }
                    if (Card.LblPorcentaje != null && !Card.LblPorcentaje.IsDisposed) { Card.LblPorcentaje.Text = "0.0%"; Card.LblPorcentaje.Refresh(); }
                    if (Card.LblVel != null && !Card.LblVel.IsDisposed) { Card.LblVel.Text = "-- /s"; Card.LblVel.Refresh(); }
                    if (Card.LblEta != null && !Card.LblEta.IsDisposed) { Card.LblEta.Text = "Restante -- min"; Card.LblEta.Refresh(); }
                    if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed)
                    {
                        Card.ProgressBar.Value = 0;
                        Card.ProgressBar.State = ProgressState.Normal;
                        Card.ProgressBar.Text = "";
                        Card.ProgressBar.Refresh();
                    }
                });

                string herr = PathManager.CarpetaHerramientas;
                string ytDlp = Path.Combine(herr, "yt-dlp.exe");
                string ffmpeg = Path.Combine(herr, "ffmpeg.exe");
                string deno = Path.Combine(herr, "deno.exe");

                if (!File.Exists(ytDlp)) { SetEstadoGlobal("❌ Falta yt-dlp.exe"); return false; }

                if (string.IsNullOrEmpty(_videoId) || string.IsNullOrEmpty(_titulo)
                    || string.IsNullOrEmpty(_pngPath) || !File.Exists(_pngPath))
                {
                    SetEstadoGlobal("⏳ Generando miniatura antes de descargar…");
                    await CargarMetadatosAsync();
                }

                if (string.IsNullOrEmpty(_pngPath) || !File.Exists(_pngPath))
                {
                    SetEstadoGlobal("❌ No hay miniatura. No se descarga para garantizar la incrustación.");
                    Iniciada = false;
                    return false;
                }

                bool esFacebook = EsUrlFacebook(Url);
                bool esInstagram = EsUrlInstagram(Url);
                bool esThreads = EsUrlThreads(Url);
                bool esTwitch = EsUrlTwitch(Url);
                bool necesitaCookies = esFacebook || esInstagram || esThreads || esTwitch;

                string nombreBase = NombreBaseArchivo();
                string plantilla = usarSubcarpeta
                    ? Path.Combine(destino, nombreBase, nombreBase + ".%(ext)s")
                    : Path.Combine(destino, nombreBase + ".%(ext)s");

               // Registrar el parcial ANTES de lanzar yt-dlp (síncrono para garantizar que se guarda)
try
{
    var form = Card?.FindForm() as Form1;
    if (form != null && !form.IsDisposed)
    {
        if (form.InvokeRequired)
            form.Invoke(new Action(() => form.RegistrarParcial(this.Url, nombreBase)));
        else
            form.RegistrarParcial(this.Url, nombreBase);
    }
}
catch { }

                var args = new StringBuilder();
                args.Append("--newline --no-playlist --restrict-filenames --merge-output-format mp4 -i ");

                if (RequiereH264Forzado(Url))
                {
                    args.Append("-S \"vcodec:h264,acodec:m4a\" ");
                    if (EsUrlYouTubeValida(Url))
                        args.Append("-f \"bv*[height<=1080][vcodec^=avc1]+ba[ext=m4a]/b[height<=1080]\" ");
                }
                else args.Append("-f \"bv*+ba/b\" ");

                args.Append("--progress ");
                args.Append("--progress-template \"%(progress._percent_str)s|%(progress._total_bytes_str)s|%(progress._speed_str)s|%(progress._eta_str)s\" ");

                if (necesitaCookies)
                {
                    args.Append(ObtenerArgsCookies(herr));
                    args.Append("--impersonate Chrome ");
                }

                if (EsUrlYouTubeValida(Url))
                {
                    if (File.Exists(deno))
                    {
                        args.Append($"--js-runtimes \"deno:{deno}\" ");
                        args.Append("--remote-components ejs:github ");
                        args.Append("--no-check-certificates --no-warnings ");
                    }
                    string cookiesFile = Path.Combine(herr, "cookies.txt");
                    if (File.Exists(cookiesFile)) args.Append($"--cookies \"{cookiesFile}\" ");
                }

                if (File.Exists(ffmpeg))
                    args.Append("--ffmpeg-location \"").Append(ffmpeg).Append("\" ");

                args.Append("-o \"").Append(plantilla).Append("\" ");
                args.Append("--print after_move:filepath ");
                args.Append("\"").Append(Url).Append("\"");

                var psi = new ProcessStartInfo
                {
                    FileName = ytDlp,
                    Arguments = args.ToString(),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                _intentosDescarga = 0;
                bool ok = false;

                while (!ok && !EnPausa && !_canceladoPorUsuario && _intentosDescarga < MAX_REINTENTOS_DESCARGA)
                {
                    _intentosDescarga++;
                    if (_intentosDescarga > 1)
                    {
                        int esperaMs = Math.Min(60000, 2000 * _intentosDescarga);
                        SetEstadoGlobal($"🔁 Reintento {_intentosDescarga}/{MAX_REINTENTOS_DESCARGA} en {esperaMs / 1000}s…");
                        ActualizarUISincrono(() =>
                        {
                            if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed)
                            {
                                Card.ProgressBar.State = ProgressState.Normal;
                                Card.ProgressBar.Text = $"REINTENTO {_intentosDescarga}/{MAX_REINTENTOS_DESCARGA}";
                                Card.ProgressBar.Refresh();
                            }
                        });
                        for (int i = 0; i < esperaMs / 100 && !EnPausa && !_canceladoPorUsuario; i++)
                            await Task.Delay(100);
                        if (EnPausa || _canceladoPorUsuario) break;
                    }

                    var completado = new TaskCompletionSource<bool>();
                    try
                    {
                        _proceso = new Process { StartInfo = psi, EnableRaisingEvents = true };
                        _proceso.OutputDataReceived += (s, e) => { if (e.Data != null) ProcesarLinea(e.Data); };
                        _proceso.ErrorDataReceived += (s, e) => { if (e.Data != null) ProcesarLinea(e.Data); };
                        _proceso.Exited += (s, e) =>
                        {
                            try { completado.TrySetResult(_proceso != null && _proceso.ExitCode == 0); }
                            catch { completado.TrySetResult(false); }
                        };

                        _proceso.Start();
                        _proceso.BeginOutputReadLine();
                        _proceso.BeginErrorReadLine();

                        var timeoutTask = Task.Delay(TimeSpan.FromMinutes(30));
                        var completedTask = await Task.WhenAny(completado.Task, timeoutTask);
                        if (completedTask == timeoutTask)
                        { try { _proceso?.Kill(); } catch { } ok = false; }
                        else ok = await completado.Task;
                    }
                    catch (Exception ex)
                    { Debug.WriteLine("[IniciarAsync intento " + _intentosDescarga + "] " + ex.Message); ok = false; }
                    finally
                    {
                        try { if (_proceso != null && !_proceso.HasExited) _proceso.Kill(); } catch { }
                        try { _proceso?.Dispose(); } catch { }
                        _proceso = null;
                    }

                    if (ok) break;
                    if (EnPausa || _canceladoPorUsuario) break;
                }

                if (ok)
                {
                    ActualizarUISincrono(() =>
                    {
                        if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed)
                        {
                            Card.ProgressBar.Value = 100;
                            Card.ProgressBar.State = ProgressState.Covering;
                            Card.ProgressBar.Text = "CONVERTIENDO 0%";
                            Card.ProgressBar.Refresh();
                        }
                        if (Card.LblMb != null && !Card.LblMb.IsDisposed) { Card.LblMb.Text = "Convirtiendo..."; Card.LblMb.Refresh(); }
                        if (Card.LblPorcentaje != null && !Card.LblPorcentaje.IsDisposed) { Card.LblPorcentaje.Text = "0.0%"; Card.LblPorcentaje.Refresh(); }
                        if (Card.LblVel != null && !Card.LblVel.IsDisposed) { Card.LblVel.Text = "⏳"; Card.LblVel.Refresh(); }
                        if (Card.LblEta != null && !Card.LblEta.IsDisposed) { Card.LblEta.Text = "Convirtiendo"; Card.LblEta.Refresh(); }
                    });

                    LocalizarMp4Final(destino, nombreBase, usarSubcarpeta);

                    if (!string.IsNullOrEmpty(_mp4PathFinal))
                    {
                        SetEstadoGlobal("⏳ Esperando cierre del archivo…");
                        await EsperarArchivoLibreAsync(_mp4PathFinal, 10, 300);
                    }

                    if (string.IsNullOrEmpty(_pngPath) || !File.Exists(_pngPath))
                        await CargarMetadatosAsync();

                    var ctsConv = new CancellationTokenSource();
                    _ = Task.Run(async () =>
                    {
                        for (int p = 30; p <= 95 && !ctsConv.IsCancellationRequested; p += 5)
                        {
                            int pLocal = p;
                            ActualizarUISincrono(() =>
                            {
                                if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed)
                                {
                                    Card.ProgressBar.Value = pLocal;
                                    Card.ProgressBar.Text = $"CONVERTIENDO {pLocal}%";
                                    Card.ProgressBar.Refresh();
                                }
                                if (Card.LblPorcentaje != null && !Card.LblPorcentaje.IsDisposed)
                                { Card.LblPorcentaje.Text = pLocal.ToString("0.0") + "%"; Card.LblPorcentaje.Refresh(); }
                            });
                            try { await Task.Delay(200); } catch { break; }
                        }
                    }, ctsConv.Token);

                    string pngParaHistorial = _pngPath;
                    bool hayMiniatura = !string.IsNullOrEmpty(_pngPath) && File.Exists(_pngPath);
                    bool incrustado = false;

                    for (int intento = 1; intento <= 3 && !incrustado; intento++)
                    {
                        incrustado = IntentarIncrustarCaratula(ffmpeg, destino, usarSubcarpeta, nombreBase);
                        if (!incrustado) await Task.Delay(800);
                    }
                    ctsConv.Cancel();

                    if (incrustado)
                    {
                        HayDescargaCompletaPendienteDeCaratula = false;
                        SetEstadoGlobal("✅ Descarga + carátula incrustada.");

                        ActualizarUISincrono(() =>
                        {
                            if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed)
                            {
                                Card.ProgressBar.Value = 100;
                                Card.ProgressBar.Text = "";
                                Card.ProgressBar.State = ProgressState.Normal;
                                Card.ProgressBar.Refresh();
                            }
                            if (Card.LblMb != null && !Card.LblMb.IsDisposed) { Card.LblMb.Text = "Completado"; Card.LblMb.Refresh(); }
                            if (Card.LblVel != null && !Card.LblVel.IsDisposed) { Card.LblVel.Text = "✅"; Card.LblVel.Refresh(); }
                            if (Card.LblEta != null && !Card.LblEta.IsDisposed) { Card.LblEta.Text = "Restante 0 min"; Card.LblEta.Refresh(); }
                            if (Card.LblPorcentaje != null && !Card.LblPorcentaje.IsDisposed) { Card.LblPorcentaje.Text = "100.0%"; Card.LblPorcentaje.Refresh(); }
                        });

                        try
                        {
                            var form = Card?.FindForm() as Form1;
                            if (form != null && !form.IsDisposed)
                            {
                                string mp4 = _mp4PathFinal;
                                string png = pngParaHistorial;
                                form.BeginInvoke(new Action(() =>
                                {
                                    form.OlvidarParcial(this.Url);
                                    form.MoverAHistorial(this, mp4, png);
                                }));
                            }
                        }
                        catch { }

                        try { if (!string.IsNullOrEmpty(_webpPath) && File.Exists(_webpPath)) File.Delete(_webpPath); } catch { }
                        if (!EnPausa && !_canceladoPorUsuario) Iniciada = false;
                        return true;
                    }
                    else
                    {
                        HayDescargaCompletaPendienteDeCaratula = true;
                        SetEstadoGlobal("⚠ Descarga OK, carátula NO incrustada. Pulsa Reintentar.");

                        ActualizarUISincrono(() =>
                        {
                            if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed)
                            {
                                Card.ProgressBar.Value = 100;
                                Card.ProgressBar.State = ProgressState.Covering;
                                Card.ProgressBar.Text = hayMiniatura ? "REINTENTAR CARÁTULA" : "SIN MINIATURA";
                                Card.ProgressBar.Refresh();
                            }
                            if (Card.LblMb != null && !Card.LblMb.IsDisposed) { Card.LblMb.Text = "Completado"; Card.LblMb.Refresh(); }
                            if (Card.LblPorcentaje != null && !Card.LblPorcentaje.IsDisposed) { Card.LblPorcentaje.Text = "100.0%"; Card.LblPorcentaje.Refresh(); }
                        });

                        if (!EnPausa && !_canceladoPorUsuario) Iniciada = false;
                        return false;
                    }
                }
                else if (!EnPausa && !_canceladoPorUsuario)
                {
                    SetEstadoGlobal($"❌ Descarga falló tras {_intentosDescarga} intento(s).");
                    ActualizarUISincrono(() =>
                    {
                        if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed)
                        {
                            Card.ProgressBar.State = ProgressState.Merging;
                            Card.ProgressBar.Text = "FALLÓ";
                            Card.ProgressBar.Refresh();
                        }
                    });
                }

                if (!EnPausa && !_canceladoPorUsuario) Iniciada = false;
                return false;
            }

            private void LocalizarMp4Final(string destino, string nombreBase, bool usarSubcarpeta)
            {
                if (!string.IsNullOrEmpty(_mp4PathFinal) && File.Exists(_mp4PathFinal)) return;

                string baseDir = usarSubcarpeta ? Path.Combine(destino, nombreBase) : destino;
                string[] exts = { ".mp4", ".mkv", ".webm", ".mov" };
                foreach (var ext in exts)
                {
                    string cand = Path.Combine(baseDir, nombreBase + ext);
                    if (File.Exists(cand)) { _mp4PathFinal = cand; return; }
                }

                if (Directory.Exists(baseDir))
                {
                    var picks = Directory.GetFiles(baseDir, "*.mp4", SearchOption.TopDirectoryOnly)
                        .Concat(Directory.GetFiles(baseDir, "*.mkv", SearchOption.TopDirectoryOnly))
                        .Concat(Directory.GetFiles(baseDir, "*.webm", SearchOption.TopDirectoryOnly))
                        .Concat(Directory.GetFiles(baseDir, "*.mov", SearchOption.TopDirectoryOnly))
                        .Where(f => !f.Contains(".cover.") && !f.Contains(".fast.") && !f.Contains(".tmp."))
                        .OrderByDescending(f => new FileInfo(f).LastWriteTimeUtc)
                        .ToList();
                    if (picks.Count > 0) { _mp4PathFinal = picks[0]; return; }
                }
            }

            private bool IntentarIncrustarCaratula(string ffmpeg, string destino, bool usarSubcarpeta, string nombreBase)
            {
                try
                {
                    LocalizarMp4Final(destino, nombreBase, usarSubcarpeta);

                    if (string.IsNullOrEmpty(_mp4PathFinal) || !File.Exists(_mp4PathFinal))
                    { SetEstadoGlobal("⚠ No se localizó el mp4."); return false; }
                    if (string.IsNullOrEmpty(_pngPath) || !File.Exists(_pngPath))
                    { SetEstadoGlobal("⚠ No hay miniatura para incrustar."); return false; }
                    if (!File.Exists(ffmpeg))
                    { SetEstadoGlobal("⚠ Falta ffmpeg.exe."); return false; }

                    string jpgTemp = Path.ChangeExtension(_pngPath, ".cover.jpg");
                    try
                    {
                        string convArgs = $"-y -hide_banner -loglevel error -i \"{_pngPath}\" " +
                                          $"-vf \"scale=trunc(iw/2)*2:trunc(ih/2)*2\" -q:v 2 \"{jpgTemp}\"";
                        bool convOk = EjecutarFfmpegSync(ffmpeg, convArgs);
                        if (!convOk || !File.Exists(jpgTemp) || new FileInfo(jpgTemp).Length == 0)
                        { SetEstadoGlobal("❌ No se pudo convertir PNG a JPG."); return false; }

                        return IncrustarJpgComoCaratula(_mp4PathFinal, jpgTemp, ffmpeg);
                    }
                    finally
                    { try { if (File.Exists(jpgTemp)) File.Delete(jpgTemp); } catch { } }
                }
                catch (Exception ex) { SetEstadoGlobal("❌ Incrustar: " + ex.Message); return false; }
            }

            private static bool IncrustarJpgComoCaratula(string mp4Path, string jpgPath, string ffmpegPath)
            {
                try
                {
                    if (!File.Exists(mp4Path) || !File.Exists(jpgPath)) return false;
                    string tempOut = mp4Path + ".cover.tmp.mp4";
                    try { if (File.Exists(tempOut)) File.Delete(tempOut); } catch { }

                    string args = $"-y -hide_banner -loglevel error " +
                                  $"-i \"{mp4Path}\" -i \"{jpgPath}\" " +
                                  $"-map 0 -map 1 -c copy -c:v:1 mjpeg " +
                                  $"-disposition:v:1 attached_pic " +
                                  $"-metadata:s:v:1 title=\"Cover\" " +
                                  $"\"{tempOut}\"";

                    bool ok = EjecutarFfmpegSync(ffmpegPath, args);
                    if (ok && File.Exists(tempOut) && new FileInfo(tempOut).Length > 0)
                    {
                        try { File.Delete(mp4Path); File.Move(tempOut, mp4Path); return true; }
                        catch { try { File.Copy(tempOut, mp4Path, true); File.Delete(tempOut); return true; } catch { } }
                    }
                    try { File.Delete(tempOut); } catch { }
                    return false;
                }
                catch { return false; }
            }

            private static bool EjecutarFfmpegSync(string ffmpegPath, string args)
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = args,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    using (var p = Process.Start(psi))
                    {
                        string stdout = p.StandardOutput.ReadToEnd();
                        string stderr = p.StandardError.ReadToEnd();
                        p.WaitForExit(600000);
                        if (p.ExitCode != 0) Debug.WriteLine("[FFMPEG ERROR] " + (stderr ?? "").Trim());
                        return p.ExitCode == 0;
                    }
                }
                catch (Exception ex) { Debug.WriteLine("[FFMPEG EXCEPTION] " + ex.Message); return false; }
            }

            public async Task PausarAsync()
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (_proceso != null && !_proceso.HasExited)
                        {
                            int pid = _proceso.Id;
                            try
                            {
                                var killPsi = new ProcessStartInfo
                                {
                                    FileName = "taskkill.exe",
                                    Arguments = $"/T /F /PID {pid}",
                                    UseShellExecute = false,
                                    CreateNoWindow = true,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true
                                };
                                using (var killer = Process.Start(killPsi))
                                {
                                    killer.StandardOutput.ReadToEnd();
                                    killer.StandardError.ReadToEnd();
                                    killer.WaitForExit(5000);
                                }
                            }
                            catch { try { _proceso.Kill(); } catch { } }
                            try { _proceso.WaitForExit(5000); } catch { }
                        }
                    }
                    catch { }
                });
                Iniciada = false;
                SetEstadoGlobal("⏸ Descarga pausada (se reanudará con .part).");
            }

            public async Task ReanudarAsync(string destino, bool usarSubcarpeta)
            {
                if (_canceladoPorUsuario) return;
                EnPausa = false;
                await Task.Delay(150);
                await Task.Delay(300);

                if (string.IsNullOrEmpty(_pngPath) || !File.Exists(_pngPath))
                    await CargarMetadatosAsync();

                await IniciarAsync(destino, usarSubcarpeta);
            }

            public async Task<bool> ReintentarIncrustacionAsync(string destino)
            {
                string ffmpeg = Path.Combine(PathManager.CarpetaHerramientas, "ffmpeg.exe");

                if (string.IsNullOrEmpty(_pngPath) || !File.Exists(_pngPath))
                    await CargarMetadatosAsync();

                bool usarSub = Card.ChkSub != null && Card.ChkSub.Checked;
                string nombreBase = NombreBaseArchivo();

                ActualizarUISincrono(() =>
                {
                    if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed)
                    {
                        Card.ProgressBar.State = ProgressState.Covering;
                        Card.ProgressBar.Refresh();
                    }
                });

                LocalizarMp4Final(destino, nombreBase, usarSub);
                if (!string.IsNullOrEmpty(_mp4PathFinal))
                    await EsperarArchivoLibreAsync(_mp4PathFinal, 10, 300);

                string pngParaHistorial = _pngPath;
                bool ok = false;
                for (int intento = 1; intento <= 3 && !ok; intento++)
                {
                    ok = IntentarIncrustarCaratula(ffmpeg, destino, usarSub, nombreBase);
                    if (!ok) await Task.Delay(800);
                }

                if (ok)
                {
                    HayDescargaCompletaPendienteDeCaratula = false;
                    SetEstadoGlobal("✅ Carátula incrustada.");

                    ActualizarUISincrono(() =>
                    {
                        if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed)
                        {
                            Card.ProgressBar.Value = 100;
                            Card.ProgressBar.Text = "";
                            Card.ProgressBar.State = ProgressState.Normal;
                            Card.ProgressBar.Refresh();
                        }
                    });

                    try
                    {
                        var form = Card?.FindForm() as Form1;
                        if (form != null && !form.IsDisposed)
                        {
                            string mp4 = _mp4PathFinal;
                            string png = pngParaHistorial;
                            form.BeginInvoke(new Action(() =>
                            {
                                form.OlvidarParcial(this.Url);
                                form.MoverAHistorial(this, mp4, png);
                            }));
                        }
                    }
                    catch { }

                    try { if (!string.IsNullOrEmpty(_webpPath) && File.Exists(_webpPath)) File.Delete(_webpPath); } catch { }
                }
                else
                {
                    ActualizarUISincrono(() =>
                    {
                        if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed)
                        {
                            Card.ProgressBar.State = ProgressState.Covering;
                            Card.ProgressBar.Text = "REINTENTAR CARÁTULA";
                            Card.ProgressBar.Refresh();
                        }
                    });
                }
                return ok;
            }

            private static string FormatearRestante(string etaTemplate)
            {
                if (string.IsNullOrWhiteSpace(etaTemplate) || etaTemplate == "N/A" || etaTemplate == "--:--")
                    return "Restante -- min";
                var partes = etaTemplate.Trim().Split(':');
                int segundos;
                try
                {
                    if (partes.Length == 2) segundos = int.Parse(partes[0]) * 60 + int.Parse(partes[1]);
                    else if (partes.Length == 3) segundos = int.Parse(partes[0]) * 3600 + int.Parse(partes[1]) * 60 + int.Parse(partes[2]);
                    else return "Restante -- min";
                }
                catch { return "Restante -- min"; }
                if (segundos < 30) return "Restante < 1 min";
                int minutos = (int)Math.Ceiling(segundos / 60.0);
                return $"Restante ~{minutos} min";
            }

            private void ProcesarLinea(string linea)
            {
                if (string.IsNullOrEmpty(linea)) return;
                if (Card == null || Card.IsDisposed) return;

                try
                {
                    string ltrim = linea.Trim();
                    if (ltrim.Length > 0 && (ltrim.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)
                        || ltrim.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase)
                        || ltrim.EndsWith(".webm", StringComparison.OrdinalIgnoreCase)
                        || ltrim.EndsWith(".mov", StringComparison.OrdinalIgnoreCase)))
                    {
                        string candidato = ltrim.Trim('"', '\'', ' ');
                        if (candidato.Length > 2 && (candidato[1] == ':' || candidato.StartsWith("\\\\")))
                        { try { if (File.Exists(candidato)) _mp4PathFinal = candidato; } catch { } }
                    }

                    var mMerge = Regex.Match(linea, @"Merging formats into ""([^""]+)""");
                    if (mMerge.Success)
                    {
                        string cand = mMerge.Groups[1].Value.Trim();
                        if (cand.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)
                            || cand.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase)
                            || cand.EndsWith(".webm", StringComparison.OrdinalIgnoreCase))
                            _mp4PathFinal = cand;

                        ActualizarUISincrono(() =>
                        {
                            if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed)
                            {
                                Card.ProgressBar.State = ProgressState.Merging;
                                Card.ProgressBar.Refresh();
                            }
                        });
                    }

                    int sep = linea.IndexOf('|');
                    if (sep > 0)
                    {
                        string[] partes = linea.Split('|');
                        if (partes.Length >= 4)
                        {
                            string pctStr = partes[0].Trim();
                            string totalStr = partes[1].Trim();
                            string speedStr = partes[2].Trim();
                            string etaTemplate = partes[3].Trim();

                            string pctLimpio = pctStr.Replace("%", "").Trim();
                            double pct = -1;
                            double.TryParse(pctLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out pct);

                            double totalMb = -1;
                            var mT = Regex.Match(totalStr, @"([\d\.]+)\s*([KMGT]i?B)", RegexOptions.IgnoreCase);
                            if (mT.Success)
                            {
                                double v = double.Parse(mT.Groups[1].Value, CultureInfo.InvariantCulture);
                                totalMb = ConvertirAMb(v, mT.Groups[2].Value);
                            }
                            if (totalMb > 0) _totalMb = totalMb;

                            double velMb = 0;
                            var mV = Regex.Match(speedStr, @"([\d\.]+)\s*([KMGT]i?B)/s", RegexOptions.IgnoreCase);
                            if (mV.Success)
                            {
                                double v = double.Parse(mV.Groups[1].Value, CultureInfo.InvariantCulture);
                                velMb = ConvertirAMb(v, mV.Groups[2].Value);
                                _ultimaVelocidad = velMb;
                            }

                            if (pct >= 0 && _totalMb > 0)
                                _descargadoMb = _totalMb * (pct / 100.0);

                            string mbLabel;
                            if (_totalMb > 0) mbLabel = $"{_descargadoMb:0.00} MB / {_totalMb:0.00} MB";
                            else mbLabel = $"{_descargadoMb:0.00} MB / -- MB";

                            string velLabel = _ultimaVelocidad > 0 ? $"{_ultimaVelocidad:0.00} MB/s"
                                : (speedStr == "N/A" ? "-- /s" : speedStr);

                            string restante = FormatearRestante(etaTemplate);

                            ActualizarUISincrono(() =>
                            {
                                if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed && pct >= 0)
                                {
                                    if (Card.ProgressBar.Maximum != 100) Card.ProgressBar.Maximum = 100;
                                    if (Card.ProgressBar.Minimum != 0) Card.ProgressBar.Minimum = 0;
                                    Card.ProgressBar.Value = Math.Max(0, Math.Min(100, (int)Math.Round(pct)));
                                    Card.ProgressBar.Refresh();
                                }
                                if (Card.LblMb != null && !Card.LblMb.IsDisposed) { Card.LblMb.Text = mbLabel; Card.LblMb.Refresh(); }
                                if (Card.LblPorcentaje != null && !Card.LblPorcentaje.IsDisposed && pct >= 0)
                                { Card.LblPorcentaje.Text = pct.ToString("0.0", CultureInfo.InvariantCulture) + "%"; Card.LblPorcentaje.Refresh(); }
                                if (Card.LblVel != null && !Card.LblVel.IsDisposed) { Card.LblVel.Text = velLabel; Card.LblVel.Refresh(); }
                                if (Card.LblEta != null && !Card.LblEta.IsDisposed) { Card.LblEta.Text = restante; Card.LblEta.Refresh(); }
                            });

                            try { if (pct >= 0) MySqlSync.PublicarDescarga(Url, _titulo, DetectarPlataforma(Url), "descargando", pct); } catch { }

                            _segundosObservados++;
                            if (_segundosObservados >= 2)
                            {
                                _segundosObservados = 0;
                                if (pct >= 0) SetEstadoGlobal($"⬇ {mbLabel} — {velLabel} — {pct:0.0}% — {restante}");
                                else SetEstadoGlobal($"⬇ {mbLabel} — {velLabel} — {restante}");
                            }
                            return;
                        }
                    }

                    if (!linea.StartsWith("[download]")) return;

                    string limpia = linea.TrimEnd('\r', '\n', ' ');

                    double pctActual = -1;
                    var mPct = Regex.Match(limpia, @"(\d{1,3})(?:[\.,](\d+))?\s*%");
                    if (mPct.Success)
                    {
                        string entero = mPct.Groups[1].Value;
                        string dec = mPct.Groups[2].Success ? mPct.Groups[2].Value : "";
                        string combinado = dec.Length > 0 ? entero + "." + dec : entero;
                        double.TryParse(combinado, NumberStyles.Any, CultureInfo.InvariantCulture, out pctActual);
                    }

                    var mDesc = Regex.Match(limpia, @"\]\s+~?\s*([\d\.]+)\s*([KMGT]i?B)\s+at", RegexOptions.IgnoreCase);
                    if (!mDesc.Success)
                        mDesc = Regex.Match(limpia, @"^\[download\]\s+([\d\.]+)\s*([KMGT]i?B)", RegexOptions.IgnoreCase);
                    if (mDesc.Success)
                    {
                        double v = double.Parse(mDesc.Groups[1].Value, CultureInfo.InvariantCulture);
                        _descargadoMb = ConvertirAMb(v, mDesc.Groups[2].Value);
                    }

                    var mTotal = Regex.Match(limpia, @"of\s+~?\s*([\d\.]+)\s*([KMGT]i?B)", RegexOptions.IgnoreCase);
                    if (mTotal.Success)
                    {
                        double v = double.Parse(mTotal.Groups[1].Value, CultureInfo.InvariantCulture);
                        _totalMb = ConvertirAMb(v, mTotal.Groups[2].Value);
                    }

                    var mVel = Regex.Match(limpia, @"at\s+~?\s*([\d\.]+)\s*([KMGT]i?B)/s", RegexOptions.IgnoreCase);
                    if (mVel.Success)
                    {
                        double v = double.Parse(mVel.Groups[1].Value, CultureInfo.InvariantCulture);
                        _ultimaVelocidad = ConvertirAMb(v, mVel.Groups[2].Value);
                    }

                    var mEta = Regex.Match(limpia, @"ETA\s+(\d{1,2}:\d{2}(?::\d{2})?)");
                    string etaClasico = mEta.Success ? mEta.Groups[1].Value : "--:--";
                    string restanteClasico = FormatearRestante(etaClasico);

                    string mbLabelF;
                    if (_totalMb > 0) mbLabelF = $"{_descargadoMb:0.00} MB / {_totalMb:0.00} MB";
                    else if (_descargadoMb > 0) mbLabelF = $"{_descargadoMb:0.00} MB / -- MB";
                    else mbLabelF = "0.01 MB / -- MB";

                    string velLabelF = _ultimaVelocidad > 0 ? $"{_ultimaVelocidad:0.00} MB/s" : "-- /s";

                    ActualizarUISincrono(() =>
                    {
                        if (Card.LblMb != null && !Card.LblMb.IsDisposed) { Card.LblMb.Text = mbLabelF; Card.LblMb.Refresh(); }
                        if (Card.LblPorcentaje != null && !Card.LblPorcentaje.IsDisposed && pctActual >= 0)
                        { Card.LblPorcentaje.Text = pctActual.ToString("0.0", CultureInfo.InvariantCulture) + "%"; Card.LblPorcentaje.Refresh(); }
                        if (Card.LblVel != null && !Card.LblVel.IsDisposed) { Card.LblVel.Text = velLabelF; Card.LblVel.Refresh(); }
                        if (Card.LblEta != null && !Card.LblEta.IsDisposed) { Card.LblEta.Text = restanteClasico; Card.LblEta.Refresh(); }
                        if (Card.ProgressBar != null && !Card.ProgressBar.IsDisposed && pctActual >= 0)
                        {
                            int v = Math.Max(0, Math.Min(100, (int)Math.Round(pctActual)));
                            if (Card.ProgressBar.Maximum != 100) Card.ProgressBar.Maximum = 100;
                            if (Card.ProgressBar.Minimum != 0) Card.ProgressBar.Minimum = 0;
                            Card.ProgressBar.Value = v;
                            Card.ProgressBar.Refresh();
                        }
                    });

                    try { if (pctActual >= 0) MySqlSync.PublicarDescarga(Url, _titulo, DetectarPlataforma(Url), "descargando", pctActual); } catch { }

                    _segundosObservados++;
                    if (_segundosObservados >= 2)
                    {
                        _segundosObservados = 0;
                        if (pctActual >= 0) SetEstadoGlobal($"⬇ {mbLabelF} — {velLabelF} — {pctActual:0.0}% — {restanteClasico}");
                        else SetEstadoGlobal($"⬇ {mbLabelF} — {velLabelF} — {restanteClasico}");
                    }
                }
                catch { }
            }

            private static double ConvertirAMb(double valor, string unidad)
            {
                unidad = unidad.ToUpperInvariant();
                if (unidad.StartsWith("K")) return valor / 1024.0;
                if (unidad.StartsWith("M")) return valor;
                if (unidad.StartsWith("G")) return valor * 1024.0;
                if (unidad.StartsWith("T")) return valor * 1024.0 * 1024.0;
                return valor;
            }

            private static ProcessStartInfo CrearPsi(string exe, string args)
            {
                return new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };
            }

            private static async Task<string> LeerSalidaAsync(ProcessStartInfo psi)
            {
                try
                {
                    using (var p = Process.Start(psi))
                    {
                        string stdout = await p.StandardOutput.ReadToEndAsync();
                        string stderr = await p.StandardError.ReadToEndAsync();
                        p.WaitForExit(30000);
                        return string.IsNullOrWhiteSpace(stdout) ? stderr : stdout;
                    }
                }
                catch (Exception ex) { return "ERROR: " + ex.Message; }
            }

            private static async Task EjecutarSinSalidaAsync(ProcessStartInfo psi)
            {
                try
                {
                    using (var p = Process.Start(psi))
                    {
                        _ = p.StandardOutput.ReadToEndAsync();
                        _ = p.StandardError.ReadToEndAsync();
                        await Task.Run(() => p.WaitForExit(60000));
                    }
                }
                catch { }
            }

            public void Cancelar()
            {
                _canceladoPorUsuario = true;
                try
                {
                    _cts?.Cancel();
                    var form = Card?.FindForm() as Form1;
                    if (form != null && !form.IsDisposed)
                        form.BeginInvoke(new Action(() => form.OlvidarParcial(this.Url)));
                }
                catch { }

                try
                {
                    if (_proceso != null && !_proceso.HasExited)
                    {
                        int pid = _proceso.Id;
                        try
                        {
                            var killPsi = new ProcessStartInfo
                            {
                                FileName = "taskkill.exe",
                                Arguments = $"/T /F /PID {pid}",
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true
                            };
                            using (var killer = Process.Start(killPsi))
                            {
                                killer.StandardOutput.ReadToEnd();
                                killer.StandardError.ReadToEnd();
                                killer.WaitForExit(3000);
                            }
                        }
                        catch { try { _proceso.Kill(); } catch { } }
                    }
                }
                catch { }
            }

            public void Dispose()
            {
                Cancelar();
                try { _proceso?.Dispose(); } catch { }
                _proceso = null;
            }

            private void ActualizarUISincrono(Action accion)
            {
                if (Card == null || Card.IsDisposed) return;
                try
                {
                    if (Card.InvokeRequired) Card.BeginInvoke(accion);
                    else accion();
                }
                catch { }
            }

            private void SetEstadoGlobal(string texto)
            {
                try
                {
                    var form = Card?.FindForm() as Form1;
                    if (form != null && !form.IsDisposed)
                        form.BeginInvoke(new Action(() =>
                        {
                            if (!form.lblEstadoGlobal.IsDisposed)
                                form.lblEstadoGlobal.Text = "  " + texto;
                        }));
                }
                catch { }
            }
        }

        private void flowLista_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        private void lblEstadoGlobal_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 40, 40));
            ConectarDragGlobal(this);
        }

        private void ConectarDragGlobal(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox || c is Button || c is CheckBox || c is ComboBox ||
                    c is NumericUpDown || c is RadioButton || c is ListBox)
                    continue;

                if (c is PictureBox pb && pb.ContextMenuStrip != null) continue;
                if (c is Label lbl && lbl.ContextMenuStrip != null) continue;

                c.MouseDown -= DragMouseDown;
                c.MouseDown += DragMouseDown;

                if (c.HasChildren) ConectarDragGlobal(c);
            }
        }

        private void DragMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Hide();
            panel1.Show();
            button2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Show();
            panel1.Hide();
            button2.Show();
        }
    }

    // ========================================================================
    // WebP / ImageMagic / ImageLoader
    // ========================================================================
    public sealed class WebP : IDisposable
    {
        private const string DLL_NAME = "libwebp_x64.dll";

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr WebPDecodeBGRA_Delegate(IntPtr data, UIntPtr data_size, out int width, out int height);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void WebPFree_Delegate(IntPtr p);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        private static IntPtr _hModule = IntPtr.Zero;
        private static WebPDecodeBGRA_Delegate _decodeBGRA;
        private static WebPFree_Delegate _free;
        private static string _dllPathUsada;
        private static string _ultimoError;

        private static bool AsegurarCargada()
        {
            if (_hModule != IntPtr.Zero && _decodeBGRA != null) return true;

            try
            {
                string exeDir = Path.GetDirectoryName(Application.ExecutablePath);
                string[] candidatos = new string[]
                {
                    Path.Combine(exeDir, DLL_NAME),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DLL_NAME),
                    DLL_NAME
                };

                foreach (var cand in candidatos)
                {
                    IntPtr h = IntPtr.Zero;
                    try { h = LoadLibrary(cand); } catch { }

                    if (h == IntPtr.Zero)
                    { _ultimoError = $"LoadLibrary({cand}) err={Marshal.GetLastWin32Error()}"; continue; }

                    IntPtr pDecode = GetProcAddress(h, "WebPDecodeBGRA");
                    IntPtr pFree = GetProcAddress(h, "WebPFree");

                    if (pDecode == IntPtr.Zero || pFree == IntPtr.Zero)
                    { _ultimoError = $"GetProcAddress({cand}) BGRA={pDecode} Free={pFree}"; FreeLibrary(h); continue; }

                    _hModule = h;
                    _dllPathUsada = cand;
                    _decodeBGRA = (WebPDecodeBGRA_Delegate)Marshal.GetDelegateForFunctionPointer(pDecode, typeof(WebPDecodeBGRA_Delegate));
                    _free = (WebPFree_Delegate)Marshal.GetDelegateForFunctionPointer(pFree, typeof(WebPFree_Delegate));
                    return true;
                }
                return false;
            }
            catch (Exception ex) { _ultimoError = "EX: " + ex.Message; return false; }
        }

        public static string Diagnose()
        {
            var sb = new StringBuilder();
            string exeDir = Path.GetDirectoryName(Application.ExecutablePath);
            string dll = Path.Combine(exeDir, DLL_NAME);
            sb.Append("DLL: ").Append(dll);
            sb.Append(" · existe=").Append(File.Exists(dll));
            if (File.Exists(dll))
            { try { sb.Append(" · ").Append(new FileInfo(dll).Length).Append(" bytes"); } catch { } }

            bool ok = AsegurarCargada();
            sb.Append(" · cargada=").Append(ok);
            if (ok) sb.Append(" · path=").Append(_dllPathUsada);
            else sb.Append(" · err=").Append(_ultimoError);
            return sb.ToString();
        }

        public Bitmap Decode(byte[] webpData)
        {
            if (webpData == null || webpData.Length == 0)
                throw new ArgumentException("webpData vacío.", nameof(webpData));

            if (!AsegurarCargada())
                throw new Exception("No se pudo cargar libwebp_x64.dll: " + _ultimoError);

            IntPtr pData = Marshal.AllocHGlobal(webpData.Length);
            IntPtr pPixels = IntPtr.Zero;
            try
            {
                Marshal.Copy(webpData, 0, pData, webpData.Length);
                int w, h;
                pPixels = _decodeBGRA(pData, (UIntPtr)(uint)webpData.Length, out w, out h);
                if (pPixels == IntPtr.Zero || w <= 0 || h <= 0)
                    throw new Exception("WebPDecodeBGRA devolvió datos inválidos.");

                Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                var bd = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                try
                {
                    int stride = bd.Stride;
                    for (int y = 0; y < h; y++)
                    {
                        IntPtr src = IntPtr.Add(pPixels, y * w * 4);
                        IntPtr dst = IntPtr.Add(bd.Scan0, y * stride);
                        for (int x = 0; x < w; x++)
                        {
                            int o = x * 4;
                            Marshal.WriteByte(dst, o + 0, Marshal.ReadByte(src, o + 0));
                            Marshal.WriteByte(dst, o + 1, Marshal.ReadByte(src, o + 1));
                            Marshal.WriteByte(dst, o + 2, Marshal.ReadByte(src, o + 2));
                            Marshal.WriteByte(dst, o + 3, Marshal.ReadByte(src, o + 3));
                        }
                    }
                }
                finally { bmp.UnlockBits(bd); }
                return bmp;
            }
            finally
            {
                if (pPixels != IntPtr.Zero && _free != null) { try { _free(pPixels); } catch { } }
                if (pData != IntPtr.Zero) Marshal.FreeHGlobal(pData);
            }
        }

        public void Dispose() => GC.SuppressFinalize(this);
    }

    public static class ImageMagic
    {
        public static string ExtensionFromMagicBytes(byte[] b)
        {
            if (b == null || b.Length < 12) return null;
            if (b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47 &&
                b[4] == 0x0D && b[5] == 0x0A && b[6] == 0x1A && b[7] == 0x0A) return ".png";
            if (b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF) return ".jpg";
            if (b[0] == 0x47 && b[1] == 0x49 && b[2] == 0x46 && b[3] == 0x38) return ".gif";
            if (b[0] == 0x42 && b[1] == 0x4D) return ".bmp";
            if (b.Length >= 12 && b[0] == 0x52 && b[1] == 0x49 && b[2] == 0x46 && b[3] == 0x46 &&
                b[8] == 0x57 && b[9] == 0x45 && b[10] == 0x42 && b[11] == 0x50) return ".webp";
            if ((b[0] == 0x49 && b[1] == 0x49 && b[2] == 0x2A && b[3] == 0x00) ||
                (b[0] == 0x4D && b[1] == 0x4D && b[2] == 0x00 && b[3] == 0x2A)) return ".tiff";
            if (b[0] == 0x00 && b[1] == 0x00 && b[2] == 0x01 && b[3] == 0x00) return ".ico";
            return null;
        }
    }

    public static class ImageLoader
    {
        public static string PrepararMiniatura(string rutaOrigen, string carpetaPng, string nombreBaseSinExt)
        {
            if (string.IsNullOrEmpty(rutaOrigen) || !File.Exists(rutaOrigen))
                throw new FileNotFoundException("No existe la miniatura.", rutaOrigen);
            if (!Directory.Exists(carpetaPng)) Directory.CreateDirectory(carpetaPng);

            byte[] bytes = File.ReadAllBytes(rutaOrigen);
            if (bytes.Length == 0) throw new Exception("El archivo está vacío: " + rutaOrigen);

            string ext = ImageMagic.ExtensionFromMagicBytes(bytes);
            if (ext != ".webp") return rutaOrigen;

            string destino = Path.Combine(carpetaPng, nombreBaseSinExt + ".png");
            if (File.Exists(destino))
            {
                try { if (File.GetLastWriteTimeUtc(destino) >= File.GetLastWriteTimeUtc(rutaOrigen)) return destino; }
                catch { }
            }

            Bitmap raw = null;
            bool decodeOk = false;
            try
            {
                using (var webp = new WebP()) raw = webp.Decode(bytes);
                decodeOk = raw != null && raw.Width > 0 && raw.Height > 0;
            }
            catch (Exception exWebp) { Debug.WriteLine("[ImageLoader] WebP.Decode EX: " + exWebp.ToString()); decodeOk = false; raw = null; }

            if (!decodeOk)
            {
                string pngFfmpeg = ConvertirConFfmpeg(rutaOrigen, carpetaPng, nombreBaseSinExt);
                if (!string.IsNullOrEmpty(pngFfmpeg) && File.Exists(pngFfmpeg)) return pngFfmpeg;
                throw new Exception("No se pudo convertir .webp a .png ni con libwebp_x64.dll ni con ffmpeg.");
            }

            try
            {
                using (Bitmap normalized = new Bitmap(raw.Width, raw.Height, PixelFormat.Format32bppArgb))
                {
                    using (var g = Graphics.FromImage(normalized))
                    {
                        g.Clear(Color.Transparent);
                        g.CompositingMode = CompositingMode.SourceCopy;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.DrawImage(raw, 0, 0, raw.Width, raw.Height);
                    }
                    normalized.Save(destino, ImageFormat.Png);
                }
                return destino;
            }
            finally { if (raw != null) { try { raw.Dispose(); } catch { } } }
        }

        public static string ConvertirConFfmpeg(string rutaWebp, string carpetaPng, string nombreBaseSinExt)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaWebp) || !File.Exists(rutaWebp)) return null;
                if (!Directory.Exists(carpetaPng)) Directory.CreateDirectory(carpetaPng);

                string exeDir = Path.GetDirectoryName(Application.ExecutablePath);
                string ffmpeg = Path.Combine(exeDir ?? "", "ffmpeg.exe");
                if (!File.Exists(ffmpeg)) return null;

                string destino = Path.Combine(carpetaPng, nombreBaseSinExt + ".png");
                try { if (File.Exists(destino)) File.Delete(destino); } catch { }

                var psi = new ProcessStartInfo
                {
                    FileName = ffmpeg,
                    Arguments = $"-y -hide_banner -loglevel error -i \"{rutaWebp}\" \"{destino}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var p = Process.Start(psi))
                {
                    string stderr = p.StandardError.ReadToEnd();
                    p.WaitForExit(30000);

                    if (p.ExitCode == 0 && File.Exists(destino) && new FileInfo(destino).Length > 0)
                        return destino;

                    Debug.WriteLine("[ImageLoader] ffmpeg falló (exit=" + p.ExitCode + "): " + stderr);
                    return null;
                }
            }
            catch (Exception ex) { Debug.WriteLine("[ImageLoader] ConvertirConFfmpeg EX: " + ex.ToString()); return null; }
        }
    }
}
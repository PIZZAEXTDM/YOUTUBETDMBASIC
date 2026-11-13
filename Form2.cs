using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace YOUTUBE
{
    public partial class Form2 : UserControl
    {
        public static readonly Color BgCard = Color.FromArgb(42, 45, 56);
        public static readonly Color BgCardHover = Color.FromArgb(50, 54, 68);
        public static readonly Color Accent = Color.FromArgb(220, 40, 40);
        public static readonly Color BgButton = Color.FromArgb(55, 58, 72);
        public static readonly Color BgVerde = Color.FromArgb(60, 110, 70);

        public event EventHandler BotonCarpetaClick;
        public event EventHandler BotonPausaClick;
        public event EventHandler BotonDescargarClick;
        public event EventHandler BotonEliminarClick;
        public event EventHandler CheckSubChanged;
        public event EventHandler AbrirUrlEnNavegador;
        public event EventHandler AbrirCarpetaDescargador;
        public event EventHandler<UrlDroppedEventArgs> UrlDropped;

        private bool _disenoDetectado;

        public Form2()
        {
            // Detectar diseño ANTES de InitializeComponent
            _disenoDetectado = DetectarDiseno();
            InitializeComponent();

            this.BackColor = BgCard;

            // ⚠ En diseño NO conectar nada que toque DataObject, Clipboard, etc.
            if (!_disenoDetectado)
            {
                WireEvents();
                ApplyHover();

                this.SetStyle(ControlStyles.UserPaint
                            | ControlStyles.AllPaintingInWmPaint
                            | ControlStyles.OptimizedDoubleBuffer
                            | ControlStyles.ResizeRedraw
                            | ControlStyles.SupportsTransparentBackColor, true);
            }
        }

        private static bool DetectarDiseno()
        {
            try
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return true;

                string proc = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (proc.Equals("devenv", StringComparison.OrdinalIgnoreCase)) return true;
                if (proc.Equals("XDesProc", StringComparison.OrdinalIgnoreCase)) return true;
                if (proc.Equals("msdev", StringComparison.OrdinalIgnoreCase)) return true;

                // Fallback: si el control no tiene un Form ancestro real, estamos en diseño
                var site = System.ComponentModel.LicenseManager.UsageMode;
                return false;
            }
            catch { return false; }
        }

        public PictureBox PicMiniatura => picMiniatura;
        public Label LblTitulo => lblTitulo;
        public Label LblCalidad => lblCalidad;
        public Label LblUrl => lblUrl;
        public ProgressBarEx ProgressBar => progress;
        public Label LblMb => lblMb;
        public Label LblPorcentaje => lblPorcentaje;
        public Label LblVel => lblVel;
        public Label LblEta => lblEta;
        public TextBox TxtDestino => txtDestino;
        public Button BtnCarpeta => btnCarpeta;
        public CheckBox ChkSub => chkSub;
        public Button BtnPausa => btnPausa;
        public Button BtnDescargar => btnDescargar;
        public Label BtnEliminar => label1;

        public string PngPathActual { get; set; }

        public void SetUrl(string url) => lblUrl.Text = url;
        public void SetCalidad(string txt) => lblCalidad.Text = txt;
        public void SetTitulo(string txt) => lblTitulo.Text = txt;
        public void SetDestino(string txt) => txtDestino.Text = txt;

        public void SetEstadoDescargando()
        { btnDescargar.Text = "⏸ Pausar"; btnDescargar.BackColor = BgVerde; btnDescargar.Enabled = true; }
        public void SetEstadoPausado()
        { btnDescargar.Text = "▶ Reanudar"; btnDescargar.BackColor = Accent; btnDescargar.Enabled = true; }
        public void SetEstadoListo()
        { btnDescargar.Text = "✅ Listo"; btnDescargar.BackColor = BgButton; btnDescargar.Enabled = false; btnPausa.Enabled = false; }
        public void SetEstadoReintentar()
        { btnDescargar.Text = "⚠ Reintentar"; btnDescargar.BackColor = Accent; btnDescargar.Enabled = true; }
        public void SetEstadoOcupado()
        { btnDescargar.Text = "⏳..."; btnDescargar.Enabled = false; }

        private void ApplyHover()
        {
            this.MouseEnter += (s, e) => { this.BackColor = BgCardHover; this.Invalidate(); };
            this.MouseLeave += (s, e) => { this.BackColor = BgCard; this.Invalidate(); };
        }

        private void WireEvents()
        {
            btnCarpeta.Click += (s, e) => BotonCarpetaClick?.Invoke(this, e);
            btnPausa.Click += (s, e) => BotonPausaClick?.Invoke(this, e);
            btnDescargar.Click += (s, e) => BotonDescargarClick?.Invoke(this, e);
            label1.Click += (s, e) => BotonEliminarClick?.Invoke(this, e);
            chkSub.CheckedChanged += (s, e) => CheckSubChanged?.Invoke(this, e);

            var menu = new ContextMenuStrip();
            menu.Items.Add("🌐 Abrir en navegador", null, (s, e) => AbrirUrlEnNavegador?.Invoke(this, e));
            menu.Items.Add("📂 Abrir carpeta del descargador", null, (s, e) => AbrirCarpetaDescargador?.Invoke(this, e));
            menu.Items.Add("📋 Copiar URL", null, (s, e) => { try { Clipboard.SetText(lblUrl.Text); } catch { } });
            this.ContextMenuStrip = menu;
            picMiniatura.ContextMenuStrip = menu;
            lblUrl.ContextMenuStrip = menu;
            lblTitulo.ContextMenuStrip = menu;

            this.AllowDrop = true;
            this.DragEnter += Form2_DragEnter;
            this.DragDrop += Form2_DragDrop;

            // ⚠ Importante: solo conectar a los controles propios, NO a los que añada el diseñador
            foreach (Control c in this.Controls)
            {
                if (c is Label || c is PictureBox || c is TextBox || c is ProgressBarEx)
                {
                    c.AllowDrop = true;
                    c.DragEnter += Form2_DragEnter;
                    c.DragDrop += Form2_DragDrop;
                }
            }
        }

        private void Form2_DragEnter(object sender, DragEventArgs e)
        {
            if (e?.Data == null) { e.Effect = DragDropEffects.None; return; }
            if (e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form2_DragDrop(object sender, DragEventArgs e)
        {
            if (e?.Data == null) return;
            string txt = null;
            if (e.Data.GetDataPresent(DataFormats.UnicodeText))
                txt = e.Data.GetData(DataFormats.UnicodeText) as string;
            else if (e.Data.GetDataPresent(DataFormats.Text))
                txt = e.Data.GetData(DataFormats.Text) as string;
            if (!string.IsNullOrWhiteSpace(txt))
                UrlDropped?.Invoke(this, new UrlDroppedEventArgs(txt.Trim()));
        }

        // FONDO REDONDEADO SIN BORDE (solo runtime)
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (_disenoDetectado)
            {
                base.OnPaintBackground(e);
                return;
            }

            try
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Color colorPadre = this.Parent?.BackColor ?? Color.FromArgb(30, 32, 40);
                using (var brush = new SolidBrush(colorPadre))
                    g.FillRectangle(brush, ClientRectangle);

                if (Width <= 2 || Height <= 2) return;

                var rect = new Rectangle(0, 0, Width, Height);
                using (var path = RoundRect(rect, 12))
                using (var fill = new SolidBrush(this.BackColor))
                    g.FillPath(fill, path);
            }
            catch
            {
                base.OnPaintBackground(e);
            }
        }

        private static GraphicsPath RoundRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            { path.AddRectangle(rect); return path; }

            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            if (d <= 0) { path.AddRectangle(rect); return path; }

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            label1.ForeColor = Color.Yellow;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            label1.ForeColor = System.Drawing.SystemColors.ControlDark;
        }
    }

    public class UrlDroppedEventArgs : EventArgs
    {
        public string Url { get; }
        public UrlDroppedEventArgs(string url) { Url = url; }
    }
}
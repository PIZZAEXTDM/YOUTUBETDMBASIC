using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace YOUTUBE
{
    public partial class Form3 : UserControl
    {
        public static readonly Color BgCard = Color.FromArgb(42, 45, 56);
        public static readonly Color BgCardHover = Color.FromArgb(50, 54, 68);

        public event EventHandler BotonAbrirCarpetaClick;
        public event EventHandler BotonAbrirArchivoClick;
        public event EventHandler BotonEliminarClick;
        public event EventHandler<UrlDroppedEventArgs> UrlDropped;

        public Form3()
        {
            InitializeComponent();
            this.BackColor = BgCard;
            this.MouseEnter += (s, e) => { this.BackColor = BgCardHover; this.Invalidate(); };
            this.MouseLeave += (s, e) => { this.BackColor = BgCard; this.Invalidate(); };

            btnCarpeta.Click += (s, e) => BotonAbrirCarpetaClick?.Invoke(this, e);
            btnAbrirArchivo.Click += (s, e) => BotonAbrirArchivoClick?.Invoke(this, e);
            btnEliminar.Click += (s, e) => BotonEliminarClick?.Invoke(this, e);

            this.AllowDrop = true;
            this.DragEnter += Form3_DragEnter;
            this.DragDrop += Form3_DragDrop;
            foreach (Control c in this.Controls)
            {
                c.AllowDrop = true;
                c.DragEnter += Form3_DragEnter;
                c.DragDrop += Form3_DragDrop;
            }

            this.SetStyle(ControlStyles.UserPaint
                        | ControlStyles.AllPaintingInWmPaint
                        | ControlStyles.OptimizedDoubleBuffer
                        | ControlStyles.ResizeRedraw
                        | ControlStyles.SupportsTransparentBackColor, true);
        }

        private void Form3_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form3_DragDrop(object sender, DragEventArgs e)
        {
            string txt = null;
            if (e.Data.GetDataPresent(DataFormats.UnicodeText))
                txt = e.Data.GetData(DataFormats.UnicodeText) as string;
            else if (e.Data.GetDataPresent(DataFormats.Text))
                txt = e.Data.GetData(DataFormats.Text) as string;
            if (!string.IsNullOrWhiteSpace(txt))
                UrlDropped?.Invoke(this, new UrlDroppedEventArgs(txt.Trim()));
        }

        public PictureBox PicMiniatura => picMiniatura;
        public Label LblTitulo => lblTitulo;
        public Label LblCalidad => lblCalidad;
        public Label LblRuta => lblRuta;
        public Label LblFecha => lblFecha;
        public Label LblTamano => lblTamano;
        public TextBox TxtRuta => txtRuta;
        public Button BtnCarpeta => btnCarpeta;
        public Button BtnAbrirArchivo => btnAbrirArchivo;
        public Label BtnEliminar => btnEliminar;

        public string ArchivoPath { get; set; }
        public string CarpetaDestino { get; set; }
        public string NombreBase { get; set; }
        public string PngPathGuardado { get; set; }

        public bool ExisteArchivo()
        {
            if (string.IsNullOrEmpty(ArchivoPath)) return false;
            if (File.Exists(ArchivoPath)) return true;

            if (string.IsNullOrEmpty(CarpetaDestino) || !Directory.Exists(CarpetaDestino)) return false;
            if (string.IsNullOrEmpty(NombreBase)) return false;

            try
            {
                var encontrados = Directory.GetFiles(CarpetaDestino, NombreBase + ".*", SearchOption.AllDirectories);
                foreach (var f in encontrados)
                {
                    string ext = Path.GetExtension(f).ToLowerInvariant();
                    if (ext == ".mp4" || ext == ".mkv" || ext == ".webm" || ext == ".mov")
                    { ArchivoPath = f; return true; }
                }
            }
            catch { }
            return false;
        }

        // ═══════════════════════════════════════════════════════════════
        // FONDO REDONDEADO (SIN BORDE). Solo FillPath, nada de DrawPath.
        // ═══════════════════════════════════════════════════════════════
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Color colorPadre = this.Parent?.BackColor ?? Color.FromArgb(30, 32, 40);
            using (var brush = new SolidBrush(colorPadre))
                g.FillRectangle(brush, ClientRectangle);

            var rect = new Rectangle(0, 0, Width, Height);
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var path = RoundRect(rect, 12))
            using (var fill = new SolidBrush(this.BackColor))
                g.FillPath(fill, path);
        }

        private static GraphicsPath RoundRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0) { path.AddRectangle(rect); return path; }

            int d = radius * 2;
            if (d > rect.Width) d = rect.Width;
            if (d > rect.Height) d = rect.Height;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void btnEliminar_MouseEnter(object sender, EventArgs e)
        {
            btnEliminar.ForeColor = Color.Yellow;
        }

        private void btnEliminar_MouseLeave(object sender, EventArgs e)
        {
            btnEliminar.ForeColor = System.Drawing.SystemColors.ControlDark;
        }
    }
}
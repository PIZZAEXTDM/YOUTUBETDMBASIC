using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace YOUTUBE
{
    public class RoundedButton : Button
    {
        private int _radius = 15;
        public int Radius { get => _radius; set { _radius = Math.Max(0, value); Invalidate(); } }

        private Color _borderColor = Color.Empty;
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        private int _borderWidth = 0;
        public int BorderWidth { get => _borderWidth; set { _borderWidth = Math.Max(0, value); Invalidate(); } }

        public RoundedButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw
                   | ControlStyles.SupportsTransparentBackColor, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.CheckedBackColor = Color.Transparent;
            UseVisualStyleBackColor = false;
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Color colorPadre = this.Parent?.BackColor ?? Color.FromArgb(30, 32, 40);
            using (var brushPadre = new SolidBrush(colorPadre))
                g.FillRectangle(brushPadre, ClientRectangle);

            int bw = _borderWidth;
            var rect = new Rectangle(bw / 2, bw / 2, Width - bw - 1, Height - bw - 1);
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var path = CreateRoundedPath(rect, _radius))
            {
                using (var brush = new SolidBrush(BackColor))
                    g.FillPath(brush, path);

                if (bw > 0 && _borderColor != Color.Empty)
                {
                    using (var pen = new Pen(_borderColor, bw))
                        g.DrawPath(pen, path);
                }
            }

            TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
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
    }

    /// <summary>
    /// Panel con esquinas redondeadas fluidas (sin Region pixelado).
    /// Usa GraphicsPath.FillPath + DrawPath con antialiasing.
    /// </summary>
    public class RoundedPanel : Panel
    {
        private int _radius = 12;
        public int Radius { get => _radius; set { _radius = Math.Max(0, value); Invalidate(); } }

        private Color _borderColor = Color.FromArgb(74, 78, 94);
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        private int _borderWidth = 1;
        public int BorderWidth { get => _borderWidth; set { _borderWidth = Math.Max(0, value); Invalidate(); } }

        private Color _fillColor = Color.FromArgb(42, 45, 56);
        public Color FillColor { get => _fillColor; set { _fillColor = value; Invalidate(); } }

        public RoundedPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw
                   | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Color colorPadre = this.Parent?.BackColor ?? Color.FromArgb(30, 32, 40);
            using (var brushPadre = new SolidBrush(colorPadre))
                g.FillRectangle(brushPadre, ClientRectangle);

            int bw = _borderWidth;
            var rect = new Rectangle(bw / 2, bw / 2, Width - bw - 1, Height - bw - 1);
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var path = RoundRect(rect, _radius))
            {
                using (var brush = new SolidBrush(_fillColor))
                    g.FillPath(brush, path);

                if (bw > 0 && _borderColor != Color.Empty)
                {
                    using (var pen = new Pen(_borderColor, bw))
                        g.DrawPath(pen, path);
                }
            }
        }

        public static GraphicsPath RoundRect(Rectangle rect, int radius)
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
    }

    public static class RegionHelper
    {
        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
                                                       int nRightRect, int nBottomRect,
                                                       int nWidthEllipse, int nHeightEllipse);

        /// <summary>
        /// Aplica Region redondeada SOLO al Form (ventana). No usar en controles
        /// internos, porque Region produce bordes pixelados.
        /// </summary>
        public static void AplicarRegionForm(Form form, int radius = 30)
        {
            if (form == null) return;
            if (form.Width <= 0 || form.Height <= 0) return;
            if (!form.IsHandleCreated) return;

            try
            {
                IntPtr hRgn = CreateRoundRectRgn(0, 0, form.Width + 1, form.Height + 1, radius * 2, radius * 2);
                if (hRgn == IntPtr.Zero) return;
                var nueva = Region.FromHrgn(hRgn);
                var vieja = form.Region;
                form.Region = nueva;
                vieja?.Dispose();
            }
            catch { }
        }
    }
}
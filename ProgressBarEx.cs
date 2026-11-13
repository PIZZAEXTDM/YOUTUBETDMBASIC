using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace YOUTUBE
{
    public enum ProgressState { Normal, Merging, Covering }

    public sealed class ProgressBarEx : Control
    {
        private int _value = 0;
        private int _max = 100;
        private ProgressState _state = ProgressState.Normal;

        public Color ColorNormal { get; set; } = Color.FromArgb(120, 220, 140);
        public Color ColorMerging { get; set; } = Color.FromArgb(220, 60, 60);
        public Color ColorCover { get; set; } = Color.FromArgb(60, 130, 220);
        public Color ColorBack { get; set; } = Color.FromArgb(30, 32, 40);
        public Color ColorBorder { get; set; } = Color.FromArgb(70, 74, 90);

        public ProgressBarEx()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Height = 22;
        }

        public int Maximum
        {
            get => _max;
            set { _max = Math.Max(1, value); Invalidate(); }
        }

        public int Minimum { get; set; } = 0;

        public int Value
        {
            get => _value;
            set
            {
                int v = Math.Max(Minimum, Math.Min(_max, value));
                if (v == _value) return;
                _value = v;
                Invalidate();
            }
        }

        public ProgressState State
        {
            get => _state;
            set { if (_state != value) { _state = value; Invalidate(); } }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        private Color CurrentColor =>
            _state == ProgressState.Merging ? ColorMerging :
            _state == ProgressState.Covering ? ColorCover : ColorNormal;

        protected override void OnPaint(PaintEventArgs e)
        {
            // ⚠ En modo diseño, dibujar un rectángulo simple para que el diseñador
            // no falle al pintar con GraphicsPath.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 32, 40)), ClientRectangle);
                return;
            }

            try
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using (var back = new SolidBrush(ColorBack))
                    g.FillRectangle(back, rect);

                int range = _max - Minimum;
                double pct = range <= 0 ? 0 : (double)(_value - Minimum) / range;
                int fillW = (int)Math.Round((Width - 2) * Math.Max(0, Math.Min(1, pct)));
                if (fillW > 0)
                {
                    using (var fore = new SolidBrush(CurrentColor))
                        g.FillRectangle(fore, 1, 1, fillW, Height - 2);
                }

                using (var pen = new Pen(ColorBorder))
                    g.DrawRectangle(pen, rect);

                if (!string.IsNullOrEmpty(Text))
                {
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    using (var br = new SolidBrush(Color.FromArgb(235, 235, 240)))
                        g.DrawString(Text, Font, br, new RectangleF(0, 0, Width, Height), sf);
                }
            }
            catch
            {
                base.OnPaint(e);
            }
        }
    }
}
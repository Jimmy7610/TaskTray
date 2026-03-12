using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TaskTray
{
    public static class DesignSystem
    {
        // Palette: Layered Midnight & Amber
        public static readonly Color BackgroundColor = Color.FromArgb(10, 10, 11);
        public static readonly Color SurfaceColor = Color.FromArgb(18, 18, 20);
        public static readonly Color SurfaceElevatedColor = Color.FromArgb(28, 28, 32);
        public static readonly Color HoverColor = Color.FromArgb(38, 38, 44);
        public static readonly Color AccentColor = Color.FromArgb(255, 179, 0); // Amber
        public static readonly Color AccentLightColor = Color.FromArgb(255, 193, 7);
        public static readonly Color TextColor = Color.FromArgb(224, 224, 224);
        public static readonly Color TextDimColor = Color.FromArgb(140, 140, 145);
        public static readonly Color BorderColor = Color.FromArgb(40, 40, 45);
        
        // Typography
        public static readonly Font MainFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font HeaderFont = new Font("Segoe UI Semibold", 11F);
        public static readonly Font TitleFont = new Font("Segoe UI Bold", 13F);
        public static readonly Font SmallFont = new Font("Segoe UI", 8.5F, FontStyle.Regular);

        public static void ApplyDarkTheme(Control control)
        {
            control.BackColor = BackgroundColor;
            control.ForeColor = TextColor;
            control.Font = MainFont;

            foreach (Control child in control.Controls)
            {
                ApplyToComponent(child);
            }
        }

        private static void ApplyToComponent(Control c)
        {
            // Default styling
            c.BackColor = SurfaceColor;
            c.ForeColor = TextColor;
            c.Font = MainFont;
            
            if (c is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = BorderColor;
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.MouseOverBackColor = HoverColor;
                btn.BackColor = SurfaceElevatedColor;
                btn.Cursor = Cursors.Hand;
            }
            else if (c is TextBox txt)
            {
                txt.BorderStyle = BorderStyle.FixedSingle;
                txt.BackColor = BackgroundColor;
            }
            else if (c is ListBox lb)
            {
                lb.BorderStyle = BorderStyle.None;
                lb.BackColor = SurfaceColor;
            }
            else if (c is Panel pnl)
            {
                // Panels are usually containers, let them be transparent or surface
            }
            
            foreach (Control child in c.Controls)
            {
                ApplyToComponent(child);
            }
        }

        public static void DrawRoundedRectangle(Graphics g, Rectangle rect, int radius, Color color, int borderSize = 0, Color? borderColor = null)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = new GraphicsPath())
            {
                int d = radius * 2;
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.X + rect.Width - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.X + rect.Width - d, rect.Y + rect.Height - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Y + rect.Height - d, d, d, 90, 90);
                path.CloseAllFigures();

                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillPath(brush, path);
                }

                if (borderSize > 0 && borderColor.HasValue)
                {
                    using (Pen pen = new Pen(borderColor.Value, borderSize))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
    }
}

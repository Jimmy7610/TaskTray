using System;
using System.Drawing;
using System.Windows.Forms;

namespace TaskTray
{
    public static class DesignSystem
    {
        // Palette: Midnight & Amber
        public static readonly Color BackColor = Color.FromArgb(18, 18, 18);
        public static readonly Color SurfaceColor = Color.FromArgb(30, 30, 30);
        public static readonly Color AccentColor = Color.FromArgb(255, 179, 0); // Amber
        public static readonly Color TextColor = Color.FromArgb(224, 224, 224);
        public static readonly Color TextDimColor = Color.FromArgb(160, 160, 160);
        public static readonly Color BorderColor = Color.FromArgb(45, 45, 45);
        
        public static readonly Font MainFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font HeaderFont = new Font("Segoe UI", 12F, FontStyle.Bold);
        public static readonly Font SmallFont = new Font("Segoe UI", 9F, FontStyle.Regular);

        public static void ApplyDarkTheme(Control control)
        {
            control.BackColor = BackColor;
            control.ForeColor = TextColor;
            control.Font = MainFont;

            foreach (Control child in control.Controls)
            {
                ApplyToComponent(child);
            }
        }

        private static void ApplyToComponent(Control c)
        {
            c.BackColor = SurfaceColor;
            c.ForeColor = TextColor;
            c.Font = MainFont;
            
            if (c is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = BorderColor;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 50);
            }
            else if (c is TextBox txt)
            {
                txt.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (c is ListBox lb)
            {
                lb.BorderStyle = BorderStyle.None;
            }
            
            foreach (Control child in c.Controls)
            {
                ApplyToComponent(child);
            }
        }
    }
}

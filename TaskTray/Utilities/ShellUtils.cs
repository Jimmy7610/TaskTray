using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TaskTray.Utilities
{
    public static class ShellUtils
    {
        public static string GetFileName(string path)
        {
            try
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            catch
            {
                return path;
            }
        }

        public static string? GetIconBase64(string path)
        {
            try
            {
                using (Icon icon = Icon.ExtractAssociatedIcon(path))
                {
                    if (icon == null) return null;
                    using (Bitmap bmp = icon.ToBitmap())
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bmp.Save(ms, ImageFormat.Png);
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        // Extremely simple shortcut resolver for .lnk files
        // Full resolution usually requires COM, but we'll try to keep it simple.
        public static string ResolveShortcut(string shortcutPath)
        {
            if (!shortcutPath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                return shortcutPath;

            try
            {
                // Note: For a truly robust solution, Shell32 COM or a dedicated library is better.
                // But let's see if we can get away with a simple check or if we need COM.
                // Since this is for a "polished" app, let's use the standard COM approach.
                Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType != null)
                {
                    dynamic shell = Activator.CreateInstance(shellType)!;
                    dynamic shortcut = shell.CreateShortcut(shortcutPath);
                    string target = shortcut.TargetPath;
                    Marshal.ReleaseComObject(shortcut);
                    Marshal.ReleaseComObject(shell);
                    return target;
                }
            }
            catch
            {
                // Fallback to original path if resolution fails
            }

            return shortcutPath;
        }
    }
}

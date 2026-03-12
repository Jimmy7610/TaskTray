using System;
using System.Drawing;
using System.IO;

namespace TaskTray.Services
{
    public static class ShellService
    {
        public static string GetFileName(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static string GetIconBase64(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    using (Icon icon = Icon.ExtractAssociatedIcon(path)!)
                    {
                        using (Bitmap bmp = icon.ToBitmap())
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                return Convert.ToBase64String(ms.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Icon extraction failed: {ex.Message}");
            }
            return string.Empty;
        }

        public static string ResolveShortcut(string filePath)
        {
            if (!filePath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                return filePath;

            try
            {
                Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType == null) return filePath;
                
                dynamic? shell = Activator.CreateInstance(shellType);
                if (shell == null) return filePath;
                
                dynamic shortcut = shell.CreateShortcut(filePath);
                return (string)shortcut.TargetPath;
            }
            catch
            {
                return filePath;
            }
        }
    }
}

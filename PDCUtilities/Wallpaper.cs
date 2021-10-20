using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace PDCUtility
{
    public class Wallpaper
    {
        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }

        public static bool Set(string filePath, Style style)
        {
            bool Success = false;
            try
            {
                Image i = System.Drawing.Image.FromFile(Path.GetFullPath(filePath));

                Set(i, style);

                Success = true;
            }
            catch (Exception) { }   // stifle

            return Success;
        }

        public static bool Set(Image image, Style style)
        {
            bool Success = false;
            try
            {
                string TempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");

                image.Save(TempPath, ImageFormat.Bmp);

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

                switch (style)
                {
                    case Style.Stretched:
                        key.SetValue(@"WallpaperStyle", 2.ToString());

                        key.SetValue(@"TileWallpaper", 0.ToString());

                        break;

                    case Style.Centered:
                        key.SetValue(@"WallpaperStyle", 1.ToString());

                        key.SetValue(@"TileWallpaper", 0.ToString());

                        break;

                    default:
                    case Style.Tiled:
                        key.SetValue(@"WallpaperStyle", 1.ToString());

                        key.SetValue(@"TileWallpaper", 1.ToString());

                        break;
                }

                NativeMethods.SystemParametersInfo(NativeMethods.SPI_SETDESKWALLPAPER, 0, TempPath, NativeMethods.SPIF_UPDATEINIFILE | NativeMethods.SPIF_SENDWININICHANGE);

                Success = true;
            }
            catch (Exception) { }   // stifle

            return Success;
        }

        private static class NativeMethods
        {
            public static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;

            public static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

            public static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);
        }
    }
}
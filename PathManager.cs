using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace YOUTUBE
{
    public static class PathManager
    {
        private const string REG_CONFIG = @"Software\YOUTUBEDownloader\Config";
        private const string KEY_DESCARGAS = "CarpetaDescargas";
        private const string KEY_HERRAMIENTAS = "CarpetaHerramientas";

        private static bool _atributosAplicados = false;

        // ── Propiedades ──────────────────────────────────────────────
        public static string CarpetaDescargas { get; private set; }
        public static string CarpetaHerramientas { get; private set; }

        // ── Carga inicial ────────────────────────────────────────────
        public static void Cargar()
        {
            string desc;
            string herr;
            bool cambio = false;

            using (var key = Registry.CurrentUser.CreateSubKey(REG_CONFIG))
            {
                if (key == null) return;
                desc = key.GetValue(KEY_DESCARGAS, "") as string ?? "";
                herr = key.GetValue(KEY_HERRAMIENTAS, "") as string ?? "";
            }

            try { desc = string.IsNullOrWhiteSpace(desc) ? "" : Path.GetFullPath(desc); } catch { desc = ""; }
            try { herr = string.IsNullOrWhiteSpace(herr) ? "" : Path.GetFullPath(herr); } catch { herr = ""; }

            if (string.IsNullOrWhiteSpace(desc))
            {
                desc = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads", "YOUTUBEDownloader");
                cambio = true;
            }

            if (string.IsNullOrWhiteSpace(herr))
            {
                herr = Application.StartupPath;
                cambio = true;
            }

            if (!File.Exists(Path.Combine(herr, "yt-dlp.exe")))
            {
                if (File.Exists(Path.Combine(Application.StartupPath, "yt-dlp.exe")))
                {
                    string nuevoHerr = Path.GetFullPath(Application.StartupPath);
                    if (!string.Equals(nuevoHerr, herr, StringComparison.OrdinalIgnoreCase))
                    {
                        herr = nuevoHerr;
                        cambio = true;
                    }
                }
            }

            if (!string.Equals(desc, CarpetaDescargas, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(herr, CarpetaHerramientas, StringComparison.OrdinalIgnoreCase))
                cambio = true;

            CarpetaDescargas = desc;
            CarpetaHerramientas = herr;

            if (cambio)
            {
                using (var key = Registry.CurrentUser.CreateSubKey(REG_CONFIG))
                {
                    key?.SetValue(KEY_DESCARGAS, CarpetaDescargas);
                    key?.SetValue(KEY_HERRAMIENTAS, CarpetaHerramientas);
                }
            }

            try { Directory.CreateDirectory(CarpetaDescargas); } catch { }
            try { Directory.CreateDirectory(Path.Combine(CarpetaDescargas, "png")); } catch { }

            AplicarAtributos();
        }

        // ── Guardar ──────────────────────────────────────────────────
        public static void Guardar(string carpetaDescargas, string carpetaHerramientas)
        {
            string d = string.IsNullOrWhiteSpace(carpetaDescargas)
                ? CarpetaDescargas
                : SafeFullPath(carpetaDescargas);
            string h = string.IsNullOrWhiteSpace(carpetaHerramientas)
                ? CarpetaHerramientas
                : SafeFullPath(carpetaHerramientas);

            bool cambio = !string.Equals(d, CarpetaDescargas, StringComparison.OrdinalIgnoreCase)
                       || !string.Equals(h, CarpetaHerramientas, StringComparison.OrdinalIgnoreCase);

            CarpetaDescargas = d;
            CarpetaHerramientas = h;

            if (cambio)
            {
                using (var key = Registry.CurrentUser.CreateSubKey(REG_CONFIG))
                {
                    key?.SetValue(KEY_DESCARGAS, CarpetaDescargas);
                    key?.SetValue(KEY_HERRAMIENTAS, CarpetaHerramientas);
                }
            }

            try { Directory.CreateDirectory(CarpetaDescargas); } catch { }
            try { Directory.CreateDirectory(Path.Combine(CarpetaDescargas, "png")); } catch { }

            AplicarAtributos();
        }

        // ── NUEVO: forzar CarpetaHerramientas (usado por Form1 al inicio) ──
        /// <summary>
        /// Fuerza la carpeta de herramientas y la persiste en el registro.
        /// Se usa cuando el .exe detecta que yt-dlp.exe está junto a él,
        /// o cuando el usuario arrastra la carpeta.
        /// </summary>
        public static void ConfigurarCarpetaHerramientas(string carpeta)
        {
            if (string.IsNullOrWhiteSpace(carpeta)) return;
            if (!Directory.Exists(carpeta)) return;

            string nueva = SafeFullPath(carpeta);
            if (string.Equals(nueva, CarpetaHerramientas, StringComparison.OrdinalIgnoreCase))
            {
                // Mismo path: solo asegurar atributos de críticos
                AplicarVisiblesCriticosSiempre();
                return;
            }

            CarpetaHerramientas = nueva;
            using (var key = Registry.CurrentUser.CreateSubKey(REG_CONFIG))
                key?.SetValue(KEY_HERRAMIENTAS, CarpetaHerramientas);

            AplicarAtributos();
        }

        // ── Reset de rutas ───────────────────────────────────────────
        public static void Reset()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REG_CONFIG, writable: true))
                {
                    if (key != null)
                    {
                        try { key.DeleteValue(KEY_DESCARGAS, false); } catch { }
                        try { key.DeleteValue(KEY_HERRAMIENTAS, false); } catch { }
                    }
                }
            }
            catch { }

            CarpetaDescargas = null;
            CarpetaHerramientas = null;
            _atributosAplicados = false;
            Cargar();
        }

        // ── Descubrir herramientas ───────────────────────────────────
        public static bool IntentarDescubrirHerramientas(string carpetaCandidata)
        {
            if (string.IsNullOrWhiteSpace(carpetaCandidata)) return false;
            if (!Directory.Exists(carpetaCandidata)) return false;
            if (!File.Exists(Path.Combine(carpetaCandidata, "yt-dlp.exe"))) return false;

            string nueva = SafeFullPath(carpetaCandidata);
            if (string.Equals(nueva, CarpetaHerramientas, StringComparison.OrdinalIgnoreCase))
            {
                AplicarVisiblesCriticosSiempre();
                return true;
            }

            CarpetaHerramientas = nueva;
            using (var key = Registry.CurrentUser.CreateSubKey(REG_CONFIG))
                key?.SetValue(KEY_HERRAMIENTAS, CarpetaHerramientas);

            AplicarAtributos();
            return true;
        }

        public static bool HerramientasValidas()
        {
            return !string.IsNullOrWhiteSpace(CarpetaHerramientas)
                && File.Exists(Path.Combine(CarpetaHerramientas, "yt-dlp.exe"));
        }

        // ── Atributos ────────────────────────────────────────────────
        public static void AplicarAtributos()
        {
            // 1) CRÍTICO: cookies.txt y libwebp_x64.dll SIEMPRE visibles, en cada llamada
            AplicarVisiblesCriticosSiempre();

            // 2) El resto, una sola vez por sesión
            if (_atributosAplicados) return;
            _atributosAplicados = true;

            try
            {
                string pngDir = Path.Combine(CarpetaDescargas, "png");
                if (Directory.Exists(pngDir))
                    Atribuir(pngDir, "+s +h +r");

                // Binarios que SÍ se ocultan (sin libwebp_x64.dll ni cookies.txt)
                if (!string.IsNullOrWhiteSpace(CarpetaHerramientas) && Directory.Exists(CarpetaHerramientas))
                {
                    string[] ocultar = { "yt-dlp.exe", "ffmpeg.exe", "ffprobe.exe", "deno.exe" };
                    foreach (var n in ocultar)
                    {
                        string f = Path.Combine(CarpetaHerramientas, n);
                        if (File.Exists(f)) Atribuir(f, "+s +h +r");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[PathManager] Atributos: " + ex.Message);
            }
        }

        private static void AplicarVisiblesCriticosSiempre()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CarpetaHerramientas)) return;
                if (!Directory.Exists(CarpetaHerramientas)) return;

                DesocultarArchivo(Path.Combine(CarpetaHerramientas, "cookies.txt"));
                DesocultarArchivo(Path.Combine(CarpetaHerramientas, "libwebp_x64.dll"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[PathManager] Visibles críticos: " + ex.Message);
            }
        }

        private static void DesocultarArchivo(string ruta)
        {
            if (string.IsNullOrEmpty(ruta)) return;
            if (!File.Exists(ruta)) return;

            Atribuir(ruta, "-s -h -r");

            if (EstaOcultoOSistema(ruta))
            {
                try
                {
                    var attrs = File.GetAttributes(ruta);
                    attrs &= ~(FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReadOnly);
                    File.SetAttributes(ruta, attrs);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[PathManager] SetAttributes falló para " + ruta + ": " + ex.Message);
                }

                if (EstaOcultoOSistema(ruta))
                    Atribuir(ruta, "-s -h -r");
            }
        }

        private static bool EstaOcultoOSistema(string ruta)
        {
            try
            {
                var attrs = File.GetAttributes(ruta);
                return (attrs & FileAttributes.Hidden) != 0
                    || (attrs & FileAttributes.System) != 0
                    || (attrs & FileAttributes.ReadOnly) != 0;
            }
            catch { return false; }
        }

        private static void Atribuir(string ruta, string atributos)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "attrib.exe",
                    Arguments = $"{atributos} \"{ruta}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using (var p = Process.Start(psi))
                {
                    p.StandardOutput.ReadToEnd();
                    p.StandardError.ReadToEnd();
                    p.WaitForExit(5000);
                }
            }
            catch { }
        }

        private static string SafeFullPath(string p)
        {
            try { return Path.GetFullPath(p.Trim().Trim('"')); }
            catch { return p; }
        }
    }
}
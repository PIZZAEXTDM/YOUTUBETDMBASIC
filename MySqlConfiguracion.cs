using Microsoft.Win32;
using System;
using System.Net;
using System.Net.Sockets;

namespace YOUTUBE
{
    public sealed class MySqlConfiguracion
    {
        private const string REG = @"Software\YOUTUBEDownloader\MySQL";

        public bool Habilitado { get; set; }
        public string Host { get; set; } = "localhost";
        public string Usuario { get; set; } = "root";
        public string Password { get; set; } = "";
        public int Puerto { get; set; } = 3306;

        // ── NUEVO: mantener carpeta png ──────────────────────────────
        public bool MantenerPngs { get; set; } = false;

        public enum TipoServidor { Local, Online }

        public TipoServidor Tipo
        {
            get
            {
                string h = (Host ?? "").Trim().ToLowerInvariant();
                if (h.Length == 0) return TipoServidor.Local;
                if (h == "localhost" || h == "::1") return TipoServidor.Local;

                if (IPAddress.TryParse(h, out var ip))
                {
                    if (IPAddress.IsLoopback(ip)) return TipoServidor.Local;
                    if (ip.AddressFamily == AddressFamily.InterNetwork && EsIpPrivada(ip))
                        return TipoServidor.Local;
                    return TipoServidor.Online;
                }
                return TipoServidor.Online;
            }
        }

        private static bool EsIpPrivada(IPAddress ip)
        {
            byte[] b = ip.GetAddressBytes();
            if (b.Length != 4) return false;
            if (b[0] == 10) return true;
            if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true;
            if (b[0] == 192 && b[1] == 168) return true;
            if (b[0] == 169 && b[1] == 254) return true;
            return false;
        }

        public bool Validar(out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(Host)) { error = "Host vacío."; return false; }
            if (string.IsNullOrWhiteSpace(Usuario)) { error = "Usuario vacío."; return false; }
            if (Puerto < 1 || Puerto > 65535) { error = "Puerto fuera de rango (1-65535)."; return false; }

            if (Tipo == TipoServidor.Online)
            {
                try
                {
                    var ips = Dns.GetHostAddresses(Host);
                    if (ips == null || ips.Length == 0)
                    { error = "El dominio online no resuelve a ninguna IP."; return false; }
                }
                catch (Exception ex)
                { error = "Dominio online no válido: " + ex.Message; return false; }
            }
            return true;
        }

        public void Guardar()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(REG))
                {
                    if (key == null) return;
                    key.SetValue("Habilitado", Habilitado ? 1 : 0);
                    key.SetValue("Host", Host ?? "");
                    key.SetValue("Usuario", Usuario ?? "");
                    key.SetValue("Password", Password ?? "");
                    key.SetValue("Puerto", Puerto);
                    key.SetValue("MantenerPngs", MantenerPngs ? 1 : 0);
                }
            }
            catch { }
        }

        public static MySqlConfiguracion Cargar()
        {
            var cfg = new MySqlConfiguracion();
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REG))
                {
                    if (key == null) return cfg;
                    cfg.Habilitado = Convert.ToInt32(key.GetValue("Habilitado", 0)) == 1;
                    cfg.Host = key.GetValue("Host", "localhost")?.ToString() ?? "localhost";
                    cfg.Usuario = key.GetValue("Usuario", "root")?.ToString() ?? "root";
                    cfg.Password = key.GetValue("Password", "")?.ToString() ?? "";
                    cfg.Puerto = Convert.ToInt32(key.GetValue("Puerto", 3306));
                    cfg.MantenerPngs = Convert.ToInt32(key.GetValue("MantenerPngs", 0)) == 1;
                }
            }
            catch { }
            return cfg;
        }

        public void Borrar()
        {
            try { Registry.CurrentUser.DeleteSubKeyTree(REG, false); } catch { }
        }
    }
}
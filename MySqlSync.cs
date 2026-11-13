using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YOUTUBE
{
    public static class MySqlSync
    {
        public static string Equipo { get; set; } = Environment.MachineName;

        public static bool Habilitado()
        {
            try
            {
                var cfg = MySqlConfiguracion.Cargar();
                return cfg.Habilitado;
            }
            catch { return false; }
        }

        private static string Cadena()
        {
            var cfg = MySqlConfiguracion.Cargar();
            if (!cfg.Habilitado) return null;
            return $"Server={cfg.Host};Port={cfg.Puerto};User ID={cfg.Usuario};" +
                   $"Password={cfg.Password};Database=youtubedownloader;" +
                   $"SslMode=None;Connection Timeout=5;";
        }

        public static void RegistrarEquipo()
        {
            string cs = Cadena(); if (cs == null) return;
            try
            {
                using (var conn = new MySqlConnection(cs))
                {
                    conn.Open();
                    string sql = @"INSERT INTO equipos (nombre, ip, estado)
                                   VALUES (@n, @ip, 'activo')
                                   ON DUPLICATE KEY UPDATE ip=@ip, estado='activo', ultima_conexion=NOW()";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@n", Equipo);
                        cmd.Parameters.AddWithValue("@ip", ObtenerIpLocal());
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("[MySqlSync] Registrar: " + ex.Message); }
        }

        public static void MarcarEstadoEquipo(string estado)
        {
            string cs = Cadena(); if (cs == null) return;
            try
            {
                using (var conn = new MySqlConnection(cs))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE equipos SET estado=@e WHERE nombre=@n", conn))
                    {
                        cmd.Parameters.AddWithValue("@e", estado);
                        cmd.Parameters.AddWithValue("@n", Equipo);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("[MySqlSync] Estado: " + ex.Message); }
        }

        public static void PublicarDescarga(string url, string titulo, string plataforma,
                                            string estado, double pct, double tamanoMb = 0,
                                            string archivo = null)
        {
            string cs = Cadena(); if (cs == null) return;
            try
            {
                using (var conn = new MySqlConnection(cs))
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO descargas (url, titulo, plataforma, estado, porcentaje, tamano_mb, nombre_archivo, equipo, fecha_inicio)
                        VALUES (@u, @t, @p, @e, @pct, @mb, @f, @eq, NOW())
                        ON DUPLICATE KEY UPDATE
                            titulo=VALUES(titulo), plataforma=VALUES(plataforma),
                            estado=VALUES(estado), porcentaje=VALUES(porcentaje),
                            tamano_mb=VALUES(tamano_mb), nombre_archivo=VALUES(nombre_archivo),
                            equipo=VALUES(equipo),
                            fecha_fin = IF(VALUES(estado)='completado', NOW(), fecha_fin)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", url);
                        cmd.Parameters.AddWithValue("@t", titulo ?? "");
                        cmd.Parameters.AddWithValue("@p", plataforma ?? "");
                        cmd.Parameters.AddWithValue("@e", estado);
                        cmd.Parameters.AddWithValue("@pct", pct);
                        cmd.Parameters.AddWithValue("@mb", tamanoMb);
                        cmd.Parameters.AddWithValue("@f", (object)archivo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@eq", Equipo);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("[MySqlSync] Publicar: " + ex.Message); }
        }

        public static List<string> ObtenerPendientes()
        {
            var lista = new List<string>();
            string cs = Cadena(); if (cs == null) return lista;
            try
            {
                using (var conn = new MySqlConnection(cs))
                {
                    conn.Open();
                    string sql = @"SELECT url FROM descargas
                                   WHERE estado='pendiente' AND (equipo=@e OR equipo IS NULL)
                                   ORDER BY fecha_inicio ASC LIMIT 5";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@e", Equipo);
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                                lista.Add(r.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("[MySqlSync] Pendientes: " + ex.Message); }
            return lista;
        }

        public static async Task PollComandosAsync(Form1 form)
        {
            string cs = Cadena(); if (cs == null) return;
            try
            {
                using (var conn = new MySqlConnection(cs))
                {
                    await conn.OpenAsync();
                    string sql = @"SELECT id, accion, tipo, valor_segundos
                                   FROM comandos
                                   WHERE equipo=@e AND ejecutado=0
                                   ORDER BY id ASC LIMIT 1";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@e", Equipo);
                        using (var r = await cmd.ExecuteReaderAsync())
                        {
                            if (!await r.ReadAsync()) return;
                            int id = r.GetInt32(0);
                            string accion = r.GetString(1);
                            string tipo = r.GetString(2);
                            int valor = r.GetInt32(3);

                            r.Close();
                            EjecutarComando(form, accion, tipo, valor);
                            MarcarEjecutado(conn, id);
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("[MySqlSync] Poll: " + ex.Message); }
        }

        private static void EjecutarComando(Form1 form, string accion, string tipo, int valor)
        {
            if (accion == "cancelar_apagado")
            {
                form.BeginInvoke(new Action(() => form.CancelarApagadoRemoto()));
                return;
            }
            if (accion == "apagar")
            {
                form.BeginInvoke(new Action(() => form.EjecutarApagadoRemoto(tipo, valor)));
            }
        }

        private static void MarcarEjecutado(MySqlConnection conn, int id)
        {
            try
            {
                using (var cmd = new MySqlCommand("UPDATE comandos SET ejecutado=1 WHERE id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private static string ObtenerIpLocal()
        {
            try
            {
                foreach (var ip in System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()))
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ip.ToString();
            }
            catch { }
            return "?";
        }
    }
}
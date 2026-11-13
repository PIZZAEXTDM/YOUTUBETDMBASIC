using System;
using System.Windows.Forms;

namespace YOUTUBE
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // La DLL se gestiona sola al primer uso (constructor estático de WebP).
        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
using System;
using System.IO;
using System.Windows.Forms;

namespace AsesoriaColon.LimpiarCertificados
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                try
                {
                    var log = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_error.txt");
                    File.WriteAllText(log, ex.ToString());
                }
                catch { }
                MessageBox.Show(ex.ToString(), "Limpiar certificados - error al abrir",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

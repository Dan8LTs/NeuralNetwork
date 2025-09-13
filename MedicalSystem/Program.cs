using System;
using System.Windows.Forms;

namespace MedicalSystem
{
    static class Program
    {
        public static SystemController Controller { get; private set; }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            Controller = new SystemController();
        }
    }
}

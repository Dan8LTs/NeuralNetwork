using System;
using System.Windows.Forms;

namespace MedicalSystem
{
    /// <summary>
    /// Точка входа медицинской системы анализа данных
    /// </summary>
    static class Program
    {
        /// <summary>Глобальный контроллер для управления нейронными сетями</summary>
        public static SystemController Controller { get; private set; }

        /// <summary>
        /// Точка входа приложения
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Controller = new SystemController();
            Application.Run(new MainForm());
        }
    }
}

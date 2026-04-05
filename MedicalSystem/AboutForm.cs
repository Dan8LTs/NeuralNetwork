using System;
using System.Reflection;
using System.Windows.Forms;

namespace MedicalSystem
{
    partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            this.labelProductName.Text = String.Format("{0} by {1}", AssemblyProduct, AssemblyCompany);
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.textBoxDescription.Text = @"Описание:

            1) Назначение:
            Приложение предоставляет интерфейс для обучения и использования нейронных сетей для двух задач:
             - прогнозирование наличия заболевания сердца по табличным данным (CSV),
             - классификация изображений клеток как заражённых или незаражённых.

            2) Работа с табличными данными (Heart CSV):
             - В главном окне укажите CSV-файл с 13 признаками и целевым столбцом.
             - Данные нормализуются (z-score), сеть обучается методом обратного распространения ошибки.
             - После обучения сеть доступна для предсказаний в окне ""Ввод данных"".

            3) Обучение по изображениям:
             - Укажите папки с PNG-изображениями ""Заражённые"" и ""Незаражённые"".
             - Изображения масштабируются до 30x30 и преобразуются в оттенки серого (0-255).
             - После обучения сеть доступна в окне ""Проверка изображения"".

            4) Ввод данных для предсказания:
             - Окно ввода строит поля автоматически на основе класса Patient (Age, Gender и т.д.).
             - Заполните поля числовыми значениями и нажмите ""Предсказать"".

            5) Технические детали:
             - Размер скрытого слоя: inputCount / 50 (сеть изображений), 16-8 (сеть данных).
             - Табличные входы стандартизируются (z-score) перед обучением.
             - Регуляризация L2: 0.001 (изображения), 0.012 (данные о сердце).

            6) Ограничения:
             - Сохранение обученных моделей не реализовано (обучение выполняется в памяти).
             - Для обучения по изображениям необходимо достаточное количество PNG-файлов.
             - CSV-файл должен содержать 14 столбцов: 13 признаков и 1 целевой.";
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }
        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
    }
}

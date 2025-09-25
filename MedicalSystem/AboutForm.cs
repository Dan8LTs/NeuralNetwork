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
            this.textBoxDescription.Text = @"Description:

1) Purpose:
The application provides a simple interface to train and use neural networks for two tasks:
 - predicting the presence of heart disease from tabular data (CSV),
 - classifying cell images as parasitized or uninfected.

2) Working with tabular data (Heart CSV):
 - In the Main -> Train section you can specify a CSV file with 13 features and a target column.
 - Data are loaded and normalized (means and standard deviations are computed), and the network is trained using backpropagation.
 - After training, the network is available for predictions from the data entry window.

3) Image training:
 - For image training, specify folders with PNG images ""Parasitized"" and ""Uninfected"".
 - Images are automatically resized to 30x30 and converted to binary pixels
   (brightness threshold default = 128), then used as network inputs.
 - After training, the network can be used in the image check window.

4) Data entry for prediction:
 - The entry window builds input fields automatically based on the Patient class (Age, Gender, etc.).
 - Fill the fields with numeric values and press Predict - the application will return a probability/result.

5) Technical details:
 - Networks are built according to the Topology class; the hidden layer size is roughly half of the inputs.
 - Tabular inputs are standardized (z-score) before training to improve convergence.
 - The image converter stores width/height and can export a binary image to a file.

6) Limitations and recommendations:
 - Saving/loading trained models is not implemented (training is performed in-memory during runtime).
 - Good image training requires a sufficient number of valid PNG files.
 - The CSV is expected to contain 14 columns: 13 features and 1 target column.";
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

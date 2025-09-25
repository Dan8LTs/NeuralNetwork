using NeuralNetwork;
using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace MedicalSystem
{
    public partial class MainForm : Form
    {
        private string heartFilename = string.Empty;

        public MainForm()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images (*.png)|*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var pictureConverter = new PictureConverter();
                var inputs = pictureConverter.Convert(openFileDialog.FileName);
                var result = Program.Controller.ImageNetwork.Predict(inputs.Count).Output;
            }
        }

        private void enterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var enterdataForm = new EnterData();
            enterdataForm.ShowForm();
        }

        private void heartCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            OpenFileDialog heartFileDialog = new OpenFileDialog();
            heartFileDialog.Filter = "Table (*.csv)|*.csv";
            if (heartFileDialog.ShowDialog() == DialogResult.OK)
            {
                heartFilename = heartFileDialog.FileName;
                heartCheckBox.Text = heartFilename;
            }
            else
            {
                heartCheckBox.Checked = false;
                heartFilename = string.Empty;
                heartCheckBox.Text = "Click to select file...";
            }
        }

        private void heartTrainButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(heartFilename) || !File.Exists(heartFilename))
            {
                heartResultLabel.Text = "Result: Select a valid path to heart.csv before starting training.";
                return;
            }

            heartResultLabel.Text = "Result: Training...";
            heartTrainButton.Enabled = false;

            Task.Run(() =>
            {
                try
                {
                    var rawLines = File.ReadAllLines(heartFilename);
                    var lines = new List<string>(rawLines.Length);
                    foreach (var ln in rawLines)
                    {
                        if (!string.IsNullOrWhiteSpace(ln))
                            lines.Add(ln.Trim());
                    }
                    int rowCount = lines.Count;
                    var outputs = new double[rowCount];
                    var inputs = new double[rowCount, 13];
                    for (int i = 0; i < rowCount; i++)
                    {
                        var parts = lines[i].Split(',');
                        for (int j = 0; j < 13; j++)
                        {
                            inputs[i, j] = Convert.ToDouble(parts[j].Trim(), new CultureInfo("en-US"));
                        }
                        outputs[i] = Convert.ToDouble(parts[13].Trim(), new CultureInfo("en-US"));
                    }

                    Program.Controller.DataNetwork.Learn(outputs, inputs, 80000);
                    Program.Controller.IsDataNetworkTrained = true;

                    this.Invoke(() =>
                    {
                        heartResultLabel.Text = "Result: Ready for prediction";
                        heartTrainButton.Enabled = true;
                    });
                }
                catch (Exception ex)
                {
                    this.Invoke(() =>
                    {
                        MessageBox.Show("Training error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        heartResultLabel.Text = "Result: Training failed";
                        heartTrainButton.Enabled = true;
                    });
                }
            });
        }

        private void parasitizedCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void uninfectedCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void imagesTrainButton_Click(object sender, EventArgs e)
        {

        }
    }
}
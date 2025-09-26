using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedicalSystem
{
    public partial class MainForm : Form
    {
        private string heartFilename = string.Empty;
        private string parasitizedFilename = string.Empty;
        private string uninfectedFilename = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            heartTrainButton.Click += heartTrainButton_Click;
            imagesTrainButton.Click += imagesTrainButton_Click;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var checkForm = new CheckImage();
            checkForm.ShowForm();
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
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select folder with Parasitized images";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    parasitizedFilename = dlg.SelectedPath;
                    parasitizedCheckBox.Text = parasitizedFilename;
                    parasitizedCheckBox.Checked = true;
                }
                else
                {
                    parasitizedCheckBox.Checked = false;
                    parasitizedFilename = string.Empty;
                    parasitizedCheckBox.Text = "(Parasitized) Click to select path... ";
                }
            }
        }

        private void uninfectedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select folder with Uninfected images";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    uninfectedFilename = dlg.SelectedPath;
                    uninfectedCheckBox.Text = uninfectedFilename;
                    uninfectedCheckBox.Checked = true;
                }
                else
                {
                    uninfectedCheckBox.Checked = false;
                    uninfectedFilename = string.Empty;
                    uninfectedCheckBox.Text = "(Uninfected) Click to select path...";
                }
            }
        }

        private void imagesTrainButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(parasitizedFilename) || string.IsNullOrWhiteSpace(uninfectedFilename) || !Directory.Exists(parasitizedFilename) || !Directory.Exists(uninfectedFilename))
            {
                imagesResultLabel.Text = "Result: Select valid folders for parasitized and uninfected images.";
                return;
            }

            imagesResultLabel.Text = "Result: Training...";
            imagesTrainButton.Enabled = false;

            Task.Run(() =>
            {
                try
                {
                    var converter = new PictureConverter();
                    var parasitizedFiles = Directory.GetFiles(parasitizedFilename, "*.png");
                    var uninfectedFiles = Directory.GetFiles(uninfectedFilename, "*.png");
                    if (parasitizedFiles.Length == 0 || uninfectedFiles.Length == 0)
                        throw new Exception("No PNG images found in one of the selected folders.");

                    var testParasitizedImageInput = converter.Convert(parasitizedFiles.First());
                    var testUninfectedImageInput = converter.Convert(uninfectedFiles.First());

                    Program.Controller.CreateImageNetwork(testParasitizedImageInput.Count);

                    double[,] parasitizedInputs = GetData(parasitizedFilename, converter, testParasitizedImageInput);
                    int parasitizedSize = parasitizedInputs.GetLength(0);
                    var expectedPar = Enumerable.Repeat(1.0, parasitizedSize).ToArray();
                    Program.Controller.ImageNetwork.Learn(expectedPar, parasitizedInputs, 1);

                    double[,] uninfectedInputs = GetData(uninfectedFilename, converter, testUninfectedImageInput);
                    int uninfectedSize = uninfectedInputs.GetLength(0);
                    var expectedUn = Enumerable.Repeat(0.0, uninfectedSize).ToArray();
                    Program.Controller.ImageNetwork.Learn(expectedUn, uninfectedInputs, 1);

                    Program.Controller.IsImageNetworkTrained = true;

                    this.Invoke(() =>
                    {
                        imagesResultLabel.Text = "Result: Ready for prediction";
                        imagesTrainButton.Enabled = true;
                    });
                }
                catch (Exception ex)
                {
                    this.Invoke(() =>
                    {
                        MessageBox.Show("Training error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        imagesResultLabel.Text = "Result: Training failed";
                        imagesTrainButton.Enabled = true;
                    });
                }
            });
        }

        private static double[,] GetData(string folderPath, PictureConverter converter, System.Collections.Generic.List<int> testImageInput)
        {
            var imageFiles = Directory.GetFiles(folderPath, "*.png");

            var rows = new List<int[]>();
            foreach (var file in imageFiles)
            {
                try
                {
                    var image = converter.Convert(file);
                    if (image != null && image.Count == testImageInput.Count)
                    {
                        rows.Add(image.ToArray());
                    }
                }
                catch
                {
                    // skip invalid files
                }
            }

            if (rows.Count == 0)
                throw new Exception("No valid PNG images found in folder: " + folderPath);

            var result = new double[rows.Count, testImageInput.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < testImageInput.Count; j++)
                {
                    result[i, j] = rows[i][j];
                }
            }
            return result;
        }
    }
}
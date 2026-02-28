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

                    if (lines.Count == 0)
                    {
                        throw new InvalidOperationException("CSV file is empty.");
                    }

                    int rowCount = lines.Count;
                    var outputs = new double[rowCount];
                    var inputs = new double[rowCount, 13];

                    for (int i = 0; i < rowCount; i++)
                    {
                        var parts = lines[i].Split(',');

                        if (parts.Length < 14)
                        {
                            throw new InvalidOperationException(
                                $"Row {i + 1}: Expected 14 columns, but found {parts.Length}. Line: '{lines[i]}'");
                        }

                        try
                        {
                            for (int j = 0; j < 13; j++)
                            {
                                if (!double.TryParse(parts[j].Trim(), NumberStyles.Any, new CultureInfo("en-US"), out var value))
                                {
                                    throw new FormatException(
                                        $"Invalid number format at row {i + 1}, column {j + 1}: '{parts[j].Trim()}'");
                                }
                                inputs[i, j] = value;
                            }

                            if (!double.TryParse(parts[13].Trim(), NumberStyles.Any, new CultureInfo("en-US"), out var output))
                            {
                                throw new FormatException(
                                    $"Invalid number format for output at row {i + 1}: '{parts[13].Trim()}'");
                            }
                            outputs[i] = output;

                            if (output != 0.0 && output != 1.0)
                            {
                                throw new InvalidOperationException(
                                    $"Row {i + 1}: Output value must be 0 or 1, but found {output}");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Error parsing row {i + 1}: {ex.Message}", ex);
                        }
                    }

                    // Split data: 70% training, 30% validation
                    var random = new Random();
                    var indices = Enumerable.Range(0, rowCount).ToList();

                    // Shuffle indices
                    for (int i = indices.Count - 1; i > 0; i--)
                    {
                        int j = random.Next(i + 1);
                        var temp = indices[i];
                        indices[i] = indices[j];
                        indices[j] = temp;
                    }

                    int trainCount = (int)(rowCount * 0.7);
                    int valCount = rowCount - trainCount;

                    var trainIndices = indices.Take(trainCount).ToArray();
                    var valIndices = indices.Skip(trainCount).ToArray();

                    var trainOutputs = trainIndices.Select(i => outputs[i]).ToArray();
                    var trainInputs = new double[trainCount, 13];
                    for (int i = 0; i < trainCount; i++)
                    {
                        for (int j = 0; j < 13; j++)
                        {
                            trainInputs[i, j] = inputs[trainIndices[i], j];
                        }
                    }

                    var valOutputs = valIndices.Select(i => outputs[i]).ToArray();
                    var valInputs = new double[valCount, 13];
                    for (int i = 0; i < valCount; i++)
                    {
                        for (int j = 0; j < 13; j++)
                        {
                            valInputs[i, j] = inputs[valIndices[i], j];
                        }
                    }

                    // Train network on training set
                    Program.Controller.ResetDataNetwork();
                    Program.Controller.DataNetwork.Learn(trainOutputs, trainInputs, 10000, (epoch, loss) =>
                    {
                        System.Diagnostics.Debug.WriteLine($"Epoch {epoch:D5}: Loss = {loss:E10}");
                    });
                    Program.Controller.IsDataNetworkTrained = true;

                    // Evaluate on training and validation sets
                    int trainCorrect = 0;
                    for (int i = 0; i < trainCount; i++)
                    {
                        var row = NeuralNetwork.NeuralNetwork.GetRow(trainInputs, i);
                        var pred = Program.Controller.DataNetwork.Predict(row).Output;
                        int predicted = pred >= 0.5 ? 1 : 0;
                        if (outputs[trainIndices[i]] == predicted)
                            trainCorrect++;
                    }
                    double trainAccuracy = (double)trainCorrect / trainCount;

                    int valCorrect = 0;
                    for (int i = 0; i < valCount; i++)
                    {
                        var row = NeuralNetwork.NeuralNetwork.GetRow(valInputs, i);
                        var pred = Program.Controller.DataNetwork.Predict(row).Output;
                        int predicted = pred >= 0.5 ? 1 : 0;
                        if (outputs[valIndices[i]] == predicted)
                            valCorrect++;
                    }
                    double valAccuracy = (double)valCorrect / valCount;

                    this.Invoke(() =>
                    {
                        heartResultLabel.Text = 
                            $"Result: Training completed\r\n" +
                            $"Train Accuracy: {trainAccuracy:P1} ({trainCorrect}/{trainCount})\r\n" +
                            $"Validation Accuracy: {valAccuracy:P1} ({valCorrect}/{valCount})";
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
                    Program.Controller.ImageNetwork.Learn(expectedPar, parasitizedInputs, 1, (epoch, loss) =>
                    {
                        System.Diagnostics.Debug.WriteLine($"Parasitized - Epoch {epoch}: Loss = {loss:E10}");
                    });

                    double[,] uninfectedInputs = GetData(uninfectedFilename, converter, testUninfectedImageInput);
                    int uninfectedSize = uninfectedInputs.GetLength(0);
                    var expectedUn = Enumerable.Repeat(0.0, uninfectedSize).ToArray();
                    Program.Controller.ImageNetwork.Learn(expectedUn, uninfectedInputs, 1, (epoch, loss) =>
                    {
                        System.Diagnostics.Debug.WriteLine($"Uninfected - Epoch {epoch}: Loss = {loss:E10}");
                    });

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
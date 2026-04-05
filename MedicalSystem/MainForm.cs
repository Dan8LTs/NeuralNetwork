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
    /// <summary>
    /// Главная форма приложения: управление обучением и тестированием нейронных сетей
    /// </summary>
    public partial class MainForm : Form
    {
        private string heartFilename = string.Empty;
        private string parasitizedFilename = string.Empty;
        private string uninfectedFilename = string.Empty;

        /// <summary>
        /// Инициализирует главную форму
        /// </summary>
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
            heartFileDialog.Filter = "Таблица (*.csv)|*.csv";
            if (heartFileDialog.ShowDialog() == DialogResult.OK)
            {
                heartFilename = heartFileDialog.FileName;
                heartCheckBox.Text = heartFilename;
            }
            else
            {
                heartCheckBox.Checked = false;
                heartFilename = string.Empty;
                heartCheckBox.Text = "Нажмите для выбора файла...";
            }
        }

        private void heartTrainButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(heartFilename) || !File.Exists(heartFilename))
            {
                heartResultLabel.Text = "Результат: Укажите корректный путь к heart.csv.";
                return;
            }

            heartResultLabel.Text = "Результат: Обучение...";
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
                    RandomProvider.ResetSeed(42);
                    var random = RandomProvider.Instance;
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
                    const int heartEpochs = 10000;
                    Program.Controller.ResetDataNetwork();
                    Program.Controller.DataNetwork.Learn(trainOutputs, trainInputs, heartEpochs, (epoch, loss) =>
                    {
                        if (epoch % 1000 == 0)
                        {
                            this.Invoke(new Action(() =>
                            {
                                heartResultLabel.Text = $"Результат: Обучение... Эпоха {epoch}/{heartEpochs}, Loss = {loss:F6}";
                            }));
                        }
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

                    this.Invoke(new Action(() =>
                    {
                        heartResultLabel.Text =
                            $"Результат: Обучение завершено\r\n" +
                            $"Точность (обучение): {trainAccuracy:P1} ({trainCorrect}/{trainCount})\r\n" +
                            $"Точность (валидация): {valAccuracy:P1} ({valCorrect}/{valCount})";
                        heartTrainButton.Enabled = true;
                    }));
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show("Ошибка обучения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        heartResultLabel.Text = "Результат: Ошибка обучения";
                        heartTrainButton.Enabled = true;
                    }));
                }
            });
        }

        private void parasitizedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Выберите папку с заражёнными изображениями";
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
                    parasitizedCheckBox.Text = "(Заражённые) Выбрать папку...";
                }
            }
        }

        private void uninfectedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Выберите папку с незаражёнными изображениями";
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
                    uninfectedCheckBox.Text = "(Незаражённые) Выбрать папку...";
                }
            }
        }

        private void imagesTrainButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(parasitizedFilename) || string.IsNullOrWhiteSpace(uninfectedFilename) || !Directory.Exists(parasitizedFilename) || !Directory.Exists(uninfectedFilename))
            {
                imagesResultLabel.Text = "Результат: Выберите папки с изображениями.";
                return;
            }

            imagesResultLabel.Text = "Результат: Обучение...";
            imagesTrainButton.Enabled = false;

            Task.Run(() =>
            {
                try
                {
                    const int size = 500;
                    const int epochs = 50;

                    var converter = new PictureConverter();
                    converter.GrayscaleMode = true;

                    var parasitizedFiles = Directory.GetFiles(parasitizedFilename, "*.png");
                    var uninfectedFiles = Directory.GetFiles(uninfectedFilename, "*.png");
                    if (parasitizedFiles.Length == 0 || uninfectedFiles.Length == 0)
                        throw new Exception("No PNG images found in one of the selected folders.");

                    var sampleImage = converter.Convert(parasitizedFiles[0]);
                    var inputSize = sampleImage.Count;

                    double[,] parasitizedInputs = GetData(parasitizedFilename, converter, inputSize, size);
                    double[,] uninfectedInputs = GetData(uninfectedFilename, converter, inputSize, size);

                    int parSize = parasitizedInputs.GetLength(0);
                    int uninfSize = uninfectedInputs.GetLength(0);
                    int totalSize = parSize + uninfSize;

                    var inputs = new double[totalSize, inputSize];
                    var outputs = new double[totalSize];

                    for (int i = 0; i < parSize; i++)
                    {
                        for (int j = 0; j < inputSize; j++)
                            inputs[i, j] = parasitizedInputs[i, j];
                        outputs[i] = 1.0;
                    }
                    for (int i = 0; i < uninfSize; i++)
                    {
                        for (int j = 0; j < inputSize; j++)
                            inputs[parSize + i, j] = uninfectedInputs[i, j];
                        outputs[parSize + i] = 0.0;
                    }

                    // Split 70% train / 30% validation — аналогично heartTrainButton_Click
                    RandomProvider.ResetSeed(42);
                    var random = RandomProvider.Instance;
                    var indices = Enumerable.Range(0, totalSize).ToList();
                    for (int i = indices.Count - 1; i > 0; i--)
                    {
                        int j = random.Next(i + 1);
                        var temp = indices[i];
                        indices[i] = indices[j];
                        indices[j] = temp;
                    }

                    int trainCount = (int)(totalSize * 0.7);
                    int valCount = totalSize - trainCount;

                    var trainIndices = indices.Take(trainCount).ToArray();
                    var valIndices = indices.Skip(trainCount).ToArray();

                    var trainOutputs = trainIndices.Select(i => outputs[i]).ToArray();
                    var trainInputs = new double[trainCount, inputSize];
                    for (int i = 0; i < trainCount; i++)
                        for (int j = 0; j < inputSize; j++)
                            trainInputs[i, j] = inputs[trainIndices[i], j];

                    var valOutputs = valIndices.Select(i => outputs[i]).ToArray();
                    var valInputs = new double[valCount, inputSize];
                    for (int i = 0; i < valCount; i++)
                        for (int j = 0; j < inputSize; j++)
                            valInputs[i, j] = inputs[valIndices[i], j];

                    Program.Controller.CreateImageNetwork(inputSize);
                    Program.Controller.ImageNetwork.Learn(trainOutputs, trainInputs, epochs, (epoch, loss) =>
                    {
                        if (epoch % 10 == 0)
                        {
                            this.Invoke(new Action(() =>
                            {
                                imagesResultLabel.Text = $"Результат: Обучение... Эпоха {epoch}/{epochs}, Loss = {loss:F6}";
                            }));
                        }
                    });

                    Program.Controller.IsImageNetworkTrained = true;

                    int trainCorrect = 0;
                    for (int i = 0; i < trainCount; i++)
                    {
                        var row = NeuralNetwork.NeuralNetwork.GetRow(trainInputs, i);
                        var pred = Program.Controller.ImageNetwork.Predict(row).Output;
                        if ((pred >= 0.5 ? 1 : 0) == (int)trainOutputs[i]) trainCorrect++;
                    }
                    double trainAccuracy = (double)trainCorrect / trainCount;

                    int valCorrect = 0;
                    for (int i = 0; i < valCount; i++)
                    {
                        var row = NeuralNetwork.NeuralNetwork.GetRow(valInputs, i);
                        var pred = Program.Controller.ImageNetwork.Predict(row).Output;
                        if ((pred >= 0.5 ? 1 : 0) == (int)valOutputs[i]) valCorrect++;
                    }
                    double valAccuracy = (double)valCorrect / valCount;

                    this.Invoke(new Action(() =>
                    {
                        imagesResultLabel.Text =
                            $"Результат: Обучение завершено\r\n" +
                            $"Точность (обучение): {trainAccuracy:P1} ({trainCorrect}/{trainCount})\r\n" +
                            $"Точность (валидация): {valAccuracy:P1} ({valCorrect}/{valCount})";
                        imagesTrainButton.Enabled = true;
                    }));
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show("Ошибка обучения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        imagesResultLabel.Text = "Результат: Ошибка обучения";
                        imagesTrainButton.Enabled = true;
                    }));
                }
            });
        }

        private static double[,] GetData(string folderPath, PictureConverter converter, int inputSize, int maxSize)
        {
            var allFiles = Directory.GetFiles(folderPath, "*.png");

            var rng = new Random(42);
            for (int i = allFiles.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                var tmp = allFiles[i];
                allFiles[i] = allFiles[j];
                allFiles[j] = tmp;
            }

            var rows = new List<double[]>();
            foreach (var file in allFiles)
            {
                if (rows.Count >= maxSize) break;
                try
                {
                    var image = converter.Convert(file);
                    if (image != null && image.Count == inputSize)
                        rows.Add(image.Select(v => (double)v).ToArray());
                }
                catch
                {
                    // skip invalid files
                }
            }

            if (rows.Count == 0)
                throw new Exception("No valid PNG images found in folder: " + folderPath);

            var result = new double[rows.Count, inputSize];
            for (int i = 0; i < rows.Count; i++)
                for (int j = 0; j < inputSize; j++)
                    result[i, j] = rows[i][j];
            return result;
        }
    }
}
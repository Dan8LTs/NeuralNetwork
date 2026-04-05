using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MedicalSystem
{
    /// <summary>
    /// Форма для классификации изображений клеток (зараженные vs незараженные)
    /// </summary>
    public partial class CheckImage : Form
    {
        private string imagePath = string.Empty;
        private List<int> lastConvertedPixels;
        private int previewScale = 8;

        /// <summary>
        /// Инициализирует форму и проверяет статус обучения сети
        /// </summary>
        public CheckImage()
        {
            InitializeComponent();
            UpdateStatusLabel();
        }

        private void UpdateStatusLabel()
        {
            if (!Program.Controller.IsImageNetworkTrained)
            {
                resultLabel.Text = "Результат: Сеть не обучена. Обучите её в главном окне.";
            }
            else
            {
                resultLabel.Text = "Результат: Готово к предсказанию";
            }
        }

        public void ShowForm()
        {
            UpdateStatusLabel();
            this.ShowDialog();
        }

        private void imageCheckBox_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Изображения (*.png)|*.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = openFileDialog.FileName;
                    imageCheckBox.Text = imagePath;
                    imageCheckBox.Checked = true;

                    try
                    {
                        var converter = new PictureConverter();
                        converter.GrayscaleMode = true;
                        lastConvertedPixels = converter.Convert(imagePath);

                        var bmp = CreatePreviewBitmap(converter.Width, converter.Height, lastConvertedPixels, previewScale);
                        previewBox.Image?.Dispose();
                        previewBox.Image = bmp;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка предпросмотра: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    imageCheckBox.Checked = false;
                }
            }
        }

        private void imageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!imageCheckBox.Checked)
            {
                imagePath = string.Empty;
                imageCheckBox.Text = "Нажмите для выбора изображения...";
                lastConvertedPixels = null;
                previewBox.Image?.Dispose();
                previewBox.Image = null;
            }
        }

        private Bitmap CreatePreviewBitmap(int width, int height, List<int> pixels, int scale)
        {
            if (pixels == null || pixels.Count != width * height)
                throw new ArgumentException("Invalid pixels for preview");

            var bmp = new Bitmap(width * scale, height * scale);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var v = pixels[y * width + x];
                        var gray = Math.Min(255, Math.Max(0, v));
                        var color = Color.FromArgb(gray, gray, gray);
                        using (var brush = new SolidBrush(color))
                        {
                            g.FillRectangle(brush, x * scale, y * scale, scale, scale);
                        }
                    }
                }
            }
            return bmp;
        }

        private void predictButton_Click(object sender, EventArgs e)
        {
            if (!Program.Controller.IsImageNetworkTrained)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                resultLabel.Text = "Результат: Выберите корректное изображение.";
                return;
            }

            try
            {
                double[] inputs;
                if (lastConvertedPixels != null)
                {
                    inputs = lastConvertedPixels.Select(i => (double)i).ToArray();
                }
                else
                {
                    var converter = new PictureConverter();
                    converter.GrayscaleMode = true;
                    var pixels = converter.Convert(imagePath);
                    inputs = pixels.Select(i => (double)i).ToArray();
                }

                var outputNeuron = Program.Controller.ImageNetwork.Predict(inputs);
                var result = outputNeuron.Output;
                if (result >= 0.5)
                {
                    resultLabel.Text = $"Результат: Клетка заражена ({result:F4})";
                }
                else
                {
                    resultLabel.Text = $"Результат: Клетка не заражена ({result:F4})";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка предсказания: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

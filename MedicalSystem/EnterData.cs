using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedicalSystem
{
    public partial class EnterData : Form
    {
        private List<TextBox> Inputs = new List<TextBox>();
        private bool isTrained = false;
        private string csvPath = @"D:\Desktop\Danil\Projects\Visual Studio\Dan8LTs\NeuralNetwork\NeuralNetworkTests\heart.csv";
        public EnterData()
        {
            InitializeComponent();
            var propInfo = typeof(Patient).GetProperties();
            for (int i = 0; i < propInfo.Length; i++)
            {
                var property = propInfo[i];
                var textBox = CreateTextBox(i, property);
                Controls.Add(textBox);
                Inputs.Add(textBox);
            }
        }
        public void ShowForm()
        {
            if (!isTrained)
            {
                resultLabel.Text = "Training...";
                Task.Run(() =>
                {
                    try
                    {
                        var rawLines = File.ReadAllLines(csvPath);
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
                        isTrained = true;
                        this.Invoke(() => resultLabel.Text = "Готово к предсказанию");
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(() => MessageBox.Show("Ошибка обучения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error));
                    }
                });
            }
            this.ShowDialog();
        }
        private TextBox CreateTextBox(int number, PropertyInfo property)
        {
            var y = number * 25 + 12;
            var textbox = new TextBox
            {
                Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right),
                Location = new System.Drawing.Point(12, y),
                Name = "textBox" + number,
                Size = new Size(454, 23),
                TabIndex = number,
                Text = property.Name,
                Tag = property.Name,
                Font = new Font("Verdana", 9F, FontStyle.Italic, GraphicsUnit.Point),
                ForeColor = Color.Gray
            };
            textbox.GotFocus += Textbox_GotFocus;
            textbox.LostFocus += Textbox_LostFocus;

            return textbox;
        }

        private void Textbox_LostFocus(object sender, EventArgs e)
        {
            var textbox = sender as TextBox;

            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                textbox.Text = textbox.Tag.ToString();
                textbox.Font = new Font("Verdana", 9F, FontStyle.Italic, GraphicsUnit.Point);
                textbox.ForeColor = Color.Gray;
            }
        }

        private void Textbox_GotFocus(object sender, EventArgs e)
        {
            var textbox = sender as TextBox;

            if (textbox.Text == textbox.Tag.ToString())
            {
                textbox.Text = "";
                textbox.Font = new Font("Verdana", 10F, FontStyle.Regular, GraphicsUnit.Point);
                textbox.ForeColor = Color.Black;
            }
        }

        private void predictButton_Click(object sender, EventArgs e)
        {
            if (!isTrained)
            {
                MessageBox.Show("Сеть ещё обучается. Пожалуйста, дождитесь окончания обучения.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var patient = new Patient();
            try
            {
                foreach (var textBox in this.Inputs)
                {
                    var text = (textBox.Text ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(text) || text == (textBox.Tag?.ToString() ?? string.Empty))
                        throw new Exception($"Поле '{textBox.Tag}' не заполнено");

                    var value = Convert.ToDouble(text, new CultureInfo("en-US"));

                    var prop = typeof(Patient).GetProperty(textBox.Tag.ToString());
                    if (prop == null)
                        throw new Exception($"Свойство '{textBox.Tag}' не найдено в Patient");

                    prop.SetValue(patient, value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Некорректный ввод: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double result = Program.Controller.DataNetwork.Predict(patient.GetInfo()).Output;
            if (result >= 0.5)
            {
                resultLabel.Text = "Result: " + $"Patient is sick ({result.ToString()})";
            }
            else
            {
                resultLabel.Text = "Result: " + $"Patient is healthy ({result.ToString()})";
            }
        }
    }
}

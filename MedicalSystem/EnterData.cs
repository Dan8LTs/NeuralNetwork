using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace MedicalSystem
{
    public partial class EnterData : Form
    {
        private List<TextBox> Inputs = new List<TextBox>();
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
            if (!Program.Controller.IsDataNetworkTrained)
            {
                resultLabel.Text = "Network not trained. Use Main window to train first.";
            }
            else
            {
                resultLabel.Text = "Ready for prediction";
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
            if (!Program.Controller.IsDataNetworkTrained)
            {
                MessageBox.Show("The network is not trained yet. Please train the network in the main window.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var patient = new Patient();
            try
            {
                foreach (var textBox in this.Inputs)
                {
                    var text = (textBox.Text ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(text) || text == (textBox.Tag?.ToString() ?? string.Empty))
                        throw new Exception($"Field '{textBox.Tag}' is empty");

                    var value = Convert.ToDouble(text, new CultureInfo("en-US"));

                    var prop = typeof(Patient).GetProperty(textBox.Tag.ToString());
                    if (prop == null)
                        throw new Exception($"Property '{textBox.Tag}' not found in Patient");

                    prop.SetValue(patient, value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid input: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

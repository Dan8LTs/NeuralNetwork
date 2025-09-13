using System;
using System.Collections.Generic;
using System.Drawing;
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
        public bool? ShowForm()
        {
            var form = new EnterData();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var patient = new Patient();

                foreach (var textBox in form.Inputs)
                {
                    patient.GetType().InvokeMember(textBox.Tag.ToString(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, Type.DefaultBinder, patient, new object[] { textBox.Text });
                }
                var result = Program.Controller.DataNetwork.Predict()?.Output;
                return result == 1.0;
            }
            return null;
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

            if (textbox.Text == "")
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
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

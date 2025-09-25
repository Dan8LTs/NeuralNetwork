namespace MedicalSystem
{
    partial class CheckImage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            resultLabel = new System.Windows.Forms.Label();
            previewBox = new System.Windows.Forms.PictureBox();
            imageCheckBox = new System.Windows.Forms.CheckBox();
            predictButton = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)previewBox).BeginInit();
            SuspendLayout();
            // 
            // resultLabel
            // 
            resultLabel.AutoSize = true;
            resultLabel.Location = new System.Drawing.Point(24, 447);
            resultLabel.Name = "resultLabel";
            resultLabel.Size = new System.Drawing.Size(97, 26);
            resultLabel.TabIndex = 0;
            resultLabel.Text = "Result: ";
            // 
            // previewBox
            // 
            previewBox.BackColor = System.Drawing.SystemColors.ControlDark;
            previewBox.Location = new System.Drawing.Point(323, 122);
            previewBox.Name = "previewBox";
            previewBox.Size = new System.Drawing.Size(300, 300);
            previewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            previewBox.TabIndex = 3;
            previewBox.TabStop = false;
            // 
            // imageCheckBox
            // 
            imageCheckBox.AutoSize = true;
            imageCheckBox.Location = new System.Drawing.Point(24, 73);
            imageCheckBox.Name = "imageCheckBox";
            imageCheckBox.Size = new System.Drawing.Size(289, 30);
            imageCheckBox.TabIndex = 1;
            imageCheckBox.Text = "Click to select image...";
            imageCheckBox.UseVisualStyleBackColor = true;
            imageCheckBox.CheckedChanged += imageCheckBox_CheckedChanged;
            imageCheckBox.Click += imageCheckBox_Click;
            // 
            // predictButton
            // 
            predictButton.Location = new System.Drawing.Point(786, 443);
            predictButton.Name = "predictButton";
            predictButton.Size = new System.Drawing.Size(122, 34);
            predictButton.TabIndex = 2;
            predictButton.Text = "Predict";
            predictButton.UseVisualStyleBackColor = true;
            predictButton.Click += predictButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(24, 31);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(426, 26);
            label1.TabIndex = 4;
            label1.Text = "Path to the cell image in PNG format:";
            // 
            // CheckImage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(14F, 26F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(930, 497);
            Controls.Add(label1);
            Controls.Add(previewBox);
            Controls.Add(predictButton);
            Controls.Add(imageCheckBox);
            Controls.Add(resultLabel);
            Font = new System.Drawing.Font("Verdana", 11F);
            Name = "CheckImage";
            Text = "Medical System (Check image)";
            ((System.ComponentModel.ISupportInitialize)previewBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label resultLabel;
        private System.Windows.Forms.PictureBox previewBox;
        private System.Windows.Forms.CheckBox imageCheckBox;
        private System.Windows.Forms.Button predictButton;
        private System.Windows.Forms.Label label1;
    }
}

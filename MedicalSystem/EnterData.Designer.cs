
namespace MedicalSystem
{
    partial class EnterData
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
            predictButton = new System.Windows.Forms.Button();
            resultLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // predictButton
            // 
            predictButton.Location = new System.Drawing.Point(539, 652);
            predictButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            predictButton.Name = "predictButton";
            predictButton.Size = new System.Drawing.Size(103, 36);
            predictButton.TabIndex = 1;
            predictButton.Text = "Predict";
            predictButton.UseVisualStyleBackColor = true;
            predictButton.Click += predictButton_Click;
            // 
            // resultLabel
            // 
            resultLabel.AutoSize = true;
            resultLabel.Location = new System.Drawing.Point(12, 659);
            resultLabel.Name = "resultLabel";
            resultLabel.Size = new System.Drawing.Size(74, 22);
            resultLabel.TabIndex = 2;
            resultLabel.Text = "Result:";
            // 
            // EnterData
            // 
            AcceptButton = predictButton;
            AutoScaleDimensions = new System.Drawing.SizeF(11F, 22F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(659, 707);
            Controls.Add(resultLabel);
            Controls.Add(predictButton);
            Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "EnterData";
            Text = "Medical System (Enter data)";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button predictButton;
        private System.Windows.Forms.Label resultLabel;
    }
}
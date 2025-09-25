
namespace MedicalSystem
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            enterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            infoLabel1 = new System.Windows.Forms.Label();
            heartCheckBox = new System.Windows.Forms.CheckBox();
            heartTrainButton = new System.Windows.Forms.Button();
            heartResultLabel = new System.Windows.Forms.Label();
            imagesResultLabel = new System.Windows.Forms.Label();
            imagesTrainButton = new System.Windows.Forms.Button();
            parasitizedCheckBox = new System.Windows.Forms.CheckBox();
            infoLabel2 = new System.Windows.Forms.Label();
            uninfectedCheckBox = new System.Windows.Forms.CheckBox();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(10, 4, 0, 4);
            menuStrip1.Size = new System.Drawing.Size(1188, 34);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { imageToolStripMenuItem, enterToolStripMenuItem });
            fileToolStripMenuItem.Font = new System.Drawing.Font("Verdana", 9F);
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(57, 26);
            fileToolStripMenuItem.Text = "File";
            // 
            // imageToolStripMenuItem
            // 
            imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            imageToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            imageToolStripMenuItem.Text = "Check image";
            imageToolStripMenuItem.Click += imageToolStripMenuItem_Click;
            // 
            // enterToolStripMenuItem
            // 
            enterToolStripMenuItem.Name = "enterToolStripMenuItem";
            enterToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            enterToolStripMenuItem.Text = "Enter data";
            enterToolStripMenuItem.Click += enterToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Font = new System.Drawing.Font("Verdana", 9F);
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(78, 26);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // infoLabel1
            // 
            infoLabel1.AutoSize = true;
            infoLabel1.Font = new System.Drawing.Font("Verdana", 11F);
            infoLabel1.Location = new System.Drawing.Point(31, 62);
            infoLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            infoLabel1.Name = "infoLabel1";
            infoLabel1.Size = new System.Drawing.Size(848, 26);
            infoLabel1.TabIndex = 2;
            infoLabel1.Text = "Path to the neural network training file with heart disease data (heart.csv):";
            // 
            // heartCheckBox
            // 
            heartCheckBox.AutoSize = true;
            heartCheckBox.Font = new System.Drawing.Font("Verdana", 11F);
            heartCheckBox.Location = new System.Drawing.Point(40, 112);
            heartCheckBox.Margin = new System.Windows.Forms.Padding(4);
            heartCheckBox.Name = "heartCheckBox";
            heartCheckBox.Size = new System.Drawing.Size(255, 30);
            heartCheckBox.TabIndex = 3;
            heartCheckBox.Text = "Click to select file...";
            heartCheckBox.UseVisualStyleBackColor = true;
            heartCheckBox.CheckedChanged += heartCheckBox_CheckedChanged;
            // 
            // heartTrainButton
            // 
            heartTrainButton.Font = new System.Drawing.Font("Verdana", 11F);
            heartTrainButton.Location = new System.Drawing.Point(40, 160);
            heartTrainButton.Margin = new System.Windows.Forms.Padding(4);
            heartTrainButton.Name = "heartTrainButton";
            heartTrainButton.Size = new System.Drawing.Size(143, 40);
            heartTrainButton.TabIndex = 4;
            heartTrainButton.Text = "Train";
            heartTrainButton.UseVisualStyleBackColor = true;
            heartTrainButton.Click += heartTrainButton_Click;
            // 
            // heartResultLabel
            // 
            heartResultLabel.AutoSize = true;
            heartResultLabel.Font = new System.Drawing.Font("Verdana", 11F);
            heartResultLabel.Location = new System.Drawing.Point(224, 167);
            heartResultLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            heartResultLabel.Name = "heartResultLabel";
            heartResultLabel.Size = new System.Drawing.Size(346, 26);
            heartResultLabel.TabIndex = 5;
            heartResultLabel.Text = "Result: Training hasn't started";
            // 
            // imagesResultLabel
            // 
            imagesResultLabel.AutoSize = true;
            imagesResultLabel.Font = new System.Drawing.Font("Verdana", 11F);
            imagesResultLabel.Location = new System.Drawing.Point(224, 392);
            imagesResultLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            imagesResultLabel.Name = "imagesResultLabel";
            imagesResultLabel.Size = new System.Drawing.Size(346, 26);
            imagesResultLabel.TabIndex = 9;
            imagesResultLabel.Text = "Result: Training hasn't started";
            // 
            // imagesTrainButton
            // 
            imagesTrainButton.Font = new System.Drawing.Font("Verdana", 11F);
            imagesTrainButton.Location = new System.Drawing.Point(40, 385);
            imagesTrainButton.Margin = new System.Windows.Forms.Padding(4);
            imagesTrainButton.Name = "imagesTrainButton";
            imagesTrainButton.Size = new System.Drawing.Size(143, 40);
            imagesTrainButton.TabIndex = 8;
            imagesTrainButton.Text = "Train";
            imagesTrainButton.UseVisualStyleBackColor = true;
            imagesTrainButton.Click += imagesTrainButton_Click;
            // 
            // parasitizedCheckBox
            // 
            parasitizedCheckBox.AutoSize = true;
            parasitizedCheckBox.Font = new System.Drawing.Font("Verdana", 11F);
            parasitizedCheckBox.Location = new System.Drawing.Point(40, 297);
            parasitizedCheckBox.Margin = new System.Windows.Forms.Padding(4);
            parasitizedCheckBox.Name = "parasitizedCheckBox";
            parasitizedCheckBox.Size = new System.Drawing.Size(436, 30);
            parasitizedCheckBox.TabIndex = 7;
            parasitizedCheckBox.Text = "(Parasitized) Click to select folder... ";
            parasitizedCheckBox.UseVisualStyleBackColor = true;
            parasitizedCheckBox.CheckedChanged += parasitizedCheckBox_CheckedChanged;
            // 
            // infoLabel2
            // 
            infoLabel2.AutoSize = true;
            infoLabel2.Font = new System.Drawing.Font("Verdana", 11F);
            infoLabel2.Location = new System.Drawing.Point(31, 247);
            infoLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            infoLabel2.Name = "infoLabel2";
            infoLabel2.Size = new System.Drawing.Size(1132, 26);
            infoLabel2.TabIndex = 6;
            infoLabel2.Text = "Paths to the neural network training folders with cell images in PNG format (Parasitized, Uninfected):";
            // 
            // uninfectedCheckBox
            // 
            uninfectedCheckBox.AutoSize = true;
            uninfectedCheckBox.Font = new System.Drawing.Font("Verdana", 11F);
            uninfectedCheckBox.Location = new System.Drawing.Point(40, 335);
            uninfectedCheckBox.Margin = new System.Windows.Forms.Padding(4);
            uninfectedCheckBox.Name = "uninfectedCheckBox";
            uninfectedCheckBox.Size = new System.Drawing.Size(428, 30);
            uninfectedCheckBox.TabIndex = 10;
            uninfectedCheckBox.Text = "(Uninfected) Click to select folder...";
            uninfectedCheckBox.UseVisualStyleBackColor = true;
            uninfectedCheckBox.CheckedChanged += uninfectedCheckBox_CheckedChanged;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(14F, 26F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1188, 460);
            Controls.Add(uninfectedCheckBox);
            Controls.Add(imagesResultLabel);
            Controls.Add(imagesTrainButton);
            Controls.Add(parasitizedCheckBox);
            Controls.Add(infoLabel2);
            Controls.Add(heartResultLabel);
            Controls.Add(heartTrainButton);
            Controls.Add(heartCheckBox);
            Controls.Add(infoLabel1);
            Controls.Add(menuStrip1);
            Font = new System.Drawing.Font("Verdana", 11F);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            Name = "MainForm";
            Text = "Medical System (Training models)";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label infoLabel1;
        private System.Windows.Forms.CheckBox heartCheckBox;
        private System.Windows.Forms.Button heartTrainButton;
        private System.Windows.Forms.Label heartResultLabel;
        private System.Windows.Forms.Label imagesResultLabel;
        private System.Windows.Forms.Button imagesTrainButton;
        private System.Windows.Forms.CheckBox parasitizedCheckBox;
        private System.Windows.Forms.Label infoLabel2;
        private System.Windows.Forms.CheckBox uninfectedCheckBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}


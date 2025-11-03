namespace BibAnalyzer
{
    partial class Form1
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
            this.panel1 = new Panel();
            this.cmdBrowseFolder = new Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = Color.Black;
            this.panel1.Controls.Add(this.cmdBrowseFolder);
            this.panel1.Dock = DockStyle.Left;
            this.panel1.ForeColor = Color.White;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(230, 538);
            this.panel1.TabIndex = 0;
            // 
            // cmdBrowseFolder
            // 
            this.cmdBrowseFolder.FlatStyle = FlatStyle.Flat;
            this.cmdBrowseFolder.Location = new Point(12, 12);
            this.cmdBrowseFolder.Name = "cmdBrowseFolder";
            this.cmdBrowseFolder.Size = new Size(204, 42);
            this.cmdBrowseFolder.TabIndex = 0;
            this.cmdBrowseFolder.Text = "Selecionar Pasta";
            this.cmdBrowseFolder.UseVisualStyleBackColor = true;
            this.cmdBrowseFolder.Click += this.cmdBrowseFolder_Click;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(857, 538);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button cmdBrowseFolder;
    }
}

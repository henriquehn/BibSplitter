namespace BibSplitter
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
            this.CmdSplit = new Button();
            this.dataGridView1 = new DataGridView();
            this.panel1 = new Panel();
            this.CmdConvert = new Button();
            this.CmdJoin = new Button();
            ((System.ComponentModel.ISupportInitialize)this.dataGridView1).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CmdSplit
            // 
            this.CmdSplit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.CmdSplit.Location = new Point(14, 16);
            this.CmdSplit.Margin = new Padding(3, 4, 3, 4);
            this.CmdSplit.Name = "CmdSplit";
            this.CmdSplit.Size = new Size(135, 56);
            this.CmdSplit.TabIndex = 0;
            this.CmdSplit.Text = "Separar BibTex por ano";
            this.CmdSplit.UseVisualStyleBackColor = true;
            this.CmdSplit.Click += this.button1_Click;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = DockStyle.Fill;
            this.dataGridView1.Location = new Point(155, 0);
            this.dataGridView1.Margin = new Padding(3, 4, 3, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new Size(635, 576);
            this.dataGridView1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CmdJoin);
            this.panel1.Controls.Add(this.CmdConvert);
            this.panel1.Controls.Add(this.CmdSplit);
            this.panel1.Dock = DockStyle.Left;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Margin = new Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(155, 576);
            this.panel1.TabIndex = 2;
            // 
            // CmdConvert
            // 
            this.CmdConvert.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.CmdConvert.Location = new Point(14, 80);
            this.CmdConvert.Margin = new Padding(3, 4, 3, 4);
            this.CmdConvert.Name = "CmdConvert";
            this.CmdConvert.Size = new Size(135, 56);
            this.CmdConvert.TabIndex = 1;
            this.CmdConvert.Text = "Converter CSV para BibTex";
            this.CmdConvert.UseVisualStyleBackColor = true;
            this.CmdConvert.Click += this.button2_Click;
            // 
            // CmdJoin
            // 
            this.CmdJoin.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.CmdJoin.Location = new Point(14, 143);
            this.CmdJoin.Name = "CmdJoin";
            this.CmdJoin.Size = new Size(135, 56);
            this.CmdJoin.TabIndex = 2;
            this.CmdJoin.Text = "Consolidar BibTex por base";
            this.CmdJoin.UseVisualStyleBackColor = true;
            this.CmdJoin.Click += this.CmdJoin_Click;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(790, 576);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Margin = new Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)this.dataGridView1).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Button CmdSplit;
        private DataGridView dataGridView1;
        private Panel panel1;
        private Button CmdConvert;
        private Button CmdJoin;
    }
}

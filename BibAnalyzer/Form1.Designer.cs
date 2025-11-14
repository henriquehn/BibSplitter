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
            this.chkClipboardMonitor = new CheckBox();
            this.cmdBrowseFolder = new Button();
            this.tableLayoutPanel1 = new TableLayoutPanel();
            this.panel4 = new Panel();
            this.undefinedEntriesGrid = new DataGridView();
            this.label3 = new Label();
            this.panel8 = new Panel();
            this.undefinedEntriesStatus = new Label();
            this.cmdSaveUndefined = new Button();
            this.panel2 = new Panel();
            this.validEntriesGrid = new DataGridView();
            this.panel6 = new Panel();
            this.validEntriesStatus = new Label();
            this.cmdSaveValid = new Button();
            this.label1 = new Label();
            this.panel3 = new Panel();
            this.invalidEntriesGrid = new DataGridView();
            this.label2 = new Label();
            this.panel7 = new Panel();
            this.invalidEntriesStatus = new Label();
            this.cmdSaveInvalid = new Button();
            this.panel5 = new Panel();
            this.txtFilter = new TextBox();
            this.label4 = new Label();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.undefinedEntriesGrid).BeginInit();
            this.panel8.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.validEntriesGrid).BeginInit();
            this.panel6.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.invalidEntriesGrid).BeginInit();
            this.panel7.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = Color.Black;
            this.panel1.Controls.Add(this.chkClipboardMonitor);
            this.panel1.Controls.Add(this.cmdBrowseFolder);
            this.panel1.Dock = DockStyle.Left;
            this.panel1.ForeColor = Color.White;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(208, 538);
            this.panel1.TabIndex = 0;
            // 
            // chkClipboardMonitor
            // 
            this.chkClipboardMonitor.AutoSize = true;
            this.chkClipboardMonitor.Location = new Point(12, 60);
            this.chkClipboardMonitor.Name = "chkClipboardMonitor";
            this.chkClipboardMonitor.Size = new Size(191, 19);
            this.chkClipboardMonitor.TabIndex = 1;
            this.chkClipboardMonitor.Text = "Monitorar área de transferência";
            this.chkClipboardMonitor.UseVisualStyleBackColor = true;
            this.chkClipboardMonitor.CheckedChanged += this.chkClipboardMonitor_CheckedChanged;
            // 
            // cmdBrowseFolder
            // 
            this.cmdBrowseFolder.FlatStyle = FlatStyle.Flat;
            this.cmdBrowseFolder.Location = new Point(12, 12);
            this.cmdBrowseFolder.Name = "cmdBrowseFolder";
            this.cmdBrowseFolder.Size = new Size(185, 42);
            this.cmdBrowseFolder.TabIndex = 0;
            this.cmdBrowseFolder.Text = "Selecionar Pasta";
            this.cmdBrowseFolder.UseVisualStyleBackColor = true;
            this.cmdBrowseFolder.Click += this.cmdBrowseFolder_Click;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            this.tableLayoutPanel1.Controls.Add(this.panel4, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel1.Dock = DockStyle.Fill;
            this.tableLayoutPanel1.Location = new Point(208, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new Size(649, 487);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.undefinedEntriesGrid);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.panel8);
            this.panel4.Dock = DockStyle.Fill;
            this.panel4.Location = new Point(431, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new Size(215, 481);
            this.panel4.TabIndex = 2;
            // 
            // undefinedEntriesGrid
            // 
            this.undefinedEntriesGrid.AllowUserToAddRows = false;
            this.undefinedEntriesGrid.AllowUserToDeleteRows = false;
            this.undefinedEntriesGrid.AllowUserToOrderColumns = true;
            this.undefinedEntriesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.undefinedEntriesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.undefinedEntriesGrid.Dock = DockStyle.Fill;
            this.undefinedEntriesGrid.Location = new Point(0, 23);
            this.undefinedEntriesGrid.Name = "undefinedEntriesGrid";
            this.undefinedEntriesGrid.Size = new Size(215, 411);
            this.undefinedEntriesGrid.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.BackColor = Color.Black;
            this.label3.Dock = DockStyle.Top;
            this.label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.label3.ForeColor = Color.White;
            this.label3.Location = new Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(215, 23);
            this.label3.TabIndex = 1;
            this.label3.Text = "Indefinido";
            this.label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.undefinedEntriesStatus);
            this.panel8.Controls.Add(this.cmdSaveUndefined);
            this.panel8.Dock = DockStyle.Bottom;
            this.panel8.Location = new Point(0, 434);
            this.panel8.Name = "panel8";
            this.panel8.Size = new Size(215, 47);
            this.panel8.TabIndex = 4;
            // 
            // undefinedEntriesStatus
            // 
            this.undefinedEntriesStatus.Dock = DockStyle.Fill;
            this.undefinedEntriesStatus.Location = new Point(0, 0);
            this.undefinedEntriesStatus.Name = "undefinedEntriesStatus";
            this.undefinedEntriesStatus.Size = new Size(164, 47);
            this.undefinedEntriesStatus.TabIndex = 3;
            this.undefinedEntriesStatus.Text = "Resultados: 0";
            this.undefinedEntriesStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmdSaveUndefined
            // 
            this.cmdSaveUndefined.Dock = DockStyle.Right;
            this.cmdSaveUndefined.Image = Properties.Resources.Save;
            this.cmdSaveUndefined.Location = new Point(164, 0);
            this.cmdSaveUndefined.Name = "cmdSaveUndefined";
            this.cmdSaveUndefined.Size = new Size(51, 47);
            this.cmdSaveUndefined.TabIndex = 3;
            this.cmdSaveUndefined.UseVisualStyleBackColor = true;
            this.cmdSaveUndefined.Click += this.cmdSaveUndefined_Click;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.validEntriesGrid);
            this.panel2.Controls.Add(this.panel6);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = DockStyle.Fill;
            this.panel2.Location = new Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(208, 481);
            this.panel2.TabIndex = 0;
            // 
            // validEntriesGrid
            // 
            this.validEntriesGrid.AllowUserToAddRows = false;
            this.validEntriesGrid.AllowUserToDeleteRows = false;
            this.validEntriesGrid.AllowUserToOrderColumns = true;
            this.validEntriesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.validEntriesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.validEntriesGrid.Dock = DockStyle.Fill;
            this.validEntriesGrid.Location = new Point(0, 23);
            this.validEntriesGrid.Name = "validEntriesGrid";
            this.validEntriesGrid.Size = new Size(208, 411);
            this.validEntriesGrid.TabIndex = 1;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.validEntriesStatus);
            this.panel6.Controls.Add(this.cmdSaveValid);
            this.panel6.Dock = DockStyle.Bottom;
            this.panel6.Location = new Point(0, 434);
            this.panel6.Name = "panel6";
            this.panel6.Size = new Size(208, 47);
            this.panel6.TabIndex = 3;
            // 
            // validEntriesStatus
            // 
            this.validEntriesStatus.Dock = DockStyle.Fill;
            this.validEntriesStatus.Location = new Point(0, 0);
            this.validEntriesStatus.Name = "validEntriesStatus";
            this.validEntriesStatus.Size = new Size(157, 47);
            this.validEntriesStatus.TabIndex = 2;
            this.validEntriesStatus.Text = "Resultados: 0";
            this.validEntriesStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmdSaveValid
            // 
            this.cmdSaveValid.Dock = DockStyle.Right;
            this.cmdSaveValid.Image = Properties.Resources.Save;
            this.cmdSaveValid.Location = new Point(157, 0);
            this.cmdSaveValid.Name = "cmdSaveValid";
            this.cmdSaveValid.Size = new Size(51, 47);
            this.cmdSaveValid.TabIndex = 3;
            this.cmdSaveValid.UseVisualStyleBackColor = true;
            this.cmdSaveValid.Click += this.cmdSaveValid_Click;
            // 
            // label1
            // 
            this.label1.BackColor = Color.Black;
            this.label1.Dock = DockStyle.Top;
            this.label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.label1.ForeColor = Color.White;
            this.label1.Location = new Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(208, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mais de 5 páginas";
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.invalidEntriesGrid);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.panel7);
            this.panel3.Dock = DockStyle.Fill;
            this.panel3.Location = new Point(217, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new Size(208, 481);
            this.panel3.TabIndex = 1;
            // 
            // invalidEntriesGrid
            // 
            this.invalidEntriesGrid.AllowUserToAddRows = false;
            this.invalidEntriesGrid.AllowUserToDeleteRows = false;
            this.invalidEntriesGrid.AllowUserToOrderColumns = true;
            this.invalidEntriesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.invalidEntriesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.invalidEntriesGrid.Dock = DockStyle.Fill;
            this.invalidEntriesGrid.Location = new Point(0, 23);
            this.invalidEntriesGrid.Name = "invalidEntriesGrid";
            this.invalidEntriesGrid.Size = new Size(208, 411);
            this.invalidEntriesGrid.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.BackColor = Color.Black;
            this.label2.Dock = DockStyle.Top;
            this.label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.label2.ForeColor = Color.White;
            this.label2.Location = new Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(208, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Menos de 5 páginas";
            this.label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.invalidEntriesStatus);
            this.panel7.Controls.Add(this.cmdSaveInvalid);
            this.panel7.Dock = DockStyle.Bottom;
            this.panel7.Location = new Point(0, 434);
            this.panel7.Name = "panel7";
            this.panel7.Size = new Size(208, 47);
            this.panel7.TabIndex = 4;
            // 
            // invalidEntriesStatus
            // 
            this.invalidEntriesStatus.Dock = DockStyle.Fill;
            this.invalidEntriesStatus.Location = new Point(0, 0);
            this.invalidEntriesStatus.Name = "invalidEntriesStatus";
            this.invalidEntriesStatus.Size = new Size(157, 47);
            this.invalidEntriesStatus.TabIndex = 3;
            this.invalidEntriesStatus.Text = "Resultados: 0";
            this.invalidEntriesStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmdSaveInvalid
            // 
            this.cmdSaveInvalid.Dock = DockStyle.Right;
            this.cmdSaveInvalid.Image = Properties.Resources.Save;
            this.cmdSaveInvalid.Location = new Point(157, 0);
            this.cmdSaveInvalid.Name = "cmdSaveInvalid";
            this.cmdSaveInvalid.Size = new Size(51, 47);
            this.cmdSaveInvalid.TabIndex = 3;
            this.cmdSaveInvalid.UseVisualStyleBackColor = true;
            this.cmdSaveInvalid.Click += this.cmdSaveInvalid_Click;
            // 
            // panel5
            // 
            this.panel5.BackColor = Color.FromArgb(224, 224, 224);
            this.panel5.Controls.Add(this.txtFilter);
            this.panel5.Controls.Add(this.label4);
            this.panel5.Dock = DockStyle.Bottom;
            this.panel5.Location = new Point(208, 487);
            this.panel5.Name = "panel5";
            this.panel5.Size = new Size(649, 51);
            this.panel5.TabIndex = 2;
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtFilter.BorderStyle = BorderStyle.FixedSingle;
            this.txtFilter.Location = new Point(6, 21);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new Size(631, 23);
            this.txtFilter.TabIndex = 1;
            this.txtFilter.TextChanged += this.txtFilter_TextChanged;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new Point(6, 3);
            this.label4.Name = "label4";
            this.label4.Size = new Size(141, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Busca por título ou autor:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(857, 538);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += this.Form1_Load;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.undefinedEntriesGrid).EndInit();
            this.panel8.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.validEntriesGrid).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.invalidEntriesGrid).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button cmdBrowseFolder;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel2;
        private Panel panel3;
        private Label label1;
        private Label label2;
        private DataGridView validEntriesGrid;
        private DataGridView invalidEntriesGrid;
        private Panel panel4;
        private DataGridView undefinedEntriesGrid;
        private Label label3;
        private Panel panel5;
        private TextBox txtFilter;
        private Label label4;
        private Label undefinedEntriesStatus;
        private Label invalidEntriesStatus;
        private CheckBox chkClipboardMonitor;
        private Panel panel6;
        private Button button1;
        private Label validEntriesStatus;
        private Panel panel8;
        private Button button3;
        private Panel panel7;
        private Button cmdSaveValid;
        private Button cmdSaveInvalid;
        private Button cmdSaveUndefined;
    }
}

using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Item_Summary_Spreadsheet
    {
        // Mouse down anywhere to drag
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Item_Summary_Spreadsheet));
            PresentationControls.CheckBoxProperties checkBoxProperties2 = new PresentationControls.CheckBoxProperties();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.close_button = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.from_month = new Financial_Journal.AdvancedComboBox();
            this.from_year = new Financial_Journal.AdvancedComboBox();
            this.to_year = new Financial_Journal.AdvancedComboBox();
            this.to_month = new Financial_Journal.AdvancedComboBox();
            this.excel_button = new System.Windows.Forms.Button();
            this.group_all = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbManual = new PresentationControls.CheckBoxComboBox();
            this.reset = new System.Windows.Forms.Button();
            this.group = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.sortByBox = new Financial_Journal.AdvancedComboBox();
            this.sortDirection = new Financial_Journal.AdvancedComboBox();
            this.showAvg = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(333, -10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 2851);
            this.textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(-31, -211);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(32, 2954);
            this.textBox2.TabIndex = 1;
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(-9, -11);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(2522, 12);
            this.textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 386);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(2522, 12);
            this.textBox4.TabIndex = 3;
            // 
            // close_button
            // 
            this.close_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.close_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.close_button.FlatAppearance.BorderSize = 0;
            this.close_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.close_button.ForeColor = System.Drawing.Color.White;
            this.close_button.Location = new System.Drawing.Point(313, 1);
            this.close_button.Margin = new System.Windows.Forms.Padding(0);
            this.close_button.Name = "close_button";
            this.close_button.Size = new System.Drawing.Size(17, 21);
            this.close_button.TabIndex = 68;
            this.close_button.Text = "X";
            this.close_button.UseVisualStyleBackColor = false;
            this.close_button.Click += new System.EventHandler(this.close_button_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.pictureBox1.BackgroundImage = global::Financial_Journal.Properties.Resources.Icons8_Ios7_Finance_Money_Box;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 22);
            this.pictureBox1.TabIndex = 70;
            this.pictureBox1.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label5.Location = new System.Drawing.Point(28, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(174, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Item Summary Spreadsheet";
            // 
            // textBox5
            // 
            this.textBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox5.Enabled = false;
            this.textBox5.Location = new System.Drawing.Point(-786, -2);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(2618, 27);
            this.textBox5.TabIndex = 72;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(21, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 16);
            this.label1.TabIndex = 111;
            this.label1.Text = "Choose Date Range:";
            // 
            // from_month
            // 
            this.from_month.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.from_month.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.from_month.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.from_month.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.from_month.ForeColor = System.Drawing.Color.White;
            this.from_month.FormattingEnabled = true;
            this.from_month.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.from_month.HighlightColor = System.Drawing.Color.Gray;
            this.from_month.Location = new System.Drawing.Point(125, 74);
            this.from_month.Name = "from_month";
            this.from_month.Size = new System.Drawing.Size(92, 23);
            this.from_month.TabIndex = 110;
            this.from_month.SelectedIndexChanged += new System.EventHandler(this.from_month_SelectedIndexChanged);
            // 
            // from_year
            // 
            this.from_year.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.from_year.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.from_year.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.from_year.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.from_year.ForeColor = System.Drawing.Color.White;
            this.from_year.FormattingEnabled = true;
            this.from_year.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.from_year.HighlightColor = System.Drawing.Color.Gray;
            this.from_year.Location = new System.Drawing.Point(223, 74);
            this.from_year.Name = "from_year";
            this.from_year.Size = new System.Drawing.Size(70, 23);
            this.from_year.TabIndex = 112;
            this.from_year.SelectedIndexChanged += new System.EventHandler(this.from_year_SelectedIndexChanged);
            // 
            // to_year
            // 
            this.to_year.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.to_year.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.to_year.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.to_year.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.to_year.ForeColor = System.Drawing.Color.White;
            this.to_year.FormattingEnabled = true;
            this.to_year.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.to_year.HighlightColor = System.Drawing.Color.Gray;
            this.to_year.Location = new System.Drawing.Point(223, 103);
            this.to_year.Name = "to_year";
            this.to_year.Size = new System.Drawing.Size(70, 23);
            this.to_year.TabIndex = 114;
            this.to_year.SelectedIndexChanged += new System.EventHandler(this.to_year_SelectedIndexChanged);
            // 
            // to_month
            // 
            this.to_month.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.to_month.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.to_month.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.to_month.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.to_month.ForeColor = System.Drawing.Color.White;
            this.to_month.FormattingEnabled = true;
            this.to_month.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.to_month.HighlightColor = System.Drawing.Color.Gray;
            this.to_month.Location = new System.Drawing.Point(125, 103);
            this.to_month.Name = "to_month";
            this.to_month.Size = new System.Drawing.Size(92, 23);
            this.to_month.TabIndex = 113;
            this.to_month.SelectedIndexChanged += new System.EventHandler(this.to_month_SelectedIndexChanged);
            // 
            // excel_button
            // 
            this.excel_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.excel_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.excel_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.excel_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.excel_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.excel_button.Image = global::Financial_Journal.Properties.Resources.excel_ico;
            this.excel_button.Location = new System.Drawing.Point(149, 321);
            this.excel_button.Margin = new System.Windows.Forms.Padding(1);
            this.excel_button.Name = "excel_button";
            this.excel_button.Size = new System.Drawing.Size(41, 43);
            this.excel_button.TabIndex = 116;
            this.excel_button.UseVisualStyleBackColor = true;
            this.excel_button.Click += new System.EventHandler(this.excel_button_Click);
            // 
            // group_all
            // 
            this.group_all.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.group_all.FlatAppearance.BorderSize = 0;
            this.group_all.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.group_all.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.group_all.ForeColor = System.Drawing.Color.White;
            this.group_all.Image = global::Financial_Journal.Properties.Resources.group;
            this.group_all.Location = new System.Drawing.Point(38, 281);
            this.group_all.Margin = new System.Windows.Forms.Padding(0);
            this.group_all.Name = "group_all";
            this.group_all.Size = new System.Drawing.Size(23, 22);
            this.group_all.TabIndex = 118;
            this.group_all.TabStop = false;
            this.group_all.UseVisualStyleBackColor = false;
            this.group_all.Click += new System.EventHandler(this.group_all_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(21, 251);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(131, 16);
            this.label8.TabIndex = 119;
            this.label8.Text = "Included Categories:";
            // 
            // cmbManual
            // 
            checkBoxProperties2.FlatAppearanceBorderColor = System.Drawing.Color.Empty;
            checkBoxProperties2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbManual.CheckBoxProperties = checkBoxProperties2;
            this.cmbManual.DisplayMemberSingleItem = "";
            this.cmbManual.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbManual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbManual.FormattingEnabled = true;
            this.cmbManual.Location = new System.Drawing.Point(63, 282);
            this.cmbManual.Name = "cmbManual";
            this.cmbManual.Size = new System.Drawing.Size(201, 21);
            this.cmbManual.TabIndex = 117;
            this.cmbManual.SelectedIndexChanged += new System.EventHandler(this.cmbManual_SelectedIndexChanged);
            // 
            // reset
            // 
            this.reset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.reset.FlatAppearance.BorderSize = 0;
            this.reset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reset.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reset.ForeColor = System.Drawing.Color.White;
            this.reset.Image = global::Financial_Journal.Properties.Resources.reload;
            this.reset.Location = new System.Drawing.Point(267, 282);
            this.reset.Margin = new System.Windows.Forms.Padding(0);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(23, 22);
            this.reset.TabIndex = 120;
            this.reset.TabStop = false;
            this.reset.UseVisualStyleBackColor = false;
            this.reset.Click += new System.EventHandler(this.reset_Click);
            // 
            // group
            // 
            this.group.AutoSize = true;
            this.group.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.group.Checked = true;
            this.group.CheckState = System.Windows.Forms.CheckState.Checked;
            this.group.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.group.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.group.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.group.Location = new System.Drawing.Point(44, 197);
            this.group.Name = "group";
            this.group.Size = new System.Drawing.Size(130, 20);
            this.group.TabIndex = 121;
            this.group.Text = "Group Categories";
            this.group.UseVisualStyleBackColor = false;
            this.group.CheckedChanged += new System.EventHandler(this.show_percent_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(21, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 16);
            this.label4.TabIndex = 134;
            this.label4.Text = "Data Options:";
            // 
            // label21
            // 
            this.label21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label21.AutoSize = true;
            this.label21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label21.Location = new System.Drawing.Point(138, 364);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(66, 13);
            this.label21.TabIndex = 169;
            this.label21.Text = "GENERATE";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(41, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 16);
            this.label2.TabIndex = 170;
            this.label2.Text = "Start Period";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.Control;
            this.label7.Location = new System.Drawing.Point(44, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 16);
            this.label7.TabIndex = 171;
            this.label7.Text = "End Period";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.Control;
            this.label9.Location = new System.Drawing.Point(44, 171);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 16);
            this.label9.TabIndex = 173;
            this.label9.Text = "Sort By:";
            // 
            // sortByBox
            // 
            this.sortByBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.sortByBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sortByBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sortByBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sortByBox.ForeColor = System.Drawing.Color.White;
            this.sortByBox.FormattingEnabled = true;
            this.sortByBox.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.sortByBox.HighlightColor = System.Drawing.Color.Gray;
            this.sortByBox.Location = new System.Drawing.Point(104, 168);
            this.sortByBox.Name = "sortByBox";
            this.sortByBox.Size = new System.Drawing.Size(92, 23);
            this.sortByBox.TabIndex = 172;
            // 
            // sortDirection
            // 
            this.sortDirection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.sortDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sortDirection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sortDirection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sortDirection.ForeColor = System.Drawing.Color.White;
            this.sortDirection.FormattingEnabled = true;
            this.sortDirection.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.sortDirection.HighlightColor = System.Drawing.Color.Gray;
            this.sortDirection.Location = new System.Drawing.Point(202, 168);
            this.sortDirection.Name = "sortDirection";
            this.sortDirection.Size = new System.Drawing.Size(98, 23);
            this.sortDirection.TabIndex = 174;
            // 
            // showAvg
            // 
            this.showAvg.AutoSize = true;
            this.showAvg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.showAvg.Checked = true;
            this.showAvg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showAvg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showAvg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showAvg.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.showAvg.Location = new System.Drawing.Point(75, 216);
            this.showAvg.Name = "showAvg";
            this.showAvg.Size = new System.Drawing.Size(142, 19);
            this.showAvg.TabIndex = 175;
            this.showAvg.Text = "Show group averages";
            this.showAvg.UseVisualStyleBackColor = false;
            this.showAvg.Visible = false;
            // 
            // Item_Summary_Spreadsheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(334, 387);
            this.Controls.Add(this.showAvg);
            this.Controls.Add(this.sortDirection);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.sortByBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.group);
            this.Controls.Add(this.reset);
            this.Controls.Add(this.group_all);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cmbManual);
            this.Controls.Add(this.excel_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.from_month);
            this.Controls.Add(this.from_year);
            this.Controls.Add(this.to_year);
            this.Controls.Add(this.to_month);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.close_button);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Item_Summary_Spreadsheet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Personal Banker: Item Summary Spreadsheet";
            this.Load += new System.EventHandler(this.Receipt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private Button close_button;
        private PictureBox pictureBox1;
        private Label label5;
        private TextBox textBox5;
        private Label label1;
        public AdvancedComboBox from_month;
        public AdvancedComboBox from_year;
        public AdvancedComboBox to_year;
        public AdvancedComboBox to_month;
        private Button excel_button;
        private Button group_all;
        private Label label8;
        private PresentationControls.CheckBoxComboBox cmbManual;
        private Button reset;
        private CheckBox group;
        private Label label4;
        private Label label21;
        private Label label2;
        private Label label7;
        private Label label9;
        public AdvancedComboBox sortByBox;
        public AdvancedComboBox sortDirection;
        private CheckBox showAvg;
    }
}


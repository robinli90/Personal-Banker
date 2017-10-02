using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Hobby_Management
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Hobby_Management));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.close_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.mainLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.freeAgentFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.maximize_button = new System.Windows.Forms.Button();
            this.bank_box = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.profile_list = new Financial_Journal.AdvancedComboBox();
            this.category_box = new Financial_Journal.AdvancedComboBox();
            this.highlight = new System.Windows.Forms.Button();
            this.import_phantom_profile = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.sort_button = new System.Windows.Forms.Button();
            this.settings_button = new System.Windows.Forms.Button();
            this.clear_all_button = new System.Windows.Forms.Button();
            this.garbagePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.memo_button = new System.Windows.Forms.Button();
            this.Add_button = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
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
            this.textBox1.Location = new System.Drawing.Point(1129, -10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 3019);
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
            this.textBox3.Size = new System.Drawing.Size(3318, 12);
            this.textBox3.TabIndex = 0;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 554);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(3318, 12);
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
            this.close_button.Location = new System.Drawing.Point(1109, 1);
            this.close_button.Margin = new System.Windows.Forms.Padding(0);
            this.close_button.Name = "close_button";
            this.close_button.Size = new System.Drawing.Size(17, 21);
            this.close_button.TabIndex = 68;
            this.close_button.Text = "X";
            this.close_button.UseVisualStyleBackColor = false;
            this.close_button.Click += new System.EventHandler(this.close_button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label5.Location = new System.Drawing.Point(28, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Hobby Management";
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
            this.textBox5.Size = new System.Drawing.Size(3414, 27);
            this.textBox5.TabIndex = 72;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(13, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(148, 16);
            this.label8.TabIndex = 105;
            this.label8.Text = "Select Hobby Category";
            // 
            // mainLayoutPanel
            // 
            this.mainLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainLayoutPanel.AutoScroll = true;
            this.mainLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainLayoutPanel.Location = new System.Drawing.Point(285, 98);
            this.mainLayoutPanel.Name = "mainLayoutPanel";
            this.mainLayoutPanel.Size = new System.Drawing.Size(844, 431);
            this.mainLayoutPanel.TabIndex = 106;
            // 
            // freeAgentFlowPanel
            // 
            this.freeAgentFlowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.freeAgentFlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.freeAgentFlowPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.freeAgentFlowPanel.ForeColor = System.Drawing.Color.White;
            this.freeAgentFlowPanel.Location = new System.Drawing.Point(16, 157);
            this.freeAgentFlowPanel.MaximumSize = new System.Drawing.Size(9999, 9999);
            this.freeAgentFlowPanel.Name = "freeAgentFlowPanel";
            this.freeAgentFlowPanel.Size = new System.Drawing.Size(230, 369);
            this.freeAgentFlowPanel.TabIndex = 107;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(285, 536);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(327, 13);
            this.label1.TabIndex = 111;
            this.label1.Text = "*Empty groups will be removed automatically when window is closed";
            // 
            // maximize_button
            // 
            this.maximize_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maximize_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.maximize_button.FlatAppearance.BorderSize = 0;
            this.maximize_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.maximize_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.maximize_button.ForeColor = System.Drawing.Color.White;
            this.maximize_button.Location = new System.Drawing.Point(1095, 1);
            this.maximize_button.Margin = new System.Windows.Forms.Padding(0);
            this.maximize_button.Name = "maximize_button";
            this.maximize_button.Size = new System.Drawing.Size(16, 22);
            this.maximize_button.TabIndex = 112;
            this.maximize_button.Text = "☐";
            this.maximize_button.UseVisualStyleBackColor = false;
            this.maximize_button.Click += new System.EventHandler(this.maximize_button_Click);
            // 
            // bank_box
            // 
            this.bank_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.bank_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bank_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bank_box.ForeColor = System.Drawing.Color.LightGray;
            this.bank_box.Location = new System.Drawing.Point(94, 125);
            this.bank_box.MaxLength = 30;
            this.bank_box.Name = "bank_box";
            this.bank_box.Size = new System.Drawing.Size(152, 22);
            this.bank_box.TabIndex = 114;
            this.bank_box.TextChanged += new System.EventHandler(this.bank_box_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(13, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 16);
            this.label2.TabIndex = 115;
            this.label2.Text = "Search Item:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label3.Location = new System.Drawing.Point(982, 534);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label3.Size = new System.Drawing.Size(76, 15);
            this.label3.TabIndex = 117;
            this.label3.Text = "Search Item:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label4.Location = new System.Drawing.Point(19, 531);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(76, 15);
            this.label4.TabIndex = 119;
            this.label4.Text = "Search Item:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(13, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 16);
            this.label6.TabIndex = 121;
            this.label6.Text = "Select Profile:";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Document = this.printDocument1;
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            this.printPreviewDialog1.Load += new System.EventHandler(this.printPreviewDialog1_Load);
            // 
            // profile_list
            // 
            this.profile_list.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.profile_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.profile_list.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.profile_list.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.profile_list.ForeColor = System.Drawing.Color.White;
            this.profile_list.FormattingEnabled = true;
            this.profile_list.FrameColor = System.Drawing.SystemColors.Desktop;
            this.profile_list.HighlightColor = System.Drawing.Color.Gray;
            this.profile_list.Location = new System.Drawing.Point(105, 39);
            this.profile_list.Name = "profile_list";
            this.profile_list.Size = new System.Drawing.Size(140, 23);
            this.profile_list.TabIndex = 0;
            this.profile_list.SelectedIndexChanged += new System.EventHandler(this.profile_list_SelectedIndexChanged);
            // 
            // category_box
            // 
            this.category_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.category_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.category_box.Enabled = false;
            this.category_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.category_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.category_box.ForeColor = System.Drawing.Color.White;
            this.category_box.FormattingEnabled = true;
            this.category_box.FrameColor = System.Drawing.SystemColors.Desktop;
            this.category_box.HighlightColor = System.Drawing.Color.Gray;
            this.category_box.Location = new System.Drawing.Point(16, 88);
            this.category_box.Name = "category_box";
            this.category_box.Size = new System.Drawing.Size(204, 23);
            this.category_box.TabIndex = 109;
            this.category_box.SelectedIndexChanged += new System.EventHandler(this.category_box_SelectedIndexChanged);
            // 
            // highlight
            // 
            this.highlight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.highlight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.highlight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.highlight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.highlight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.highlight.Image = global::Financial_Journal.Properties.Resources.highlighter;
            this.highlight.Location = new System.Drawing.Point(865, 39);
            this.highlight.Margin = new System.Windows.Forms.Padding(1);
            this.highlight.Name = "highlight";
            this.highlight.Size = new System.Drawing.Size(49, 44);
            this.highlight.TabIndex = 163;
            this.highlight.UseVisualStyleBackColor = true;
            this.highlight.Click += new System.EventHandler(this.highlight_Click);
            // 
            // import_phantom_profile
            // 
            this.import_phantom_profile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.import_phantom_profile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.import_phantom_profile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.import_phantom_profile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.import_phantom_profile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.import_phantom_profile.Image = global::Financial_Journal.Properties.Resources.import__1_;
            this.import_phantom_profile.Location = new System.Drawing.Point(914, 39);
            this.import_phantom_profile.Margin = new System.Windows.Forms.Padding(1);
            this.import_phantom_profile.Name = "import_phantom_profile";
            this.import_phantom_profile.Size = new System.Drawing.Size(49, 44);
            this.import_phantom_profile.TabIndex = 126;
            this.import_phantom_profile.UseVisualStyleBackColor = true;
            this.import_phantom_profile.Click += new System.EventHandler(this.import_phantom_profile_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button2.Image = global::Financial_Journal.Properties.Resources.printer__1_;
            this.button2.Location = new System.Drawing.Point(1016, 36);
            this.button2.Margin = new System.Windows.Forms.Padding(1);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(51, 51);
            this.button2.TabIndex = 125;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Image = global::Financial_Journal.Properties.Resources.question_circular_button;
            this.button3.Location = new System.Drawing.Point(223, 527);
            this.button3.Margin = new System.Windows.Forms.Padding(0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(23, 24);
            this.button3.TabIndex = 124;
            this.button3.TabStop = false;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Image = global::Financial_Journal.Properties.Resources.add_file_symbol;
            this.button1.Location = new System.Drawing.Point(200, 527);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 24);
            this.button1.TabIndex = 123;
            this.button1.TabStop = false;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // sort_button
            // 
            this.sort_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sort_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.sort_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sort_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sort_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sort_button.Image = global::Financial_Journal.Properties.Resources.list;
            this.sort_button.Location = new System.Drawing.Point(965, 40);
            this.sort_button.Margin = new System.Windows.Forms.Padding(1);
            this.sort_button.Name = "sort_button";
            this.sort_button.Size = new System.Drawing.Size(49, 44);
            this.sort_button.TabIndex = 118;
            this.sort_button.UseVisualStyleBackColor = true;
            this.sort_button.Click += new System.EventHandler(this.sort_button_Click);
            // 
            // settings_button
            // 
            this.settings_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.settings_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.settings_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settings_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settings_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.settings_button.Image = global::Financial_Journal.Properties.Resources.settings;
            this.settings_button.Location = new System.Drawing.Point(1069, 40);
            this.settings_button.Margin = new System.Windows.Forms.Padding(1);
            this.settings_button.Name = "settings_button";
            this.settings_button.Size = new System.Drawing.Size(49, 44);
            this.settings_button.TabIndex = 116;
            this.settings_button.UseVisualStyleBackColor = true;
            this.settings_button.Click += new System.EventHandler(this.settings_button_Click);
            // 
            // clear_all_button
            // 
            this.clear_all_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clear_all_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.clear_all_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clear_all_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clear_all_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.clear_all_button.Image = global::Financial_Journal.Properties.Resources.three_circling_arrows;
            this.clear_all_button.Location = new System.Drawing.Point(758, 38);
            this.clear_all_button.Margin = new System.Windows.Forms.Padding(1);
            this.clear_all_button.Name = "clear_all_button";
            this.clear_all_button.Size = new System.Drawing.Size(49, 44);
            this.clear_all_button.TabIndex = 113;
            this.clear_all_button.UseVisualStyleBackColor = true;
            this.clear_all_button.Click += new System.EventHandler(this.clear_all_button_Click);
            // 
            // garbagePanel
            // 
            this.garbagePanel.AllowDrop = true;
            this.garbagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.garbagePanel.AutoScroll = true;
            this.garbagePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.garbagePanel.BackgroundImage = global::Financial_Journal.Properties.Resources.garbage;
            this.garbagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.garbagePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.garbagePanel.Location = new System.Drawing.Point(287, 31);
            this.garbagePanel.Name = "garbagePanel";
            this.garbagePanel.Size = new System.Drawing.Size(465, 61);
            this.garbagePanel.TabIndex = 110;
            this.garbagePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.garbagePanel_Paint);
            // 
            // memo_button
            // 
            this.memo_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.memo_button.FlatAppearance.BorderSize = 0;
            this.memo_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.memo_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memo_button.ForeColor = System.Drawing.Color.White;
            this.memo_button.Image = global::Financial_Journal.Properties.Resources.unlocked;
            this.memo_button.Location = new System.Drawing.Point(223, 86);
            this.memo_button.Margin = new System.Windows.Forms.Padding(0);
            this.memo_button.Name = "memo_button";
            this.memo_button.Size = new System.Drawing.Size(23, 24);
            this.memo_button.TabIndex = 108;
            this.memo_button.TabStop = false;
            this.memo_button.UseVisualStyleBackColor = false;
            this.memo_button.Click += new System.EventHandler(this.memo_button_Click);
            // 
            // Add_button
            // 
            this.Add_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Add_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Add_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Add_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Add_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Add_button.Image = global::Financial_Journal.Properties.Resources.package;
            this.Add_button.Location = new System.Drawing.Point(812, 40);
            this.Add_button.Margin = new System.Windows.Forms.Padding(1);
            this.Add_button.Name = "Add_button";
            this.Add_button.Size = new System.Drawing.Size(49, 44);
            this.Add_button.TabIndex = 1;
            this.Add_button.UseVisualStyleBackColor = true;
            this.Add_button.Click += new System.EventHandler(this.Add_button_Click);
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
            // Hobby_Management
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1130, 555);
            this.Controls.Add(this.highlight);
            this.Controls.Add(this.import_phantom_profile);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.freeAgentFlowPanel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.profile_list);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sort_button);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.settings_button);
            this.Controls.Add(this.bank_box);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.clear_all_button);
            this.Controls.Add(this.maximize_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.garbagePanel);
            this.Controls.Add(this.category_box);
            this.Controls.Add(this.memo_button);
            this.Controls.Add(this.Add_button);
            this.Controls.Add(this.mainLayoutPanel);
            this.Controls.Add(this.label8);
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
            this.MinimumSize = new System.Drawing.Size(669, 532);
            this.Name = "Hobby_Management";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Personal Banker: Hobby Management";
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
        private Label label8;
        private FlowLayoutPanel mainLayoutPanel;
        private FlowLayoutPanel freeAgentFlowPanel;
        private Button Add_button;
        private Button memo_button;
        public AdvancedComboBox category_box;
        private FlowLayoutPanel garbagePanel;
        private Label label1;
        private Button maximize_button;
        private Button clear_all_button;
        public TextBox bank_box;
        private Label label2;
        private Button settings_button;
        private Label label3;
        private Button sort_button;
        private Label label4;
        public AdvancedComboBox profile_list;
        private Label label6;
        private Button button1;
        private Button button3;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private Button button2;
        private PrintPreviewDialog printPreviewDialog1;
        private Button import_phantom_profile;
        private Button highlight;
    }
}


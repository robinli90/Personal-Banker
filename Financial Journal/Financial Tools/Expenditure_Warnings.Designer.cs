using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Expenditure_Warnings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Expenditure_Warnings));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.close_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bufferedPanel1 = new Financial_Journal.BufferedPanel();
            this.limit_by_percent = new System.Windows.Forms.TextBox();
            this.Add_button = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.limit_by_price_box = new System.Windows.Forms.TextBox();
            this.percent_check = new System.Windows.Forms.CheckBox();
            this.dollar_check = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.final_level_box = new Financial_Journal.AdvancedComboBox();
            this.secondary_level_box = new Financial_Journal.AdvancedComboBox();
            this.first_level_box = new Financial_Journal.AdvancedComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.category_box = new Financial_Journal.AdvancedComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.bufferedPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(573, -10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 2724);
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
            this.textBox3.Size = new System.Drawing.Size(2762, 12);
            this.textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 259);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(2762, 12);
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
            this.close_button.Location = new System.Drawing.Point(553, 1);
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
            this.label5.Size = new System.Drawing.Size(139, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Expenditure Warnings";
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
            this.textBox5.Size = new System.Drawing.Size(2858, 27);
            this.textBox5.TabIndex = 72;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.pictureBox1.BackgroundImage = global::Financial_Journal.Properties.Resources.Icons8_Ios7_Finance_Money_Box;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 22);
            this.pictureBox1.TabIndex = 70;
            this.pictureBox1.TabStop = false;
            // 
            // bufferedPanel1
            // 
            this.bufferedPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bufferedPanel1.Controls.Add(this.limit_by_percent);
            this.bufferedPanel1.Controls.Add(this.Add_button);
            this.bufferedPanel1.Controls.Add(this.label7);
            this.bufferedPanel1.Controls.Add(this.limit_by_price_box);
            this.bufferedPanel1.Controls.Add(this.percent_check);
            this.bufferedPanel1.Controls.Add(this.dollar_check);
            this.bufferedPanel1.Controls.Add(this.label6);
            this.bufferedPanel1.Controls.Add(this.label4);
            this.bufferedPanel1.Controls.Add(this.label3);
            this.bufferedPanel1.Controls.Add(this.final_level_box);
            this.bufferedPanel1.Controls.Add(this.secondary_level_box);
            this.bufferedPanel1.Controls.Add(this.first_level_box);
            this.bufferedPanel1.Controls.Add(this.label1);
            this.bufferedPanel1.Controls.Add(this.label2);
            this.bufferedPanel1.Controls.Add(this.category_box);
            this.bufferedPanel1.Controls.Add(this.label8);
            this.bufferedPanel1.Controls.Add(this.label9);
            this.bufferedPanel1.Location = new System.Drawing.Point(8, 32);
            this.bufferedPanel1.Name = "bufferedPanel1";
            this.bufferedPanel1.Size = new System.Drawing.Size(557, 227);
            this.bufferedPanel1.TabIndex = 73;
            // 
            // limit_by_percent
            // 
            this.limit_by_percent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.limit_by_percent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.limit_by_percent.Enabled = false;
            this.limit_by_percent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.limit_by_percent.ForeColor = System.Drawing.Color.White;
            this.limit_by_percent.Location = new System.Drawing.Point(365, 195);
            this.limit_by_percent.MaxLength = 2;
            this.limit_by_percent.Name = "limit_by_percent";
            this.limit_by_percent.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.limit_by_percent.Size = new System.Drawing.Size(64, 22);
            this.limit_by_percent.TabIndex = 75;
            this.limit_by_percent.TextChanged += new System.EventHandler(this.limit_by_percent_TextChanged);
            // 
            // Add_button
            // 
            this.Add_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Add_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Add_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Add_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Add_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Add_button.Image = global::Financial_Journal.Properties.Resources.warning_exclamation_sign_in_a_circle_with_verification_mark;
            this.Add_button.Location = new System.Drawing.Point(507, 175);
            this.Add_button.Margin = new System.Windows.Forms.Padding(1);
            this.Add_button.Name = "Add_button";
            this.Add_button.Size = new System.Drawing.Size(49, 44);
            this.Add_button.TabIndex = 74;
            this.Add_button.UseVisualStyleBackColor = true;
            this.Add_button.Click += new System.EventHandler(this.Add_button_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.Control;
            this.label7.Location = new System.Drawing.Point(20, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(133, 16);
            this.label7.TabIndex = 28;
            this.label7.Text = "2) Set warning levels:";
            // 
            // limit_by_price_box
            // 
            this.limit_by_price_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.limit_by_price_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.limit_by_price_box.Enabled = false;
            this.limit_by_price_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.limit_by_price_box.ForeColor = System.Drawing.Color.White;
            this.limit_by_price_box.Location = new System.Drawing.Point(289, 167);
            this.limit_by_price_box.MaxLength = 10;
            this.limit_by_price_box.Name = "limit_by_price_box";
            this.limit_by_price_box.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.limit_by_price_box.Size = new System.Drawing.Size(64, 22);
            this.limit_by_price_box.TabIndex = 27;
            this.limit_by_price_box.TextChanged += new System.EventHandler(this.limit_by_price_box_TextChanged);
            // 
            // percent_check
            // 
            this.percent_check.AutoSize = true;
            this.percent_check.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.percent_check.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.percent_check.ForeColor = System.Drawing.SystemColors.Control;
            this.percent_check.Location = new System.Drawing.Point(203, 197);
            this.percent_check.Name = "percent_check";
            this.percent_check.Size = new System.Drawing.Size(163, 20);
            this.percent_check.TabIndex = 25;
            this.percent_check.Text = "or by percent of income";
            this.percent_check.UseVisualStyleBackColor = true;
            this.percent_check.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // dollar_check
            // 
            this.dollar_check.AutoSize = true;
            this.dollar_check.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dollar_check.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dollar_check.ForeColor = System.Drawing.SystemColors.Control;
            this.dollar_check.Location = new System.Drawing.Point(203, 169);
            this.dollar_check.Name = "dollar_check";
            this.dollar_check.Size = new System.Drawing.Size(89, 20);
            this.dollar_check.TabIndex = 23;
            this.dollar_check.Text = "by limit of $";
            this.dollar_check.UseVisualStyleBackColor = true;
            this.dollar_check.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(197, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 16);
            this.label6.TabIndex = 21;
            this.label6.Text = "Final";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(160, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 16);
            this.label4.TabIndex = 20;
            this.label4.Text = "Secondary";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(180, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 16);
            this.label3.TabIndex = 19;
            this.label3.Text = "Primary";
            // 
            // final_level_box
            // 
            this.final_level_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.final_level_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.final_level_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.final_level_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.final_level_box.ForeColor = System.Drawing.Color.White;
            this.final_level_box.FormattingEnabled = true;
            this.final_level_box.FrameColor = System.Drawing.SystemColors.Desktop;
            this.final_level_box.HighlightColor = System.Drawing.Color.Gray;
            this.final_level_box.Location = new System.Drawing.Point(240, 132);
            this.final_level_box.Name = "final_level_box";
            this.final_level_box.Size = new System.Drawing.Size(70, 23);
            this.final_level_box.TabIndex = 18;
            this.final_level_box.SelectedIndexChanged += new System.EventHandler(this.final_level_box_SelectedIndexChanged);
            // 
            // secondary_level_box
            // 
            this.secondary_level_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.secondary_level_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.secondary_level_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.secondary_level_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondary_level_box.ForeColor = System.Drawing.Color.White;
            this.secondary_level_box.FormattingEnabled = true;
            this.secondary_level_box.FrameColor = System.Drawing.SystemColors.Desktop;
            this.secondary_level_box.HighlightColor = System.Drawing.Color.Gray;
            this.secondary_level_box.Location = new System.Drawing.Point(240, 103);
            this.secondary_level_box.Name = "secondary_level_box";
            this.secondary_level_box.Size = new System.Drawing.Size(70, 23);
            this.secondary_level_box.TabIndex = 17;
            this.secondary_level_box.SelectedIndexChanged += new System.EventHandler(this.secondary_level_box_SelectedIndexChanged);
            // 
            // first_level_box
            // 
            this.first_level_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.first_level_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.first_level_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.first_level_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.first_level_box.ForeColor = System.Drawing.Color.White;
            this.first_level_box.FormattingEnabled = true;
            this.first_level_box.FrameColor = System.Drawing.SystemColors.Desktop;
            this.first_level_box.HighlightColor = System.Drawing.Color.Gray;
            this.first_level_box.Location = new System.Drawing.Point(240, 74);
            this.first_level_box.Name = "first_level_box";
            this.first_level_box.Size = new System.Drawing.Size(70, 23);
            this.first_level_box.TabIndex = 16;
            this.first_level_box.SelectedIndexChanged += new System.EventHandler(this.first_level_box_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 16);
            this.label1.TabIndex = 15;
            this.label1.Text = "Setup spending warnings below:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(18, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 16);
            this.label2.TabIndex = 14;
            this.label2.Text = "1)  Choose Category:";
            // 
            // category_box
            // 
            this.category_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.category_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.category_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.category_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.category_box.ForeColor = System.Drawing.Color.White;
            this.category_box.FormattingEnabled = true;
            this.category_box.FrameColor = System.Drawing.SystemColors.Desktop;
            this.category_box.HighlightColor = System.Drawing.Color.Gray;
            this.category_box.Location = new System.Drawing.Point(154, 41);
            this.category_box.Name = "category_box";
            this.category_box.Size = new System.Drawing.Size(156, 23);
            this.category_box.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(20, 184);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(181, 16);
            this.label8.TabIndex = 24;
            this.label8.Text = "3) Choose monthly reference:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.Control;
            this.label9.Location = new System.Drawing.Point(428, 198);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(20, 16);
            this.label9.TabIndex = 76;
            this.label9.Text = "%";
            // 
            // Expenditure_Warnings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(574, 260);
            this.Controls.Add(this.bufferedPanel1);
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
            this.Name = "Expenditure_Warnings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Personal Banker: Expenditure Warnings";
            this.Load += new System.EventHandler(this.Receipt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.bufferedPanel1.ResumeLayout(false);
            this.bufferedPanel1.PerformLayout();
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
        private BufferedPanel bufferedPanel1;
        private Label label2;
        public AdvancedComboBox category_box;
        private CheckBox percent_check;
        private Label label8;
        private CheckBox dollar_check;
        private Label label6;
        private Label label4;
        private Label label3;
        public AdvancedComboBox final_level_box;
        public AdvancedComboBox secondary_level_box;
        public AdvancedComboBox first_level_box;
        private Label label1;
        private Label label7;
        private TextBox limit_by_price_box;
        private Button Add_button;
        private Label label9;
        private TextBox limit_by_percent;
    }
}


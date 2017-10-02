using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Payment_Information
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Payment_Information));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.close_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bufferedPanel1 = new Financial_Journal.BufferedPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.search_desc_button = new System.Windows.Forms.Button();
            this.check3 = new System.Windows.Forms.CheckBox();
            this.check2 = new System.Windows.Forms.CheckBox();
            this.check1 = new System.Windows.Forms.CheckBox();
            this.check0 = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.phone_box = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.Add_button = new System.Windows.Forms.Button();
            this.billing_date_box = new Financial_Journal.AdvancedComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.limit_box = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.digit_box = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bank_box = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.company_box = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.type_box = new Financial_Journal.AdvancedComboBox();
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
            this.textBox1.Location = new System.Drawing.Point(799, -10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 2533);
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
            this.textBox3.Size = new System.Drawing.Size(2988, 12);
            this.textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 68);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(2988, 12);
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
            this.close_button.Location = new System.Drawing.Point(779, 1);
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
            this.label5.Size = new System.Drawing.Size(110, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Payment Options";
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
            this.textBox5.Size = new System.Drawing.Size(3084, 27);
            this.textBox5.TabIndex = 72;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.Control;
            this.label10.Location = new System.Drawing.Point(35, 39);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(163, 16);
            this.label10.TabIndex = 98;
            this.label10.Text = "Add new payment method";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Image = global::Financial_Journal.Properties.Resources.add;
            this.button1.Location = new System.Drawing.Point(12, 34);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 24);
            this.button1.TabIndex = 97;
            this.button1.TabStop = false;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            this.bufferedPanel1.Controls.Add(this.label12);
            this.bufferedPanel1.Controls.Add(this.textBox6);
            this.bufferedPanel1.Controls.Add(this.label11);
            this.bufferedPanel1.Controls.Add(this.search_desc_button);
            this.bufferedPanel1.Controls.Add(this.check3);
            this.bufferedPanel1.Controls.Add(this.check2);
            this.bufferedPanel1.Controls.Add(this.check1);
            this.bufferedPanel1.Controls.Add(this.check0);
            this.bufferedPanel1.Controls.Add(this.label9);
            this.bufferedPanel1.Controls.Add(this.phone_box);
            this.bufferedPanel1.Controls.Add(this.label8);
            this.bufferedPanel1.Controls.Add(this.Add_button);
            this.bufferedPanel1.Controls.Add(this.billing_date_box);
            this.bufferedPanel1.Controls.Add(this.label7);
            this.bufferedPanel1.Controls.Add(this.limit_box);
            this.bufferedPanel1.Controls.Add(this.label6);
            this.bufferedPanel1.Controls.Add(this.digit_box);
            this.bufferedPanel1.Controls.Add(this.label4);
            this.bufferedPanel1.Controls.Add(this.bank_box);
            this.bufferedPanel1.Controls.Add(this.label2);
            this.bufferedPanel1.Controls.Add(this.company_box);
            this.bufferedPanel1.Controls.Add(this.label3);
            this.bufferedPanel1.Controls.Add(this.label1);
            this.bufferedPanel1.Controls.Add(this.type_box);
            this.bufferedPanel1.Location = new System.Drawing.Point(6, 34);
            this.bufferedPanel1.Name = "bufferedPanel1";
            this.bufferedPanel1.Size = new System.Drawing.Size(788, 178);
            this.bufferedPanel1.TabIndex = 99;
            this.bufferedPanel1.Visible = false;
            this.bufferedPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.bufferedPanel1_Paint);
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.SystemColors.Control;
            this.label12.Location = new System.Drawing.Point(465, 69);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 16);
            this.label12.TabIndex = 100;
            this.label12.Text = "(optional)";
            // 
            // textBox6
            // 
            this.textBox6.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.textBox6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox6.ForeColor = System.Drawing.Color.White;
            this.textBox6.Location = new System.Drawing.Point(369, 67);
            this.textBox6.MaxLength = 30;
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(90, 22);
            this.textBox6.TabIndex = 98;
            this.textBox6.Text = "$";
            this.textBox6.TextChanged += new System.EventHandler(this.textBox6_TextChanged);
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.Control;
            this.label11.Location = new System.Drawing.Point(305, 67);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 16);
            this.label11.TabIndex = 99;
            this.label11.Text = "Balance";
            // 
            // search_desc_button
            // 
            this.search_desc_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.search_desc_button.FlatAppearance.BorderSize = 0;
            this.search_desc_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.search_desc_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.search_desc_button.ForeColor = System.Drawing.Color.White;
            this.search_desc_button.Image = global::Financial_Journal.Properties.Resources.error;
            this.search_desc_button.Location = new System.Drawing.Point(6, 1);
            this.search_desc_button.Margin = new System.Windows.Forms.Padding(0);
            this.search_desc_button.Name = "search_desc_button";
            this.search_desc_button.Size = new System.Drawing.Size(23, 22);
            this.search_desc_button.TabIndex = 97;
            this.search_desc_button.TabStop = false;
            this.search_desc_button.UseVisualStyleBackColor = false;
            this.search_desc_button.Click += new System.EventHandler(this.search_desc_button_Click);
            // 
            // check3
            // 
            this.check3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.check3.AutoSize = true;
            this.check3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.check3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.check3.ForeColor = System.Drawing.SystemColors.Control;
            this.check3.Location = new System.Drawing.Point(444, 145);
            this.check3.Name = "check3";
            this.check3.Size = new System.Drawing.Size(195, 20);
            this.check3.TabIndex = 96;
            this.check3.Text = "4)  90% of spending limit alert";
            this.check3.UseVisualStyleBackColor = true;
            // 
            // check2
            // 
            this.check2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.check2.AutoSize = true;
            this.check2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.check2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.check2.ForeColor = System.Drawing.SystemColors.Control;
            this.check2.Location = new System.Drawing.Point(444, 119);
            this.check2.Name = "check2";
            this.check2.Size = new System.Drawing.Size(195, 20);
            this.check2.TabIndex = 95;
            this.check2.Text = "3)  50% of spending limit alert";
            this.check2.UseVisualStyleBackColor = true;
            // 
            // check1
            // 
            this.check1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.check1.AutoSize = true;
            this.check1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.check1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.check1.ForeColor = System.Drawing.SystemColors.Control;
            this.check1.Location = new System.Drawing.Point(144, 145);
            this.check1.Name = "check1";
            this.check1.Size = new System.Drawing.Size(195, 20);
            this.check1.TabIndex = 94;
            this.check1.Text = "2)  20% of spending limit alert";
            this.check1.UseVisualStyleBackColor = true;
            // 
            // check0
            // 
            this.check0.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.check0.AutoSize = true;
            this.check0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.check0.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.check0.ForeColor = System.Drawing.SystemColors.Control;
            this.check0.Location = new System.Drawing.Point(144, 119);
            this.check0.Name = "check0";
            this.check0.Size = new System.Drawing.Size(264, 20);
            this.check0.TabIndex = 93;
            this.check0.Text = "1)  Upcoming payment due alert (5 days)";
            this.check0.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.Control;
            this.label9.Location = new System.Drawing.Point(124, 94);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 16);
            this.label9.TabIndex = 88;
            this.label9.Text = "Alerts";
            // 
            // phone_box
            // 
            this.phone_box.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.phone_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.phone_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.phone_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.phone_box.ForeColor = System.Drawing.Color.White;
            this.phone_box.Location = new System.Drawing.Point(174, 65);
            this.phone_box.MaxLength = 30;
            this.phone_box.Name = "phone_box";
            this.phone_box.Size = new System.Drawing.Size(118, 22);
            this.phone_box.TabIndex = 86;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(111, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 16);
            this.label8.TabIndex = 87;
            this.label8.Text = "Phone #";
            // 
            // Add_button
            // 
            this.Add_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Add_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Add_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Add_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Add_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Add_button.Image = global::Financial_Journal.Properties.Resources.add_notes;
            this.Add_button.Location = new System.Drawing.Point(682, 121);
            this.Add_button.Margin = new System.Windows.Forms.Padding(1);
            this.Add_button.Name = "Add_button";
            this.Add_button.Size = new System.Drawing.Size(49, 44);
            this.Add_button.TabIndex = 85;
            this.Add_button.UseVisualStyleBackColor = true;
            this.Add_button.Click += new System.EventHandler(this.Add_button_Click);
            // 
            // billing_date_box
            // 
            this.billing_date_box.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.billing_date_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.billing_date_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.billing_date_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.billing_date_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.billing_date_box.ForeColor = System.Drawing.Color.White;
            this.billing_date_box.FormattingEnabled = true;
            this.billing_date_box.FrameColor = System.Drawing.Color.Lime;
            this.billing_date_box.HighlightColor = System.Drawing.Color.Gray;
            this.billing_date_box.Location = new System.Drawing.Point(657, 37);
            this.billing_date_box.Name = "billing_date_box";
            this.billing_date_box.Size = new System.Drawing.Size(54, 23);
            this.billing_date_box.TabIndex = 84;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.Control;
            this.label7.Location = new System.Drawing.Point(545, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 16);
            this.label7.TabIndex = 83;
            this.label7.Text = "Billing Start Date";
            // 
            // limit_box
            // 
            this.limit_box.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.limit_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.limit_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.limit_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.limit_box.ForeColor = System.Drawing.Color.White;
            this.limit_box.Location = new System.Drawing.Point(369, 37);
            this.limit_box.MaxLength = 30;
            this.limit_box.Name = "limit_box";
            this.limit_box.Size = new System.Drawing.Size(90, 22);
            this.limit_box.TabIndex = 81;
            this.limit_box.Text = "$";
            this.limit_box.TextChanged += new System.EventHandler(this.limit_box_TextChanged);
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(328, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 16);
            this.label6.TabIndex = 82;
            this.label6.Text = "Limit";
            // 
            // digit_box
            // 
            this.digit_box.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.digit_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.digit_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.digit_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.digit_box.ForeColor = System.Drawing.Color.White;
            this.digit_box.Location = new System.Drawing.Point(657, 9);
            this.digit_box.MaxLength = 4;
            this.digit_box.Name = "digit_box";
            this.digit_box.Size = new System.Drawing.Size(54, 22);
            this.digit_box.TabIndex = 79;
            this.digit_box.TextChanged += new System.EventHandler(this.digit_box_TextChanged);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(571, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 80;
            this.label4.Text = "Last 4 Digits";
            // 
            // bank_box
            // 
            this.bank_box.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bank_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.bank_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bank_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bank_box.ForeColor = System.Drawing.Color.LightGray;
            this.bank_box.Location = new System.Drawing.Point(370, 9);
            this.bank_box.MaxLength = 30;
            this.bank_box.Name = "bank_box";
            this.bank_box.Size = new System.Drawing.Size(159, 22);
            this.bank_box.TabIndex = 77;
            this.bank_box.Text = "CIBC";
            this.bank_box.TextChanged += new System.EventHandler(this.bank_box_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(324, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 16);
            this.label2.TabIndex = 78;
            this.label2.Text = "Bank";
            // 
            // company_box
            // 
            this.company_box.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.company_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.company_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.company_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.company_box.ForeColor = System.Drawing.Color.LightGray;
            this.company_box.Location = new System.Drawing.Point(174, 37);
            this.company_box.MaxLength = 30;
            this.company_box.Name = "company_box";
            this.company_box.Size = new System.Drawing.Size(118, 22);
            this.company_box.TabIndex = 75;
            this.company_box.Text = "Mastercard";
            this.company_box.TextChanged += new System.EventHandler(this.company_box_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(102, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 16);
            this.label3.TabIndex = 76;
            this.label3.Text = "Company";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(128, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 74;
            this.label1.Text = "Type";
            // 
            // type_box
            // 
            this.type_box.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.type_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.type_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.type_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.type_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.type_box.ForeColor = System.Drawing.Color.White;
            this.type_box.FormattingEnabled = true;
            this.type_box.FrameColor = System.Drawing.Color.Lime;
            this.type_box.HighlightColor = System.Drawing.Color.Gray;
            this.type_box.Location = new System.Drawing.Point(174, 8);
            this.type_box.Name = "type_box";
            this.type_box.Size = new System.Drawing.Size(118, 23);
            this.type_box.TabIndex = 73;
            this.type_box.SelectedIndexChanged += new System.EventHandler(this.type_box_SelectedIndexChanged);
            // 
            // Payment_Information
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(800, 69);
            this.Controls.Add(this.bufferedPanel1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.button1);
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
            this.Name = "Payment_Information";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Personal Banker: Payment Options";
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
        public AdvancedComboBox type_box;
        private Label label1;
        private Label label3;
        public TextBox company_box;
        private Label label2;
        public TextBox bank_box;
        private Label label4;
        public TextBox digit_box;
        private Label label6;
        public TextBox limit_box;
        private Label label7;
        public AdvancedComboBox billing_date_box;
        private Button Add_button;
        private Label label8;
        public TextBox phone_box;
        private Label label9;
        private CheckBox check0;
        private CheckBox check1;
        private CheckBox check2;
        private Button button1;
        private Label label10;
        private CheckBox check3;
        private BufferedPanel bufferedPanel1;
        private Button search_desc_button;
        private Label label12;
        public TextBox textBox6;
        private Label label11;
    }
}


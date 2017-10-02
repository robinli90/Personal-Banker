using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Manage_Payment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manage_Payment));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.close_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.bufferedPanel5 = new Financial_Journal.BufferedPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.bufferedPanel4 = new Financial_Journal.BufferedPanel();
            this.next_page_button = new System.Windows.Forms.Button();
            this.back_page_button = new System.Windows.Forms.Button();
            this.bufferedPanel3 = new Financial_Journal.BufferedPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.payMemo = new System.Windows.Forms.TextBox();
            this.payAR = new Financial_Journal.AdvancedComboBox();
            this.dtpPay = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.payAMT = new System.Windows.Forms.TextBox();
            this.datePay = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.pay = new System.Windows.Forms.Button();
            this.bufferedPanel2 = new Financial_Journal.BufferedPanel();
            this.withdrawMemo = new System.Windows.Forms.TextBox();
            this.dtpWithdraw = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.withdrawAMT = new System.Windows.Forms.TextBox();
            this.dateWithdraw = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.withdraw = new System.Windows.Forms.Button();
            this.bufferedPanel1 = new Financial_Journal.BufferedPanel();
            this.label16 = new System.Windows.Forms.Label();
            this.depositMemo = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.depositAR = new Financial_Journal.AdvancedComboBox();
            this.depositAMT = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpDeposit = new System.Windows.Forms.DateTimePicker();
            this.dateDeposit = new System.Windows.Forms.Button();
            this.deposit = new System.Windows.Forms.Button();
            this.close5 = new System.Windows.Forms.Button();
            this.open5 = new System.Windows.Forms.Button();
            this.close4 = new System.Windows.Forms.Button();
            this.open4 = new System.Windows.Forms.Button();
            this.close3 = new System.Windows.Forms.Button();
            this.open3 = new System.Windows.Forms.Button();
            this.close2 = new System.Windows.Forms.Button();
            this.open2 = new System.Windows.Forms.Button();
            this.close1 = new System.Windows.Forms.Button();
            this.open1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bufferedPanel5.SuspendLayout();
            this.bufferedPanel4.SuspendLayout();
            this.bufferedPanel3.SuspendLayout();
            this.bufferedPanel2.SuspendLayout();
            this.bufferedPanel1.SuspendLayout();
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
            this.textBox1.Location = new System.Drawing.Point(380, -10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 2669);
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
            this.textBox3.Size = new System.Drawing.Size(2569, 12);
            this.textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 204);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(2569, 12);
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
            this.close_button.Location = new System.Drawing.Point(360, 1);
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
            this.label5.Size = new System.Drawing.Size(58, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Manage";
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
            this.textBox5.Size = new System.Drawing.Size(2665, 27);
            this.textBox5.TabIndex = 72;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.Control;
            this.label11.Location = new System.Drawing.Point(44, 42);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(103, 16);
            this.label11.TabIndex = 100;
            this.label11.Text = "Make a Deposit";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.SystemColors.Control;
            this.label12.Location = new System.Drawing.Point(44, 86);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(122, 16);
            this.label12.TabIndex = 103;
            this.label12.Text = "Make a Withdrawal";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.SystemColors.Control;
            this.label13.Location = new System.Drawing.Point(44, 130);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(109, 16);
            this.label13.TabIndex = 106;
            this.label13.Text = "Make a Payment";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.SystemColors.Control;
            this.label14.Location = new System.Drawing.Point(44, 172);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(82, 16);
            this.label14.TabIndex = 109;
            this.label14.Text = "View History";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.SystemColors.Control;
            this.label15.Location = new System.Drawing.Point(47, 213);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(138, 16);
            this.label15.TabIndex = 116;
            this.label15.Text = "View Payment Details";
            this.label15.Visible = false;
            // 
            // bufferedPanel5
            // 
            this.bufferedPanel5.Controls.Add(this.label6);
            this.bufferedPanel5.Location = new System.Drawing.Point(737, 240);
            this.bufferedPanel5.Name = "bufferedPanel5";
            this.bufferedPanel5.Size = new System.Drawing.Size(346, 78);
            this.bufferedPanel5.TabIndex = 118;
            this.bufferedPanel5.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(106, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(138, 16);
            this.label6.TabIndex = 110;
            this.label6.Text = "View Payment Details";
            // 
            // bufferedPanel4
            // 
            this.bufferedPanel4.Controls.Add(this.next_page_button);
            this.bufferedPanel4.Controls.Add(this.back_page_button);
            this.bufferedPanel4.Location = new System.Drawing.Point(385, 156);
            this.bufferedPanel4.Name = "bufferedPanel4";
            this.bufferedPanel4.Size = new System.Drawing.Size(346, 303);
            this.bufferedPanel4.TabIndex = 114;
            this.bufferedPanel4.Visible = false;
            // 
            // next_page_button
            // 
            this.next_page_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.next_page_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.next_page_button.FlatAppearance.BorderSize = 0;
            this.next_page_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.next_page_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next_page_button.ForeColor = System.Drawing.Color.White;
            this.next_page_button.Image = global::Financial_Journal.Properties.Resources.right_arrow;
            this.next_page_button.Location = new System.Drawing.Point(316, 273);
            this.next_page_button.Margin = new System.Windows.Forms.Padding(0);
            this.next_page_button.Name = "next_page_button";
            this.next_page_button.Size = new System.Drawing.Size(29, 29);
            this.next_page_button.TabIndex = 120;
            this.next_page_button.TabStop = false;
            this.next_page_button.UseVisualStyleBackColor = false;
            this.next_page_button.Visible = false;
            this.next_page_button.Click += new System.EventHandler(this.next_page_button_Click);
            // 
            // back_page_button
            // 
            this.back_page_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.back_page_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.back_page_button.FlatAppearance.BorderSize = 0;
            this.back_page_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.back_page_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back_page_button.ForeColor = System.Drawing.Color.White;
            this.back_page_button.Image = global::Financial_Journal.Properties.Resources.left_arrow;
            this.back_page_button.Location = new System.Drawing.Point(1, 273);
            this.back_page_button.Margin = new System.Windows.Forms.Padding(0);
            this.back_page_button.Name = "back_page_button";
            this.back_page_button.Size = new System.Drawing.Size(29, 29);
            this.back_page_button.TabIndex = 119;
            this.back_page_button.TabStop = false;
            this.back_page_button.UseVisualStyleBackColor = false;
            this.back_page_button.Visible = false;
            this.back_page_button.Click += new System.EventHandler(this.back_page_button_Click);
            // 
            // bufferedPanel3
            // 
            this.bufferedPanel3.Controls.Add(this.label10);
            this.bufferedPanel3.Controls.Add(this.label4);
            this.bufferedPanel3.Controls.Add(this.payMemo);
            this.bufferedPanel3.Controls.Add(this.payAR);
            this.bufferedPanel3.Controls.Add(this.dtpPay);
            this.bufferedPanel3.Controls.Add(this.label8);
            this.bufferedPanel3.Controls.Add(this.payAMT);
            this.bufferedPanel3.Controls.Add(this.datePay);
            this.bufferedPanel3.Controls.Add(this.label9);
            this.bufferedPanel3.Controls.Add(this.pay);
            this.bufferedPanel3.Location = new System.Drawing.Point(737, 121);
            this.bufferedPanel3.Name = "bufferedPanel3";
            this.bufferedPanel3.Size = new System.Drawing.Size(346, 113);
            this.bufferedPanel3.TabIndex = 113;
            this.bufferedPanel3.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.Control;
            this.label10.Location = new System.Drawing.Point(258, 84);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 16);
            this.label10.TabIndex = 121;
            this.label10.Text = "(optional)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(9, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 16);
            this.label4.TabIndex = 120;
            this.label4.Text = "To Payable";
            // 
            // payMemo
            // 
            this.payMemo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.payMemo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.payMemo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.payMemo.ForeColor = System.Drawing.Color.White;
            this.payMemo.Location = new System.Drawing.Point(68, 48);
            this.payMemo.MaxLength = 30;
            this.payMemo.Name = "payMemo";
            this.payMemo.Size = new System.Drawing.Size(218, 22);
            this.payMemo.TabIndex = 132;
            // 
            // payAR
            // 
            this.payAR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.payAR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.payAR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.payAR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.payAR.ForeColor = System.Drawing.Color.White;
            this.payAR.FormattingEnabled = true;
            this.payAR.FrameColor = System.Drawing.SystemColors.ControlLightLight;
            this.payAR.HighlightColor = System.Drawing.Color.Gray;
            this.payAR.Location = new System.Drawing.Point(94, 82);
            this.payAR.Name = "payAR";
            this.payAR.Size = new System.Drawing.Size(158, 21);
            this.payAR.TabIndex = 119;
            this.payAR.SelectedIndexChanged += new System.EventHandler(this.payment_type_SelectedIndexChanged);
            // 
            // dtpPay
            // 
            this.dtpPay.CalendarMonthBackground = System.Drawing.SystemColors.HotTrack;
            this.dtpPay.Enabled = false;
            this.dtpPay.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpPay.Location = new System.Drawing.Point(185, 17);
            this.dtpPay.Name = "dtpPay";
            this.dtpPay.Size = new System.Drawing.Size(99, 20);
            this.dtpPay.TabIndex = 120;
            this.dtpPay.Value = new System.DateTime(2016, 8, 13, 18, 48, 56, 0);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(16, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 16);
            this.label8.TabIndex = 133;
            this.label8.Text = "Memo";
            // 
            // payAMT
            // 
            this.payAMT.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.payAMT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.payAMT.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.payAMT.ForeColor = System.Drawing.Color.White;
            this.payAMT.Location = new System.Drawing.Point(68, 15);
            this.payAMT.MaxLength = 10;
            this.payAMT.Name = "payAMT";
            this.payAMT.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.payAMT.Size = new System.Drawing.Size(71, 22);
            this.payAMT.TabIndex = 131;
            this.payAMT.Text = "$";
            this.payAMT.TextChanged += new System.EventHandler(this.payAMT_TextChanged);
            // 
            // datePay
            // 
            this.datePay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.datePay.FlatAppearance.BorderSize = 0;
            this.datePay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.datePay.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.datePay.ForeColor = System.Drawing.Color.White;
            this.datePay.Image = global::Financial_Journal.Properties.Resources.add_calendar;
            this.datePay.Location = new System.Drawing.Point(153, 12);
            this.datePay.Margin = new System.Windows.Forms.Padding(0);
            this.datePay.Name = "datePay";
            this.datePay.Size = new System.Drawing.Size(29, 29);
            this.datePay.TabIndex = 119;
            this.datePay.TabStop = false;
            this.datePay.UseVisualStyleBackColor = false;
            this.datePay.Click += new System.EventHandler(this.datePay_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.Control;
            this.label9.Location = new System.Drawing.Point(10, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 16);
            this.label9.TabIndex = 130;
            this.label9.Text = "Amount";
            // 
            // pay
            // 
            this.pay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pay.Image = global::Financial_Journal.Properties.Resources.icon;
            this.pay.Location = new System.Drawing.Point(296, 22);
            this.pay.Margin = new System.Windows.Forms.Padding(1);
            this.pay.Name = "pay";
            this.pay.Size = new System.Drawing.Size(49, 44);
            this.pay.TabIndex = 121;
            this.pay.UseVisualStyleBackColor = true;
            this.pay.Click += new System.EventHandler(this.pay_Click);
            // 
            // bufferedPanel2
            // 
            this.bufferedPanel2.Controls.Add(this.withdrawMemo);
            this.bufferedPanel2.Controls.Add(this.dtpWithdraw);
            this.bufferedPanel2.Controls.Add(this.label2);
            this.bufferedPanel2.Controls.Add(this.withdrawAMT);
            this.bufferedPanel2.Controls.Add(this.dateWithdraw);
            this.bufferedPanel2.Controls.Add(this.label7);
            this.bufferedPanel2.Controls.Add(this.withdraw);
            this.bufferedPanel2.Location = new System.Drawing.Point(737, 37);
            this.bufferedPanel2.Name = "bufferedPanel2";
            this.bufferedPanel2.Size = new System.Drawing.Size(346, 78);
            this.bufferedPanel2.TabIndex = 112;
            this.bufferedPanel2.Visible = false;
            // 
            // withdrawMemo
            // 
            this.withdrawMemo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.withdrawMemo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.withdrawMemo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.withdrawMemo.ForeColor = System.Drawing.Color.White;
            this.withdrawMemo.Location = new System.Drawing.Point(67, 48);
            this.withdrawMemo.MaxLength = 30;
            this.withdrawMemo.Name = "withdrawMemo";
            this.withdrawMemo.Size = new System.Drawing.Size(218, 22);
            this.withdrawMemo.TabIndex = 128;
            // 
            // dtpWithdraw
            // 
            this.dtpWithdraw.CalendarMonthBackground = System.Drawing.SystemColors.HotTrack;
            this.dtpWithdraw.Enabled = false;
            this.dtpWithdraw.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpWithdraw.Location = new System.Drawing.Point(185, 17);
            this.dtpWithdraw.Name = "dtpWithdraw";
            this.dtpWithdraw.Size = new System.Drawing.Size(99, 20);
            this.dtpWithdraw.TabIndex = 120;
            this.dtpWithdraw.Value = new System.DateTime(2016, 8, 13, 18, 48, 56, 0);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(15, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 16);
            this.label2.TabIndex = 129;
            this.label2.Text = "Memo";
            // 
            // withdrawAMT
            // 
            this.withdrawAMT.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.withdrawAMT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.withdrawAMT.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.withdrawAMT.ForeColor = System.Drawing.Color.White;
            this.withdrawAMT.Location = new System.Drawing.Point(67, 15);
            this.withdrawAMT.MaxLength = 10;
            this.withdrawAMT.Name = "withdrawAMT";
            this.withdrawAMT.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.withdrawAMT.Size = new System.Drawing.Size(71, 22);
            this.withdrawAMT.TabIndex = 127;
            this.withdrawAMT.Text = "$";
            this.withdrawAMT.TextChanged += new System.EventHandler(this.withdrawAMT_TextChanged);
            // 
            // dateWithdraw
            // 
            this.dateWithdraw.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dateWithdraw.FlatAppearance.BorderSize = 0;
            this.dateWithdraw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dateWithdraw.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateWithdraw.ForeColor = System.Drawing.Color.White;
            this.dateWithdraw.Image = global::Financial_Journal.Properties.Resources.add_calendar;
            this.dateWithdraw.Location = new System.Drawing.Point(153, 13);
            this.dateWithdraw.Margin = new System.Windows.Forms.Padding(0);
            this.dateWithdraw.Name = "dateWithdraw";
            this.dateWithdraw.Size = new System.Drawing.Size(29, 29);
            this.dateWithdraw.TabIndex = 119;
            this.dateWithdraw.TabStop = false;
            this.dateWithdraw.UseVisualStyleBackColor = false;
            this.dateWithdraw.Click += new System.EventHandler(this.dateWithdraw_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.Control;
            this.label7.Location = new System.Drawing.Point(9, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 16);
            this.label7.TabIndex = 126;
            this.label7.Text = "Amount";
            // 
            // withdraw
            // 
            this.withdraw.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.withdraw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.withdraw.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.withdraw.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.withdraw.Image = global::Financial_Journal.Properties.Resources.supermarket_atm;
            this.withdraw.Location = new System.Drawing.Point(296, 22);
            this.withdraw.Margin = new System.Windows.Forms.Padding(1);
            this.withdraw.Name = "withdraw";
            this.withdraw.Size = new System.Drawing.Size(49, 44);
            this.withdraw.TabIndex = 120;
            this.withdraw.UseVisualStyleBackColor = true;
            this.withdraw.Click += new System.EventHandler(this.withdraw_Click);
            // 
            // bufferedPanel1
            // 
            this.bufferedPanel1.Controls.Add(this.label16);
            this.bufferedPanel1.Controls.Add(this.depositMemo);
            this.bufferedPanel1.Controls.Add(this.label17);
            this.bufferedPanel1.Controls.Add(this.label1);
            this.bufferedPanel1.Controls.Add(this.depositAR);
            this.bufferedPanel1.Controls.Add(this.depositAMT);
            this.bufferedPanel1.Controls.Add(this.label3);
            this.bufferedPanel1.Controls.Add(this.dtpDeposit);
            this.bufferedPanel1.Controls.Add(this.dateDeposit);
            this.bufferedPanel1.Controls.Add(this.deposit);
            this.bufferedPanel1.Location = new System.Drawing.Point(385, 37);
            this.bufferedPanel1.Name = "bufferedPanel1";
            this.bufferedPanel1.Size = new System.Drawing.Size(346, 113);
            this.bufferedPanel1.TabIndex = 111;
            this.bufferedPanel1.Visible = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.SystemColors.Control;
            this.label16.Location = new System.Drawing.Point(249, 86);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(64, 16);
            this.label16.TabIndex = 124;
            this.label16.Text = "(optional)";
            // 
            // depositMemo
            // 
            this.depositMemo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.depositMemo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.depositMemo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.depositMemo.ForeColor = System.Drawing.Color.White;
            this.depositMemo.Location = new System.Drawing.Point(66, 51);
            this.depositMemo.MaxLength = 30;
            this.depositMemo.Name = "depositMemo";
            this.depositMemo.Size = new System.Drawing.Size(218, 22);
            this.depositMemo.TabIndex = 124;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.SystemColors.Control;
            this.label17.Location = new System.Drawing.Point(9, 86);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(70, 16);
            this.label17.TabIndex = 123;
            this.label17.Text = "From Rec.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(14, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 16);
            this.label1.TabIndex = 125;
            this.label1.Text = "Memo";
            // 
            // depositAR
            // 
            this.depositAR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.depositAR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.depositAR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.depositAR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.depositAR.ForeColor = System.Drawing.Color.White;
            this.depositAR.FormattingEnabled = true;
            this.depositAR.FrameColor = System.Drawing.SystemColors.ControlLightLight;
            this.depositAR.HighlightColor = System.Drawing.Color.Gray;
            this.depositAR.Location = new System.Drawing.Point(85, 84);
            this.depositAR.Name = "depositAR";
            this.depositAR.Size = new System.Drawing.Size(158, 21);
            this.depositAR.TabIndex = 122;
            this.depositAR.SelectedIndexChanged += new System.EventHandler(this.depositAR_SelectedIndexChanged);
            // 
            // depositAMT
            // 
            this.depositAMT.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.depositAMT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.depositAMT.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.depositAMT.ForeColor = System.Drawing.Color.White;
            this.depositAMT.Location = new System.Drawing.Point(66, 18);
            this.depositAMT.MaxLength = 10;
            this.depositAMT.Name = "depositAMT";
            this.depositAMT.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.depositAMT.Size = new System.Drawing.Size(71, 22);
            this.depositAMT.TabIndex = 123;
            this.depositAMT.Text = "$";
            this.depositAMT.TextChanged += new System.EventHandler(this.depositAMT_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(8, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 16);
            this.label3.TabIndex = 122;
            this.label3.Text = "Amount";
            // 
            // dtpDeposit
            // 
            this.dtpDeposit.CalendarMonthBackground = System.Drawing.SystemColors.HotTrack;
            this.dtpDeposit.Enabled = false;
            this.dtpDeposit.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDeposit.Location = new System.Drawing.Point(185, 20);
            this.dtpDeposit.Name = "dtpDeposit";
            this.dtpDeposit.Size = new System.Drawing.Size(99, 20);
            this.dtpDeposit.TabIndex = 120;
            this.dtpDeposit.Value = new System.DateTime(2016, 8, 13, 18, 48, 56, 0);
            // 
            // dateDeposit
            // 
            this.dateDeposit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dateDeposit.FlatAppearance.BorderSize = 0;
            this.dateDeposit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dateDeposit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateDeposit.ForeColor = System.Drawing.Color.White;
            this.dateDeposit.Image = global::Financial_Journal.Properties.Resources.add_calendar;
            this.dateDeposit.Location = new System.Drawing.Point(154, 15);
            this.dateDeposit.Margin = new System.Windows.Forms.Padding(0);
            this.dateDeposit.Name = "dateDeposit";
            this.dateDeposit.Size = new System.Drawing.Size(29, 29);
            this.dateDeposit.TabIndex = 119;
            this.dateDeposit.TabStop = false;
            this.dateDeposit.UseVisualStyleBackColor = false;
            this.dateDeposit.Click += new System.EventHandler(this.dateDeposit_Click);
            // 
            // deposit
            // 
            this.deposit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.deposit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deposit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deposit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.deposit.Image = global::Financial_Journal.Properties.Resources.piggy_bank;
            this.deposit.Location = new System.Drawing.Point(296, 22);
            this.deposit.Margin = new System.Windows.Forms.Padding(1);
            this.deposit.Name = "deposit";
            this.deposit.Size = new System.Drawing.Size(49, 44);
            this.deposit.TabIndex = 119;
            this.deposit.UseVisualStyleBackColor = true;
            this.deposit.Click += new System.EventHandler(this.deposit_Click);
            // 
            // close5
            // 
            this.close5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.close5.FlatAppearance.BorderSize = 0;
            this.close5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close5.ForeColor = System.Drawing.Color.White;
            this.close5.Image = global::Financial_Journal.Properties.Resources.error;
            this.close5.Location = new System.Drawing.Point(21, 210);
            this.close5.Margin = new System.Windows.Forms.Padding(0);
            this.close5.Name = "close5";
            this.close5.Size = new System.Drawing.Size(23, 22);
            this.close5.TabIndex = 117;
            this.close5.TabStop = false;
            this.close5.UseVisualStyleBackColor = false;
            this.close5.Visible = false;
            this.close5.Click += new System.EventHandler(this.close5_Click);
            // 
            // open5
            // 
            this.open5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.open5.FlatAppearance.BorderSize = 0;
            this.open5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open5.ForeColor = System.Drawing.Color.White;
            this.open5.Image = global::Financial_Journal.Properties.Resources.add;
            this.open5.Location = new System.Drawing.Point(21, 208);
            this.open5.Margin = new System.Windows.Forms.Padding(0);
            this.open5.Name = "open5";
            this.open5.Size = new System.Drawing.Size(23, 24);
            this.open5.TabIndex = 115;
            this.open5.TabStop = false;
            this.open5.UseVisualStyleBackColor = false;
            this.open5.Click += new System.EventHandler(this.open5_Click);
            // 
            // close4
            // 
            this.close4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.close4.FlatAppearance.BorderSize = 0;
            this.close4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close4.ForeColor = System.Drawing.Color.White;
            this.close4.Image = global::Financial_Journal.Properties.Resources.error;
            this.close4.Location = new System.Drawing.Point(21, 167);
            this.close4.Margin = new System.Windows.Forms.Padding(0);
            this.close4.Name = "close4";
            this.close4.Size = new System.Drawing.Size(23, 22);
            this.close4.TabIndex = 110;
            this.close4.TabStop = false;
            this.close4.UseVisualStyleBackColor = false;
            this.close4.Visible = false;
            this.close4.Click += new System.EventHandler(this.close4_Click);
            // 
            // open4
            // 
            this.open4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.open4.FlatAppearance.BorderSize = 0;
            this.open4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open4.ForeColor = System.Drawing.Color.White;
            this.open4.Image = global::Financial_Journal.Properties.Resources.add;
            this.open4.Location = new System.Drawing.Point(21, 167);
            this.open4.Margin = new System.Windows.Forms.Padding(0);
            this.open4.Name = "open4";
            this.open4.Size = new System.Drawing.Size(23, 24);
            this.open4.TabIndex = 108;
            this.open4.TabStop = false;
            this.open4.UseVisualStyleBackColor = false;
            this.open4.Click += new System.EventHandler(this.open4_Click);
            // 
            // close3
            // 
            this.close3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.close3.FlatAppearance.BorderSize = 0;
            this.close3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close3.ForeColor = System.Drawing.Color.White;
            this.close3.Image = global::Financial_Journal.Properties.Resources.error;
            this.close3.Location = new System.Drawing.Point(21, 125);
            this.close3.Margin = new System.Windows.Forms.Padding(0);
            this.close3.Name = "close3";
            this.close3.Size = new System.Drawing.Size(23, 22);
            this.close3.TabIndex = 107;
            this.close3.TabStop = false;
            this.close3.UseVisualStyleBackColor = false;
            this.close3.Visible = false;
            this.close3.Click += new System.EventHandler(this.close3_Click);
            // 
            // open3
            // 
            this.open3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.open3.FlatAppearance.BorderSize = 0;
            this.open3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open3.ForeColor = System.Drawing.Color.White;
            this.open3.Image = global::Financial_Journal.Properties.Resources.add;
            this.open3.Location = new System.Drawing.Point(21, 125);
            this.open3.Margin = new System.Windows.Forms.Padding(0);
            this.open3.Name = "open3";
            this.open3.Size = new System.Drawing.Size(23, 24);
            this.open3.TabIndex = 105;
            this.open3.TabStop = false;
            this.open3.UseVisualStyleBackColor = false;
            this.open3.Click += new System.EventHandler(this.open3_Click);
            // 
            // close2
            // 
            this.close2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.close2.FlatAppearance.BorderSize = 0;
            this.close2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close2.ForeColor = System.Drawing.Color.White;
            this.close2.Image = global::Financial_Journal.Properties.Resources.error;
            this.close2.Location = new System.Drawing.Point(21, 81);
            this.close2.Margin = new System.Windows.Forms.Padding(0);
            this.close2.Name = "close2";
            this.close2.Size = new System.Drawing.Size(23, 22);
            this.close2.TabIndex = 104;
            this.close2.TabStop = false;
            this.close2.UseVisualStyleBackColor = false;
            this.close2.Visible = false;
            this.close2.Click += new System.EventHandler(this.close2_Click);
            // 
            // open2
            // 
            this.open2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.open2.FlatAppearance.BorderSize = 0;
            this.open2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open2.ForeColor = System.Drawing.Color.White;
            this.open2.Image = global::Financial_Journal.Properties.Resources.add;
            this.open2.Location = new System.Drawing.Point(21, 81);
            this.open2.Margin = new System.Windows.Forms.Padding(0);
            this.open2.Name = "open2";
            this.open2.Size = new System.Drawing.Size(23, 24);
            this.open2.TabIndex = 102;
            this.open2.TabStop = false;
            this.open2.UseVisualStyleBackColor = false;
            this.open2.Click += new System.EventHandler(this.open2_Click);
            // 
            // close1
            // 
            this.close1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.close1.FlatAppearance.BorderSize = 0;
            this.close1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close1.ForeColor = System.Drawing.Color.White;
            this.close1.Image = global::Financial_Journal.Properties.Resources.error;
            this.close1.Location = new System.Drawing.Point(21, 37);
            this.close1.Margin = new System.Windows.Forms.Padding(0);
            this.close1.Name = "close1";
            this.close1.Size = new System.Drawing.Size(23, 22);
            this.close1.TabIndex = 101;
            this.close1.TabStop = false;
            this.close1.UseVisualStyleBackColor = false;
            this.close1.Visible = false;
            this.close1.Click += new System.EventHandler(this.close1_Click);
            // 
            // open1
            // 
            this.open1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.open1.FlatAppearance.BorderSize = 0;
            this.open1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open1.ForeColor = System.Drawing.Color.White;
            this.open1.Image = global::Financial_Journal.Properties.Resources.add;
            this.open1.Location = new System.Drawing.Point(21, 37);
            this.open1.Margin = new System.Windows.Forms.Padding(0);
            this.open1.Name = "open1";
            this.open1.Size = new System.Drawing.Size(23, 24);
            this.open1.TabIndex = 99;
            this.open1.TabStop = false;
            this.open1.UseVisualStyleBackColor = false;
            this.open1.Click += new System.EventHandler(this.open1_Click);
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
            // Manage_Payment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(381, 205);
            this.Controls.Add(this.bufferedPanel5);
            this.Controls.Add(this.close5);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.open5);
            this.Controls.Add(this.bufferedPanel4);
            this.Controls.Add(this.bufferedPanel3);
            this.Controls.Add(this.bufferedPanel2);
            this.Controls.Add(this.bufferedPanel1);
            this.Controls.Add(this.close4);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.open4);
            this.Controls.Add(this.close3);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.open3);
            this.Controls.Add(this.close2);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.open2);
            this.Controls.Add(this.close1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.open1);
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
            this.Name = "Manage_Payment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Personal Banker: Manage Payment";
            this.Load += new System.EventHandler(this.Receipt_Load);
            this.bufferedPanel5.ResumeLayout(false);
            this.bufferedPanel5.PerformLayout();
            this.bufferedPanel4.ResumeLayout(false);
            this.bufferedPanel3.ResumeLayout(false);
            this.bufferedPanel3.PerformLayout();
            this.bufferedPanel2.ResumeLayout(false);
            this.bufferedPanel2.PerformLayout();
            this.bufferedPanel1.ResumeLayout(false);
            this.bufferedPanel1.PerformLayout();
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
        private Label label11;
        private Button open1;
        private Button close1;
        private Button close2;
        private Label label12;
        private Button open2;
        private Button close3;
        private Label label13;
        private Button open3;
        private Button close4;
        private Label label14;
        private Button open4;
        private BufferedPanel bufferedPanel1;
        private BufferedPanel bufferedPanel2;
        private BufferedPanel bufferedPanel3;
        private BufferedPanel bufferedPanel4;
        private Button close5;
        private Label label15;
        private Button open5;
        private BufferedPanel bufferedPanel5;
        private Label label6;
        private Button deposit;
        private Button withdraw;
        private Button pay;
        private DateTimePicker dtpDeposit;
        private Button dateDeposit;
        private DateTimePicker dtpWithdraw;
        private Button dateWithdraw;
        private DateTimePicker dtpPay;
        private Button datePay;
        private Label label3;
        private TextBox depositAMT;
        public TextBox depositMemo;
        private Label label1;
        public TextBox withdrawMemo;
        private Label label2;
        private TextBox withdrawAMT;
        private Label label7;
        public TextBox payMemo;
        private Label label8;
        private TextBox payAMT;
        private Label label9;
        private Button next_page_button;
        private Button back_page_button;
        private Label label10;
        private Label label4;
        public AdvancedComboBox payAR;
        private Label label16;
        private Label label17;
        public AdvancedComboBox depositAR;
    }
}


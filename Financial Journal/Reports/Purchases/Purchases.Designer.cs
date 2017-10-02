using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Purchases
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Purchases));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.close_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.Add_button = new System.Windows.Forms.Button();
            this.next_page_button = new System.Windows.Forms.Button();
            this.back_page_button = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bufferedPanel2 = new Financial_Journal.BufferedPanel();
            this.periodbox2 = new Financial_Journal.AdvancedComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bufferedPanel1 = new Financial_Journal.BufferedPanel();
            this.view_by_category = new Financial_Journal.AdvancedComboBox();
            this.payment_type = new Financial_Journal.AdvancedComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.view_type = new Financial_Journal.AdvancedComboBox();
            this.year_box = new Financial_Journal.AdvancedComboBox();
            this.view_by_location = new Financial_Journal.AdvancedComboBox();
            this.CustomDTP1 = new Financial_Journal.CustomDTP();
            this.view_type_date_select = new Financial_Journal.AdvancedComboBox();
            this.excel_button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.bufferedPanel2.SuspendLayout();
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
            this.textBox1.Location = new System.Drawing.Point(767, -10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 2741);
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
            this.textBox3.Size = new System.Drawing.Size(2956, 12);
            this.textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 276);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(2956, 12);
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
            this.close_button.Location = new System.Drawing.Point(747, 1);
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
            this.label5.Size = new System.Drawing.Size(72, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Purchases";
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
            this.textBox5.Size = new System.Drawing.Size(3052, 27);
            this.textBox5.TabIndex = 72;
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage_1);
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
            // 
            // Add_button
            // 
            this.Add_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Add_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Add_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Add_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Add_button.Image = global::Financial_Journal.Properties.Resources.printer__1_;
            this.Add_button.Location = new System.Drawing.Point(726, 29);
            this.Add_button.Margin = new System.Windows.Forms.Padding(1);
            this.Add_button.Name = "Add_button";
            this.Add_button.Size = new System.Drawing.Size(35, 39);
            this.Add_button.TabIndex = 104;
            this.Add_button.UseVisualStyleBackColor = true;
            this.Add_button.Click += new System.EventHandler(this.Add_button_Click);
            // 
            // next_page_button
            // 
            this.next_page_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.next_page_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.next_page_button.FlatAppearance.BorderSize = 0;
            this.next_page_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.next_page_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next_page_button.ForeColor = System.Drawing.Color.White;
            this.next_page_button.Image = ((System.Drawing.Image)(resources.GetObject("next_page_button.Image")));
            this.next_page_button.Location = new System.Drawing.Point(733, 243);
            this.next_page_button.Margin = new System.Windows.Forms.Padding(0);
            this.next_page_button.Name = "next_page_button";
            this.next_page_button.Size = new System.Drawing.Size(30, 30);
            this.next_page_button.TabIndex = 105;
            this.next_page_button.TabStop = false;
            this.next_page_button.UseVisualStyleBackColor = false;
            this.next_page_button.Visible = false;
            this.next_page_button.Click += new System.EventHandler(this.next_page_button_Click);
            // 
            // back_page_button
            // 
            this.back_page_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.back_page_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.back_page_button.FlatAppearance.BorderSize = 0;
            this.back_page_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.back_page_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back_page_button.ForeColor = System.Drawing.Color.White;
            this.back_page_button.Image = ((System.Drawing.Image)(resources.GetObject("back_page_button.Image")));
            this.back_page_button.Location = new System.Drawing.Point(3, 243);
            this.back_page_button.Margin = new System.Windows.Forms.Padding(0);
            this.back_page_button.Name = "back_page_button";
            this.back_page_button.Size = new System.Drawing.Size(30, 30);
            this.back_page_button.TabIndex = 104;
            this.back_page_button.TabStop = false;
            this.back_page_button.UseVisualStyleBackColor = false;
            this.back_page_button.Visible = false;
            this.back_page_button.Click += new System.EventHandler(this.back_page_button_Click);
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
            // bufferedPanel2
            // 
            this.bufferedPanel2.Controls.Add(this.periodbox2);
            this.bufferedPanel2.Controls.Add(this.label2);
            this.bufferedPanel2.Location = new System.Drawing.Point(10, 34);
            this.bufferedPanel2.Name = "bufferedPanel2";
            this.bufferedPanel2.Size = new System.Drawing.Size(425, 29);
            this.bufferedPanel2.TabIndex = 108;
            this.bufferedPanel2.Visible = false;
            // 
            // periodbox2
            // 
            this.periodbox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.periodbox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.periodbox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.periodbox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.periodbox2.ForeColor = System.Drawing.Color.White;
            this.periodbox2.FormattingEnabled = true;
            this.periodbox2.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.periodbox2.HighlightColor = System.Drawing.Color.Gray;
            this.periodbox2.Location = new System.Drawing.Point(119, 3);
            this.periodbox2.Name = "periodbox2";
            this.periodbox2.Size = new System.Drawing.Size(303, 23);
            this.periodbox2.TabIndex = 106;
            this.periodbox2.Visible = false;
            this.periodbox2.SelectedIndexChanged += new System.EventHandler(this.periodbox2_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 16);
            this.label2.TabIndex = 107;
            this.label2.Text = "Statement Period:";
            this.label2.Visible = false;
            // 
            // bufferedPanel1
            // 
            this.bufferedPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bufferedPanel1.Controls.Add(this.view_by_category);
            this.bufferedPanel1.Controls.Add(this.payment_type);
            this.bufferedPanel1.Controls.Add(this.label6);
            this.bufferedPanel1.Controls.Add(this.label1);
            this.bufferedPanel1.Controls.Add(this.view_type);
            this.bufferedPanel1.Controls.Add(this.year_box);
            this.bufferedPanel1.Controls.Add(this.view_by_location);
            this.bufferedPanel1.Controls.Add(this.CustomDTP1);
            this.bufferedPanel1.Controls.Add(this.view_type_date_select);
            this.bufferedPanel1.Location = new System.Drawing.Point(3, 31);
            this.bufferedPanel1.Name = "bufferedPanel1";
            this.bufferedPanel1.Size = new System.Drawing.Size(761, 74);
            this.bufferedPanel1.TabIndex = 105;
            this.bufferedPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.bufferedPanel1_Paint);
            // 
            // view_by_category
            // 
            this.view_by_category.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.view_by_category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.view_by_category.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.view_by_category.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.view_by_category.ForeColor = System.Drawing.Color.White;
            this.view_by_category.FormattingEnabled = true;
            this.view_by_category.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.view_by_category.HighlightColor = System.Drawing.Color.Gray;
            this.view_by_category.Location = new System.Drawing.Point(257, 12);
            this.view_by_category.Name = "view_by_category";
            this.view_by_category.Size = new System.Drawing.Size(153, 23);
            this.view_by_category.TabIndex = 106;
            this.view_by_category.Visible = false;
            this.view_by_category.SelectedIndexChanged += new System.EventHandler(this.view_by_category_SelectedIndexChanged);
            // 
            // payment_type
            // 
            this.payment_type.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.payment_type.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.payment_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.payment_type.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.payment_type.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.payment_type.ForeColor = System.Drawing.Color.White;
            this.payment_type.FormattingEnabled = true;
            this.payment_type.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.payment_type.HighlightColor = System.Drawing.Color.Gray;
            this.payment_type.Location = new System.Drawing.Point(143, 41);
            this.payment_type.Name = "payment_type";
            this.payment_type.Size = new System.Drawing.Size(157, 23);
            this.payment_type.TabIndex = 3;
            this.payment_type.SelectedIndexChanged += new System.EventHandler(this.payment_type_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(4, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 16);
            this.label6.TabIndex = 98;
            this.label6.Text = "View Purchases from:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(41, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 16);
            this.label1.TabIndex = 103;
            this.label1.Text = "Payment Type:";
            // 
            // view_type
            // 
            this.view_type.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.view_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.view_type.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.view_type.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.view_type.ForeColor = System.Drawing.Color.White;
            this.view_type.FormattingEnabled = true;
            this.view_type.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.view_type.HighlightColor = System.Drawing.Color.Gray;
            this.view_type.Location = new System.Drawing.Point(143, 12);
            this.view_type.Name = "view_type";
            this.view_type.Size = new System.Drawing.Size(108, 23);
            this.view_type.TabIndex = 0;
            this.view_type.SelectedIndexChanged += new System.EventHandler(this.view_type_SelectedIndexChanged);
            // 
            // year_box
            // 
            this.year_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.year_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.year_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.year_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.year_box.ForeColor = System.Drawing.Color.White;
            this.year_box.FormattingEnabled = true;
            this.year_box.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.year_box.HighlightColor = System.Drawing.Color.Gray;
            this.year_box.Location = new System.Drawing.Point(416, 12);
            this.year_box.Name = "year_box";
            this.year_box.Size = new System.Drawing.Size(87, 23);
            this.year_box.TabIndex = 2;
            this.year_box.Visible = false;
            this.year_box.SelectedIndexChanged += new System.EventHandler(this.year_box_SelectedIndexChanged);
            // 
            // view_by_location
            // 
            this.view_by_location.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.view_by_location.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.view_by_location.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.view_by_location.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.view_by_location.ForeColor = System.Drawing.Color.White;
            this.view_by_location.FormattingEnabled = true;
            this.view_by_location.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.view_by_location.HighlightColor = System.Drawing.Color.Gray;
            this.view_by_location.Location = new System.Drawing.Point(257, 12);
            this.view_by_location.Name = "view_by_location";
            this.view_by_location.Size = new System.Drawing.Size(153, 23);
            this.view_by_location.TabIndex = 1;
            this.view_by_location.Visible = false;
            this.view_by_location.SelectedIndexChanged += new System.EventHandler(this.view_by_location_SelectedIndexChanged);
            // 
            // CustomDTP1
            // 
            this.CustomDTP1.BackColor_DTP = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CustomDTP1.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CustomDTP1.CalendarForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CustomDTP1.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CustomDTP1.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CustomDTP1.CalendarTitleForeColor = System.Drawing.Color.White;
            this.CustomDTP1.CustomFormat = "MM/yyyy";
            this.CustomDTP1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.CustomDTP1.Location = new System.Drawing.Point(416, 13);
            this.CustomDTP1.Name = "CustomDTP1";
            this.CustomDTP1.Size = new System.Drawing.Size(82, 20);
            this.CustomDTP1.TabIndex = 101;
            this.CustomDTP1.Value = new System.DateTime(2016, 8, 1, 0, 0, 0, 0);
            this.CustomDTP1.Visible = false;
            this.CustomDTP1.ValueChanged += new System.EventHandler(this.CustomDTP1_ValueChanged);
            // 
            // view_type_date_select
            // 
            this.view_type_date_select.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.view_type_date_select.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.view_type_date_select.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.view_type_date_select.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.view_type_date_select.ForeColor = System.Drawing.Color.White;
            this.view_type_date_select.FormattingEnabled = true;
            this.view_type_date_select.FrameColor = System.Drawing.SystemColors.HotTrack;
            this.view_type_date_select.HighlightColor = System.Drawing.Color.Gray;
            this.view_type_date_select.Location = new System.Drawing.Point(257, 12);
            this.view_type_date_select.Name = "view_type_date_select";
            this.view_type_date_select.Size = new System.Drawing.Size(153, 23);
            this.view_type_date_select.TabIndex = 100;
            this.view_type_date_select.Visible = false;
            this.view_type_date_select.SelectedIndexChanged += new System.EventHandler(this.view_type_date_select_SelectedIndexChanged);
            // 
            // excel_button
            // 
            this.excel_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.excel_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.excel_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.excel_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.excel_button.Image = global::Financial_Journal.Properties.Resources.excel_ico;
            this.excel_button.Location = new System.Drawing.Point(683, 29);
            this.excel_button.Margin = new System.Windows.Forms.Padding(1);
            this.excel_button.Name = "excel_button";
            this.excel_button.Size = new System.Drawing.Size(35, 39);
            this.excel_button.TabIndex = 106;
            this.excel_button.UseVisualStyleBackColor = true;
            this.excel_button.Click += new System.EventHandler(this.excel_button_Click);
            // 
            // Purchases
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(768, 277);
            this.Controls.Add(this.excel_button);
            this.Controls.Add(this.Add_button);
            this.Controls.Add(this.bufferedPanel2);
            this.Controls.Add(this.next_page_button);
            this.Controls.Add(this.back_page_button);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.close_button);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.bufferedPanel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Purchases";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Personal Banker: Purchases";
            this.Load += new System.EventHandler(this.Receipt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.bufferedPanel2.ResumeLayout(false);
            this.bufferedPanel2.PerformLayout();
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
        private Button next_page_button;
        private Button back_page_button;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private PrintPreviewDialog printPreviewDialog1;
        private Button Add_button;
        public AdvancedComboBox view_type_date_select;
        private CustomDTP CustomDTP1;
        public AdvancedComboBox view_by_location;
        public AdvancedComboBox year_box;
        public AdvancedComboBox view_type;
        private Label label1;
        private Label label6;
        public AdvancedComboBox payment_type;
        private BufferedPanel bufferedPanel1;
        public AdvancedComboBox view_by_category;
        private Button excel_button;
        private Label label2;
        public AdvancedComboBox periodbox2;
        private BufferedPanel bufferedPanel2;
    }
}


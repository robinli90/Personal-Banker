using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Category_Summary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Category_Summary));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.minimize_button = new System.Windows.Forms.Button();
            this.close_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.sort_item_price_button = new System.Windows.Forms.Button();
            this.sort_item_date_button = new System.Windows.Forms.Button();
            this.sort_item_location_button = new System.Windows.Forms.Button();
            this.sort_item_name_button = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.next_page_button = new System.Windows.Forms.Button();
            this.back_page_button = new System.Windows.Forms.Button();
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
            this.textBox1.Location = new System.Drawing.Point(693, -10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 2640);
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
            this.textBox3.Size = new System.Drawing.Size(2882, 12);
            this.textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 175);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(2882, 12);
            this.textBox4.TabIndex = 3;
            // 
            // minimize_button
            // 
            this.minimize_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimize_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.minimize_button.FlatAppearance.BorderSize = 0;
            this.minimize_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimize_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.minimize_button.ForeColor = System.Drawing.Color.White;
            this.minimize_button.Location = new System.Drawing.Point(660, -5);
            this.minimize_button.Margin = new System.Windows.Forms.Padding(0);
            this.minimize_button.Name = "minimize_button";
            this.minimize_button.Size = new System.Drawing.Size(12, 22);
            this.minimize_button.TabIndex = 69;
            this.minimize_button.Text = "_";
            this.minimize_button.UseVisualStyleBackColor = false;
            this.minimize_button.Click += new System.EventHandler(this.minimize_button_Click);
            // 
            // close_button
            // 
            this.close_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.close_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.close_button.FlatAppearance.BorderSize = 0;
            this.close_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.close_button.ForeColor = System.Drawing.Color.White;
            this.close_button.Location = new System.Drawing.Point(673, 1);
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
            this.label5.Size = new System.Drawing.Size(104, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Template_Form";
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
            this.textBox5.Size = new System.Drawing.Size(2978, 27);
            this.textBox5.TabIndex = 72;
            // 
            // sort_item_price_button
            // 
            this.sort_item_price_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sort_item_price_button.FlatAppearance.BorderSize = 0;
            this.sort_item_price_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sort_item_price_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sort_item_price_button.ForeColor = System.Drawing.Color.White;
            this.sort_item_price_button.Image = ((System.Drawing.Image)(resources.GetObject("sort_item_price_button.Image")));
            this.sort_item_price_button.Location = new System.Drawing.Point(520, 26);
            this.sort_item_price_button.Margin = new System.Windows.Forms.Padding(0);
            this.sort_item_price_button.Name = "sort_item_price_button";
            this.sort_item_price_button.Size = new System.Drawing.Size(23, 24);
            this.sort_item_price_button.TabIndex = 81;
            this.sort_item_price_button.TabStop = false;
            this.sort_item_price_button.UseVisualStyleBackColor = false;
            this.sort_item_price_button.Click += new System.EventHandler(this.sort_item_price_button_Click);
            // 
            // sort_item_date_button
            // 
            this.sort_item_date_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sort_item_date_button.FlatAppearance.BorderSize = 0;
            this.sort_item_date_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sort_item_date_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sort_item_date_button.ForeColor = System.Drawing.Color.White;
            this.sort_item_date_button.Image = ((System.Drawing.Image)(resources.GetObject("sort_item_date_button.Image")));
            this.sort_item_date_button.Location = new System.Drawing.Point(382, 26);
            this.sort_item_date_button.Margin = new System.Windows.Forms.Padding(0);
            this.sort_item_date_button.Name = "sort_item_date_button";
            this.sort_item_date_button.Size = new System.Drawing.Size(23, 24);
            this.sort_item_date_button.TabIndex = 80;
            this.sort_item_date_button.TabStop = false;
            this.sort_item_date_button.UseVisualStyleBackColor = false;
            this.sort_item_date_button.Click += new System.EventHandler(this.sort_item_date_button_Click);
            // 
            // sort_item_location_button
            // 
            this.sort_item_location_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sort_item_location_button.FlatAppearance.BorderSize = 0;
            this.sort_item_location_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sort_item_location_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sort_item_location_button.ForeColor = System.Drawing.Color.White;
            this.sort_item_location_button.Image = ((System.Drawing.Image)(resources.GetObject("sort_item_location_button.Image")));
            this.sort_item_location_button.Location = new System.Drawing.Point(232, 26);
            this.sort_item_location_button.Margin = new System.Windows.Forms.Padding(0);
            this.sort_item_location_button.Name = "sort_item_location_button";
            this.sort_item_location_button.Size = new System.Drawing.Size(23, 24);
            this.sort_item_location_button.TabIndex = 79;
            this.sort_item_location_button.TabStop = false;
            this.sort_item_location_button.UseVisualStyleBackColor = false;
            this.sort_item_location_button.Click += new System.EventHandler(this.sort_item_location_button_Click);
            // 
            // sort_item_name_button
            // 
            this.sort_item_name_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sort_item_name_button.FlatAppearance.BorderSize = 0;
            this.sort_item_name_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sort_item_name_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sort_item_name_button.ForeColor = System.Drawing.Color.White;
            this.sort_item_name_button.Image = ((System.Drawing.Image)(resources.GetObject("sort_item_name_button.Image")));
            this.sort_item_name_button.Location = new System.Drawing.Point(7, 26);
            this.sort_item_name_button.Margin = new System.Windows.Forms.Padding(0);
            this.sort_item_name_button.Name = "sort_item_name_button";
            this.sort_item_name_button.Size = new System.Drawing.Size(23, 24);
            this.sort_item_name_button.TabIndex = 78;
            this.sort_item_name_button.TabStop = false;
            this.sort_item_name_button.UseVisualStyleBackColor = false;
            this.sort_item_name_button.Click += new System.EventHandler(this.sort_item_name_button_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.pictureBox1.BackgroundImage = global::System.Drawing.PieChart.Properties.Resources.Icons8_Ios7_Finance_Money_Box;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(4, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 22);
            this.pictureBox1.TabIndex = 77;
            this.pictureBox1.TabStop = false;
            // 
            // next_page_button
            // 
            this.next_page_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.next_page_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.next_page_button.FlatAppearance.BorderSize = 0;
            this.next_page_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.next_page_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next_page_button.ForeColor = System.Drawing.Color.White;
            this.next_page_button.Image = ((System.Drawing.Image)(resources.GetObject("next_page_button.Image")));
            this.next_page_button.Location = new System.Drawing.Point(656, 137);
            this.next_page_button.Margin = new System.Windows.Forms.Padding(0);
            this.next_page_button.Name = "next_page_button";
            this.next_page_button.Size = new System.Drawing.Size(30, 30);
            this.next_page_button.TabIndex = 83;
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
            this.back_page_button.Image = ((System.Drawing.Image)(resources.GetObject("back_page_button.Image")));
            this.back_page_button.Location = new System.Drawing.Point(8, 137);
            this.back_page_button.Margin = new System.Windows.Forms.Padding(0);
            this.back_page_button.Name = "back_page_button";
            this.back_page_button.Size = new System.Drawing.Size(30, 30);
            this.back_page_button.TabIndex = 82;
            this.back_page_button.TabStop = false;
            this.back_page_button.UseVisualStyleBackColor = false;
            this.back_page_button.Visible = false;
            this.back_page_button.Click += new System.EventHandler(this.back_page_button_Click);
            // 
            // Form_Template
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(694, 176);
            this.Controls.Add(this.next_page_button);
            this.Controls.Add(this.back_page_button);
            this.Controls.Add(this.sort_item_price_button);
            this.Controls.Add(this.sort_item_date_button);
            this.Controls.Add(this.sort_item_location_button);
            this.Controls.Add(this.sort_item_name_button);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.close_button);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.minimize_button);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_Template";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
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
        private Button minimize_button;
        private Button close_button;
        private Label label5;
        private TextBox textBox5;
        private PictureBox pictureBox1;
        private Button sort_item_name_button;
        private Button sort_item_location_button;
        private Button sort_item_date_button;
        private Button sort_item_price_button;
        private Button next_page_button;
        private Button back_page_button;
    }
}


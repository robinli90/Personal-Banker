using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Calendar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Calendar));
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.close_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.weatherbutton = new System.Windows.Forms.Button();
            this.import = new System.Windows.Forms.Button();
            this.export = new System.Windows.Forms.Button();
            this.undo_import = new System.Windows.Forms.Button();
            this.email_button = new System.Windows.Forms.Button();
            this.screenshot_button = new System.Windows.Forms.Button();
            this.edit_calendar = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.settings_button = new System.Windows.Forms.Button();
            this.next_page_button = new System.Windows.Forms.Button();
            this.back_page_button = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
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
            this.textBox3.Size = new System.Drawing.Size(5382, 12);
            this.textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 675);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(3382, 12);
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
            this.close_button.Location = new System.Drawing.Point(1173, 1);
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
            this.label5.Size = new System.Drawing.Size(120, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Personal Calendar";
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
            this.textBox5.Size = new System.Drawing.Size(3478, 27);
            this.textBox5.TabIndex = 72;
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label15.Location = new System.Drawing.Point(936, 658);
            this.label15.Name = "label15";
            this.label15.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label15.Size = new System.Drawing.Size(256, 12);
            this.label15.TabIndex = 176;
            this.label15.Text = "Reminder is right here";
            this.label15.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(1193, -980);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 2954);
            this.textBox1.TabIndex = 178;
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // weatherbutton
            // 
            this.weatherbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.weatherbutton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.weatherbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.weatherbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.weatherbutton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.weatherbutton.Image = global::Financial_Journal.Properties.Resources.rain;
            this.weatherbutton.Location = new System.Drawing.Point(1140, 449);
            this.weatherbutton.Margin = new System.Windows.Forms.Padding(1);
            this.weatherbutton.Name = "weatherbutton";
            this.weatherbutton.Size = new System.Drawing.Size(49, 49);
            this.weatherbutton.TabIndex = 186;
            this.weatherbutton.UseVisualStyleBackColor = true;
            this.weatherbutton.Click += new System.EventHandler(this.button1_Click);
            // 
            // import
            // 
            this.import.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.import.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.import.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.import.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.import.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.import.Image = ((System.Drawing.Image)(resources.GetObject("import.Image")));
            this.import.Location = new System.Drawing.Point(1141, 347);
            this.import.Margin = new System.Windows.Forms.Padding(1);
            this.import.Name = "import";
            this.import.Size = new System.Drawing.Size(49, 49);
            this.import.TabIndex = 185;
            this.import.UseVisualStyleBackColor = true;
            this.import.Click += new System.EventHandler(this.import_Click);
            // 
            // export
            // 
            this.export.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.export.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.export.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.export.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.export.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.export.Image = ((System.Drawing.Image)(resources.GetObject("export.Image")));
            this.export.Location = new System.Drawing.Point(1141, 296);
            this.export.Margin = new System.Windows.Forms.Padding(1);
            this.export.Name = "export";
            this.export.Size = new System.Drawing.Size(49, 49);
            this.export.TabIndex = 184;
            this.export.UseVisualStyleBackColor = true;
            this.export.Click += new System.EventHandler(this.export_Click);
            // 
            // undo_import
            // 
            this.undo_import.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.undo_import.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.undo_import.Enabled = false;
            this.undo_import.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.undo_import.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.undo_import.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.undo_import.Image = global::Financial_Journal.Properties.Resources.undo_grey;
            this.undo_import.Location = new System.Drawing.Point(1141, 398);
            this.undo_import.Margin = new System.Windows.Forms.Padding(1);
            this.undo_import.Name = "undo_import";
            this.undo_import.Size = new System.Drawing.Size(49, 49);
            this.undo_import.TabIndex = 183;
            this.undo_import.UseVisualStyleBackColor = true;
            this.undo_import.Click += new System.EventHandler(this.undo_import_Click);
            // 
            // email_button
            // 
            this.email_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.email_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.email_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.email_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.email_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.email_button.Image = ((System.Drawing.Image)(resources.GetObject("email_button.Image")));
            this.email_button.Location = new System.Drawing.Point(1141, 245);
            this.email_button.Margin = new System.Windows.Forms.Padding(1);
            this.email_button.Name = "email_button";
            this.email_button.Size = new System.Drawing.Size(49, 49);
            this.email_button.TabIndex = 182;
            this.email_button.UseVisualStyleBackColor = true;
            this.email_button.Click += new System.EventHandler(this.email_button_Click);
            // 
            // screenshot_button
            // 
            this.screenshot_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.screenshot_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.screenshot_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.screenshot_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.screenshot_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.screenshot_button.Image = ((System.Drawing.Image)(resources.GetObject("screenshot_button.Image")));
            this.screenshot_button.Location = new System.Drawing.Point(1141, 194);
            this.screenshot_button.Margin = new System.Windows.Forms.Padding(1);
            this.screenshot_button.Name = "screenshot_button";
            this.screenshot_button.Size = new System.Drawing.Size(49, 49);
            this.screenshot_button.TabIndex = 181;
            this.screenshot_button.UseVisualStyleBackColor = true;
            this.screenshot_button.Click += new System.EventHandler(this.screenshot_button_Click);
            // 
            // edit_calendar
            // 
            this.edit_calendar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.edit_calendar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.edit_calendar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.edit_calendar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_calendar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.edit_calendar.Image = global::Financial_Journal.Properties.Resources.edit_on;
            this.edit_calendar.Location = new System.Drawing.Point(1141, 92);
            this.edit_calendar.Margin = new System.Windows.Forms.Padding(1);
            this.edit_calendar.Name = "edit_calendar";
            this.edit_calendar.Size = new System.Drawing.Size(49, 49);
            this.edit_calendar.TabIndex = 180;
            this.edit_calendar.UseVisualStyleBackColor = true;
            this.edit_calendar.Click += new System.EventHandler(this.edit_calendar_Click_1);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button2.Image = global::Financial_Journal.Properties.Resources.printer__1_;
            this.button2.Location = new System.Drawing.Point(1141, 143);
            this.button2.Margin = new System.Windows.Forms.Padding(1);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(49, 49);
            this.button2.TabIndex = 179;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // settings_button
            // 
            this.settings_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.settings_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.settings_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settings_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settings_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.settings_button.Image = global::Financial_Journal.Properties.Resources.settings;
            this.settings_button.Location = new System.Drawing.Point(1139, 41);
            this.settings_button.Margin = new System.Windows.Forms.Padding(1);
            this.settings_button.Name = "settings_button";
            this.settings_button.Size = new System.Drawing.Size(49, 49);
            this.settings_button.TabIndex = 177;
            this.settings_button.UseVisualStyleBackColor = true;
            this.settings_button.Click += new System.EventHandler(this.settings_button_Click);
            // 
            // next_page_button
            // 
            this.next_page_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.next_page_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.next_page_button.FlatAppearance.BorderSize = 0;
            this.next_page_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.next_page_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next_page_button.ForeColor = System.Drawing.Color.White;
            this.next_page_button.Image = ((System.Drawing.Image)(resources.GetObject("next_page_button.Image")));
            this.next_page_button.Location = new System.Drawing.Point(1105, 53);
            this.next_page_button.Margin = new System.Windows.Forms.Padding(0);
            this.next_page_button.Name = "next_page_button";
            this.next_page_button.Size = new System.Drawing.Size(30, 30);
            this.next_page_button.TabIndex = 85;
            this.next_page_button.TabStop = false;
            this.next_page_button.UseVisualStyleBackColor = false;
            this.next_page_button.Click += new System.EventHandler(this.next_page_button_Click);
            // 
            // back_page_button
            // 
            this.back_page_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.back_page_button.FlatAppearance.BorderSize = 0;
            this.back_page_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.back_page_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back_page_button.ForeColor = System.Drawing.Color.White;
            this.back_page_button.Image = ((System.Drawing.Image)(resources.GetObject("back_page_button.Image")));
            this.back_page_button.Location = new System.Drawing.Point(27, 53);
            this.back_page_button.Margin = new System.Windows.Forms.Padding(0);
            this.back_page_button.Name = "back_page_button";
            this.back_page_button.Size = new System.Drawing.Size(30, 30);
            this.back_page_button.TabIndex = 84;
            this.back_page_button.TabStop = false;
            this.back_page_button.UseVisualStyleBackColor = false;
            this.back_page_button.Click += new System.EventHandler(this.back_page_button_Click);
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
            // Calendar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1194, 676);
            this.Controls.Add(this.weatherbutton);
            this.Controls.Add(this.import);
            this.Controls.Add(this.export);
            this.Controls.Add(this.undo_import);
            this.Controls.Add(this.email_button);
            this.Controls.Add(this.screenshot_button);
            this.Controls.Add(this.edit_calendar);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.settings_button);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.next_page_button);
            this.Controls.Add(this.back_page_button);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.close_button);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1000, 650);
            this.Name = "Calendar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Personal Banker: Personal Calendar";
            this.Load += new System.EventHandler(this.Receipt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private Button close_button;
        private PictureBox pictureBox1;
        private Label label5;
        private TextBox textBox5;
        private Button next_page_button;
        private Button back_page_button;
        public Label label15;
        private Button settings_button;
        private TextBox textBox1;
        private Button button2;
        private Button edit_calendar;
        private Button screenshot_button;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private PrintPreviewDialog printPreviewDialog1;
        private Button email_button;
        private Button undo_import;
        private Button import;
        private Button export;
        private Button weatherbutton;
    }
}


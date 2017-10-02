using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Add_Edit_Calendar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Add_Edit_Calendar));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.close_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.add_entry_button = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.add_agenda_item = new System.Windows.Forms.Button();
            this.timePickerPanel1 = new Opulos.Core.UI.TimePickerPanel();
            this.bufferedPanel1 = new Financial_Journal.BufferedPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.multiDate = new System.Windows.Forms.DateTimePicker();
            this.multiDayBox = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.remind_me_x_days = new Financial_Journal.AdvancedComboBox();
            this.alert_on = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.title_box = new System.Windows.Forms.TextBox();
            this.close_entry_button = new System.Windows.Forms.Button();
            this.Add_button = new System.Windows.Forms.Button();
            this.desc_box = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.importance_box = new Financial_Journal.AdvancedComboBox();
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
            this.textBox1.Location = new System.Drawing.Point(808, -10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 2504);
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
            this.textBox3.Size = new System.Drawing.Size(2997, 12);
            this.textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 39);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(2997, 12);
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
            this.close_button.Location = new System.Drawing.Point(788, 1);
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
            this.textBox5.Size = new System.Drawing.Size(3093, 27);
            this.textBox5.TabIndex = 72;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(46, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 16);
            this.label3.TabIndex = 104;
            this.label3.Text = "Add a new event";
            // 
            // add_entry_button
            // 
            this.add_entry_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.add_entry_button.FlatAppearance.BorderSize = 0;
            this.add_entry_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.add_entry_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.add_entry_button.ForeColor = System.Drawing.Color.White;
            this.add_entry_button.Image = global::Financial_Journal.Properties.Resources.add;
            this.add_entry_button.Location = new System.Drawing.Point(19, 36);
            this.add_entry_button.Margin = new System.Windows.Forms.Padding(0);
            this.add_entry_button.Name = "add_entry_button";
            this.add_entry_button.Size = new System.Drawing.Size(23, 24);
            this.add_entry_button.TabIndex = 103;
            this.add_entry_button.TabStop = false;
            this.add_entry_button.UseVisualStyleBackColor = false;
            this.add_entry_button.Click += new System.EventHandler(this.add_entry_button_Click);
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
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.Control;
            this.label7.Location = new System.Drawing.Point(46, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(141, 16);
            this.label7.TabIndex = 106;
            this.label7.Text = "Add item from Agenda";
            // 
            // add_agenda_item
            // 
            this.add_agenda_item.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.add_agenda_item.FlatAppearance.BorderSize = 0;
            this.add_agenda_item.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.add_agenda_item.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.add_agenda_item.ForeColor = System.Drawing.Color.White;
            this.add_agenda_item.Image = global::Financial_Journal.Properties.Resources.add;
            this.add_agenda_item.Location = new System.Drawing.Point(19, 68);
            this.add_agenda_item.Margin = new System.Windows.Forms.Padding(0);
            this.add_agenda_item.Name = "add_agenda_item";
            this.add_agenda_item.Size = new System.Drawing.Size(23, 24);
            this.add_agenda_item.TabIndex = 105;
            this.add_agenda_item.TabStop = false;
            this.add_agenda_item.UseVisualStyleBackColor = false;
            this.add_agenda_item.Click += new System.EventHandler(this.add_agenda_item_Click);
            // 
            // timePickerPanel1
            // 
            this.timePickerPanel1.Location = new System.Drawing.Point(766, 433);
            this.timePickerPanel1.Name = "timePickerPanel1";
            this.timePickerPanel1.Size = new System.Drawing.Size(150, 150);
            this.timePickerPanel1.TabIndex = 107;
            this.timePickerPanel1.Visible = false;
            // 
            // bufferedPanel1
            // 
            this.bufferedPanel1.Controls.Add(this.label8);
            this.bufferedPanel1.Controls.Add(this.multiDate);
            this.bufferedPanel1.Controls.Add(this.multiDayBox);
            this.bufferedPanel1.Controls.Add(this.button3);
            this.bufferedPanel1.Controls.Add(this.label6);
            this.bufferedPanel1.Controls.Add(this.remind_me_x_days);
            this.bufferedPanel1.Controls.Add(this.alert_on);
            this.bufferedPanel1.Controls.Add(this.label4);
            this.bufferedPanel1.Controls.Add(this.label2);
            this.bufferedPanel1.Controls.Add(this.title_box);
            this.bufferedPanel1.Controls.Add(this.close_entry_button);
            this.bufferedPanel1.Controls.Add(this.Add_button);
            this.bufferedPanel1.Controls.Add(this.desc_box);
            this.bufferedPanel1.Controls.Add(this.label1);
            this.bufferedPanel1.Controls.Add(this.importance_box);
            this.bufferedPanel1.Location = new System.Drawing.Point(13, 35);
            this.bufferedPanel1.Name = "bufferedPanel1";
            this.bufferedPanel1.Size = new System.Drawing.Size(784, 181);
            this.bufferedPanel1.TabIndex = 102;
            this.bufferedPanel1.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(617, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 16);
            this.label8.TabIndex = 273;
            this.label8.Text = "Until:";
            this.label8.Visible = false;
            // 
            // multiDate
            // 
            this.multiDate.CalendarMonthBackground = System.Drawing.SystemColors.HotTrack;
            this.multiDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.multiDate.Location = new System.Drawing.Point(660, 30);
            this.multiDate.Name = "multiDate";
            this.multiDate.Size = new System.Drawing.Size(81, 20);
            this.multiDate.TabIndex = 272;
            this.multiDate.Value = new System.DateTime(2016, 8, 13, 18, 48, 56, 0);
            this.multiDate.Visible = false;
            this.multiDate.ValueChanged += new System.EventHandler(this.multiDate_ValueChanged);
            // 
            // multiDayBox
            // 
            this.multiDayBox.AutoSize = true;
            this.multiDayBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.multiDayBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.multiDayBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.multiDayBox.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.multiDayBox.Location = new System.Drawing.Point(590, 7);
            this.multiDayBox.Name = "multiDayBox";
            this.multiDayBox.Size = new System.Drawing.Size(121, 20);
            this.multiDayBox.TabIndex = 122;
            this.multiDayBox.Text = "Multi-day event?";
            this.multiDayBox.UseVisualStyleBackColor = false;
            this.multiDayBox.CheckedChanged += new System.EventHandler(this.multiDayBox_CheckedChanged);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Image = global::Financial_Journal.Properties.Resources.question_circular_button;
            this.button3.Location = new System.Drawing.Point(286, 115);
            this.button3.Margin = new System.Windows.Forms.Padding(0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(23, 24);
            this.button3.TabIndex = 121;
            this.button3.TabStop = false;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(357, 147);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 16);
            this.label6.TabIndex = 120;
            this.label6.Text = "days in advance";
            // 
            // remind_me_x_days
            // 
            this.remind_me_x_days.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.remind_me_x_days.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.remind_me_x_days.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.remind_me_x_days.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remind_me_x_days.ForeColor = System.Drawing.Color.White;
            this.remind_me_x_days.FormattingEnabled = true;
            this.remind_me_x_days.FrameColor = System.Drawing.Color.Lime;
            this.remind_me_x_days.HighlightColor = System.Drawing.Color.Gray;
            this.remind_me_x_days.Location = new System.Drawing.Point(283, 144);
            this.remind_me_x_days.Name = "remind_me_x_days";
            this.remind_me_x_days.Size = new System.Drawing.Size(67, 23);
            this.remind_me_x_days.TabIndex = 3;
            this.remind_me_x_days.SelectedIndexChanged += new System.EventHandler(this.remind_me_x_days_SelectedIndexChanged);
            // 
            // alert_on
            // 
            this.alert_on.AutoSize = true;
            this.alert_on.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.alert_on.Checked = true;
            this.alert_on.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alert_on.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.alert_on.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alert_on.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.alert_on.Location = new System.Drawing.Point(124, 145);
            this.alert_on.Name = "alert_on";
            this.alert_on.Size = new System.Drawing.Size(153, 20);
            this.alert_on.TabIndex = 118;
            this.alert_on.Text = "Remind me everyday";
            this.alert_on.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(37, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 16);
            this.label4.TabIndex = 105;
            this.label4.Text = "Significance";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(42, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 16);
            this.label2.TabIndex = 100;
            this.label2.Text = "Description";
            // 
            // title_box
            // 
            this.title_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.title_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.title_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title_box.ForeColor = System.Drawing.Color.White;
            this.title_box.Location = new System.Drawing.Point(124, 4);
            this.title_box.MaxLength = 40;
            this.title_box.Name = "title_box";
            this.title_box.Size = new System.Drawing.Size(280, 22);
            this.title_box.TabIndex = 0;
            // 
            // close_entry_button
            // 
            this.close_entry_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.close_entry_button.FlatAppearance.BorderSize = 0;
            this.close_entry_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close_entry_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close_entry_button.ForeColor = System.Drawing.Color.White;
            this.close_entry_button.Image = global::Financial_Journal.Properties.Resources.error;
            this.close_entry_button.Location = new System.Drawing.Point(6, 1);
            this.close_entry_button.Margin = new System.Windows.Forms.Padding(0);
            this.close_entry_button.Name = "close_entry_button";
            this.close_entry_button.Size = new System.Drawing.Size(23, 22);
            this.close_entry_button.TabIndex = 97;
            this.close_entry_button.TabStop = false;
            this.close_entry_button.UseVisualStyleBackColor = false;
            this.close_entry_button.Click += new System.EventHandler(this.close_entry_button_Click);
            // 
            // Add_button
            // 
            this.Add_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Add_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Add_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Add_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Add_button.Image = global::Financial_Journal.Properties.Resources.add_notes;
            this.Add_button.Location = new System.Drawing.Point(724, 128);
            this.Add_button.Margin = new System.Windows.Forms.Padding(1);
            this.Add_button.Name = "Add_button";
            this.Add_button.Size = new System.Drawing.Size(49, 44);
            this.Add_button.TabIndex = 4;
            this.Add_button.UseVisualStyleBackColor = true;
            this.Add_button.Click += new System.EventHandler(this.Add_button_Click);
            // 
            // desc_box
            // 
            this.desc_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.desc_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.desc_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.desc_box.ForeColor = System.Drawing.Color.White;
            this.desc_box.Location = new System.Drawing.Point(124, 32);
            this.desc_box.MaxLength = 600;
            this.desc_box.Multiline = true;
            this.desc_box.Name = "desc_box";
            this.desc_box.Size = new System.Drawing.Size(280, 78);
            this.desc_box.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(47, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 74;
            this.label1.Text = "Event Title";
            // 
            // importance_box
            // 
            this.importance_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.importance_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.importance_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.importance_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.importance_box.ForeColor = System.Drawing.Color.White;
            this.importance_box.FormattingEnabled = true;
            this.importance_box.FrameColor = System.Drawing.Color.Lime;
            this.importance_box.HighlightColor = System.Drawing.Color.Gray;
            this.importance_box.Location = new System.Drawing.Point(124, 116);
            this.importance_box.Name = "importance_box";
            this.importance_box.Size = new System.Drawing.Size(159, 23);
            this.importance_box.TabIndex = 2;
            // 
            // Add_Edit_Calendar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(809, 40);
            this.Controls.Add(this.timePickerPanel1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.add_agenda_item);
            this.Controls.Add(this.bufferedPanel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.add_entry_button);
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
            this.Name = "Add_Edit_Calendar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Personal Banker: Add/Edit Calendar";
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
        private Button add_entry_button;
        private BufferedPanel bufferedPanel1;
        private Label label2;
        public TextBox title_box;
        private Button close_entry_button;
        private Button Add_button;
        public TextBox desc_box;
        private Label label1;
        public AdvancedComboBox importance_box;
        private Label label3;
        private Label label4;
        public AdvancedComboBox remind_me_x_days;
        private CheckBox alert_on;
        private Label label6;
        private Button button3;
        private Label label7;
        private Button add_agenda_item;
        private Opulos.Core.UI.TimePickerPanel timePickerPanel1;
        private CheckBox multiDayBox;
        private Label label8;
        private DateTimePicker multiDate;
    }
}


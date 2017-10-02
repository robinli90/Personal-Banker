using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Tier_View
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tier_View));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.close_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.b_date_2 = new Financial_Journal.AdvancedComboBox();
            this.b_date_1 = new Financial_Journal.AdvancedComboBox();
            this.a_date_2 = new Financial_Journal.AdvancedComboBox();
            this.a_date_1 = new Financial_Journal.AdvancedComboBox();
            this.advancedComboBox1 = new Financial_Journal.AdvancedComboBox();
            this.type_box = new Financial_Journal.AdvancedComboBox();
            this.a_date_3 = new Financial_Journal.AdvancedComboBox();
            this.b_date_3 = new Financial_Journal.AdvancedComboBox();
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
            this.textBox1.Location = new System.Drawing.Point(1293, -10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 3210);
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
            this.textBox3.Size = new System.Drawing.Size(3482, 12);
            this.textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 745);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(3482, 12);
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
            this.close_button.Location = new System.Drawing.Point(1273, 1);
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
            this.label5.Size = new System.Drawing.Size(154, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Spending Tier Visualizer";
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
            this.textBox5.Size = new System.Drawing.Size(3578, 27);
            this.textBox5.TabIndex = 72;
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
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(1143, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 16);
            this.label1.TabIndex = 76;
            this.label1.Text = "Type:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(1159, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 16);
            this.label2.TabIndex = 78;
            this.label2.Text = "By:";
            // 
            // b_date_2
            // 
            this.b_date_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.b_date_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.b_date_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.b_date_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.b_date_2.ForeColor = System.Drawing.Color.White;
            this.b_date_2.FormattingEnabled = true;
            this.b_date_2.FrameColor = System.Drawing.Color.Lime;
            this.b_date_2.HighlightColor = System.Drawing.Color.Gray;
            this.b_date_2.Location = new System.Drawing.Point(21, 375);
            this.b_date_2.Name = "b_date_2";
            this.b_date_2.Size = new System.Drawing.Size(61, 23);
            this.b_date_2.TabIndex = 82;
            this.b_date_2.Visible = false;
            this.b_date_2.SelectedIndexChanged += new System.EventHandler(this.b_date_2_SelectedIndexChanged);
            // 
            // b_date_1
            // 
            this.b_date_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.b_date_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.b_date_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.b_date_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.b_date_1.ForeColor = System.Drawing.Color.White;
            this.b_date_1.FormattingEnabled = true;
            this.b_date_1.FrameColor = System.Drawing.Color.Lime;
            this.b_date_1.HighlightColor = System.Drawing.Color.Gray;
            this.b_date_1.Location = new System.Drawing.Point(21, 346);
            this.b_date_1.Name = "b_date_1";
            this.b_date_1.Size = new System.Drawing.Size(134, 23);
            this.b_date_1.TabIndex = 81;
            this.b_date_1.SelectedIndexChanged += new System.EventHandler(this.b_date_1_SelectedIndexChanged);
            // 
            // a_date_2
            // 
            this.a_date_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.a_date_2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.a_date_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.a_date_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.a_date_2.ForeColor = System.Drawing.Color.White;
            this.a_date_2.FormattingEnabled = true;
            this.a_date_2.FrameColor = System.Drawing.Color.Lime;
            this.a_date_2.HighlightColor = System.Drawing.Color.Gray;
            this.a_date_2.Location = new System.Drawing.Point(21, 151);
            this.a_date_2.Name = "a_date_2";
            this.a_date_2.Size = new System.Drawing.Size(61, 23);
            this.a_date_2.TabIndex = 80;
            this.a_date_2.Visible = false;
            this.a_date_2.SelectedIndexChanged += new System.EventHandler(this.a_date_2_SelectedIndexChanged);
            // 
            // a_date_1
            // 
            this.a_date_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.a_date_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.a_date_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.a_date_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.a_date_1.ForeColor = System.Drawing.Color.White;
            this.a_date_1.FormattingEnabled = true;
            this.a_date_1.FrameColor = System.Drawing.Color.Lime;
            this.a_date_1.HighlightColor = System.Drawing.Color.Gray;
            this.a_date_1.Location = new System.Drawing.Point(21, 122);
            this.a_date_1.Name = "a_date_1";
            this.a_date_1.Size = new System.Drawing.Size(134, 23);
            this.a_date_1.TabIndex = 79;
            this.a_date_1.SelectedIndexChanged += new System.EventHandler(this.a_date_1_SelectedIndexChanged);
            // 
            // advancedComboBox1
            // 
            this.advancedComboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedComboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.advancedComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.advancedComboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.advancedComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.advancedComboBox1.ForeColor = System.Drawing.Color.White;
            this.advancedComboBox1.FormattingEnabled = true;
            this.advancedComboBox1.FrameColor = System.Drawing.Color.Lime;
            this.advancedComboBox1.HighlightColor = System.Drawing.Color.Gray;
            this.advancedComboBox1.Location = new System.Drawing.Point(1189, 72);
            this.advancedComboBox1.Name = "advancedComboBox1";
            this.advancedComboBox1.Size = new System.Drawing.Size(93, 23);
            this.advancedComboBox1.TabIndex = 77;
            this.advancedComboBox1.SelectedIndexChanged += new System.EventHandler(this.advancedComboBox1_SelectedIndexChanged);
            // 
            // type_box
            // 
            this.type_box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.type_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.type_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.type_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.type_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.type_box.ForeColor = System.Drawing.Color.White;
            this.type_box.FormattingEnabled = true;
            this.type_box.FrameColor = System.Drawing.Color.Lime;
            this.type_box.HighlightColor = System.Drawing.Color.Gray;
            this.type_box.Location = new System.Drawing.Point(1189, 43);
            this.type_box.Name = "type_box";
            this.type_box.Size = new System.Drawing.Size(93, 23);
            this.type_box.TabIndex = 75;
            this.type_box.SelectedIndexChanged += new System.EventHandler(this.type_box_SelectedIndexChanged);
            // 
            // a_date_3
            // 
            this.a_date_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.a_date_3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.a_date_3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.a_date_3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.a_date_3.ForeColor = System.Drawing.Color.White;
            this.a_date_3.FormattingEnabled = true;
            this.a_date_3.FrameColor = System.Drawing.Color.Lime;
            this.a_date_3.HighlightColor = System.Drawing.Color.Gray;
            this.a_date_3.Location = new System.Drawing.Point(88, 151);
            this.a_date_3.Name = "a_date_3";
            this.a_date_3.Size = new System.Drawing.Size(67, 23);
            this.a_date_3.TabIndex = 83;
            this.a_date_3.Visible = false;
            this.a_date_3.SelectedIndexChanged += new System.EventHandler(this.a_date_3_SelectedIndexChanged);
            // 
            // b_date_3
            // 
            this.b_date_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.b_date_3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.b_date_3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.b_date_3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.b_date_3.ForeColor = System.Drawing.Color.White;
            this.b_date_3.FormattingEnabled = true;
            this.b_date_3.FrameColor = System.Drawing.Color.Lime;
            this.b_date_3.HighlightColor = System.Drawing.Color.Gray;
            this.b_date_3.Location = new System.Drawing.Point(88, 375);
            this.b_date_3.Name = "b_date_3";
            this.b_date_3.Size = new System.Drawing.Size(67, 23);
            this.b_date_3.TabIndex = 84;
            this.b_date_3.Visible = false;
            this.b_date_3.SelectedIndexChanged += new System.EventHandler(this.b_date_3_SelectedIndexChanged);
            // 
            // Tier_View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1294, 746);
            this.Controls.Add(this.b_date_1);
            this.Controls.Add(this.b_date_3);
            this.Controls.Add(this.a_date_1);
            this.Controls.Add(this.b_date_2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.a_date_3);
            this.Controls.Add(this.advancedComboBox1);
            this.Controls.Add(this.a_date_2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.type_box);
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
            this.Name = "Tier_View";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Personal Banker: Tier Visualizer";
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
        public AdvancedComboBox type_box;
        private Label label2;
        public AdvancedComboBox advancedComboBox1;
        public AdvancedComboBox a_date_1;
        public AdvancedComboBox a_date_2;
        public AdvancedComboBox b_date_2;
        public AdvancedComboBox b_date_1;
        public AdvancedComboBox a_date_3;
        public AdvancedComboBox b_date_3;
    }
}


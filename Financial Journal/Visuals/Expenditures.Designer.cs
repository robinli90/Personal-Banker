using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Expenditures
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
            PresentationControls.CheckBoxProperties checkBoxProperties1 = new PresentationControls.CheckBoxProperties();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Expenditures));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.close_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.comboBoxEdgeType = new Financial_Journal.AdvancedComboBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.comboBoxShadowStyle = new Financial_Journal.AdvancedComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.height_text = new System.Windows.Forms.Label();
            this.transparency_text = new System.Windows.Forms.Label();
            this.rotation_text = new System.Windows.Forms.Label();
            this.view_mode = new Financial_Journal.AdvancedComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.month_box = new Financial_Journal.AdvancedComboBox();
            this.year_box = new Financial_Journal.AdvancedComboBox();
            this.bufferedPanel1 = new Financial_Journal.BufferedPanel();
            this.m_panelDrawing = new System.Drawing.PieChart.PieChartControl();
            this.sort_check = new System.Windows.Forms.CheckBox();
            this.show_expenses_box = new System.Windows.Forms.CheckBox();
            this.show_general_box = new System.Windows.Forms.CheckBox();
            this.total_text = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.income_box = new System.Windows.Forms.CheckBox();
            this.spread_data_box = new System.Windows.Forms.CheckBox();
            this.cmbManual = new PresentationControls.CheckBoxComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.reset = new System.Windows.Forms.Button();
            this.close_view_button = new System.Windows.Forms.Button();
            this.open_appearance = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.group_all = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
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
            this.textBox1.Location = new System.Drawing.Point(994, -10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 2874);
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
            this.textBox3.Size = new System.Drawing.Size(3183, 12);
            this.textBox3.TabIndex = 2;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 409);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(3183, 12);
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
            this.close_button.Location = new System.Drawing.Point(974, 1);
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
            this.label5.Size = new System.Drawing.Size(86, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Expenditures";
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
            this.textBox5.Size = new System.Drawing.Size(3279, 27);
            this.textBox5.TabIndex = 72;
            // 
            // comboBoxEdgeType
            // 
            this.comboBoxEdgeType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.comboBoxEdgeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEdgeType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxEdgeType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxEdgeType.ForeColor = System.Drawing.Color.White;
            this.comboBoxEdgeType.FormattingEnabled = true;
            this.comboBoxEdgeType.FrameColor = System.Drawing.SystemColors.Desktop;
            this.comboBoxEdgeType.HighlightColor = System.Drawing.Color.Gray;
            this.comboBoxEdgeType.Location = new System.Drawing.Point(835, 351);
            this.comboBoxEdgeType.Name = "comboBoxEdgeType";
            this.comboBoxEdgeType.Size = new System.Drawing.Size(153, 23);
            this.comboBoxEdgeType.TabIndex = 13;
            this.comboBoxEdgeType.SelectedIndexChanged += new System.EventHandler(this.comboBoxEdgeType_SelectedIndexChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(828, 323);
            this.trackBar1.Maximum = 255;
            this.trackBar1.Minimum = 10;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(135, 45);
            this.trackBar1.TabIndex = 12;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Value = 122;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(774, 323);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 16);
            this.label1.TabIndex = 78;
            this.label1.Text = "Opacity";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(747, 354);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 16);
            this.label3.TabIndex = 80;
            this.label3.Text = "Outline Style";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(781, 295);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 16);
            this.label4.TabIndex = 82;
            this.label4.Text = "Height";
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(828, 295);
            this.trackBar3.Maximum = 50;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(135, 45);
            this.trackBar3.TabIndex = 11;
            this.trackBar3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar3.Value = 25;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // comboBoxShadowStyle
            // 
            this.comboBoxShadowStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.comboBoxShadowStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxShadowStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxShadowStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxShadowStyle.ForeColor = System.Drawing.Color.White;
            this.comboBoxShadowStyle.FormattingEnabled = true;
            this.comboBoxShadowStyle.FrameColor = System.Drawing.SystemColors.Desktop;
            this.comboBoxShadowStyle.HighlightColor = System.Drawing.Color.Gray;
            this.comboBoxShadowStyle.Location = new System.Drawing.Point(835, 380);
            this.comboBoxShadowStyle.Name = "comboBoxShadowStyle";
            this.comboBoxShadowStyle.Size = new System.Drawing.Size(153, 23);
            this.comboBoxShadowStyle.TabIndex = 14;
            this.comboBoxShadowStyle.SelectedIndexChanged += new System.EventHandler(this.comboBoxShadowStyle_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(739, 383);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 16);
            this.label6.TabIndex = 84;
            this.label6.Text = "Shadow Style";
            // 
            // height_text
            // 
            this.height_text.AutoSize = true;
            this.height_text.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.height_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.height_text.ForeColor = System.Drawing.SystemColors.Control;
            this.height_text.Location = new System.Drawing.Point(959, 297);
            this.height_text.Name = "height_text";
            this.height_text.Size = new System.Drawing.Size(0, 16);
            this.height_text.TabIndex = 85;
            this.height_text.Click += new System.EventHandler(this.height_text_Click);
            // 
            // transparency_text
            // 
            this.transparency_text.AutoSize = true;
            this.transparency_text.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.transparency_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transparency_text.ForeColor = System.Drawing.SystemColors.Control;
            this.transparency_text.Location = new System.Drawing.Point(959, 326);
            this.transparency_text.Name = "transparency_text";
            this.transparency_text.Size = new System.Drawing.Size(0, 16);
            this.transparency_text.TabIndex = 86;
            this.transparency_text.Click += new System.EventHandler(this.transparency_text_Click);
            // 
            // rotation_text
            // 
            this.rotation_text.AutoSize = true;
            this.rotation_text.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rotation_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rotation_text.ForeColor = System.Drawing.SystemColors.Control;
            this.rotation_text.Location = new System.Drawing.Point(959, 334);
            this.rotation_text.Name = "rotation_text";
            this.rotation_text.Size = new System.Drawing.Size(0, 16);
            this.rotation_text.TabIndex = 87;
            this.rotation_text.Click += new System.EventHandler(this.rotation_text_Click);
            // 
            // view_mode
            // 
            this.view_mode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.view_mode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.view_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.view_mode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.view_mode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.view_mode.ForeColor = System.Drawing.Color.White;
            this.view_mode.FormattingEnabled = true;
            this.view_mode.FrameColor = System.Drawing.SystemColors.Desktop;
            this.view_mode.HighlightColor = System.Drawing.Color.Gray;
            this.view_mode.Location = new System.Drawing.Point(802, 31);
            this.view_mode.Name = "view_mode";
            this.view_mode.Size = new System.Drawing.Size(153, 23);
            this.view_mode.TabIndex = 0;
            this.view_mode.SelectedIndexChanged += new System.EventHandler(this.view_mode_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(759, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 16);
            this.label2.TabIndex = 92;
            this.label2.Text = "View";
            // 
            // month_box
            // 
            this.month_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.month_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.month_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.month_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.month_box.ForeColor = System.Drawing.Color.White;
            this.month_box.FormattingEnabled = true;
            this.month_box.FrameColor = System.Drawing.SystemColors.Desktop;
            this.month_box.HighlightColor = System.Drawing.Color.Gray;
            this.month_box.Location = new System.Drawing.Point(802, 60);
            this.month_box.Name = "month_box";
            this.month_box.Size = new System.Drawing.Size(73, 23);
            this.month_box.TabIndex = 1;
            this.month_box.Visible = false;
            this.month_box.SelectedIndexChanged += new System.EventHandler(this.month_box_SelectedIndexChanged);
            // 
            // year_box
            // 
            this.year_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.year_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.year_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.year_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.year_box.ForeColor = System.Drawing.Color.White;
            this.year_box.FormattingEnabled = true;
            this.year_box.FrameColor = System.Drawing.SystemColors.Desktop;
            this.year_box.HighlightColor = System.Drawing.Color.Gray;
            this.year_box.Location = new System.Drawing.Point(882, 60);
            this.year_box.Name = "year_box";
            this.year_box.Size = new System.Drawing.Size(73, 23);
            this.year_box.TabIndex = 2;
            this.year_box.Visible = false;
            this.year_box.SelectedIndexChanged += new System.EventHandler(this.year_box_SelectedIndexChanged);
            // 
            // bufferedPanel1
            // 
            this.bufferedPanel1.Controls.Add(this.m_panelDrawing);
            this.bufferedPanel1.Location = new System.Drawing.Point(13, 68);
            this.bufferedPanel1.Name = "bufferedPanel1";
            this.bufferedPanel1.Size = new System.Drawing.Size(715, 310);
            this.bufferedPanel1.TabIndex = 90;
            // 
            // m_panelDrawing
            // 
            this.m_panelDrawing.InitialAngle = 0F;
            this.m_panelDrawing.Location = new System.Drawing.Point(1, 3);
            this.m_panelDrawing.Name = "m_panelDrawing";
            this.m_panelDrawing.Return_Value = null;
            this.m_panelDrawing.ShowToolTip = true;
            this.m_panelDrawing.Size = new System.Drawing.Size(712, 306);
            this.m_panelDrawing.TabIndex = 0;
            this.m_panelDrawing.Texts = null;
            this.m_panelDrawing.ToolTips = null;
            // 
            // sort_check
            // 
            this.sort_check.AutoSize = true;
            this.sort_check.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sort_check.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sort_check.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sort_check.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.sort_check.Location = new System.Drawing.Point(748, 179);
            this.sort_check.Name = "sort_check";
            this.sort_check.Size = new System.Drawing.Size(88, 20);
            this.sort_check.TabIndex = 6;
            this.sort_check.Text = "Sort Graph";
            this.sort_check.UseVisualStyleBackColor = false;
            this.sort_check.CheckedChanged += new System.EventHandler(this.sort_check_CheckedChanged);
            // 
            // show_expenses_box
            // 
            this.show_expenses_box.AutoSize = true;
            this.show_expenses_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.show_expenses_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.show_expenses_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.show_expenses_box.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.show_expenses_box.Location = new System.Drawing.Point(748, 127);
            this.show_expenses_box.Name = "show_expenses_box";
            this.show_expenses_box.Size = new System.Drawing.Size(181, 20);
            this.show_expenses_box.TabIndex = 4;
            this.show_expenses_box.Text = "Show Recurring Expenses";
            this.show_expenses_box.UseVisualStyleBackColor = false;
            this.show_expenses_box.CheckedChanged += new System.EventHandler(this.show_expenses_box_CheckedChanged);
            // 
            // show_general_box
            // 
            this.show_general_box.AutoSize = true;
            this.show_general_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.show_general_box.Checked = true;
            this.show_general_box.CheckState = System.Windows.Forms.CheckState.Checked;
            this.show_general_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.show_general_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.show_general_box.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.show_general_box.Location = new System.Drawing.Point(748, 101);
            this.show_general_box.Name = "show_general_box";
            this.show_general_box.Size = new System.Drawing.Size(189, 20);
            this.show_general_box.TabIndex = 3;
            this.show_general_box.Text = "Show General Expenditures";
            this.show_general_box.UseVisualStyleBackColor = false;
            this.show_general_box.CheckedChanged += new System.EventHandler(this.show_general_box_CheckedChanged_1);
            // 
            // total_text
            // 
            this.total_text.AutoSize = true;
            this.total_text.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.total_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total_text.ForeColor = System.Drawing.SystemColors.Control;
            this.total_text.Location = new System.Drawing.Point(10, 385);
            this.total_text.Name = "total_text";
            this.total_text.Size = new System.Drawing.Size(82, 16);
            this.total_text.TabIndex = 99;
            this.total_text.Text = "Outline Style";
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.Control;
            this.label7.Location = new System.Drawing.Point(16, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(711, 18);
            this.label7.TabIndex = 100;
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // income_box
            // 
            this.income_box.AutoSize = true;
            this.income_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.income_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.income_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.income_box.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.income_box.Location = new System.Drawing.Point(748, 153);
            this.income_box.Name = "income_box";
            this.income_box.Size = new System.Drawing.Size(171, 20);
            this.income_box.TabIndex = 5;
            this.income_box.Text = "Include Personal Income";
            this.income_box.UseVisualStyleBackColor = false;
            this.income_box.CheckedChanged += new System.EventHandler(this.income_box_CheckedChanged);
            // 
            // spread_data_box
            // 
            this.spread_data_box.AutoSize = true;
            this.spread_data_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.spread_data_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spread_data_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spread_data_box.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.spread_data_box.Location = new System.Drawing.Point(748, 205);
            this.spread_data_box.Name = "spread_data_box";
            this.spread_data_box.Size = new System.Drawing.Size(234, 20);
            this.spread_data_box.TabIndex = 7;
            this.spread_data_box.Text = "Spread Data (improves readability)";
            this.spread_data_box.UseVisualStyleBackColor = false;
            this.spread_data_box.CheckedChanged += new System.EventHandler(this.spread_data_box_CheckedChanged);
            // 
            // cmbManual
            // 
            checkBoxProperties1.FlatAppearanceBorderColor = System.Drawing.Color.Empty;
            checkBoxProperties1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbManual.CheckBoxProperties = checkBoxProperties1;
            this.cmbManual.DisplayMemberSingleItem = "";
            this.cmbManual.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbManual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbManual.FormattingEnabled = true;
            this.cmbManual.Location = new System.Drawing.Point(765, 256);
            this.cmbManual.Name = "cmbManual";
            this.cmbManual.Size = new System.Drawing.Size(201, 21);
            this.cmbManual.TabIndex = 8;
            this.cmbManual.SelectedIndexChanged += new System.EventHandler(this.cmbManual_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(762, 235);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(114, 16);
            this.label8.TabIndex = 104;
            this.label8.Text = "Group Categories";
            // 
            // reset
            // 
            this.reset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.reset.FlatAppearance.BorderSize = 0;
            this.reset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reset.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reset.ForeColor = System.Drawing.Color.White;
            this.reset.Image = global::Financial_Journal.Properties.Resources.reload;
            this.reset.Location = new System.Drawing.Point(967, 254);
            this.reset.Margin = new System.Windows.Forms.Padding(0);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(23, 22);
            this.reset.TabIndex = 10;
            this.reset.TabStop = false;
            this.reset.UseVisualStyleBackColor = false;
            this.reset.Click += new System.EventHandler(this.search_desc_button_Click);
            // 
            // close_view_button
            // 
            this.close_view_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.close_view_button.FlatAppearance.BorderSize = 0;
            this.close_view_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close_view_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close_view_button.ForeColor = System.Drawing.Color.White;
            this.close_view_button.Image = ((System.Drawing.Image)(resources.GetObject("close_view_button.Image")));
            this.close_view_button.Location = new System.Drawing.Point(963, 29);
            this.close_view_button.Margin = new System.Windows.Forms.Padding(0);
            this.close_view_button.Name = "close_view_button";
            this.close_view_button.Size = new System.Drawing.Size(23, 24);
            this.close_view_button.TabIndex = 89;
            this.close_view_button.TabStop = false;
            this.close_view_button.UseVisualStyleBackColor = false;
            this.close_view_button.Click += new System.EventHandler(this.close_view_button_Click);
            // 
            // open_appearance
            // 
            this.open_appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.open_appearance.FlatAppearance.BorderSize = 0;
            this.open_appearance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open_appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open_appearance.ForeColor = System.Drawing.Color.White;
            this.open_appearance.Image = ((System.Drawing.Image)(resources.GetObject("open_appearance.Image")));
            this.open_appearance.Location = new System.Drawing.Point(708, 30);
            this.open_appearance.Margin = new System.Windows.Forms.Padding(0);
            this.open_appearance.Name = "open_appearance";
            this.open_appearance.Size = new System.Drawing.Size(23, 24);
            this.open_appearance.TabIndex = 88;
            this.open_appearance.TabStop = false;
            this.open_appearance.UseVisualStyleBackColor = false;
            this.open_appearance.Click += new System.EventHandler(this.open_appearance_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 22);
            this.pictureBox1.TabIndex = 70;
            this.pictureBox1.TabStop = false;
            // 
            // group_all
            // 
            this.group_all.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.group_all.FlatAppearance.BorderSize = 0;
            this.group_all.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.group_all.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.group_all.ForeColor = System.Drawing.Color.White;
            this.group_all.Image = global::Financial_Journal.Properties.Resources.group;
            this.group_all.Location = new System.Drawing.Point(740, 255);
            this.group_all.Margin = new System.Windows.Forms.Padding(0);
            this.group_all.Name = "group_all";
            this.group_all.Size = new System.Drawing.Size(23, 22);
            this.group_all.TabIndex = 9;
            this.group_all.TabStop = false;
            this.group_all.UseVisualStyleBackColor = false;
            this.group_all.Click += new System.EventHandler(this.group_all_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Image = global::Financial_Journal.Properties.Resources.question_circular_button;
            this.button3.Location = new System.Drawing.Point(5, 28);
            this.button3.Margin = new System.Windows.Forms.Padding(0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(23, 24);
            this.button3.TabIndex = 83;
            this.button3.TabStop = false;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Expenditures
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(995, 410);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.group_all);
            this.Controls.Add(this.reset);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cmbManual);
            this.Controls.Add(this.spread_data_box);
            this.Controls.Add(this.income_box);
            this.Controls.Add(this.total_text);
            this.Controls.Add(this.show_general_box);
            this.Controls.Add(this.show_expenses_box);
            this.Controls.Add(this.sort_check);
            this.Controls.Add(this.year_box);
            this.Controls.Add(this.month_box);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.view_mode);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.close_view_button);
            this.Controls.Add(this.open_appearance);
            this.Controls.Add(this.rotation_text);
            this.Controls.Add(this.transparency_text);
            this.Controls.Add(this.height_text);
            this.Controls.Add(this.comboBoxShadowStyle);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBoxEdgeType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.close_button);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.trackBar3);
            this.Controls.Add(this.bufferedPanel1);
            this.Controls.Add(this.label7);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Expenditures";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Expenditures";
            this.Load += new System.EventHandler(this.Receipt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            this.bufferedPanel1.ResumeLayout(false);
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
        public AdvancedComboBox comboBoxEdgeType;
        private TrackBar trackBar1;
        private Label label1;
        private Label label3;
        private Label label4;
        private TrackBar trackBar3;
        public AdvancedComboBox comboBoxShadowStyle;
        private Label label6;
        private Label height_text;
        private Label transparency_text;
        private Label rotation_text;
        private Button open_appearance;
        private Button close_view_button;
        public AdvancedComboBox view_mode;
        private Label label2;
        public AdvancedComboBox month_box;
        public AdvancedComboBox year_box;
        private BufferedPanel bufferedPanel1;
        private CheckBox sort_check;
        private CheckBox show_expenses_box;
        private CheckBox show_general_box;
        private Label total_text;
        private Label label7;
        private CheckBox income_box;
        private CheckBox spread_data_box;
        private PresentationControls.CheckBoxComboBox cmbManual;
        private Label label8;
        private Button reset;
        private Button group_all;
        private Button button3;
        private System.Drawing.PieChart.PieChartControl m_panelDrawing;


    }
}


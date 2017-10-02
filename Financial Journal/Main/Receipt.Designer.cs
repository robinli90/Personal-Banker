using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Financial_Journal
{
    partial class Receipt
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
                //Set_Form_Color(Color.Cyan);
                //Set_Form_Color(SystemColors.Highlight);
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                //Set_Form_Color(SystemColors.HotTrack);
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Receipt));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.minimize_button = new System.Windows.Forms.Button();
            this.close_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fILEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fILE2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem21 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem30 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem39 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem22 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem20 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem52 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem53 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem55 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem54 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem34 = new System.Windows.Forms.ToolStripMenuItem();
            this.eDITToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem16 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem32 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.editTest1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem36 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem37 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem56 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem57 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem58 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem41 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem42 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem24 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem27 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem26 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem38 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem25 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.cALCULATORToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem31 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem40 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem29 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem23 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem43 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem44 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem51 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem50 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem49 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem48 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem47 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem46 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem45 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem17 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem19 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem18 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem35 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem28 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem33 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.label6 = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.statusBarPanel = new System.Windows.Forms.Panel();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.statusLabel = new System.Windows.Forms.Label();
            this.spendingLabel = new System.Windows.Forms.Label();
            this.weatherPanel = new System.Windows.Forms.Panel();
            this.weatherIcon = new System.Windows.Forms.PictureBox();
            this.weatherLabel = new System.Windows.Forms.Label();
            this.dateLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.marqueePanel = new System.Windows.Forms.Panel();
            this.marqueeLabel2 = new System.Windows.Forms.Label();
            this.marqueeLabel = new System.Windows.Forms.Label();
            this.marqueeInitial = new System.Windows.Forms.Label();
            this.bufferedPanel1 = new Financial_Journal.BufferedPanel();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.button3 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.quantity = new Financial_Journal.AdvancedComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.payment_type = new Financial_Journal.AdvancedComboBox();
            this.memo_button = new System.Windows.Forms.Button();
            this.Add_button = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.item_price = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.item_desc = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.category_box = new Financial_Journal.AdvancedComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.location_box = new Financial_Journal.AdvancedComboBox();
            this.bufferedPanel2 = new Financial_Journal.BufferedPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.submit_button = new System.Windows.Forms.Button();
            this.tax_override_button = new System.Windows.Forms.Button();
            this.reset_button = new System.Windows.Forms.Button();
            this.order_memo_button = new System.Windows.Forms.Button();
            this.panel2 = new Financial_Journal.BufferedPanel();
            this.menuStrip1.SuspendLayout();
            this.statusBarPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.weatherPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.weatherIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.marqueePanel.SuspendLayout();
            this.bufferedPanel1.SuspendLayout();
            this.bufferedPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(643, -10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(32, 2673);
            this.textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(-31, -223);
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
            this.textBox3.Size = new System.Drawing.Size(2832, 12);
            this.textBox3.TabIndex = 0;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(-90, 208);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(2832, 12);
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
            this.minimize_button.Location = new System.Drawing.Point(600, 1);
            this.minimize_button.Margin = new System.Windows.Forms.Padding(0);
            this.minimize_button.Name = "minimize_button";
            this.minimize_button.Size = new System.Drawing.Size(23, 22);
            this.minimize_button.TabIndex = 14;
            this.minimize_button.TabStop = false;
            this.minimize_button.Text = "--";
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
            this.close_button.Location = new System.Drawing.Point(619, 2);
            this.close_button.Margin = new System.Windows.Forms.Padding(0);
            this.close_button.Name = "close_button";
            this.close_button.Size = new System.Drawing.Size(25, 21);
            this.close_button.TabIndex = 17;
            this.close_button.TabStop = false;
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
            this.label5.Size = new System.Drawing.Size(108, 16);
            this.label5.TabIndex = 23;
            this.label5.Text = "Personal Banker";
            // 
            // textBox5
            // 
            this.textBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox5.Enabled = false;
            this.textBox5.Location = new System.Drawing.Point(-787, -2);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(2832, 27);
            this.textBox5.TabIndex = 24;
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fILEToolStripMenuItem,
            this.eDITToolStripMenuItem,
            this.toolStripMenuItem1,
            this.cALCULATORToolStripMenuItem,
            this.toolStripMenuItem43,
            this.toolStripMenuItem17});
            this.menuStrip1.Location = new System.Drawing.Point(0, 24);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.menuStrip1.Size = new System.Drawing.Size(481, 24);
            this.menuStrip1.TabIndex = 30;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fILEToolStripMenuItem
            // 
            this.fILEToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fILE2ToolStripMenuItem,
            this.toolStripMenuItem21,
            this.toolStripMenuItem14,
            this.toolStripMenuItem13,
            this.toolStripMenuItem30,
            this.toolStripMenuItem52,
            this.toolStripMenuItem34});
            this.fILEToolStripMenuItem.Name = "fILEToolStripMenuItem";
            this.fILEToolStripMenuItem.Size = new System.Drawing.Size(40, 24);
            this.fILEToolStripMenuItem.Text = "FILE";
            this.fILEToolStripMenuItem.Click += new System.EventHandler(this.fILEToolStripMenuItem_Click);
            // 
            // fILE2ToolStripMenuItem
            // 
            this.fILE2ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fILE2ToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fILE2ToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.fILE2ToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.fILE2ToolStripMenuItem.Name = "fILE2ToolStripMenuItem";
            this.fILE2ToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.fILE2ToolStripMenuItem.Text = "Save";
            this.fILE2ToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.fILE2ToolStripMenuItem.Click += new System.EventHandler(this.fILE2ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem21
            // 
            this.toolStripMenuItem21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem21.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem21.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripMenuItem21.Name = "toolStripMenuItem21";
            this.toolStripMenuItem21.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem21.Text = "New Profile";
            this.toolStripMenuItem21.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripMenuItem21.Click += new System.EventHandler(this.toolStripMenuItem21_Click);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem14.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem14.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem14.Text = "Import Profile";
            this.toolStripMenuItem14.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripMenuItem14.Click += new System.EventHandler(this.toolStripMenuItem14_Click);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem13.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem13.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem13.Text = "Export Profile";
            this.toolStripMenuItem13.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripMenuItem13.Click += new System.EventHandler(this.toolStripMenuItem13_Click);
            // 
            // toolStripMenuItem30
            // 
            this.toolStripMenuItem30.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem30.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem30.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem39,
            this.toolStripMenuItem22,
            this.toolStripMenuItem20});
            this.toolStripMenuItem30.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripMenuItem30.Name = "toolStripMenuItem30";
            this.toolStripMenuItem30.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem30.Text = "Cloud Services";
            this.toolStripMenuItem30.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // toolStripMenuItem39
            // 
            this.toolStripMenuItem39.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem39.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem39.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem39.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripMenuItem39.Name = "toolStripMenuItem39";
            this.toolStripMenuItem39.Size = new System.Drawing.Size(164, 22);
            this.toolStripMenuItem39.Text = "Cloud Settings";
            this.toolStripMenuItem39.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripMenuItem39.Click += new System.EventHandler(this.toolStripMenuItem39_Click);
            // 
            // toolStripMenuItem22
            // 
            this.toolStripMenuItem22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem22.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem22.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem22.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripMenuItem22.Name = "toolStripMenuItem22";
            this.toolStripMenuItem22.Size = new System.Drawing.Size(164, 22);
            this.toolStripMenuItem22.Text = "Load from Cloud";
            this.toolStripMenuItem22.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripMenuItem22.Click += new System.EventHandler(this.toolStripMenuItem22_Click_2);
            // 
            // toolStripMenuItem20
            // 
            this.toolStripMenuItem20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem20.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem20.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem20.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripMenuItem20.Name = "toolStripMenuItem20";
            this.toolStripMenuItem20.Size = new System.Drawing.Size(164, 22);
            this.toolStripMenuItem20.Text = "Save to Cloud";
            this.toolStripMenuItem20.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripMenuItem20.Click += new System.EventHandler(this.toolStripMenuItem20_Click_3);
            // 
            // toolStripMenuItem52
            // 
            this.toolStripMenuItem52.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem52.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem52.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem53,
            this.toolStripMenuItem55,
            this.toolStripMenuItem54});
            this.toolStripMenuItem52.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripMenuItem52.Name = "toolStripMenuItem52";
            this.toolStripMenuItem52.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem52.Text = "Mobile Services";
            this.toolStripMenuItem52.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // toolStripMenuItem53
            // 
            this.toolStripMenuItem53.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem53.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem53.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem53.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripMenuItem53.Name = "toolStripMenuItem53";
            this.toolStripMenuItem53.Size = new System.Drawing.Size(186, 22);
            this.toolStripMenuItem53.Text = "Synchronize Orders";
            this.toolStripMenuItem53.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripMenuItem53.Click += new System.EventHandler(this.toolStripMenuItem53_Click);
            // 
            // toolStripMenuItem55
            // 
            this.toolStripMenuItem55.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem55.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem55.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem55.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripMenuItem55.Name = "toolStripMenuItem55";
            this.toolStripMenuItem55.Size = new System.Drawing.Size(186, 22);
            this.toolStripMenuItem55.Text = "Manage Associations";
            this.toolStripMenuItem55.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripMenuItem55.Click += new System.EventHandler(this.toolStripMenuItem55_Click);
            // 
            // toolStripMenuItem54
            // 
            this.toolStripMenuItem54.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem54.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem54.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem54.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripMenuItem54.Name = "toolStripMenuItem54";
            this.toolStripMenuItem54.Size = new System.Drawing.Size(186, 22);
            this.toolStripMenuItem54.Text = "Settings";
            this.toolStripMenuItem54.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripMenuItem54.Click += new System.EventHandler(this.toolStripMenuItem54_Click);
            // 
            // toolStripMenuItem34
            // 
            this.toolStripMenuItem34.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem34.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem34.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.toolStripMenuItem34.Name = "toolStripMenuItem34";
            this.toolStripMenuItem34.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem34.Text = "Exit Without Saving";
            this.toolStripMenuItem34.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripMenuItem34.Click += new System.EventHandler(this.toolStripMenuItem34_Click);
            // 
            // eDITToolStripMenuItem
            // 
            this.eDITToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.eDITToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem16,
            this.toolStripMenuItem32,
            this.toolStripMenuItem11,
            this.editTest1ToolStripMenuItem,
            this.toolStripMenuItem36});
            this.eDITToolStripMenuItem.Name = "eDITToolStripMenuItem";
            this.eDITToolStripMenuItem.Size = new System.Drawing.Size(68, 24);
            this.eDITToolStripMenuItem.Text = "REPORTS";
            this.eDITToolStripMenuItem.Click += new System.EventHandler(this.eDITToolStripMenuItem_Click);
            // 
            // toolStripMenuItem16
            // 
            this.toolStripMenuItem16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem16.Enabled = false;
            this.toolStripMenuItem16.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            this.toolStripMenuItem16.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItem16.Text = "Analysis";
            this.toolStripMenuItem16.Visible = false;
            this.toolStripMenuItem16.Click += new System.EventHandler(this.toolStripMenuItem16_Click);
            // 
            // toolStripMenuItem32
            // 
            this.toolStripMenuItem32.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem32.Enabled = false;
            this.toolStripMenuItem32.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem32.Name = "toolStripMenuItem32";
            this.toolStripMenuItem32.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItem32.Text = "Online Orders";
            this.toolStripMenuItem32.Click += new System.EventHandler(this.toolStripMenuItem32_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem11.Enabled = false;
            this.toolStripMenuItem11.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItem11.Text = "Purchases";
            this.toolStripMenuItem11.Click += new System.EventHandler(this.toolStripMenuItem11_Click);
            // 
            // editTest1ToolStripMenuItem
            // 
            this.editTest1ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.editTest1ToolStripMenuItem.Enabled = false;
            this.editTest1ToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.editTest1ToolStripMenuItem.Name = "editTest1ToolStripMenuItem";
            this.editTest1ToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.editTest1ToolStripMenuItem.Text = "Refunds";
            this.editTest1ToolStripMenuItem.Click += new System.EventHandler(this.editTest1ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem36
            // 
            this.toolStripMenuItem36.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem36.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem37,
            this.toolStripMenuItem56,
            this.toolStripMenuItem41});
            this.toolStripMenuItem36.Enabled = false;
            this.toolStripMenuItem36.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem36.Name = "toolStripMenuItem36";
            this.toolStripMenuItem36.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItem36.Text = "Spreadsheets";
            this.toolStripMenuItem36.Click += new System.EventHandler(this.toolStripMenuItem36_Click);
            // 
            // toolStripMenuItem37
            // 
            this.toolStripMenuItem37.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem37.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem37.Name = "toolStripMenuItem37";
            this.toolStripMenuItem37.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem37.Text = "Cash Flow";
            this.toolStripMenuItem37.Click += new System.EventHandler(this.toolStripMenuItem37_Click);
            // 
            // toolStripMenuItem56
            // 
            this.toolStripMenuItem56.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem56.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem57,
            this.toolStripMenuItem58});
            this.toolStripMenuItem56.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem56.Name = "toolStripMenuItem56";
            this.toolStripMenuItem56.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem56.Text = "Categorical ";
            // 
            // toolStripMenuItem57
            // 
            this.toolStripMenuItem57.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem57.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem57.Name = "toolStripMenuItem57";
            this.toolStripMenuItem57.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem57.Text = "Group Settings";
            this.toolStripMenuItem57.Click += new System.EventHandler(this.toolStripMenuItem57_Click);
            // 
            // toolStripMenuItem58
            // 
            this.toolStripMenuItem58.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem58.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem58.Name = "toolStripMenuItem58";
            this.toolStripMenuItem58.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem58.Text = "Spending";
            this.toolStripMenuItem58.Click += new System.EventHandler(this.toolStripMenuItem58_Click);
            // 
            // toolStripMenuItem41
            // 
            this.toolStripMenuItem41.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem41.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem41.Name = "toolStripMenuItem41";
            this.toolStripMenuItem41.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem41.Text = "Item Summary";
            this.toolStripMenuItem41.Click += new System.EventHandler(this.toolStripMenuItem41_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem42,
            this.toolStripMenuItem5,
            this.toolStripMenuItem2,
            this.toolStripMenuItem15,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(71, 24);
            this.toolStripMenuItem1.Text = "SETTINGS";
            // 
            // toolStripMenuItem42
            // 
            this.toolStripMenuItem42.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem42.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem42.Name = "toolStripMenuItem42";
            this.toolStripMenuItem42.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem42.Text = "Expiration Settings";
            this.toolStripMenuItem42.Click += new System.EventHandler(this.toolStripMenuItem42_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem5.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem5.Text = "Link Settings";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem24,
            this.toolStripMenuItem7,
            this.toolStripMenuItem27,
            this.toolStripMenuItem26,
            this.toolStripMenuItem38,
            this.toolStripMenuItem25});
            this.toolStripMenuItem2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem2.Text = "Preferences";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem24
            // 
            this.toolStripMenuItem24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem24.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem24.Name = "toolStripMenuItem24";
            this.toolStripMenuItem24.Size = new System.Drawing.Size(219, 22);
            this.toolStripMenuItem24.Text = "Alerts and Windows";
            this.toolStripMenuItem24.Click += new System.EventHandler(this.toolStripMenuItem24_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem7.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(219, 22);
            this.toolStripMenuItem7.Text = "Appearance";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // toolStripMenuItem27
            // 
            this.toolStripMenuItem27.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem27.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem27.Name = "toolStripMenuItem27";
            this.toolStripMenuItem27.Size = new System.Drawing.Size(219, 22);
            this.toolStripMenuItem27.Text = "Backup and Saving Settings";
            this.toolStripMenuItem27.Click += new System.EventHandler(this.toolStripMenuItem27_Click);
            // 
            // toolStripMenuItem26
            // 
            this.toolStripMenuItem26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem26.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem26.Name = "toolStripMenuItem26";
            this.toolStripMenuItem26.Size = new System.Drawing.Size(219, 22);
            this.toolStripMenuItem26.Text = "Personal Information";
            this.toolStripMenuItem26.Click += new System.EventHandler(this.toolStripMenuItem26_Click);
            // 
            // toolStripMenuItem38
            // 
            this.toolStripMenuItem38.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem38.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem38.Name = "toolStripMenuItem38";
            this.toolStripMenuItem38.Size = new System.Drawing.Size(219, 22);
            this.toolStripMenuItem38.Text = "Quick Links Settings";
            this.toolStripMenuItem38.Click += new System.EventHandler(this.toolStripMenuItem38_Click);
            // 
            // toolStripMenuItem25
            // 
            this.toolStripMenuItem25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem25.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem25.Name = "toolStripMenuItem25";
            this.toolStripMenuItem25.Size = new System.Drawing.Size(219, 22);
            this.toolStripMenuItem25.Text = "Security";
            this.toolStripMenuItem25.Click += new System.EventHandler(this.toolStripMenuItem25_Click);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem15.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem15.Text = "Refund Settings";
            this.toolStripMenuItem15.Click += new System.EventHandler(this.toolStripMenuItem15_Click_1);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem3.Text = "Tax Settings";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem4.Text = "View Configuration";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // cALCULATORToolStripMenuItem
            // 
            this.cALCULATORToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem31,
            this.toolStripMenuItem9,
            this.toolStripMenuItem40,
            this.toolStripMenuItem29,
            this.toolStripMenuItem23,
            this.toolStripMenuItem10});
            this.cALCULATORToolStripMenuItem.Name = "cALCULATORToolStripMenuItem";
            this.cALCULATORToolStripMenuItem.Size = new System.Drawing.Size(117, 24);
            this.cALCULATORToolStripMenuItem.Text = "PERSONAL TOOLS";
            this.cALCULATORToolStripMenuItem.Click += new System.EventHandler(this.cALCULATORToolStripMenuItem_Click);
            // 
            // toolStripMenuItem31
            // 
            this.toolStripMenuItem31.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem31.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem31.Name = "toolStripMenuItem31";
            this.toolStripMenuItem31.Size = new System.Drawing.Size(184, 22);
            this.toolStripMenuItem31.Text = "Agenda";
            this.toolStripMenuItem31.Click += new System.EventHandler(this.toolStripMenuItem31_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem9.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(184, 22);
            this.toolStripMenuItem9.Text = "Asset Manager";
            this.toolStripMenuItem9.Click += new System.EventHandler(this.toolStripMenuItem9_Click);
            // 
            // toolStripMenuItem40
            // 
            this.toolStripMenuItem40.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem40.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem40.Name = "toolStripMenuItem40";
            this.toolStripMenuItem40.Size = new System.Drawing.Size(184, 22);
            this.toolStripMenuItem40.Text = "Contacts";
            this.toolStripMenuItem40.Click += new System.EventHandler(this.toolStripMenuItem40_Click);
            // 
            // toolStripMenuItem29
            // 
            this.toolStripMenuItem29.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem29.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem29.Name = "toolStripMenuItem29";
            this.toolStripMenuItem29.Size = new System.Drawing.Size(184, 22);
            this.toolStripMenuItem29.Text = "Hobby Management";
            this.toolStripMenuItem29.Click += new System.EventHandler(this.toolStripMenuItem29_Click);
            // 
            // toolStripMenuItem23
            // 
            this.toolStripMenuItem23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem23.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem23.Name = "toolStripMenuItem23";
            this.toolStripMenuItem23.Size = new System.Drawing.Size(184, 22);
            this.toolStripMenuItem23.Text = "Personal Calendar";
            this.toolStripMenuItem23.Click += new System.EventHandler(this.toolStripMenuItem23_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem10.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(184, 22);
            this.toolStripMenuItem10.Text = "SMS Alerts";
            this.toolStripMenuItem10.Click += new System.EventHandler(this.toolStripMenuItem10_Click);
            // 
            // toolStripMenuItem43
            // 
            this.toolStripMenuItem43.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem44,
            this.toolStripMenuItem6,
            this.toolStripMenuItem51,
            this.toolStripMenuItem50,
            this.toolStripMenuItem49,
            this.toolStripMenuItem48,
            this.toolStripMenuItem47,
            this.toolStripMenuItem46,
            this.toolStripMenuItem45});
            this.toolStripMenuItem43.Name = "toolStripMenuItem43";
            this.toolStripMenuItem43.Size = new System.Drawing.Size(119, 24);
            this.toolStripMenuItem43.Text = "FINANCIAL TOOLS";
            // 
            // toolStripMenuItem44
            // 
            this.toolStripMenuItem44.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem44.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem44.Name = "toolStripMenuItem44";
            this.toolStripMenuItem44.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItem44.Text = "Accounts R/P";
            this.toolStripMenuItem44.Click += new System.EventHandler(this.toolStripMenuItem44_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem6.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItem6.Text = "Budget Allocations";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click);
            // 
            // toolStripMenuItem51
            // 
            this.toolStripMenuItem51.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem51.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem51.Name = "toolStripMenuItem51";
            this.toolStripMenuItem51.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItem51.Text = "Expenditure Warnings";
            this.toolStripMenuItem51.Click += new System.EventHandler(this.toolStripMenuItem51_Click);
            // 
            // toolStripMenuItem50
            // 
            this.toolStripMenuItem50.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem50.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem50.Name = "toolStripMenuItem50";
            this.toolStripMenuItem50.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItem50.Text = "Investments";
            this.toolStripMenuItem50.Click += new System.EventHandler(this.toolStripMenuItem50_Click);
            // 
            // toolStripMenuItem49
            // 
            this.toolStripMenuItem49.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem49.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem49.Name = "toolStripMenuItem49";
            this.toolStripMenuItem49.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItem49.Text = "Manage Gift Cards";
            this.toolStripMenuItem49.Click += new System.EventHandler(this.toolStripMenuItem49_Click);
            // 
            // toolStripMenuItem48
            // 
            this.toolStripMenuItem48.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem48.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem48.Name = "toolStripMenuItem48";
            this.toolStripMenuItem48.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItem48.Text = "Payment Options";
            this.toolStripMenuItem48.Click += new System.EventHandler(this.toolStripMenuItem48_Click);
            // 
            // toolStripMenuItem47
            // 
            this.toolStripMenuItem47.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem47.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem47.Name = "toolStripMenuItem47";
            this.toolStripMenuItem47.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItem47.Text = "Personal Income";
            this.toolStripMenuItem47.Click += new System.EventHandler(this.toolStripMenuItem47_Click);
            // 
            // toolStripMenuItem46
            // 
            this.toolStripMenuItem46.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem46.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem46.Name = "toolStripMenuItem46";
            this.toolStripMenuItem46.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItem46.Text = "Recurring Expenses";
            this.toolStripMenuItem46.Click += new System.EventHandler(this.toolStripMenuItem46_Click);
            // 
            // toolStripMenuItem45
            // 
            this.toolStripMenuItem45.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem45.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem45.Name = "toolStripMenuItem45";
            this.toolStripMenuItem45.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItem45.Text = "Savings Helper";
            this.toolStripMenuItem45.Click += new System.EventHandler(this.toolStripMenuItem45_Click);
            // 
            // toolStripMenuItem17
            // 
            this.toolStripMenuItem17.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem19,
            this.toolStripMenuItem12,
            this.toolStripMenuItem18,
            this.toolStripMenuItem35,
            this.toolStripMenuItem28,
            this.toolStripMenuItem33});
            this.toolStripMenuItem17.Name = "toolStripMenuItem17";
            this.toolStripMenuItem17.Size = new System.Drawing.Size(63, 24);
            this.toolStripMenuItem17.Text = "VISUALS";
            this.toolStripMenuItem17.Click += new System.EventHandler(this.toolStripMenuItem17_Click);
            // 
            // toolStripMenuItem19
            // 
            this.toolStripMenuItem19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem19.Enabled = false;
            this.toolStripMenuItem19.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem19.Name = "toolStripMenuItem19";
            this.toolStripMenuItem19.Size = new System.Drawing.Size(174, 22);
            this.toolStripMenuItem19.Text = "Categorical Graphs";
            this.toolStripMenuItem19.Click += new System.EventHandler(this.toolStripMenuItem19_Click);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem12.Enabled = false;
            this.toolStripMenuItem12.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(174, 22);
            this.toolStripMenuItem12.Text = "Expenditures";
            this.toolStripMenuItem12.Click += new System.EventHandler(this.toolStripMenuItem12_Click);
            // 
            // toolStripMenuItem18
            // 
            this.toolStripMenuItem18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem18.Enabled = false;
            this.toolStripMenuItem18.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem18.Name = "toolStripMenuItem18";
            this.toolStripMenuItem18.Size = new System.Drawing.Size(174, 22);
            this.toolStripMenuItem18.Text = "Month-To-Month";
            this.toolStripMenuItem18.Click += new System.EventHandler(this.toolStripMenuItem18_Click);
            // 
            // toolStripMenuItem35
            // 
            this.toolStripMenuItem35.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem35.Enabled = false;
            this.toolStripMenuItem35.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem35.Name = "toolStripMenuItem35";
            this.toolStripMenuItem35.Size = new System.Drawing.Size(174, 22);
            this.toolStripMenuItem35.Text = "Spending Timeline";
            this.toolStripMenuItem35.Click += new System.EventHandler(this.toolStripMenuItem35_Click_1);
            // 
            // toolStripMenuItem28
            // 
            this.toolStripMenuItem28.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem28.Enabled = false;
            this.toolStripMenuItem28.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem28.Name = "toolStripMenuItem28";
            this.toolStripMenuItem28.Size = new System.Drawing.Size(174, 22);
            this.toolStripMenuItem28.Text = "Tier Visualizer";
            this.toolStripMenuItem28.Click += new System.EventHandler(this.toolStripMenuItem28_Click);
            // 
            // toolStripMenuItem33
            // 
            this.toolStripMenuItem33.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem33.Enabled = false;
            this.toolStripMenuItem33.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem33.Name = "toolStripMenuItem33";
            this.toolStripMenuItem33.Size = new System.Drawing.Size(174, 22);
            this.toolStripMenuItem33.Text = "Wealth Visualizer";
            this.toolStripMenuItem33.Click += new System.EventHandler(this.toolStripMenuItem33_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripMenuItem8.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(113, 24);
            this.toolStripMenuItem8.Text = "Personal Finances";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(-51, -5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 16);
            this.label6.TabIndex = 26;
            this.label6.Text = "Quantity";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Personal Banker";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // statusBarPanel
            // 
            this.statusBarPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.statusBarPanel.Controls.Add(this.statusPanel);
            this.statusBarPanel.Controls.Add(this.spendingLabel);
            this.statusBarPanel.Controls.Add(this.weatherPanel);
            this.statusBarPanel.Controls.Add(this.dateLabel);
            this.statusBarPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusBarPanel.Location = new System.Drawing.Point(0, 22);
            this.statusBarPanel.Name = "statusBarPanel";
            this.statusBarPanel.Size = new System.Drawing.Size(648, 25);
            this.statusBarPanel.TabIndex = 35;
            // 
            // statusPanel
            // 
            this.statusPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.statusPanel.Controls.Add(this.statusLabel);
            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusPanel.Location = new System.Drawing.Point(0, 0);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(239, 23);
            this.statusPanel.TabIndex = 21;
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = System.Drawing.Color.Silver;
            this.statusLabel.Location = new System.Drawing.Point(0, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Padding = new System.Windows.Forms.Padding(4);
            this.statusLabel.Size = new System.Drawing.Size(237, 21);
            this.statusLabel.TabIndex = 14;
            // 
            // spendingLabel
            // 
            this.spendingLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.spendingLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spendingLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.spendingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spendingLabel.ForeColor = System.Drawing.Color.Silver;
            this.spendingLabel.Location = new System.Drawing.Point(239, 0);
            this.spendingLabel.Margin = new System.Windows.Forms.Padding(0);
            this.spendingLabel.Name = "spendingLabel";
            this.spendingLabel.Padding = new System.Windows.Forms.Padding(4);
            this.spendingLabel.Size = new System.Drawing.Size(216, 23);
            this.spendingLabel.TabIndex = 20;
            this.spendingLabel.Click += new System.EventHandler(this.spendingLabel_Click);
            // 
            // weatherPanel
            // 
            this.weatherPanel.AutoSize = true;
            this.weatherPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.weatherPanel.Controls.Add(this.weatherIcon);
            this.weatherPanel.Controls.Add(this.weatherLabel);
            this.weatherPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.weatherPanel.Location = new System.Drawing.Point(455, 0);
            this.weatherPanel.Margin = new System.Windows.Forms.Padding(0);
            this.weatherPanel.Name = "weatherPanel";
            this.weatherPanel.Padding = new System.Windows.Forms.Padding(1);
            this.weatherPanel.Size = new System.Drawing.Size(181, 23);
            this.weatherPanel.TabIndex = 19;
            // 
            // weatherIcon
            // 
            this.weatherIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.weatherIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.weatherIcon.Location = new System.Drawing.Point(1, 1);
            this.weatherIcon.Name = "weatherIcon";
            this.weatherIcon.Padding = new System.Windows.Forms.Padding(8, 1, 1, 1);
            this.weatherIcon.Size = new System.Drawing.Size(32, 19);
            this.weatherIcon.TabIndex = 18;
            this.weatherIcon.TabStop = false;
            // 
            // weatherLabel
            // 
            this.weatherLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.weatherLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.weatherLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.weatherLabel.ForeColor = System.Drawing.Color.Silver;
            this.weatherLabel.Location = new System.Drawing.Point(33, 1);
            this.weatherLabel.Margin = new System.Windows.Forms.Padding(0);
            this.weatherLabel.Name = "weatherLabel";
            this.weatherLabel.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.weatherLabel.Size = new System.Drawing.Size(145, 19);
            this.weatherLabel.TabIndex = 16;
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dateLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dateLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.dateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateLabel.ForeColor = System.Drawing.Color.Silver;
            this.dateLabel.Location = new System.Drawing.Point(636, 0);
            this.dateLabel.Margin = new System.Windows.Forms.Padding(0);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Padding = new System.Windows.Forms.Padding(4);
            this.dateLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dateLabel.Size = new System.Drawing.Size(10, 23);
            this.dateLabel.TabIndex = 15;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 22);
            this.pictureBox1.TabIndex = 22;
            this.pictureBox1.TabStop = false;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bottomPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bottomPanel.Controls.Add(this.marqueePanel);
            this.bottomPanel.Controls.Add(this.statusBarPanel);
            this.bottomPanel.Location = new System.Drawing.Point(-5, 161);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(650, 49);
            this.bottomPanel.TabIndex = 165;
            // 
            // marqueePanel
            // 
            this.marqueePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.marqueePanel.Controls.Add(this.marqueeLabel2);
            this.marqueePanel.Controls.Add(this.marqueeLabel);
            this.marqueePanel.Controls.Add(this.marqueeInitial);
            this.marqueePanel.Location = new System.Drawing.Point(0, 0);
            this.marqueePanel.Name = "marqueePanel";
            this.marqueePanel.Size = new System.Drawing.Size(648, 23);
            this.marqueePanel.TabIndex = 166;
            // 
            // marqueeLabel2
            // 
            this.marqueeLabel2.AutoSize = true;
            this.marqueeLabel2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.marqueeLabel2.Location = new System.Drawing.Point(2005, 4);
            this.marqueeLabel2.Name = "marqueeLabel2";
            this.marqueeLabel2.Size = new System.Drawing.Size(0, 13);
            this.marqueeLabel2.TabIndex = 1;
            // 
            // marqueeLabel
            // 
            this.marqueeLabel.AutoSize = true;
            this.marqueeLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.marqueeLabel.Location = new System.Drawing.Point(643, 4);
            this.marqueeLabel.Name = "marqueeLabel";
            this.marqueeLabel.Size = new System.Drawing.Size(0, 13);
            this.marqueeLabel.TabIndex = 0;
            // 
            // marqueeInitial
            // 
            this.marqueeInitial.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.marqueeInitial.Location = new System.Drawing.Point(4, -2);
            this.marqueeInitial.Name = "marqueeInitial";
            this.marqueeInitial.Size = new System.Drawing.Size(644, 24);
            this.marqueeInitial.TabIndex = 2;
            this.marqueeInitial.Text = "Loading currency and quotes database...";
            this.marqueeInitial.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bufferedPanel1
            // 
            this.bufferedPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bufferedPanel1.Controls.Add(this.label14);
            this.bufferedPanel1.Controls.Add(this.label13);
            this.bufferedPanel1.Controls.Add(this.button6);
            this.bufferedPanel1.Controls.Add(this.button5);
            this.bufferedPanel1.Controls.Add(this.dateTimePicker1);
            this.bufferedPanel1.Controls.Add(this.button3);
            this.bufferedPanel1.Controls.Add(this.label8);
            this.bufferedPanel1.Controls.Add(this.quantity);
            this.bufferedPanel1.Controls.Add(this.label7);
            this.bufferedPanel1.Controls.Add(this.payment_type);
            this.bufferedPanel1.Controls.Add(this.memo_button);
            this.bufferedPanel1.Controls.Add(this.Add_button);
            this.bufferedPanel1.Controls.Add(this.button1);
            this.bufferedPanel1.Controls.Add(this.item_price);
            this.bufferedPanel1.Controls.Add(this.label4);
            this.bufferedPanel1.Controls.Add(this.item_desc);
            this.bufferedPanel1.Controls.Add(this.label3);
            this.bufferedPanel1.Controls.Add(this.button2);
            this.bufferedPanel1.Controls.Add(this.label2);
            this.bufferedPanel1.Controls.Add(this.category_box);
            this.bufferedPanel1.Controls.Add(this.label1);
            this.bufferedPanel1.Controls.Add(this.location_box);
            this.bufferedPanel1.Location = new System.Drawing.Point(5, 53);
            this.bufferedPanel1.Name = "bufferedPanel1";
            this.bufferedPanel1.Size = new System.Drawing.Size(634, 106);
            this.bufferedPanel1.TabIndex = 33;
            this.bufferedPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.bufferedPanel1_Paint);
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label14.Location = new System.Drawing.Point(579, 86);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(51, 12);
            this.label14.TabIndex = 330;
            this.label14.Text = "ADD ITEM";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label13.Location = new System.Drawing.Point(577, 36);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(50, 12);
            this.label13.TabIndex = 329;
            this.label13.Text = "SETTINGS";
            // 
            // button6
            // 
            this.button6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button6.Image = global::Financial_Journal.Properties.Resources.settings;
            this.button6.Location = new System.Drawing.Point(583, 0);
            this.button6.Margin = new System.Windows.Forms.Padding(1);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(36, 39);
            this.button6.TabIndex = 37;
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button5.Enabled = false;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Image = global::Financial_Journal.Properties.Resources.gift;
            this.button5.Location = new System.Drawing.Point(272, 63);
            this.button5.Margin = new System.Windows.Forms.Padding(0);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(23, 24);
            this.button5.TabIndex = 36;
            this.button5.TabStop = false;
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarMonthBackground = System.Drawing.SystemColors.HotTrack;
            this.dateTimePicker1.Enabled = false;
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(361, 66);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(99, 20);
            this.dateTimePicker1.TabIndex = 35;
            this.dateTimePicker1.Value = new System.DateTime(2016, 8, 13, 18, 48, 56, 0);
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Image = global::Financial_Journal.Properties.Resources.add_calendar;
            this.button3.Location = new System.Drawing.Point(325, 61);
            this.button3.Margin = new System.Windows.Forms.Padding(0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(29, 29);
            this.button3.TabIndex = 34;
            this.button3.TabStop = false;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(439, 39);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 16);
            this.label8.TabIndex = 32;
            this.label8.Text = "Quantity";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // quantity
            // 
            this.quantity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.quantity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.quantity.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.quantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.quantity.ForeColor = System.Drawing.Color.White;
            this.quantity.FormattingEnabled = true;
            this.quantity.FrameColor = System.Drawing.SystemColors.ControlLightLight;
            this.quantity.HighlightColor = System.Drawing.Color.Gray;
            this.quantity.Location = new System.Drawing.Point(500, 36);
            this.quantity.Name = "quantity";
            this.quantity.Size = new System.Drawing.Size(58, 23);
            this.quantity.TabIndex = 5;
            this.quantity.SelectedIndexChanged += new System.EventHandler(this.quantity_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.Control;
            this.label7.Location = new System.Drawing.Point(16, 66);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 16);
            this.label7.TabIndex = 31;
            this.label7.Text = "Payment";
            // 
            // payment_type
            // 
            this.payment_type.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.payment_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.payment_type.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.payment_type.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.payment_type.ForeColor = System.Drawing.Color.White;
            this.payment_type.FormattingEnabled = true;
            this.payment_type.FrameColor = System.Drawing.SystemColors.ControlLightLight;
            this.payment_type.HighlightColor = System.Drawing.Color.Gray;
            this.payment_type.Location = new System.Drawing.Point(84, 64);
            this.payment_type.Name = "payment_type";
            this.payment_type.Size = new System.Drawing.Size(182, 23);
            this.payment_type.TabIndex = 2;
            // 
            // memo_button
            // 
            this.memo_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.memo_button.FlatAppearance.BorderSize = 0;
            this.memo_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.memo_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memo_button.ForeColor = System.Drawing.Color.White;
            this.memo_button.Image = global::Financial_Journal.Properties.Resources.memo;
            this.memo_button.Location = new System.Drawing.Point(538, 7);
            this.memo_button.Margin = new System.Windows.Forms.Padding(0);
            this.memo_button.Name = "memo_button";
            this.memo_button.Size = new System.Drawing.Size(23, 20);
            this.memo_button.TabIndex = 20;
            this.memo_button.TabStop = false;
            this.memo_button.UseVisualStyleBackColor = false;
            this.memo_button.Click += new System.EventHandler(this.memo_button_Click);
            // 
            // Add_button
            // 
            this.Add_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Add_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Add_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Add_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Add_button.Image = global::Financial_Journal.Properties.Resources.add_notes;
            this.Add_button.Location = new System.Drawing.Point(587, 51);
            this.Add_button.Margin = new System.Windows.Forms.Padding(1);
            this.Add_button.Name = "Add_button";
            this.Add_button.Size = new System.Drawing.Size(36, 38);
            this.Add_button.TabIndex = 6;
            this.Add_button.UseVisualStyleBackColor = true;
            this.Add_button.Click += new System.EventHandler(this.start_button_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Image = global::Financial_Journal.Properties.Resources.add;
            this.button1.Location = new System.Drawing.Point(272, 3);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 24);
            this.button1.TabIndex = 15;
            this.button1.TabStop = false;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // item_price
            // 
            this.item_price.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.item_price.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.item_price.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.item_price.ForeColor = System.Drawing.Color.White;
            this.item_price.Location = new System.Drawing.Point(361, 36);
            this.item_price.MaxLength = 10;
            this.item_price.Name = "item_price";
            this.item_price.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.item_price.Size = new System.Drawing.Size(71, 22);
            this.item_price.TabIndex = 4;
            this.item_price.Text = "$";
            this.item_price.TextChanged += new System.EventHandler(this.textBox6_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(315, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "Price";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // item_desc
            // 
            this.item_desc.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.item_desc.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.item_desc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.item_desc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.item_desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.item_desc.ForeColor = System.Drawing.Color.White;
            this.item_desc.Location = new System.Drawing.Point(361, 6);
            this.item_desc.MaxLength = 30;
            this.item_desc.Name = "item_desc";
            this.item_desc.Size = new System.Drawing.Size(171, 22);
            this.item_desc.TabIndex = 3;
            this.item_desc.TextChanged += new System.EventHandler(this.item_desc_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(321, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Item";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Image = global::Financial_Journal.Properties.Resources.add;
            this.button2.Location = new System.Drawing.Point(272, 33);
            this.button2.Margin = new System.Windows.Forms.Padding(0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(23, 24);
            this.button2.TabIndex = 16;
            this.button2.TabStop = false;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(11, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 16);
            this.label2.TabIndex = 12;
            this.label2.Text = "Category";
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
            this.category_box.Location = new System.Drawing.Point(84, 35);
            this.category_box.Name = "category_box";
            this.category_box.Size = new System.Drawing.Size(182, 23);
            this.category_box.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(16, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 16);
            this.label1.TabIndex = 13;
            this.label1.Text = "Location";
            // 
            // location_box
            // 
            this.location_box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(72)))), ((int)(((byte)(72)))));
            this.location_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.location_box.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.location_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.location_box.ForeColor = System.Drawing.Color.White;
            this.location_box.FormattingEnabled = true;
            this.location_box.FrameColor = System.Drawing.Color.SandyBrown;
            this.location_box.HighlightColor = System.Drawing.Color.Gray;
            this.location_box.Location = new System.Drawing.Point(84, 5);
            this.location_box.Name = "location_box";
            this.location_box.Size = new System.Drawing.Size(182, 23);
            this.location_box.TabIndex = 0;
            this.location_box.SelectedIndexChanged += new System.EventHandler(this.location_box_SelectedIndexChanged);
            // 
            // bufferedPanel2
            // 
            this.bufferedPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bufferedPanel2.Controls.Add(this.label12);
            this.bufferedPanel2.Controls.Add(this.label11);
            this.bufferedPanel2.Controls.Add(this.label10);
            this.bufferedPanel2.Controls.Add(this.label9);
            this.bufferedPanel2.Controls.Add(this.label16);
            this.bufferedPanel2.Controls.Add(this.button4);
            this.bufferedPanel2.Controls.Add(this.submit_button);
            this.bufferedPanel2.Controls.Add(this.tax_override_button);
            this.bufferedPanel2.Controls.Add(this.reset_button);
            this.bufferedPanel2.Controls.Add(this.order_memo_button);
            this.bufferedPanel2.Location = new System.Drawing.Point(334, 91);
            this.bufferedPanel2.Name = "bufferedPanel2";
            this.bufferedPanel2.Size = new System.Drawing.Size(309, 66);
            this.bufferedPanel2.TabIndex = 34;
            this.bufferedPanel2.Visible = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label12.Location = new System.Drawing.Point(259, 45);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 13);
            this.label12.TabIndex = 164;
            this.label12.Text = "SUBMIT";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label11.Location = new System.Drawing.Point(199, 45);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 13);
            this.label11.TabIndex = 164;
            this.label11.Text = "TAXES";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label10.Location = new System.Drawing.Point(140, 45);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 13);
            this.label10.TabIndex = 164;
            this.label10.Text = "MEMO";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label9.Location = new System.Drawing.Point(78, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 163;
            this.label9.Text = "RESET";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label16.Location = new System.Drawing.Point(7, 44);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 13);
            this.label16.TabIndex = 163;
            this.label16.Text = "TRACKING";
            // 
            // button4
            // 
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button4.Image = global::Financial_Journal.Properties.Resources.paper_airplane_outline;
            this.button4.Location = new System.Drawing.Point(11, 3);
            this.button4.Margin = new System.Windows.Forms.Padding(1);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(49, 44);
            this.button4.TabIndex = 45;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // submit_button
            // 
            this.submit_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.submit_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.submit_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.submit_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.submit_button.Image = global::Financial_Journal.Properties.Resources.checkout;
            this.submit_button.Location = new System.Drawing.Point(256, 3);
            this.submit_button.Margin = new System.Windows.Forms.Padding(1);
            this.submit_button.Name = "submit_button";
            this.submit_button.Size = new System.Drawing.Size(49, 44);
            this.submit_button.TabIndex = 44;
            this.submit_button.UseVisualStyleBackColor = true;
            this.submit_button.Click += new System.EventHandler(this.submit_button_Click);
            // 
            // tax_override_button
            // 
            this.tax_override_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tax_override_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tax_override_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tax_override_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tax_override_button.Image = global::Financial_Journal.Properties.Resources.tax;
            this.tax_override_button.Location = new System.Drawing.Point(197, 3);
            this.tax_override_button.Margin = new System.Windows.Forms.Padding(1);
            this.tax_override_button.Name = "tax_override_button";
            this.tax_override_button.Size = new System.Drawing.Size(49, 44);
            this.tax_override_button.TabIndex = 43;
            this.tax_override_button.UseVisualStyleBackColor = true;
            this.tax_override_button.Click += new System.EventHandler(this.tax_override_button_Click);
            // 
            // reset_button
            // 
            this.reset_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.reset_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reset_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reset_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.reset_button.Image = global::Financial_Journal.Properties.Resources.bar_chart_reload;
            this.reset_button.Location = new System.Drawing.Point(75, 3);
            this.reset_button.Margin = new System.Windows.Forms.Padding(1);
            this.reset_button.Name = "reset_button";
            this.reset_button.Size = new System.Drawing.Size(49, 44);
            this.reset_button.TabIndex = 41;
            this.reset_button.UseVisualStyleBackColor = true;
            this.reset_button.Click += new System.EventHandler(this.reset_button_Click);
            // 
            // order_memo_button
            // 
            this.order_memo_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.order_memo_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.order_memo_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.order_memo_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.order_memo_button.Image = global::Financial_Journal.Properties.Resources.notebook;
            this.order_memo_button.Location = new System.Drawing.Point(134, 3);
            this.order_memo_button.Margin = new System.Windows.Forms.Padding(1);
            this.order_memo_button.Name = "order_memo_button";
            this.order_memo_button.Size = new System.Drawing.Size(49, 44);
            this.order_memo_button.TabIndex = 42;
            this.order_memo_button.UseVisualStyleBackColor = true;
            this.order_memo_button.Click += new System.EventHandler(this.order_memo_button_Click);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(79, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(324, 27);
            this.panel2.TabIndex = 20;
            // 
            // Receipt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(644, 209);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.minimize_button);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.close_button);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.bufferedPanel1);
            this.Controls.Add(this.bufferedPanel2);
            this.Controls.Add(this.bottomPanel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Receipt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Personal Banker";
            this.Load += new System.EventHandler(this.Receipt_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusBarPanel.ResumeLayout(false);
            this.statusBarPanel.PerformLayout();
            this.statusPanel.ResumeLayout(false);
            this.weatherPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.weatherIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.marqueePanel.ResumeLayout(false);
            this.marqueePanel.PerformLayout();
            this.bufferedPanel1.ResumeLayout(false);
            this.bufferedPanel1.PerformLayout();
            this.bufferedPanel2.ResumeLayout(false);
            this.bufferedPanel2.PerformLayout();
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
        private Label label1;
        private Button button2;
        private Label label2;
        private Label label3;
        private TextBox item_price;
        private Label label4;
        private Button button1;
        private Button Add_button;
        public AdvancedComboBox location_box;
        public AdvancedComboBox category_box;
        private PictureBox pictureBox1;
        private Label label5;
        private TextBox textBox5;
        private Label label6;
        public AdvancedComboBox payment_type;
        public TextBox item_desc;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fILEToolStripMenuItem;
        private ToolStripMenuItem fILE2ToolStripMenuItem;
        private ToolStripMenuItem eDITToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        public AdvancedComboBox quantity;
        private Label label7;
        private ToolStripMenuItem toolStripMenuItem5;
        private BufferedPanel panel2;
        private ToolStripMenuItem editTest1ToolStripMenuItem;
        private Button memo_button;
        private BufferedPanel bufferedPanel1;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem cALCULATORToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem8;
        private ToolStripMenuItem toolStripMenuItem11;
        private Label label8;
        private ToolStripMenuItem toolStripMenuItem13;
        private ToolStripMenuItem toolStripMenuItem14;
        private BufferedPanel bufferedPanel2;
        private Button button3;
        private DateTimePicker dateTimePicker1;
        private ToolStripMenuItem toolStripMenuItem17;
        private ToolStripMenuItem toolStripMenuItem12;
        private ToolStripMenuItem toolStripMenuItem18;
        private ToolStripMenuItem toolStripMenuItem16;
        private ToolStripMenuItem toolStripMenuItem21;
        private ToolStripMenuItem toolStripMenuItem23;
        private ToolStripMenuItem toolStripMenuItem24;
        private ToolStripMenuItem toolStripMenuItem25;
        private ToolStripMenuItem toolStripMenuItem26;
        private ToolStripMenuItem toolStripMenuItem27;
        private Button submit_button;
        private Button tax_override_button;
        private Button order_memo_button;
        public Button reset_button;
        private ToolStripMenuItem toolStripMenuItem28;
        private ToolStripMenuItem toolStripMenuItem29;
        private ToolStripMenuItem toolStripMenuItem31;
        public Button button4;
        private ToolStripMenuItem toolStripMenuItem32;
        private Label label12;
        private Label label11;
        private Label label10;
        private Label label9;
        private Label label16;
        private ToolStripMenuItem toolStripMenuItem33;
        private ToolStripMenuItem toolStripMenuItem34;
        private Button button5;
        private ToolStripMenuItem toolStripMenuItem36;
        private ToolStripMenuItem toolStripMenuItem37;
        private ToolStripMenuItem toolStripMenuItem38;
        private ToolStripMenuItem toolStripMenuItem40;
        private ToolStripMenuItem toolStripMenuItem41;
        private ToolStripMenuItem toolStripMenuItem42;
        private ToolStripMenuItem toolStripMenuItem43;
        private ToolStripMenuItem toolStripMenuItem44;
        private ToolStripMenuItem toolStripMenuItem51;
        private ToolStripMenuItem toolStripMenuItem50;
        private ToolStripMenuItem toolStripMenuItem49;
        private ToolStripMenuItem toolStripMenuItem48;
        private ToolStripMenuItem toolStripMenuItem47;
        private ToolStripMenuItem toolStripMenuItem46;
        private ToolStripMenuItem toolStripMenuItem45;
        private ToolStripMenuItem toolStripMenuItem9;
        private NotifyIcon notifyIcon1;
        private ToolStripMenuItem toolStripMenuItem10;
        private Panel statusBarPanel;
        private Label statusLabel;
        private Label dateLabel;
        private Label weatherLabel;
        private PictureBox weatherIcon;
        private Panel weatherPanel;
        private Label spendingLabel;
        private Panel statusPanel;
        private ToolStripMenuItem toolStripMenuItem15;
        private ToolStripMenuItem toolStripMenuItem19;
        private ToolStripMenuItem toolStripMenuItem30;
        private ToolStripMenuItem toolStripMenuItem22;
        private ToolStripMenuItem toolStripMenuItem20;
        private ToolStripMenuItem toolStripMenuItem39;
        private ToolStripMenuItem toolStripMenuItem35;
        private ToolStripMenuItem toolStripMenuItem52;
        private ToolStripMenuItem toolStripMenuItem53;
        private ToolStripMenuItem toolStripMenuItem54;
        private ToolStripMenuItem toolStripMenuItem55;
        private ToolStripMenuItem toolStripMenuItem56;
        private ToolStripMenuItem toolStripMenuItem57;
        private ToolStripMenuItem toolStripMenuItem58;
        private ToolStripMenuItem toolStripMenuItem6;
        private Button button6;
        private Label label14;
        private Label label13;
        private Panel bottomPanel;
        private Panel marqueePanel;
        private Label marqueeLabel;
        private Label marqueeLabel2;
        private Label marqueeInitial;
    }
}


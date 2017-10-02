using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Financial_Journal
{
    public partial class Backup_Settings : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Backup_Settings(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            Apply_Toggle_Style();

            ToolTip1.SetToolTip(Add_button, "Load a backup file");

            req_authentication.Checked = parent.Settings_Dictionary.ContainsKey("BACKUP_REQ") &&
                                         parent.Settings_Dictionary["BACKUP_REQ"] == "1";
            toggleSwitch1.Checked = parent.Settings_Dictionary.ContainsKey("BACKUP_DEL") &&
                                    parent.Settings_Dictionary["BACKUP_DEL"] == "1";
            auto_save.Checked = parent.Settings_Dictionary.ContainsKey("AUTO_SAVE") &&
                                parent.Settings_Dictionary["AUTO_SAVE"] == "1";
            auto_delete.Checked = parent.Settings_Dictionary.ContainsKey("AUTO_DELETE") &&
                                  parent.Settings_Dictionary["AUTO_DELETE"] == "1";

            string Backup_Path = parent.localSavePath + @"\Backups";
            double Total_File_Size = 0;
            bool Use_MB = false;

            if (Directory.Exists(Backup_Path))
            {
                string[] File_in_dir;
                File_in_dir = Directory.GetFiles(Backup_Path, "*", SearchOption.AllDirectories);
                foreach (string file in File_in_dir)
                {
                    Total_File_Size += new FileInfo(file).Length;
                }

                // Convert to KB
                Total_File_Size = Total_File_Size / 1000;


                if (Total_File_Size > 1000)
                {
                    Use_MB = true;
                    Total_File_Size = Total_File_Size / 1000;
                }
            }
            else
            {
                Total_File_Size = 0;
            }

            label7.Text = "You currently have " + String.Format("{0:n}", Total_File_Size) + " " + (Use_MB ? "MB" : "KB");

            // Fade Box
            TFLP = new FadeControl();
            TFLP.Size = new Size(this.Width - 2, this.Height - 2);
            TFLP.Location = new Point(999, 999);
            TFLP.Visible = true;
            TFLP.BackColor = this.BackColor;
            TFLP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            TFLP.AllowDrop = true;
            TFLP.BringToFront();
            this.Controls.Add(TFLP);
            TFLP.BringToFront();

            TFLP.Opacity = 80;
        }

        FadeControl TFLP;

        private void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        private void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
        }


        private void Apply_Toggle_Style()
        {
            req_authentication.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            req_authentication.Size = new Size(68, 25);
            req_authentication.OnText = "On";
            req_authentication.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            req_authentication.OnForeColor = Color.White;
            req_authentication.OffText = "Off";
            req_authentication.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            req_authentication.OffForeColor = Color.White;

            toggleSwitch1.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch1.Size = new Size(68, 25);
            toggleSwitch1.OnText = "On";
            toggleSwitch1.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch1.OnForeColor = Color.White;
            toggleSwitch1.OffText = "Off";
            toggleSwitch1.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch1.OffForeColor = Color.White;

            auto_save.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            auto_save.Size = new Size(68, 25);
            auto_save.OnText = "On";
            auto_save.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            auto_save.OnForeColor = Color.White;
            auto_save.OffText = "Off";
            auto_save.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            auto_save.OffForeColor = Color.White;

            auto_delete.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            auto_delete.Size = new Size(68, 25);
            auto_delete.OnText = "On";
            auto_delete.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            auto_delete.OnForeColor = Color.White;
            auto_delete.OffText = "Off";
            auto_delete.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            auto_delete.OffForeColor = Color.White;
        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        public void Set_Form_Color(Color randomColor)
        {
            //minimize_button.ForeColor = randomColor;
            //close_button.ForeColor = randomColor;
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void req_authentication_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["BACKUP_REQ"] = req_authentication.Checked ? "1" : "0";
        }

        private void toggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["BACKUP_DEL"] = toggleSwitch1.Checked ? "1" : "0";
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            string file_path = string.Empty;
            string Backup_Path = parent.localSavePath + @"\Backups";
            OpenFileDialog file = new OpenFileDialog();
            if (Directory.Exists(Backup_Path))
            {
                file.InitialDirectory = Backup_Path;
            }
            file.Title = "Load Profile Backups";
            file.Multiselect = false;
            if (file.ShowDialog() == DialogResult.OK)
            {
                parent.SaveHelper.Regular_Save();
                file_path = file.FileName;
                if (file_path.Contains(".cfg") && file_path.Contains("personal_banker_backup"))
                {
                    if (file_path.Contains(".cfg"))
                    {
                        parent.reset_button.PerformClick();
                        parent.Reload_Program(file_path);

                        Grey_Out();
                        // If authentication set already
                        if (parent.Settings_Dictionary.ContainsKey("AUTHENTICATION_REQ") && parent.Settings_Dictionary["AUTHENTICATION_REQ"] == "1")
                        {
                            using (var form = new Authentication_Form(parent, "Profile is locked. Please unlock using appropriate email and password.", "Authentication", parent.Settings_Dictionary["LOGIN_EMAIL"], parent.Settings_Dictionary["LOGIN_PASSWORD"], true, false, this.Location, this.Size))
                            {
                                var result2 = form.ShowDialog();
                                if (result2 == DialogResult.OK)
                                {
                                    if (form.ReturnValue1 == "1")
                                    {
                                        this.Show();
                                        this.Activate();
                                        this.TopMost = true;
                                        this.TopMost = false;
                                        //this.TopMost = true;


                                        Form_Message_Box FMB = new Form_Message_Box(parent, "Backup successfully loaded", true, 0, this.Location, this.Size);
                                        FMB.ShowDialog();
                                    }
                                    else
                                    {
                                        parent.reset_button.PerformClick();
                                        parent.Reload_Program();
                                    }
                                }
                                else
                                {
                                    parent.reset_button.PerformClick();
                                    parent.Reload_Program();
                                }
                            }
                        }
                        Grey_In();
                        parent.SaveHelper.Regular_Save();
                        base.Close();
                    }
                }
                else
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Invalid backup file detected. Please select the appropriate backup .cfg file", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
            }
        }

        private void auto_save_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["AUTO_SAVE"] = auto_save.Checked ? "1" : "0";
        }

        private void auto_delete_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["AUTO_DELETE"] = auto_delete.Checked ? "1" : "0";
        }

        /*
        public void Load_Backups()
        {
            string file_path = string.Empty;
            OpenFileDialog file = new OpenFileDialog();
            if (Directory.Exists(Global_Settings["backup_location"]))
                file.InitialDirectory = Global_Settings["backup_location"];
            file.Title = "Load IT Management Config Backup";
            file.Multiselect = false;
            if (file.ShowDialog() == DialogResult.OK)
            {
                file_path = file.FileName;
                if (file_path.Contains("20") && file_path.Contains("_Hr-") && file_path.Contains(".ini"))
                {
                    DialogResult dialogResult = MessageBox.Show("You have selected the file: " + Environment.NewLine + file_path + Environment.NewLine + "Do you wish to load this backup file? This will overwrite all your current unsaved changes and overwrite the main config.ini file.", "", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        _parent._Reload_Config_Info(file_path);
                        File.Copy(file_path, "\\\\10.0.0.6\\shopdata\\Development\\Robin\\ITManagement_Config.ini", true);

                        _parent._APPEND_TO_SETUP_INFORMATION(2, "Loaded configuration from path '" + file_path + "' by " + _parent._EMPLOYEE_LIST[_parent._MASTER_LOGIN_EMPLOYEE_NUMBER]);
                        this.Close();
                        this.Dispose();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                    }
                }
                else
                {
                    MessageBox.Show("Invalid backup file detected. Please select the appropriate backup .ini file");
                }
            }
        }
         * */
    }
}

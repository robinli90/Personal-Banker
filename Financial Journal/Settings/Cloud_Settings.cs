using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace Financial_Journal
{
    public partial class Cloud_Settings : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }


        Receipt parent;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Cloud_Settings(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Memo Tooltip (Hover)
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            ToolTip1.SetToolTip(findID, "Locate Newest ID");

            cloudEmailLabel.Text = parent.Settings_Dictionary.ContainsKey("LOGIN_EMAIL") ? parent.Settings_Dictionary["LOGIN_EMAIL"] : "";
            cloudKeyLabel.Text = parent.Settings_Dictionary.ContainsKey("UNIQUE_IDENTIFIER") ? parent.Settings_Dictionary["UNIQUE_IDENTIFIER"] : "";
            lastSyncLabel.Text = parent.Settings_Dictionary.ContainsKey("CLOUD_SYNC_TIME") ? parent.Settings_Dictionary["CLOUD_SYNC_TIME"] : "";

            cloudSyncOnClose.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            cloudSyncOnClose.Size = new Size(68, 25);
            cloudSyncOnClose.OnText = "On";
            cloudSyncOnClose.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            cloudSyncOnClose.OnForeColor = Color.White;
            cloudSyncOnClose.OffText = "Off";
            cloudSyncOnClose.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            cloudSyncOnClose.OffForeColor = Color.White;

            cloudLoad.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            cloudLoad.Size = new Size(68, 25);
            cloudLoad.OnText = "On";
            cloudLoad.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            cloudLoad.OnForeColor = Color.White;
            cloudLoad.OffText = "Off";
            cloudLoad.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            cloudLoad.OffForeColor = Color.White;

            cloudLoad.CheckedChanged -= cloudLoad_CheckedChanged;

            cloudSyncOnClose.Checked = parent.Settings_Dictionary.ContainsKey("CLOUD_SYNC_ON_CLOSE") && parent.Settings_Dictionary["CLOUD_SYNC_ON_CLOSE"] == "1";
            cloudLoad.Checked = parent.Settings_Dictionary.ContainsKey("CLOUD_LOAD") && parent.Settings_Dictionary["CLOUD_LOAD"] == "1";

            cloudLoad.CheckedChanged += cloudLoad_CheckedChanged;

            #region Fade Box
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
            #endregion

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
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void memo_button_Click(object sender, EventArgs e)
        {
            cloudKeyLabel.Enabled = !cloudKeyLabel.Enabled;

            if (cloudKeyLabel.Enabled)
            {
                Grey_Out();
                using (var form = new Yes_No_Dialog(parent, "Beware that altering the cloud key may cause connectivity issues. Continue?", "Warning", "No", "Yes", 15, this.Location, this.Size))
                {
                    var result = form.ShowDialog();
                    if (!(result == DialogResult.OK) || form.ReturnValue1 != "1")
                    {
                        cloudKeyLabel.Enabled = false;
                    }
                }
            }
            Grey_In();
        }

        private void cloudKeyLabel_TextChanged(object sender, EventArgs e)
        {
            if (cloudKeyLabel.Text.All(char.IsDigit) && cloudKeyLabel.Text.Length > 0)
            {
                parent.Settings_Dictionary["UNIQUE_IDENTIFIER"] = cloudKeyLabel.Text;
            }
            else
            {
                // If letter in SO_number box, do not output and move CARET to end
                try
                {
                    cloudKeyLabel.Text = cloudKeyLabel.Text.Substring(0, cloudKeyLabel.Text.Length - 1);
                    cloudKeyLabel.SelectionStart = cloudKeyLabel.Text.Length;
                    cloudKeyLabel.SelectionLength = 0;
                }
                catch
                { }
            }


        }


        private void cloudLoad_CheckedChanged(object sender, EventArgs e)
        {
            if (!cloudSyncOnClose.Checked && cloudLoad.Checked)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "We noticed that you have not enabled cloud sync on close. You should enable this in order to prevent data loss", true, +15, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
            parent.Settings_Dictionary["CLOUD_LOAD"] = cloudLoad.Checked ? "1" : "0";

        }

        private void cloudSyncOnClose_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["CLOUD_SYNC_ON_CLOSE"] = cloudSyncOnClose.Checked ? "1" : "0";
        }


        private void findID_Click(object sender, EventArgs e)
        {

            Grey_Out();
            Application.DoEvents();
            Cursor.Current = Cursors.WaitCursor;

            if (secondThreadFormHandle == IntPtr.Zero)
            {

                Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size, "FINDING YOUR", "CLOUD KEY", 12)
                {
                };
                form.HandleCreated += SecondFormHandleCreated;
                form.HandleDestroyed += SecondFormHandleDestroyed;
                form.RunInNewThread(false);
            }

            List<string> FileList = Cloud_Services.FTP_List_Files(true, parent.Settings_Dictionary["LOGIN_EMAIL"]);
            
            bool noFileFound = false;

            if (FileList.Count > 0)
            {
                string[] temp = Path.GetFileName(FileList[0]).Split(new string[] { "_" }, StringSplitOptions.None);
                cloudKeyLabel.Text = temp[temp.Length - 1].Substring(0, temp[temp.Length - 1 ].IndexOf(".cfg")); // get last index
            }
            else noFileFound = true;

            if (secondThreadFormHandle != IntPtr.Zero)
                PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            if (noFileFound)
            {
                System.Threading.Thread.Sleep(1000);
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "No cloud key found for your email", true, -15, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }

            Grey_In();
        }


        #region handler thread

        private IntPtr secondThreadFormHandle;

        void SecondFormHandleCreated(object sender, EventArgs e)
        {
            Control second = sender as Control;
            secondThreadFormHandle = second.Handle;
            second.HandleCreated -= SecondFormHandleCreated;
        }

        void SecondFormHandleDestroyed(object sender, EventArgs e)
        {
            Control second = sender as Control;
            secondThreadFormHandle = IntPtr.Zero;
            second.HandleDestroyed -= SecondFormHandleDestroyed;
        }

        const int WM_CLOSE = 0x0010;
        [DllImport("User32.dll")]
        extern static IntPtr PostMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);
        #endregion
    }
}

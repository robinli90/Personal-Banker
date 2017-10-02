using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Mail;

namespace Financial_Journal
{
    public partial class SyncSettings : Form
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
        public SyncSettings(Receipt _parent, Point g = new Point(), Size s = new Size())
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
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);


            cloudSyncOnClose.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            cloudSyncOnClose.Size = new Size(68, 25);
            cloudSyncOnClose.OnText = "On";
            cloudSyncOnClose.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            cloudSyncOnClose.OnForeColor = Color.White;
            cloudSyncOnClose.OffText = "Off";
            cloudSyncOnClose.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            cloudSyncOnClose.OffForeColor = Color.White;

            cloudSyncOnClose.Checked = parent.Settings_Dictionary["AUTO_MOBILE_SYNC"] == "1";

            toggleSwitch1.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch1.Size = new Size(68, 25);
            toggleSwitch1.OnText = "On";
            toggleSwitch1.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch1.OnForeColor = Color.White;
            toggleSwitch1.OffText = "Off";
            toggleSwitch1.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch1.OffForeColor = Color.White;

            toggleSwitch1.Checked = parent.Settings_Dictionary["MOBILE_SYNC_ON_LOAD"] == "1";


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

        private void cloudSyncOnClose_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["AUTO_MOBILE_SYNC"] = cloudSyncOnClose.Checked ? "1" : "0";
        }

        private void toggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["MOBILE_SYNC_ON_LOAD"] = toggleSwitch1.Checked ? "1" : "0";
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                Random r = new Random();
                string Verification_Code = r.Next(1000, 9999).ToString();

                // Generate FTP File
                bool ftpError = false;

                // Create temp sync file
                string tempAuthFile = parent.localSavePath + "\\" + parent.Settings_Dictionary["LOGIN_EMAIL"].ToLower() + "_auth.pbf";

                using (StreamWriter sw = File.CreateText(tempAuthFile)) // create local copy
                {
                    sw.Write(AESGCM.SimpleEncryptWithPassword(Verification_Code, MobileSync.AESGCMKey));
                    sw.Close();
                }

                try
                {
                    // Create mapping file on FTP
                    string ftpPath = @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/Authentication/" + parent.Settings_Dictionary["LOGIN_EMAIL"] + "_auth.pbf";
                    Cloud_Services.FTP_Upload_Synced(ftpPath, tempAuthFile);
                    File.Delete(tempAuthFile); // delete local copy
                }
                catch (Exception ex)
                {
                    ftpError = true;
                    Diagnostics.WriteLine("Cannot delete temporary sync file");
                }

                if (!ftpError)
                {
                    /*
                    MailMessage mailmsg = new MailMessage();
                    MailAddress from = new MailAddress("automatedpersonalbanker@gmail.com");
                    mailmsg.From = from;
                    mailmsg.To.Add(parent.Settings_Dictionary["LOGIN_EMAIL"]);
                    mailmsg.Subject = "Personal Banker Mobile Authentication";
                    mailmsg.Body = "This is an automated message. Please do not reply to this email. " +
                                   Environment.NewLine + Environment.NewLine +
                                   "Please use the authetication code below to verify your mobile environment. " +
                                   Environment.NewLine +
                                   "" + Environment.NewLine +
                                   "   " + Verification_Code + Environment.NewLine +
                                   "" + Environment.NewLine +
                                   "This code will expire upon closing of the application. " + Environment.NewLine +
                                   "" + Environment.NewLine;
                    // smtp client
                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    NetworkCredential credential =
                        new NetworkCredential("automatedpersonalbanker@gmail.com", "R5o2b6i8R5o2b6i8");
                    client.Credentials = credential;
                    //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                    client.Send(mailmsg);
                    */

                    Grey_Out();
                    Form_Message_Box FMB =
                        new Form_Message_Box(parent,
                            //"A verification code has been sent to your current email. Please enter this code on your mobile device", true, 0,
                            "Your mobile verification code is: " + Verification_Code, true, -20,
                            this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
                else
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Unable to generate code", true, -15, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }

            }
            catch
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Invalid email address provided", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Financial_Journal
{
    public partial class Authentication_Form : Form
    {

        protected override void OnPaint(PaintEventArgs e)
        {
            TFLP.Size = new Size(this.Width - 2, this.Height - 2);
            base.OnPaint(e);
        }

        Receipt parent;
        Size Start_Size = new Size();

        public string ReturnValue1 { get; set; }
        //public string ReturnValue2 { get; set; }
        private string match_email = "";
        private string match_password = "";
        bool allow_remember_me = true;

        public Authentication_Form(Receipt _parent, string Dialog_Message, string Title, string match_email_, string match_password_, bool enable_password_reset = false, bool allow_remember_me_ = true, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            label1.Text = Dialog_Message;
            label5.Text = Title;
            match_email = match_email_;
            match_password = match_password_;

            if (Title == "Account Recovery")
            {
                Add_button.Image = global::Financial_Journal.Properties.Resources.send;
                label2.Text = "Verification code";
                pw1.Left += 35;
                pw1.Width -= 35;
                Recovery_Mode = true;
                pw1.PasswordChar = '\0';
            }

            if (Dialog_Message.Contains("Continuing will reset your current receipt"))
            {
                this.Height += 10;
                close_button.Visible = false;
            }

            if (Dialog_Message.Contains("Are you sure you wish to remove this order?"))
            {
                close_button.Visible = false;
            }
            else
            {
                //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            }

            if (enable_password_reset)
                this.Height += 29;

            allow_remember_me = allow_remember_me_;
            if (!allow_remember_me)
            {
                label8.Visible = false;
                req_authentication.Visible = false;
            }

            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            pw1.KeyPress += new KeyPressEventHandler(keypress);
            button1.FlatAppearance.BorderSize = 0;
            Apply_Toggle_Style();
            if (parent.Settings_Dictionary["REMEMBER_ME"] == "1" && !label1.Text.Contains("Please unlock using appropriate email and password") && allow_remember_me)
            {
                email.Text = match_email;
                req_authentication.Checked = true;
                this.ActiveControl = pw1;
            }
            else
            {
                this.ActiveControl = email;
            }

            // Fade Box
            TFLP = new FadeControl();
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


        // If Enter, submit
        private void keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !Recovery_Mode)
            {

                if (email.Text == match_email && match_password == pw1.Text)
                {
                    Grey_Out();
                }
                Add_button.PerformClick();
            }
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

        private string Verification_Code = "asd4";

        private void Add_button_Click(object sender, EventArgs e)
        {
            TimeSpan Start_Time = DateTime.Now.TimeOfDay;
            if (pw1.Text.Length > 0 || Recovery_Mode)
            {
                //submit password
                if (email.Text == match_email && match_password == pw1.Text)
                {

                    if (parent.FTP_Logging)
                    {
                        Task.Run(() => { parent.FTP_Log(); });
                    }

                    ReturnValue1 = "1";
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    this.Dispose();
                }
                else if (Recovery_Mode && email.Text == match_email)
                {
                    Random r = new Random();
                    Verification_Code = r.Next(100000000, 999999999).ToString();

                    MailMessage mailmsg = new MailMessage();
                    MailAddress from = new MailAddress("automatedpersonalbanker@gmail.com");
                    mailmsg.From = from;
                    mailmsg.To.Add(email.Text);
                    mailmsg.Subject = "Personal Banker Email Verification";
                    mailmsg.Body = "This is an automated message. Please do not reply to this email. " + Environment.NewLine + Environment.NewLine +
                        "Please use the authetication code below to temporarily access your account. " + Environment.NewLine +
                        "" + Environment.NewLine +
                        Verification_Code + Environment.NewLine +
                        "" + Environment.NewLine +
                        "This code will expire upon closing of the application. " + Environment.NewLine +
                        "" + Environment.NewLine;
                    // smtp client
                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    NetworkCredential credential = new NetworkCredential("automatedpersonalbanker@gmail.com", "R5o2b6i8R5o2b6i8");
                    client.Credentials = credential;
                    //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                    client.Send(mailmsg);

                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "A verification code has been sent to you. Please copy and paste code below", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
                else if (email.Text != match_email)
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "The emailed entered does not match any of our records", true, 0, this.Location, this.Size);
                    email.ForeColor = Color.LightPink;
                    Grey_In();
                    FMB.ShowDialog();
                    error_count++;
                }
                else
                {
                    if (error_count < 2)
                    {
                        Task.Run(() => { parent.FTP_Log("AUTHENTICATION FAILED: Invalid Password Provided"); });
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Invalid Password. Attempts remaining: " + (2 - error_count), true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        Grey_In();
                    }
                    pw1.Text = "";
                    error_count++;
                }
                if (error_count >= 3)
                {
                    Environment.Exit(0);
                }
                // Force top
                else if (error_count > 0)
                {
                    this.TopMost = true;
                    this.TopMost = false;
                }
            }
        }


        private int error_count = 0;

        private void email_TextChanged(object sender, EventArgs e)
        {
            email.ForeColor = SystemColors.Control;
            pw1.ForeColor = SystemColors.Control;
        }

        private void pw1_TextChanged(object sender, EventArgs e)
        {
            email.ForeColor = SystemColors.Control;
            pw1.ForeColor = SystemColors.Control;
            if (Recovery_Mode)
            {
                if (pw1.Text == Verification_Code)
                {
                    // grant access
                    ReturnValue1 = "1";
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    this.Dispose();
                }
            }
        }

        #region Thread Lines

        private IntPtr secondThreadFormHandle2;

        void SecondFormHandleCreated2(object sender, EventArgs e)
        {
            Control second = sender as Control;
            this.secondThreadFormHandle2 = second.Handle;
            second.HandleCreated -= this.SecondFormHandleCreated2;
        }

        void SecondFormHandleDestroyed2(object sender, EventArgs e)
        {
            Control second = sender as Control;
            this.secondThreadFormHandle2 = IntPtr.Zero;
            second.HandleDestroyed -= this.SecondFormHandleDestroyed2;
        }

        const int WM_CLOSE = 0x0010;
        [DllImport("User32.dll")]
        extern static IntPtr PostMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);

        #endregion

        private void Apply_Toggle_Style()
        {
            req_authentication.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            req_authentication.Size = new Size(68, 20);
            req_authentication.OnText = "On";
            req_authentication.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            req_authentication.OnForeColor = Color.White;
            req_authentication.OffText = "Off";
            req_authentication.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            req_authentication.OffForeColor = Color.White;
        }

        bool Recovery_Mode = false;

        // Forgot password
        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Grey_Out();
            using (var form = new Authentication_Form(parent, "Please enter in email associated with your account. You will receive email with temporary access code", "Account Recovery", parent.Settings_Dictionary["LOGIN_EMAIL"], parent.Settings_Dictionary["LOGIN_PASSWORD"], false, allow_remember_me, this.Location, this.Size))
            {
                var result2 = form.ShowDialog();
                if (result2 == DialogResult.OK)
                {
                    if (form.ReturnValue1 == "1")
                    {
                        ReturnValue1 = "1";
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        this.Dispose();
                        parent.Settings_Dictionary["AUTHENTICATION_REQ"] = "0";

                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(parent, "You have successfully accessed your account. You must re-enter and verify your email again. Your old credentials have been deleted", true, 40, this.Location, this.Size);
                        FMB.ShowDialog();
                        parent.SaveHelper.Regular_Save();
                        Grey_In();
                    }
                }
            }
            Grey_In();
            try
            {
                Visible = true;
            }
            catch
            {
            }
        }

        private void req_authentication_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["REMEMBER_ME"] = req_authentication.Checked ? "1" : "0";
            //parent.SaveHelper.Regular_Save();
        }
    }
}

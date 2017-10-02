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
using System.Net.Mail;

namespace Financial_Journal
{
    public partial class Security : Form
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            TFLP.Size = new Size(this.Size.Width - 2, this.Size.Height - 2);
            base.OnPaint(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Size Start_Size = new Size();

        int small_open = 156;
        int med_open = 296;
        int full_open = 345;

        public Security(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);

            req_authentication.Checked = parent.Settings_Dictionary.ContainsKey("AUTHENTICATION_REQ") && parent.Settings_Dictionary["AUTHENTICATION_REQ"] == "1";
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        ToolTip ToolTip1 = new ToolTip();

        private void Receipt_Load(object sender, EventArgs e)
        {
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            ToolTip1.SetToolTip(Add_button, "Set authentication credentials");

            Apply_Toggle_Style();

            toggleSwitch1.Checked = parent.Settings_Dictionary.ContainsKey("LOGIN_BYPASS") && parent.Settings_Dictionary["LOGIN_BYPASS"] == "1";
            toggleSwitch2.Checked = parent.Settings_Dictionary.ContainsKey("SESSION_EXPIRY") && parent.Settings_Dictionary["SESSION_EXPIRY"] == "1";
            if (parent.Settings_Dictionary.ContainsKey("AUTHENTICATION_REQ") && parent.Settings_Dictionary["AUTHENTICATION_REQ"] == "1")
            {
                email.Enabled = false;
                pw1.Enabled = false;
                pw2.Enabled = false;
                Add_button.Enabled = false;
                //req_authentication.Checked = true;
                req_authentication.Enabled = true;
                toggleSwitch1.Enabled = true;
                toggleSwitch2.Enabled = true;
                this.Height = small_open;
            }
            else
            {
                //req_authentication.Checked = false;
                req_authentication.Enabled = false;
                toggleSwitch1.Enabled = false;
                toggleSwitch2.Enabled = false;
            }

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
            
            toggleSwitch2.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch2.Size = new Size(68, 25);
            toggleSwitch2.OnText = "On";
            toggleSwitch2.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch2.OnForeColor = Color.White;
            toggleSwitch2.OffText = "Off";
            toggleSwitch2.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch2.OffForeColor = Color.White;
            
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

        private string Verification_Code = "a8df9";

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (email.Text.Contains("@") && email.Text.Contains(".") &&
                pw1.Text == pw2.Text)
            {
                try
                {

                    Random r = new Random();
                    Verification_Code = r.Next(10000000, 999999999).ToString();

                    MailMessage mailmsg = new MailMessage();
                    MailAddress from = new MailAddress("automatedpersonalbanker@gmail.com");
                    mailmsg.From = from;
                    mailmsg.To.Add(email.Text);
                    mailmsg.Subject = "Personal Banker Email Verification";
                    mailmsg.Body = "This is an automated message. Please do not reply to this email. " + Environment.NewLine + Environment.NewLine +
                        "Please use the authetication code below to verify email address. " + Environment.NewLine +
                        "" + Environment.NewLine +
                        "   " + Verification_Code + Environment.NewLine +
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
                    Form_Message_Box FMB = new Form_Message_Box(parent, "A verification email has been sent to you. Please copy and paste code below", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();

                    SendKeys.Send("{TAB}");

                    this.Height = full_open;
                    email.Enabled = false;
                    pw1.Enabled = false;
                    pw2.Enabled = false;
                    ToolTip1.SetToolTip(Add_button, "Resend verification code");
                    Add_button.Image = global::Financial_Journal.Properties.Resources.reload;
                    //Add_button.Enabled = false;
                    req_authentication.Enabled = false;
                    toggleSwitch1.Enabled = false;
                }
                catch
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Invalid email address provided", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                }
            }
            else if (pw1.Text != pw2.Text)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Passwords do not match", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                
            }
            else if (!email.Text.Contains("@") && !email.Text.Contains("."))
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Incorrect email format", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
            }
            Grey_In();
        }

        private void req_authentication_CheckedChanged(object sender, EventArgs e)
        {
            // If authentication set already
            if (parent.Settings_Dictionary["AUTHENTICATION_REQ"] == "1" && req_authentication.Checked == false)
            {
                Grey_Out();
                using (var form = new Authentication_Form(parent, "Please enter your current email and password to disable authentication", "Authentication", parent.Settings_Dictionary["LOGIN_EMAIL"], parent.Settings_Dictionary["LOGIN_PASSWORD"], false, true, this.Location, this.Size)) 
                {
                    var result2 = form.ShowDialog();
                    if (result2 == DialogResult.OK)
                    {
                        if (form.ReturnValue1 == "1")
                        {
                            if (req_authentication.Checked == false)
                            {
                                email.Enabled = true;
                                pw1.Enabled = true;
                                pw2.Enabled = true;
                                Add_button.Enabled = true;
                                req_authentication.Checked = false;
                                req_authentication.Enabled = false;
                                toggleSwitch1.Checked = false;
                                toggleSwitch1.Enabled = false;
                                toggleSwitch2.Checked = false;
                                toggleSwitch2.Enabled = false;
                                this.Height = med_open;
                                parent.Settings_Dictionary["AUTHENTICATION_REQ"] = "0";
                                parent.Settings_Dictionary["LOGIN_EMAIL"] = ""; //remove email
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        req_authentication.Checked = true;
                    }
                }
                Grey_In();
            }
        }

        private void verification_TextChanged(object sender, EventArgs e)
        {
            if (verification.Text == Verification_Code)
            {
                parent.Settings_Dictionary["LOGIN_PASSWORD"] = pw1.Text;
                parent.Settings_Dictionary["LOGIN_EMAIL"] = email.Text;
                parent.Settings_Dictionary["AUTHENTICATION_REQ"] = "1";

                email.Enabled = false;
                pw1.Enabled = false;
                pw2.Enabled = false;
                Add_button.Enabled = false;
                req_authentication.Checked = true;
                req_authentication.Enabled = true;
                verification.Text = "";
                verification.Enabled = false;
                toggleSwitch1.Enabled = true;
                toggleSwitch2.Enabled = true;
                this.Height = small_open;
            }
        }

        private void toggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["LOGIN_BYPASS"] = toggleSwitch1.Checked ? "1" : "0";
        }

        private void toggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["SESSION_EXPIRY"] = toggleSwitch2.Checked ? "1" : "0";
        }
    }
}

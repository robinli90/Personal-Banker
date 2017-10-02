using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace Financial_Journal
{
    public partial class Email_Calendar_Range : Form
    {

        private List<Button> Icon_Button = new List<Button>();
        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        Calendar parent_Calendar;

        public List<string> Email_List = new List<string>();

        public void Populate_Emails()
        {
            Email_List.Add(parent.Settings_Dictionary["PERSONAL_EMAIL"]);
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            // Remove existing buttons
            Icon_Button.ForEach(button => this.Controls.Remove(button));
            Icon_Button.ForEach(button => button.Image.Dispose());
            Icon_Button.ForEach(button => button.Dispose());
            Icon_Button = new List<Button>();

            int data_height = 21;
            int start_height = label8.Top - 8;
            int start_margin = 64;
            int height_offset = 9;
            int row_count = 0;

            Color DrawForeColor = Color.White;
            Color BackColor = Color.FromArgb(64, 64, 64);
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 2);

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font f_italic = new Font("MS Reference Sans Serif", 8, FontStyle.Italic);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_title = new Font("MS Reference Sans Serif", 11, FontStyle.Bold);

            int item_index = 0;

            // If has order
            if (true)
            {

                foreach (string Email in Email_List)
                {
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button Delete_Button = new Button();
                    Delete_Button.BackColor = this.BackColor;
                    Delete_Button.ForeColor = this.BackColor;
                    Delete_Button.FlatStyle = FlatStyle.Flat;
                    Delete_Button.Image = global::Financial_Journal.Properties.Resources.delete_hobby;
                    Delete_Button.Size = new Size(25, 25);
                    Delete_Button.Location = new Point(start_margin - 26, start_height + height_offset + (row_count * data_height) - 4);
                    Delete_Button.Name = item_index.ToString();
                    Delete_Button.Text = "";
                    Delete_Button.Click += new EventHandler(this.view_order_Click);
                    ToolTip1.SetToolTip(Delete_Button, "Remove " + Email_List[item_index]);
                    Icon_Button.Add(Delete_Button);

                    if (Email != parent.Settings_Dictionary["PERSONAL_EMAIL"] || item_index > 0) this.Controls.Add(Delete_Button);

                    e.Graphics.DrawString(Email, f, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
                    row_count++;
                    item_index++;
                }
            }

            //this.Height = start_height + height_offset + row_count * data_height + (parent.GC_List.Count > 0 ? 30 : 0) + (use_GCard ? 18 : 0) + (parent.GC_List.Count == 0 ? 15 : 0);

            if (Email_List.Count > 5) this.Height = Start_Size.Height + ((Email_List.Count - 5) * data_height);

            TFLP.Size = new Size(this.Width - 2, this.Height - 2);

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            GreenBrush.Dispose();
            RedBrush.Dispose();
            GreyBrush.Dispose();
            WritingBrush.Dispose();
            f_asterisk.Dispose();
            f.Dispose();
            f_strike.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            f_italic.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        private void view_order_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            Email_List.RemoveAt(Convert.ToInt32(b.Name));
            Invalidate();
        }

        public Email_Calendar_Range(Receipt _parent, Calendar c, Point g = new Point(), Size s = new Size())
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            parent_Calendar = c;
            Start_Size = this.Size;
            this.Location = new Point(g.X + s.Width / 2 - this.Width / 2, g.Y + s.Height / 2 - this.Height / 2);
            Set_Form_Color(parent.Frame_Color);
        }

        // Converting month number to name
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            for (int i = 1; i < 32; i++)
            {
                from_day.Items.Add(i.ToString("D2"));
                to_day.Items.Add(i.ToString("D2"));
            }

            for (int i = 1; i < 13; i++)
            {
                from_month.Items.Add(mfi.GetMonthName(i));
                to_month.Items.Add(mfi.GetMonthName(i));
            }

            for (int i = DateTime.Now.AddYears(-5).Year; i <= DateTime.Now.Year; i++)
            {
                from_year.Items.Add(i);
                to_year.Items.Add(i);
            }

            DateTime Temp_Date = new DateTime(parent_Calendar.current_year, parent_Calendar.current_month, 1).AddMonths(1).AddDays(-1);

            from_day.SelectedIndex = 0;
            to_day.SelectedIndex = Temp_Date.Day - 1;

            from_month.Text = mfi.GetMonthName(parent_Calendar.current_month);
            to_month.Text = mfi.GetMonthName(parent_Calendar.current_month);

            from_year.Text = parent_Calendar.current_year.ToString();
            to_year.Text = parent_Calendar.current_year.ToString();

            Loaded = true;

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

            Apply_Toggle_Style();

            Populate_Emails();
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
            //minimize_button.ForeColor = randomColor;
            //close_button.ForeColor = randomColor;
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        string ind = "     ";


        // Email
        private void button2_Click(object sender, EventArgs e)
        {

            Calculate_Months();
            parent_Calendar.Filter_Lists(From_Date, To_Date, true);
            this.Visible = false;
            
            parent_Calendar.Activate();
            Cursor.Current = Cursors.WaitCursor;

            parent.Grey_Out();
            if (secondThreadFormHandle == IntPtr.Zero)
            {
                Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size, "SENDING", "EMAIL")
                {
                };
                form.HandleCreated += SecondFormHandleCreated;
                form.HandleDestroyed += SecondFormHandleDestroyed;
                form.RunInNewThread(false);
            }

            string Email_Str = "";
            // Get string
            DateTime Curr_Print_Date = From_Date.Date;

            while (Curr_Print_Date <= To_Date.Date)
            {
                List<Calendar_Events> CE_List = new List<Calendar_Events>();
                List<Agenda_Item> AI_List = new List<Agenda_Item>();
                List<Shopping_Item> SI_List = new List<Shopping_Item>();

                // Reset toggles
                parent.Agenda_Item_List.ForEach(x => x.Shopping_List.ForEach(y => y.Toggle_IUO = false));
                // Get Calendar Events
                CE_List = parent_Calendar.CE_Filtered_List.Where(x => x.Date.Date == Curr_Print_Date.Date).ToList();
                // Get Agenda Items
                AI_List = parent_Calendar.AI_Filtered_List.Where(x => x.Calendar_Date.Date == Curr_Print_Date.Date).ToList();
                // Get Shopping Items
                parent_Calendar.AI_Filtered_List.ForEach(x => SI_List.AddRange(x.Shopping_List.Where(y => y.Calendar_Date.Date == Curr_Print_Date.Date).ToList()));

                // If ignoring empty values
                if (CE_List.Count + AI_List.Count + SI_List.Count == 0)
                {
                    goto End;
                }

                Email_Str += Curr_Print_Date.ToShortDateString() + Environment.NewLine;

                // Order calendar events and agenda items by time
                for (int i = 0; i < 24; i++) // Hours
                {
                    for (int j = 0; j < 60; j++) // Minutes
                    {
                        #region Draw calendar events
                        foreach (Calendar_Events CE in CE_List.Where(x => x.Date.Hour == i && x.Date.Minute == j).ToList())
                        {
                            #region Get Importance String
                            string imp_string = "";
                            switch (CE.Importance)
                            {
                                case 0:
                                    imp_string = "Not Important";
                                    break;
                                case 1:
                                    imp_string = "Important";
                                    break;
                                case 2:
                                    imp_string = "Very Important";
                                    break;
                                default:
                                    MessageBox.Show("Invalid Importance level");
                                    break;
                            }
                            #endregion

                            // Sub item
                            Email_Str += ind + (CE.Time_Set ? ("[" + CE.Date.ToString("hh:mm tt") + "] : ") : "") + CE.Title + " (" + imp_string + ")" + Environment.NewLine;

                            string[] desc_lines = CE.Description.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                            foreach (string g in desc_lines)
                            {
                                // Sub sub item
                                if (g.Length > 1)
                                {
                                    Email_Str += ind + ind + "- " + g + Environment.NewLine;
                                }
                                // Append contact information
                                if (CE.Contact_Hash_Value.Length > 0 && parent_Calendar.CO_Filtered_List.Any(x => x.Hash_Value == CE.Contact_Hash_Value)) //has contact
                                {
                                    Contact Ref_Contact = parent.Contact_List.First(x => x.Hash_Value == CE.Contact_Hash_Value);
                                    Email_Str += Ref_Contact.ToStringIndent(8);;
                                }
                            }
                        }
                        #endregion
                        #region Draw agenda items
                        foreach (Agenda_Item AI in AI_List.Where(x => x.Calendar_Date.Hour == i && x.Calendar_Date.Minute == j).ToList())
                        {
                            if (include_sub.Checked)
                            {
                                Email_Str += ind + ((AI.Time_Set ? ("[" + AI.Calendar_Date.ToString("hh:mm tt") + "] : ") : "") + AI.Name + " (From Agenda)") + Environment.NewLine;

                                // Append contact information
                                if (AI.Contact_Hash_Value.Length > 0 && parent_Calendar.CO_Filtered_List.Any(x => x.Hash_Value == AI.Contact_Hash_Value)) //has contact
                                {
                                    Contact Ref_Contact = parent.Contact_List.First(x => x.Hash_Value == AI.Contact_Hash_Value);
                                    Email_Str += Ref_Contact.ToStringIndent(8);;
                                }

                                // draw linked shopping items
                                foreach (Shopping_Item SI in AI.Shopping_List)
                                {
                                    Email_Str += ind + ind + "- " + ((SI.Time_Set ? ("[" + SI.Calendar_Date.ToString("hh:mm tt") + "] : ") : "") + SI.Name) + Environment.NewLine;

                                    // Append contact information
                                    if (SI.Contact_Hash_Value.Length > 0 && parent_Calendar.CO_Filtered_List.Any(x => x.Hash_Value == SI.Contact_Hash_Value)) //has contact
                                    {
                                        Contact Ref_Contact = parent.Contact_List.First(x => x.Hash_Value == SI.Contact_Hash_Value);
                                        Email_Str += Ref_Contact.ToStringIndent(8);;
                                    }
                                }
                            }
                            else
                            {
                                Email_Str += ind + ((AI.Time_Set ? ("[" + AI.Calendar_Date.ToString("hh:mm tt") + "] : ") : "") + AI.Name + " (From Agenda)") + Environment.NewLine;
                                // Append contact information
                                if (AI.Contact_Hash_Value.Length > 0 && parent_Calendar.CO_Filtered_List.Any(x => x.Hash_Value == AI.Contact_Hash_Value)) //has contact
                                {
                                    Contact Ref_Contact = parent.Contact_List.First(x => x.Hash_Value == AI.Contact_Hash_Value);
                                    Email_Str += Ref_Contact.ToStringIndent(8);;
                                }
                            }
                        }
                        #endregion
                        #region Draw shopping items
                        foreach (Shopping_Item AI in SI_List.Where(x => x.Calendar_Date.Hour == i && x.Calendar_Date.Minute == j && x.Toggle_IUO == false).ToList())
                        {
                            Email_Str += ind + ((AI.Time_Set ? ("[" + AI.Calendar_Date.ToString("hh:mm tt") + "] : ") : "") + AI.Name + " ('" + parent.Agenda_Item_List.First(x => x.ID == AI.ID).Name + "' in Agenda)") + Environment.NewLine;
                            // Append contact information
                            if (AI.Contact_Hash_Value.Length > 0 && parent_Calendar.CO_Filtered_List.Any(x => x.Hash_Value == AI.Contact_Hash_Value)) //has contact
                            {
                                Contact Ref_Contact = parent.Contact_List.First(x => x.Hash_Value == AI.Contact_Hash_Value);
                                Email_Str += Ref_Contact.ToStringIndent(8);;
                            }
                        }
                        #endregion
                    }
                }

                Email_Str += Environment.NewLine;

                End:
                    Curr_Print_Date = Curr_Print_Date.AddDays(1);
            }

            string Name_Str = parent.Settings_Dictionary["PERSONAL_FIRST_NAME"] + " " + parent.Settings_Dictionary["PERSONAL_LAST_NAME"];

            try
            {
                MailMessage mailmsg = new MailMessage();
                MailAddress from = new MailAddress("automatedpersonalbanker@gmail.com");
                mailmsg.From = from;
                Email_List.Where(x => x.Contains("@") && x.Contains(".")).ToList().ForEach(x => mailmsg.To.Add(x));
                mailmsg.Subject = Name_Str + " has shared their calendar with you!";
                mailmsg.Body += Name_Str + "\'s" + " Calendar summary (" + From_Date.ToShortDateString() + " to " + To_Date.ToShortDateString() + ")" + Environment.NewLine + Environment.NewLine+ Environment.NewLine;
                mailmsg.Body += Email_Str;
                mailmsg.Body += Environment.NewLine + "This is an automated message. Please do not reply to this email. " + Environment.NewLine;
                mailmsg.Body += "This message was created using Personal Banker application!" + Environment.NewLine;
                // smtp client
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                NetworkCredential credential = new NetworkCredential("automatedpersonalbanker@gmail.com", "R5o2b6i8R5o2b6i8");
                client.Credentials = credential;
                client.Send(mailmsg);

                if (secondThreadFormHandle != IntPtr.Zero)
                    PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

                parent_Calendar.Activate();
                parent_Calendar.Grey_Out();
                Form_Message_Box FMB2 = new Form_Message_Box(parent, "Email sent!", true, -20, this.Location, this.Size);
                FMB2.ShowDialog();
                parent_Calendar.Grey_In();
            }
            catch
            {
                parent_Calendar.Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Email server unreachable or one or more emails invalid", true, 20, this.Location, this.Size);
                FMB.ShowDialog();
                parent_Calendar.Grey_In();
            }
            
            
            Cursor.Current = Cursors.Default;


            parent.Grey_In();
            this.Close();
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

        private void to_month_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void from_month_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void from_year_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void to_year_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void to_day_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void from_day_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        DateTime From_Date;
        DateTime To_Date;

        DateTime Curr_Print_Date;

        bool Ignore_Empty_Dates = true;
        bool Show_Spent_Figures = true;

        bool Loaded = false;

        private void Calculate_Months()
        {
            // Only check after loaded
            if (Loaded)
            {
                try
                {
                    From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, from_day.SelectedIndex + 1);
                    To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, to_day.SelectedIndex + 1);
                }
                catch
                {
                    from_day.Text = "01";
                    to_day.Text = "01";

                    from_month.Text = from_month.Items[From_Date.Month - 1].ToString();
                    to_month.Text = to_month.Items[To_Date.Month - 1].ToString();

                    from_year.Text = From_Date.Year.ToString();
                    to_year.Text = To_Date.Year.ToString();

                    From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
                    To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, 1);
                }

                // If invalid date selection, set dates to be hte same
                if (From_Date > To_Date)
                {
                    from_month.Text = to_month.Text = mfi.GetMonthName(DateTime.Now.Month);
                    from_year.Text = to_year.Text = (DateTime.Now.Year).ToString();

                    From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
                    To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, 1);
                }
                else
                {
                }
            }
        }

        private void Apply_Toggle_Style()
        {

            include_sub.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            include_sub.Size = new Size(68, 25);
            include_sub.OnText = "On";
            include_sub.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            include_sub.OnForeColor = Color.White;
            include_sub.OffText = "Off";
            include_sub.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            include_sub.OffForeColor = Color.White;
        }

        private void show_spent_CheckedChanged(object sender, EventArgs e)
        {
            Show_Spent_Figures = include_sub.Checked;
        }

        // Add email recipient
        private void button1_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Email_Add_Recipient(parent, "Please enter recipient email address", "", "Done", null, this.Location, this.Size, 15))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Email_List.Add(form.Pass_String);
                    Invalidate();
                }
            }
            Grey_In();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}

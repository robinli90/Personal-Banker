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
    public partial class Export_Calendar_Range : Form
    {

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        Calendar parent_Calendar;

        public Export_Calendar_Range(Receipt _parent, Calendar c, Point g = new Point(), Size s = new Size())
        {
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

        private void button2_Click(object sender, EventArgs e)
        {

            Calculate_Months();

            
            Grey_Out();

            using (var form1 = new Yes_No_Dialog(parent, "In the next step, you can filter 'Calendar Events' and 'Agenda Items' within the selected range that will be exported. Continue?", "Warning", "No", "Yes",  28, this.Location, this.Size))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                {

                    parent_Calendar.Filter_Lists(From_Date, To_Date, true);

                    // generate
                    // get passcode
                    string Passcode = "";
                    Grey_Out();
                    parent_Calendar.Activate();
                    using (var form = new Input_Box_Small(parent, "Please enter a password for the export file", "", "Done", null, this.Location, this.Size, 25))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Passcode = form.Pass_String;
                        }
                        else
                        {
                            goto End;
                            /*
                            Grey_Out();
                            Form_Message_Box FMB = new Form_Message_Box(parent, "Invalid passcode entered", true, -30, this.Location, this.Size);
                            FMB.ShowDialog();
                            Grey_In();
                            */
                        }
                    }

                    bool error = false;
                    if (Passcode.Length == 0)
                    {
                        error = true;
                    }
                    else
                    {
                        parent_Calendar.Activate();
                        Cursor.Current = Cursors.WaitCursor;

                        if (secondThreadFormHandle == IntPtr.Zero)
                        {
                            Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size, "GENERATING", "EXPORT FILE")
                            {
                            };
                            form.HandleCreated += SecondFormHandleCreated;
                            form.HandleDestroyed += SecondFormHandleDestroyed;
                            form.RunInNewThread(false);
                        }

                        // Main export function
                        error = Export_To_FTP(Passcode);

                        if (secondThreadFormHandle != IntPtr.Zero)
                            PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }

                    parent_Calendar.Activate();

                    string str = "";
                    if (!error)
                    {
                        str = "Export file created!" + Environment.NewLine + "File code: " + randomID + Environment.NewLine + "Passcode: " + Passcode + " (File Code Copied to Clipboard)";
                        parent_Calendar.label15.Text = "Recently exported Code#: " + randomID + " / Passcode: " + Passcode;
                        Clipboard.SetText(randomID);
                        parent_Calendar.label15.Visible = true;
                    }
                    else
                    {
                        str = "Error creating export file. Please try again in a minute!";
                    }

                    Form_Message_Box FMB2 = new Form_Message_Box(parent, str, true, 20, this.Location, this.Size);
                    FMB2.ShowDialog();

                    Grey_In();

                    Cursor.Current = Cursors.Default;
                }
            }
            this.Close();
            End: ;
            Grey_In();
        }

        string randomID;

        public bool Export_To_FTP(string Passcode)
        {
            // Generate file hash
            Random OrderID_Gen = new Random();
            randomID = OrderID_Gen.Next(100000000, 999999999).ToString();

            Diagnostics.WriteLine("Export start: " + DateTime.Now.TimeOfDay);
            TimeSpan Start_Time = DateTime.Now.TimeOfDay;

            // Export string (Same as classical load string)
            string export_line = "";

            export_line += Passcode + Environment.NewLine;
            export_line += this.Save_Agenda();
            export_line += this.Save_Calendar_Events();
            export_line += this.Save_Contacts();

            // Encrypt export line using passcode as AES key
            export_line = AESGCM.SimpleEncryptWithPassword(export_line, Passcode + "PASSWORDisHERE");

            // Create new local file
            string Local_Path = Path.Combine(parent.localSavePath, randomID + ".txt");
            using (var myFile = File.Create(Local_Path))
            {
                myFile.Close();
            }

            // Login log to FTP server
            try
            {
                string ftpUsername = "Guest";
                string ftpPassword = "robinisthebest";

                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                    string ftpPath = @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Export/" + randomID + ".txt";

                    // write to local file
                    File.AppendAllText(Local_Path, export_line);

                    // copy local file to FTP server
                    client.UploadFile(ftpPath, "STOR", Local_Path);
                }
            }
            catch (Exception ez)
            {
                Diagnostics.WriteLine("FTP ERROR : " + ez.ToString());
                return true;
                // FTP Error
            }

            try
            {
                File.Delete(Local_Path);
            }
            catch
            {
                Diagnostics.WriteLine("Cannot delete file");
                return true;
            }
            Diagnostics.WriteLine("FTP Thread end at " + DateTime.Now.TimeOfDay);

            return false;
            
        }
        //12, 30
        public string Save_Contacts()
        {
            string line = "";
            foreach (Contact C in parent_Calendar.CO_Filtered_List)
            {
                line += "[CONTACT_FIRST_NAME]=" + C.First_Name +
                        "||[CONTACT_LAST_NAME]=" + C.Last_Name +
                        "||[CONTACT_EMAIL]=" + C.Email +
                        "||[CONTACT_EMAIL_SECOND]=" + C.Email_Second +
                        "||[CONTACT_PRIMARY]=" + C.Phone_No_Primary +
                        "||[CONTACT_SECONDARY]=" + C.Phone_No_Second +
                        "||[CONTACT_ASSOCIATION]=" + C.Association +
                        "||[CONTACT_HASHVALUE]=" + C.Hash_Value + Environment.NewLine;
            }
            return line;
        }

        public string Save_Calendar_Events()
        {
            string line = "";
            foreach (Calendar_Events CE in parent_Calendar.CE_Filtered_List)
            {
                line += "[CALENDAR_EVENT]";
                line += "||[CALENDAR_TITLE]=" + CE.Title;
                line += "||[CALENDAR_HASHVALUE]=" + CE.Hash_Value;
                line += "||[CALENDAR_CONTACT_HASH]=" + CE.Contact_Hash_Value;
                line += "||[CALENDAR_ACTIVE]=" + CE.Is_Active;
                line += "||[CALENDAR_DESC]=" + CE.Description.Replace(Environment.NewLine, "~~");
                line += "||[CALENDAR_IMPORTANCE]=" + CE.Importance.ToString();
                line += "||[CALENDAR_TIMESET]=" + (CE.Time_Set ? "1" : "0");
                line += "||[CALENDAR_DATE]=" + CE.Date.ToString();
                if (CE.Alert_Dates.Count > 0)
                {
                    line += "||[CALENDAR_ALERT_SEQUENCE]=";
                    line += String.Join("~", CE.Alert_Dates);
                    /*
                    foreach (DateTime DT in CE.Alert_Dates.GetRange(1, CE.Alert_Dates.Count - 1))
                    {
                        line += "~" + DT.ToShortDateString() ;
                    }
                    */
                }
                line += Environment.NewLine;
            }
            return line;
        }

        public string Save_Agenda()
        {
            string line = "";

            if (parent.Agenda_Item_List.Count > 0)
            {
                foreach (Agenda_Item AI in parent_Calendar.AI_Filtered_List)
                {
                    line += "[AGENDA_ITEM]" +
                            "||[A_NAME]=" + AI.Name +
                            "||[HASH_VALUE]=" + AI.Hash_Value +
                            "||[C_HASH_VALUE]=" + AI.Contact_Hash_Value +
                            "||[A_DATE]=" + AI.Date.ToShortDateString() +
                            "||[A_ID]=" + AI.ID.ToString() +
                            "||[TIME_SET]=" + ((AI.Time_Set) ? "1" : "0") +
                            "||[A_CALENDAR_DATE]=" + AI.Calendar_Date.ToString() +
                            "||[A_CHECK_STATE]=" + ((AI.Check_State) ? "1" : "0") + Environment.NewLine;

                    foreach (Shopping_Item SI in AI.Shopping_List.Where(x => x.Calendar_Date <= To_Date && x.Calendar_Date >= From_Date).ToList())
                    {
                        line += "[SHOPPING_ITEM]" +
                            "||[S_NAME]=" + SI.Name +
                            "||[HASH_VALUE]=" + SI.Hash_Value +
                            "||[C_HASH_VALUE]=" + SI.Contact_Hash_Value +
                            "||[S_CHECK_STATE]=" + ((SI.Check_State) ? "1" : "0") +
                            "||[S_DATE]=" + SI.Calendar_Date.ToString() +
                            "||[TIME_SET]=" + ((SI.Time_Set) ? "1" : "0") +
                            "||[S_ID]=" + AI.ID.ToString() + Environment.NewLine;
                    }
                }
            }
            return line;
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

        bool Loaded = false;
        DateTime From_Date;
        DateTime To_Date;

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

                    from_year.Text = From_Date.ToString();
                    to_year.Text = To_Date.ToString();

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

    }
}

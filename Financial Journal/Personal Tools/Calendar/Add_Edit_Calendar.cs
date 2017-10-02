using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opulos.Core.UI;
using System.Windows.Forms;

namespace Financial_Journal
{
    public partial class Add_Edit_Calendar : Form
    {
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        private int start_height = 125;
        private List<Calendar_Events> Calendar_Events_List = new List<Calendar_Events>();

        string Edit_Str = "";

        protected override void OnPaint(PaintEventArgs e)
        {
            // Disable agenda add if no agenda item
            add_agenda_item.Enabled = parent.Agenda_Item_List.Where(x => (!x.Check_State || x.Shopping_List.Any(y => !y.Check_State)) && (x.Calendar_Date.Year < 1801 || x.Shopping_List.Any(y => y.Calendar_Date.Year < 1801))).ToList().Count > 0;

            Populate_Events();

            int data_height = 23;
            int height_offset = 0;

            int start_margin = 15;              //EventTitle
            int margin1 = start_margin + 10;   //EventDescription
            int margin2 = margin1 + 320;// 340;        //EventImportance
            int margin3 = margin2 + 190;// 340;        //EventImportance

            int row_count = 0;

            Color DrawForeColor = Color.White;
            Color BackColor = Color.FromArgb(64, 64, 64);
            Color HighlightColor = Color.FromArgb(76, 76, 76);
            
            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            SolidBrush OrangeBrush = new SolidBrush(Color.Orange);
            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 2);
            Pen Light_Grey_Pen = new Pen(GreyBrush, 1);

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font fs = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font f_small = new Font("MS Reference Sans Serif", 8, FontStyle.Italic);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);

            // Draw gray header line
            e.Graphics.DrawLine(Grey_Pen, start_margin, start_height - 15, this.Width - 16, start_height - 15);

            // Remove existing buttons
            Delete_Button_List.ForEach(button => button.Image.Dispose());
            Delete_Button_List.ForEach(button => button.Dispose());
            Delete_Button_List.ForEach(button => this.Controls.Remove(button));
            Delete_Button_List = new List<Button>();

            Edit_Button_List.ForEach(button => button.Image.Dispose());
            Edit_Button_List.ForEach(button => button.Dispose());
            Edit_Button_List.ForEach(button => this.Controls.Remove(button));
            Edit_Button_List = new List<Button>();

            int item_index = 0;

            #region Actual Calendar Items
            foreach (Calendar_Events CE in Calendar_Events_List)
            {
                ToolTip ToolTip1 = new ToolTip();
                ToolTip1.InitialDelay = 1;
                ToolTip1.ReshowDelay = 1;

                Button delete_button = new Button();
                delete_button.BackColor = this.BackColor;
                delete_button.ForeColor = this.BackColor;
                delete_button.FlatStyle = FlatStyle.Flat;
                delete_button.Image = global::Financial_Journal.Properties.Resources.delete;
                delete_button.Size = new Size(29, 29);
                delete_button.Location = new Point(this.Width - 42, start_height + height_offset + (row_count * data_height) - 5);
                delete_button.Name = item_index.ToString();
                delete_button.Text = "";
                delete_button.Click += new EventHandler(this.delete_item_Click);
                Delete_Button_List.Add(delete_button);
                ToolTip1.SetToolTip(delete_button, "Delete " + CE.Title);
                this.Controls.Add(delete_button);

                Button edit_button = new Button();
                edit_button.BackColor = this.BackColor;
                edit_button.ForeColor = this.BackColor;
                edit_button.FlatStyle = FlatStyle.Flat;
                edit_button.Image = CE.Time_Set ? global::Financial_Journal.Properties.Resources.greenclock : global::Financial_Journal.Properties.Resources.clock;
                edit_button.Size = new Size(29, 29);
                edit_button.Location = new Point(this.Width - 70, start_height + height_offset + (row_count * data_height) - 5);
                edit_button.Name = "CE_" + item_index.ToString();
                edit_button.Text = "";
                edit_button.Click += new EventHandler(this.set_item_time_Click);
                Edit_Button_List.Add(edit_button);
                ToolTip1.SetToolTip(edit_button, "Set time for " + CE.Title);
                this.Controls.Add(edit_button);

                //append contact
                edit_button = new Button();
                edit_button.BackColor = this.BackColor;
                edit_button.ForeColor = this.BackColor;
                edit_button.FlatStyle = FlatStyle.Flat;
                edit_button.Image = (CE.Contact_Hash_Value.Length > 0 ? global::Financial_Journal.Properties.Resources.contactsgreen : global::Financial_Journal.Properties.Resources.contacts);
                edit_button.Size = new Size(29, 29);
                edit_button.Location = new Point(this.Width - 98, start_height + height_offset + (row_count * data_height) - 5);
                edit_button.Name = (CE.Contact_Hash_Value.Length > 0 ? "CR" : "CO") + item_index.ToString(); // contact remove, contact set
                edit_button.Text = "";
                edit_button.Click += new EventHandler(this.set_contact_Click);
                Edit_Button_List.Add(edit_button);
                ToolTip1.SetToolTip(edit_button, (CE.Contact_Hash_Value.Length == 0 ? "Add contact to" : "Remove contact from") + " " + CE.Title);
                this.Controls.Add(edit_button);

                //date change
                edit_button = new Button();
                edit_button.BackColor = this.BackColor;
                edit_button.ForeColor = this.BackColor;
                edit_button.FlatStyle = FlatStyle.Flat;
                edit_button.Image = global::Financial_Journal.Properties.Resources.changedate;
                edit_button.Size = new Size(29, 29);
                edit_button.Location = new Point(this.Width - 127, start_height + height_offset + (row_count * data_height) - 5);
                edit_button.Name = "CC" + item_index.ToString(); // DC
                edit_button.Text = "";
                edit_button.Click += new EventHandler(this.set_contact_Click);
                Edit_Button_List.Add(edit_button);
                ToolTip1.SetToolTip(edit_button, ("Change date for ") + CE.Title);
                this.Controls.Add(edit_button);

                // Edit button
                edit_button = new Button();
                edit_button.BackColor = this.BackColor;
                edit_button.ForeColor = this.BackColor;
                edit_button.FlatStyle = FlatStyle.Flat;
                edit_button.Image = global::Financial_Journal.Properties.Resources.edit;
                edit_button.Size = new Size(29, 29);
                edit_button.Location = new Point(this.Width - 155, start_height + height_offset + (row_count * data_height) - 5);
                edit_button.Name = item_index.ToString();
                edit_button.Text = "";
                edit_button.Click += new EventHandler(this.edit_item_Click);
                Edit_Button_List.Add(edit_button);
                ToolTip1.SetToolTip(edit_button, "Edit " + CE.Title);
                this.Controls.Add(edit_button);


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


                e.Graphics.DrawString((CE.Time_Set ? ("[" + CE.Date.ToString("hh:mm tt") + "] : ") : "") + CE.Title + (CE.MultiDays <= 0 ? "" : String.Format(" (until {0})", CE.Date.AddDays(CE.MultiDays).ToShortDateString()))
                    , f_total, WritingBrush, start_margin + 5, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString(imp_string, f, (imp_string.Contains("Very") ? RedBrush : imp_string.Contains("Not") ? GreenBrush : OrangeBrush), margin3 + 11, start_height + height_offset + (row_count * data_height));
                row_count++;
                height_offset += 4;

                if (CE.Alert_Dates.Count > 0) e.Graphics.DrawString("Reminder: " + CE.Alert_Dates.Count + " day(s) before", f, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));

                string[] desc_lines = CE.Description.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                foreach (string g in desc_lines)
                {
                    height_offset -= 8;
                    e.Graphics.DrawString(g, fs, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
                    row_count++;
                }

                item_index++;
                height_offset += 9;
            }
            #endregion

            #region Agenda Items
            foreach (Agenda_Item AI in parent.Agenda_Item_List.Where(x => x.Calendar_Date.Date == calendarDate.Date).ToList())
            {
                ToolTip ToolTip1 = new ToolTip();
                ToolTip1.InitialDelay = 1;
                ToolTip1.ReshowDelay = 1;

                Button delete_button = new Button();
                delete_button.BackColor = this.BackColor;
                delete_button.ForeColor = this.BackColor;
                delete_button.FlatStyle = FlatStyle.Flat;
                delete_button.Image = global::Financial_Journal.Properties.Resources.delete;
                delete_button.Size = new Size(29, 29);
                delete_button.Location = new Point(this.Width - 42, start_height + height_offset + (row_count * data_height) - 5);
                delete_button.Name = "AI_" + AI.ID;
                delete_button.Text = "";
                delete_button.Click += new EventHandler(this.delete_item_Click);
                Delete_Button_List.Add(delete_button);
                ToolTip1.SetToolTip(delete_button, "Delete " + AI.Name);
                this.Controls.Add(delete_button);

                Button edit_button = new Button();
                edit_button.BackColor = this.BackColor;
                edit_button.ForeColor = this.BackColor;
                edit_button.FlatStyle = FlatStyle.Flat;
                edit_button.Image = AI.Time_Set ? global::Financial_Journal.Properties.Resources.greenclock : global::Financial_Journal.Properties.Resources.clock;
                edit_button.Size = new Size(29, 29);
                edit_button.Location = new Point(this.Width - 70, start_height + height_offset + (row_count * data_height) - 5);
                edit_button.Name = "AI_" + AI.ID + "_CLOCK";
                edit_button.Text = "";
                edit_button.Click += new EventHandler(this.set_item_time_Click);
                Edit_Button_List.Add(edit_button);
                ToolTip1.SetToolTip(edit_button, "Set time for " + AI.Name);
                this.Controls.Add(edit_button);

                //append contact
                edit_button = new Button();
                edit_button.BackColor = this.BackColor;
                edit_button.ForeColor = this.BackColor;
                edit_button.FlatStyle = FlatStyle.Flat;
                edit_button.Image = (AI.Contact_Hash_Value.Length > 0 ? global::Financial_Journal.Properties.Resources.contactsgreen : global::Financial_Journal.Properties.Resources.contacts);
                edit_button.Size = new Size(29, 29);
                edit_button.Location = new Point(this.Width - 98, start_height + height_offset + (row_count * data_height) - 5);
                edit_button.Name = (AI.Contact_Hash_Value.Length > 0 ? "AR" : "AO") + AI.ID; // contact remove, contact set
                edit_button.Text = "";
                edit_button.Click += new EventHandler(this.set_contact_Click);
                Edit_Button_List.Add(edit_button);
                ToolTip1.SetToolTip(edit_button, (AI.Contact_Hash_Value.Length == 0 ? "Add contact to" : "Remove contact from") + " " + AI.Name);
                this.Controls.Add(edit_button);

                //date change
                edit_button = new Button();
                edit_button.BackColor = this.BackColor;
                edit_button.ForeColor = this.BackColor;
                edit_button.FlatStyle = FlatStyle.Flat;
                edit_button.Image = global::Financial_Journal.Properties.Resources.changedate;
                edit_button.Size = new Size(29, 29);
                edit_button.Location = new Point(this.Width - 127, start_height + height_offset + (row_count * data_height) - 5);
                edit_button.Name = "AC" + AI.ID; // DC
                edit_button.Text = "";
                edit_button.Click += new EventHandler(this.set_contact_Click);
                Edit_Button_List.Add(edit_button);
                ToolTip1.SetToolTip(edit_button, ("Change date for ") + AI.Name);
                this.Controls.Add(edit_button);

                string imp_string = "From Agenda";

                e.Graphics.DrawString((AI.Time_Set ? ("[" + AI.Calendar_Date.ToString("hh:mm tt") + "] : ") : "") + AI.Name, f_total, WritingBrush, start_margin + 5, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString(imp_string, f, OrangeBrush, margin3 + 11, start_height + height_offset + (row_count * data_height));
                row_count++;
                height_offset += 4;
            }
            #endregion

            #region Shopping Items
            foreach (Agenda_Item AI in parent.Agenda_Item_List)
            {

                foreach (Shopping_Item SI in AI.Shopping_List.Where(x => x.Calendar_Date.Date == calendarDate).ToList())
                {
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button delete_button = new Button();
                    delete_button.BackColor = this.BackColor;
                    delete_button.ForeColor = this.BackColor;
                    delete_button.FlatStyle = FlatStyle.Flat;
                    delete_button.Image = global::Financial_Journal.Properties.Resources.delete;
                    delete_button.Size = new Size(29, 29);
                    delete_button.Location = new Point(this.Width - 42, start_height + height_offset + (row_count * data_height) - 5);
                    delete_button.Name = "SI||" + AI.ID + "||" + SI.Name;
                    delete_button.Text = "";
                    delete_button.Click += new EventHandler(this.delete_item_Click);
                    Delete_Button_List.Add(delete_button);
                    ToolTip1.SetToolTip(delete_button, "Delete " + SI.Name);
                    this.Controls.Add(delete_button);

                    Button edit_button = new Button();
                    edit_button.BackColor = this.BackColor;
                    edit_button.ForeColor = this.BackColor;
                    edit_button.FlatStyle = FlatStyle.Flat;
                    edit_button.Image = SI.Time_Set ? global::Financial_Journal.Properties.Resources.greenclock : global::Financial_Journal.Properties.Resources.clock;
                    edit_button.Size = new Size(29, 29);
                    edit_button.Location = new Point(this.Width - 70, start_height + height_offset + (row_count * data_height) - 5);
                    edit_button.Name = "SI_" + AI.ID + "_" + SI.Name + "_CLOCK";
                    edit_button.Text = "";
                    edit_button.Click += new EventHandler(this.set_item_time_Click);
                    Edit_Button_List.Add(edit_button);
                    ToolTip1.SetToolTip(edit_button, "Set time for " + SI.Name);
                    this.Controls.Add(edit_button);

                    //append contact
                    edit_button = new Button();
                    edit_button.BackColor = this.BackColor;
                    edit_button.ForeColor = this.BackColor;
                    edit_button.FlatStyle = FlatStyle.Flat;
                    edit_button.Image = (SI.Contact_Hash_Value.Length > 0 ? global::Financial_Journal.Properties.Resources.contactsgreen : global::Financial_Journal.Properties.Resources.contacts);
                    edit_button.Size = new Size(29, 29);
                    edit_button.Location = new Point(this.Width - 98, start_height + height_offset + (row_count * data_height) - 5);
                    edit_button.Name = (SI.Contact_Hash_Value.Length > 0 ? "SR" : "SO") + "_" + AI.ID + "_" + SI.Name; // contact remove, contact set
                    edit_button.Text = "";
                    edit_button.Click += new EventHandler(this.set_contact_Click);
                    Edit_Button_List.Add(edit_button);
                    ToolTip1.SetToolTip(edit_button, (SI.Contact_Hash_Value.Length == 0 ? "Add contact to" : "Remove contact from") + " " + SI.Name);
                    this.Controls.Add(edit_button);
                    string imp_string = "From Agenda";

                    //date change
                    edit_button = new Button();
                    edit_button.BackColor = this.BackColor;
                    edit_button.ForeColor = this.BackColor;
                    edit_button.FlatStyle = FlatStyle.Flat;
                    edit_button.Image = global::Financial_Journal.Properties.Resources.changedate;
                    edit_button.Size = new Size(29, 29);
                    edit_button.Location = new Point(this.Width - 127, start_height + height_offset + (row_count * data_height) - 5);
                    edit_button.Name = "SC" + "_" + AI.ID + "_" + SI.Name;
                    edit_button.Text = "";
                    edit_button.Click += new EventHandler(this.set_contact_Click);
                    Edit_Button_List.Add(edit_button);
                    ToolTip1.SetToolTip(edit_button, ("Change date for ") + SI.Name);
                    this.Controls.Add(edit_button);

                    e.Graphics.DrawString((SI.Time_Set ? ("[" + SI.Calendar_Date.ToString("hh:mm tt") + "] : ") : "") + SI.Name, f_total, WritingBrush, start_margin + 5, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(imp_string, f, OrangeBrush, margin3 + 11, start_height + height_offset + (row_count * data_height));
                    
                    height_offset += 15;
                    e.Graphics.DrawString("(From: " + AI.Name + ")", f_small, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    row_count++;
                    e.Graphics.DrawLine(Light_Grey_Pen, start_margin, start_height + height_offset + (row_count * data_height) - 10, this.Width - 16, start_height + height_offset + (row_count * data_height) - 10);
                }
            }
            #endregion

            this.Height = Start_Size.Height + start_height + height_offset + row_count * data_height - 30;

            TFLP.Size = new Size(this.Width - 2, this.Height - 2);

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            GreenBrush.Dispose();
            RedBrush.Dispose();
            GreyBrush.Dispose();
            OrangeBrush.Dispose();
            WritingBrush.Dispose();
            f_asterisk.Dispose();
            fs.Dispose();
            f.Dispose();
            f_small.Dispose();
            f_strike.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        private void set_contact_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Button b = (Button)sender;

            if (b.Name.StartsWith("C")) // IF IS CALENDAR
            {
                Calendar_Events Ref_CE = parent.Calendar_Events_List.FirstOrDefault(x => x == Calendar_Events_List[Convert.ToInt32(b.Name.Substring(2))]);

                if (b.Name.StartsWith("CO")) // IF ADDING NEW
                {
                    using (var form1 = new Contacts(parent, this.Location, this.Size, "calendar event"))
                    {
                        var result = form1.ShowDialog();
                        if (result == DialogResult.OK && form1.Return_Contact != null)
                        {
                            Ref_CE.Contact_Hash_Value = form1.Return_Contact.Hash_Value;
                            Invalidate();
                        }
                    }
                }
                else if (b.Name.StartsWith("CR")) // REMOVING
                {
                    Contact Ref_Contact = parent.Contact_List.FirstOrDefault(x => x.Hash_Value == Ref_CE.Contact_Hash_Value);

                    using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to remove linked contact? " + Environment.NewLine + Ref_Contact.ToString() + "", "Warning", "No", "Yes", 40, this.Location, this.Size))
                    {
                        var result21 = form1.ShowDialog();
                        if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                        {
                            Ref_CE.Contact_Hash_Value = "";
                            Invalidate();
                        }
                    }
                }
                else if (b.Name.StartsWith("CC")) // CALENDAR EVENT CHANGE DATE
                {
                    using (var form1 = new Date_Change_Dialog(parent, Ref_CE.Date, this.Location, this.Size))
                    {
                        var result21 = form1.ShowDialog();
                        if (result21 == DialogResult.OK && form1.Return_Date != null)
                        {
                            Ref_CE.Date = form1.Return_Date;
                            Invalidate();
                        }
                    }
                }
            }
            else if (b.Name.StartsWith("A")) // IF IS AGENDA
            {
                Agenda_Item Ref_AI = parent.Agenda_Item_List.First(x => x.ID.ToString() == b.Name.Substring(2));

                if (b.Name.StartsWith("AO"))
                {
                    using (var form1 = new Contacts(parent, this.Location, this.Size, "agenda event"))
                    {
                        var result = form1.ShowDialog();
                        if (result == DialogResult.OK && form1.Return_Contact != null)
                        {
                            Ref_AI.Contact_Hash_Value = form1.Return_Contact.Hash_Value;
                            Invalidate();
                        }
                    }
                }
                else if (b.Name.StartsWith("AR")) // REMOVING
                {
                    Contact Ref_Contact = parent.Contact_List.FirstOrDefault(x => x.Hash_Value == Ref_AI.Contact_Hash_Value);

                    using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to remove linked contact? " + Environment.NewLine + Ref_Contact.ToString() + "", "Warning", "No", "Yes", 40, this.Location, this.Size))
                    {
                        var result21 = form1.ShowDialog();
                        if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                        {
                            Ref_AI.Contact_Hash_Value = "";
                            Invalidate();
                        }
                    }
                }
                else if (b.Name.StartsWith("AC")) // AGENDA CHANGE DATE
                {
                    using (var form1 = new Date_Change_Dialog(parent, Ref_AI.Calendar_Date, this.Location, this.Size))
                    {
                        var result21 = form1.ShowDialog();
                        if (result21 == DialogResult.OK && form1.Return_Date != null)
                        {
                            Ref_AI.Calendar_Date = form1.Return_Date;
                            Invalidate();
                        }
                    }
                }
            }
            else if (b.Name.StartsWith("S")) // IF IS SHOPPING ITEM
            {
                string[] temp = b.Name.Split(new string[] { "_" }, StringSplitOptions.None);
                // Set calendar date to null to remove
                Shopping_Item Ref_SI = parent.Agenda_Item_List.First(x => x.ID.ToString() == temp[1]).Shopping_List.First(x => x.Name == temp[2]);
                 
                if (b.Name.StartsWith("SO"))
                {
                    using (var form1 = new Contacts(parent, this.Location, this.Size, "shopping event"))
                    {
                        var result = form1.ShowDialog();
                        if (result == DialogResult.OK && form1.Return_Contact != null)
                        {
                            Ref_SI.Contact_Hash_Value = form1.Return_Contact.Hash_Value;
                            Invalidate();
                        }
                    }
                }
                else if (b.Name.StartsWith("SR")) // REMOVING
                {
                    Contact Ref_Contact = parent.Contact_List.FirstOrDefault(x => x.Hash_Value == Ref_SI.Contact_Hash_Value);

                    using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to remove linked contact? " + Environment.NewLine + Ref_Contact.ToString() + "", "Warning", "No", "Yes", 40, this.Location, this.Size))
                    {
                        var result21 = form1.ShowDialog();
                        if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                        {
                            Ref_SI.Contact_Hash_Value = "";
                            Invalidate();
                        }
                    }
                }
                else if (b.Name.StartsWith("SC")) // SHOPPING CHANGE DATE
                {
                    using (var form1 = new Date_Change_Dialog(parent, Ref_SI.Calendar_Date, this.Location, this.Size))
                    {
                        var result21 = form1.ShowDialog();
                        if (result21 == DialogResult.OK && form1.Return_Date != null)
                        {
                            Ref_SI.Calendar_Date = form1.Return_Date;
                            Invalidate();
                        }
                    }
                }
            }
            Grey_In();
        }

        private void delete_item_Click(object sender, EventArgs e)
        {
            
            Grey_Out();

            using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to remove this item?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                {

                    Button b = (Button)sender;

                    if (b.Name.StartsWith("AI_"))
                    {
                        string[] temp = b.Name.Split(new string[] { "_" }, StringSplitOptions.None);
                        // Set calendar date to null to remove
                        parent.Agenda_Item_List.First(x => x.ID.ToString() == temp[1]).Calendar_Date = new DateTime(1800, 1, 1);
                        parent.Agenda_Item_List.First(x => x.ID.ToString() == temp[1]).Contact_Hash_Value = "";
                    }
                    else if (b.Name.StartsWith("SI||"))
                    {
                        string[] temp = b.Name.Split(new string[] { "||" }, StringSplitOptions.None);
                        // Set calendar date to null to remove
                        parent.Agenda_Item_List.First(x => x.ID.ToString() == temp[1]).Shopping_List.First(x => x.Name == temp[2]).Calendar_Date = new DateTime(1800, 1, 1);
                        parent.Agenda_Item_List.First(x => x.ID.ToString() == temp[1]).Shopping_List.First(x => x.Name == temp[2]).Contact_Hash_Value = "";
                    }
                    else
                    {
                        parent.Calendar_Events_List.Remove(Calendar_Events_List[Convert.ToInt32(b.Name)]);
                    }
                    Invalidate();
                }
            }

            Grey_In();
        }

        Calendar_Events Edit_Calendar;


        private void edit_item_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            Edit_Index = Convert.ToInt32(b.Name);
            Edit_Calendar = Calendar_Events_List[Edit_Index];
            Calendar_Events temp = Calendar_Events_List[Edit_Index];
            title_box.Text = temp.Title;
            importance_box.Text = importance_box.Items[temp.Importance].ToString();
            desc_box.Text = temp.Description;
            alert_on.Checked = temp.Alert_Dates.Count > 0;
            remind_me_x_days.Text = temp.Alert_Dates.Count > 0 ? temp.Alert_Dates.Count.ToString() : "";
            if (temp.MultiDays > 0)
            {
                multiDayBox.Checked = true;
                multiDate.Value = temp.Date.AddDays(temp.MultiDays);
            }
            else
            {
                multiDate.Value = temp.Date.AddDays(1);
            }

            // Change tooltip
            ToolTip2.RemoveAll();
            ToolTip2.SetToolTip(Add_button, "Save changes to " + Calendar_Events_List[Edit_Index].Title);

            Add_button.Image = global::Financial_Journal.Properties.Resources.save;

            add_entry_button.PerformClick();
        }

        private void set_item_time_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            Edit_Str = b.Name;

            string[] temp = Edit_Str.Split(new string[] { "_" }, StringSplitOptions.None);
            // Calendar
            if (Edit_Str.StartsWith("CE"))
            {
                Calendar_Events ref_CE = parent.Calendar_Events_List.First(x => x == Calendar_Events_List[Convert.ToInt32(temp[1])]);
                if (ref_CE.Time_Set)
                {
                    timePickerPanel1.timePicker.Value = ref_CE.Date;
                }
            }
            else if (Edit_Str.StartsWith("AI"))
            {
                Agenda_Item ref_AI = parent.Agenda_Item_List.First(x => x.ID.ToString() == temp[1]);
                if (ref_AI.Time_Set)
                {
                    timePickerPanel1.timePicker.Value = ref_AI.Calendar_Date;
                }
            }
            else if (Edit_Str.StartsWith("SI"))
            {
                Shopping_Item ref_SI = parent.Agenda_Item_List.First(x => x.ID.ToString() == temp[1]).Shopping_List.First(x => x.Name == temp[2]);

                if (ref_SI.Time_Set)
                {
                    timePickerPanel1.timePicker.Value = ref_SI.Calendar_Date;
                }
            }
            Grey_Out();

            //DateTime temp1 = DateTime.Now;
            //DateTime temp2 = temp1.Date + new TimeSpan(temp1.Hour, temp1.Minute, 0);
            //timePickerPanel1.timePicker.ClockMenu.Value = temp2;

            timePickerPanel1.timePicker.ClockMenu.Set_To_Hour();

            timePickerPanel1.timePicker.ClockMenu.Show(this,
                    (this.Size.Width / 2 - 98),// - timePickerPanel1.timePicker.ClockMenu.Width / 2),
                    (this.Size.Height / 2 - 145) //- timePickerPanel1.timePicker.ClockMenu.Height / 2)
                );
            timePickerPanel1.timePicker.ClockMenu.Visible = true;
            SetWindowPos(timePickerPanel1.timePicker.ClockMenu.Handle, (IntPtr)(-1), 0, 0, 0, 0,
                                                            SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
        }

        private int Edit_Index = -1;
        List<Button> Delete_Button_List = new List<Button>();
        List<Button> Edit_Button_List = new List<Button>();

        Receipt parent;
        Size Start_Size = new Size();
        DateTime calendarDate = new DateTime();
        Calendar parent_Calendar;

        public Add_Edit_Calendar(Receipt _parent, DateTime calendarDate_, Calendar c, Point g = new Point(), Size s = new Size())
        {
            calendarDate = calendarDate_;
            //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            parent_Calendar = c;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);

            this.Location = new Point(g.X + s.Width / 2 - this.Width / 2, g.Y + s.Height / 4 - this.Height / 2);
        }

        /// <summary>
        /// Get all events on this current date (calendarDate_);
        /// </summary>
        private void Populate_Events()
        {
            
            Calendar_Events_List = new List<Calendar_Events>();
            foreach (Calendar_Events CE in parent.Calendar_Events_List.Where(x => x.Date.Date == calendarDate.Date))
            {
                Calendar_Events_List.Add(CE);
            }
        }

        ToolTip ToolTip2 = new ToolTip();

        FadeControl TFLP;

        private void Receipt_Load(object sender, EventArgs e)
        {
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

            TFLP.Opacity = 75;

            multiDate.Value = calendarDate.AddDays(1);

            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.label5.Text = "Add/Edit events for " + calendarDate.DayOfWeek + " " + mfi.GetAbbreviatedMonthName(calendarDate.Month) + " " + calendarDate.Day + ", " + calendarDate.Year;
            importance_box.Items.Add("Low Importance");
            importance_box.Items.Add("Important");
            importance_box.Items.Add("Very Important");

            importance_box.SelectedIndex = 1;

            for (int i = 1; i < 31; i++)
            {
                if (DateTime.Now.AddDays(i) <= calendarDate)
                    remind_me_x_days.Items.Add(i.ToString());
            }
            if (DateTime.Now.AddDays(31) <= calendarDate)
                remind_me_x_days.Items.Add("31");
            if (DateTime.Now.AddDays(60) <= calendarDate)
                remind_me_x_days.Items.Add("60");
            if (DateTime.Now.AddDays(90) <= calendarDate)
                remind_me_x_days.Items.Add("90");

            ToolTip2.InitialDelay = 1;
            ToolTip2.ReshowDelay = 1;

            ToolTip2.SetToolTip(Add_button, "Add new event");

            Populate_Events();
            if (Calendar_Events_List.Count == 0)
            {
                //add_entry_button.PerformClick();
            }

            //timePickerPanel1.timePicker.ClockMenu.Location = new Point(200, 200);

            timePickerPanel1.timePicker.ClockMenu.ClockButtonOK.Click += TimeOK;
            timePickerPanel1.timePicker.ClockMenu.ClockButtonCancel.Click += TimeCancel;
            timePickerPanel1.timePicker.ClockMenu.Closed += TimeClose;
                
            /*
            delegate(object o, ToolStripDropDownClosedEventArgs evnt)
            {
                Time_Chooser = timePickerPanel1.timePicker.Value.TimeOfDay;
                Grey_In();
            };*/

        }

        private void TimeOK(object sender, EventArgs e)
        {
            if (Edit_Str.Length >= 4)
            {
                string[] temp = Edit_Str.Split(new string[] { "_" }, StringSplitOptions.None);
                Time_Chooser = timePickerPanel1.timePicker.Value.TimeOfDay;

                // Calendar
                if (Edit_Str.StartsWith("CE"))
                {
                    Calendar_Events ref_CE = parent.Calendar_Events_List.First(x => x == Calendar_Events_List[Convert.ToInt32(temp[1])]);
                    ref_CE.Time_Set = true;
                    ref_CE.Date = new DateTime(ref_CE.Date.Year, ref_CE.Date.Month, ref_CE.Date.Day, Time_Chooser.Hours, Time_Chooser.Minutes, 0);
                }
                else if (Edit_Str.StartsWith("AI"))
                {
                    Agenda_Item ref_AI = parent.Agenda_Item_List.First(x => x.ID.ToString() == temp[1]);
                    ref_AI.Time_Set = true;
                    ref_AI.Calendar_Date = new DateTime(ref_AI.Calendar_Date.Year, ref_AI.Calendar_Date.Month, ref_AI.Calendar_Date.Day, Time_Chooser.Hours, Time_Chooser.Minutes, 0);
                }
                else if (Edit_Str.StartsWith("SI"))
                {
                    Shopping_Item ref_SI = parent.Agenda_Item_List.First(x => x.ID.ToString() == temp[1]).Shopping_List.First(x => x.Name == temp[2]);
                    ref_SI.Time_Set = true;
                    ref_SI.Calendar_Date = new DateTime(ref_SI.Calendar_Date.Year, ref_SI.Calendar_Date.Month, ref_SI.Calendar_Date.Day, Time_Chooser.Hours, Time_Chooser.Minutes, 0);
                }
                Invalidate();
            }
            Grey_In();
            Edit_Str = "";
        }

        private void TimeCancel(object sender, EventArgs e)
        {
            if (Edit_Str.Length >= 4)
            {
                string[] temp = Edit_Str.Split(new string[] { "_" }, StringSplitOptions.None);
                Time_Chooser = timePickerPanel1.timePicker.Value.TimeOfDay;

                // Calendar
                if (Edit_Str.StartsWith("CE"))
                {
                    Calendar_Events ref_CE = parent.Calendar_Events_List.First(x => x == Calendar_Events_List[Convert.ToInt32(temp[1])]);
                    ref_CE.Time_Set = false;
                }
                else if (Edit_Str.StartsWith("AI"))
                {
                    Agenda_Item ref_AI = parent.Agenda_Item_List.First(x => x.ID.ToString() == temp[1]);
                    ref_AI.Time_Set = false;
                }
                else if (Edit_Str.StartsWith("SI"))
                {
                    Shopping_Item ref_SI = parent.Agenda_Item_List.First(x => x.ID.ToString() == temp[1]).Shopping_List.First(x => x.Name == temp[2]);
                    ref_SI.Time_Set = false;
                }
                Invalidate();
            }
            Grey_In();
            Edit_Str = "";
        }

        private void TimeClose(object sender, ToolStripDropDownClosedEventArgs e)
        {
            Grey_In();
        }

        private TimeSpan Time_Chooser;

        private void TimePicker_Choosed(object sender, EventArgs e)
        {
            Time_Chooser = timePickerPanel1.timePicker.Value.TimeOfDay;
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

        private void close_entry_button_Click(object sender, EventArgs e)
        {
            start_height = 125;
            bufferedPanel1.Visible = false;
            add_entry_button.Visible = true;
            add_agenda_item.Visible = true;
            label3.Visible = true;
            label7.Visible = true;
            Invalidate();

            // Reset boxes
            title_box.Text = "";
            importance_box.Text = "";
            desc_box.Text = "";
            alert_on.Checked = false;
            remind_me_x_days.Text = "";
            Edit_Index = -1;
        }

        private void add_entry_button_Click(object sender, EventArgs e)
        {
            start_height = 246;
            bufferedPanel1.Visible = true;
            add_entry_button.Visible = false;
            add_agenda_item.Visible = false;
            label3.Visible = false;
            label7.Visible = false;
            Invalidate();
        }

        // Get Random Order ID
        Random OrderID_Gen = new Random();

        // Return available hash value for Calendar Events
        private string Return_Available_Hash_CE()
        {
            string RandomHash = OrderID_Gen.Next(100000000, 999999999).ToString();

            while (parent.Calendar_Events_List.Any(x => x.Hash_Value == RandomHash))
            {
                RandomHash = OrderID_Gen.Next(100000000, 999999999).ToString();
            }

            return RandomHash;
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (title_box.Text.Length > 0 && importance_box.Text.Length > 0 && desc_box.Text.Length >= 0)
            {
                if (Edit_Index > -1)
                {
                    parent.Calendar_Events_List.Remove(Calendar_Events_List[Edit_Index]);
                    ToolTip2.RemoveAll();
                    ToolTip2.SetToolTip(Add_button, "Add new event");
                    Add_button.Image = global::Financial_Journal.Properties.Resources.add_notes;
                }

                Calendar_Events CE = new Calendar_Events();
                CE.Title = title_box.Text.Trim();
                CE.Is_Active = "1";
                CE.Hash_Value = Edit_Index > -1 ? Edit_Calendar.Hash_Value : Return_Available_Hash_CE();
                CE.Contact_Hash_Value = Edit_Index > -1 ? Edit_Calendar.Contact_Hash_Value : "";
                CE.Time_Set = Edit_Index > -1 ? Edit_Calendar.Time_Set : false;
                CE.Importance = importance_box.Items.IndexOf(importance_box.Text);
                CE.Description = desc_box.Text.Trim();
                CE.Date = Edit_Index > -1 ? Edit_Calendar.Date : calendarDate;
                if (alert_on.Checked && remind_me_x_days.Text.Length > 0)
                {
                    DateTime temp_DT = calendarDate;
                    for (int i = 0; i < Convert.ToInt32(remind_me_x_days.Text); i++)
                    {
                        temp_DT = temp_DT.AddDays(-1);
                        CE.Alert_Dates.Add(temp_DT);
                    }
                }
                if (multiDayBox.Checked)
                {
                    CE.MultiDays = (int)Math.Round((decimal)(multiDate.Value - CE.Date).TotalDays);
                }
                parent.Calendar_Events_List.Add(CE);

                // Reset boxes
                title_box.Text = "";
                importance_box.Text = "";
                desc_box.Text = "";
                alert_on.Checked = false;
                remind_me_x_days.Text = "";
                Edit_Index = -1;


                close_entry_button.PerformClick();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Form_Message_Box FMB = new Form_Message_Box(parent, "Low Importance - Calendar shows only icon " + Environment.NewLine + "Important - Calendar shows icon and title" + Environment.NewLine + "Very Important - Show icon, title, and bold box", true, 50, this.Location, this.Size);
            FMB.ShowDialog();
            Grey_In();
        }

        private void remind_me_x_days_SelectedIndexChanged(object sender, EventArgs e)
        {
            alert_on.Checked = true;
        }

        private void add_agenda_item_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Select_Agenda SA = new Select_Agenda(parent, calendarDate, this.Location, this.Size);
            SA.ShowDialog();
            parent_Calendar.Activate();
            Invalidate();
            Grey_In();
        }


        private void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        private void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
        }


        #region Extension .dll function

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int SWP_NOMOVE = 0x0002;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
                                                 int X, int Y, int cx, int cy, uint uFlags);

        enum idCursor
        {
            HAND = 32649,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr LoadCursor(IntPtr hInstance, idCursor cursor);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern bool PlaySound(string pszSound, UIntPtr hmod, uint fdwSound);
        [Flags]
        public enum sndFlags
        {
            SND_SYNC = 0x0000,
            SND_ASYNC = 0x0001,
            SND_NODEFAULT = 0x0002,
            SND_LOOP = 0x0008,
            SND_NOSTOP = 0x0010,
            SND_NOWAIT = 0x00002000,
            SND_FILENAME = 0x00020000,
            SND_RESOURCE = 0x00040004
        }

        #endregion

        private void multiDayBox_CheckedChanged(object sender, EventArgs e)
        {
            multiDate.Visible = label8.Visible = multiDayBox.Checked;
        }

        private void multiDate_ValueChanged(object sender, EventArgs e)
        {
            if (multiDate.Value.Date <= calendarDate.Date)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Invalid date entry", true, -20, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
                multiDate.Value = calendarDate.AddDays(1).Date;
                multiDayBox.Checked = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Financial_Journal
{
    public partial class ContactImpExp : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }

        Receipt parent;
        Contacts parent_Contacts;
        Size Start_Size = new Size();

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public ContactImpExp(Receipt _parent, Contacts c, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            parent_Contacts = c;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

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

            button1.Image = (parent_Contacts.Editing_Contacts ? global::Financial_Journal.Properties.Resources.edit_on : global::Financial_Journal.Properties.Resources.edit_off);

            if (parent_Contacts.hasImported)
            {
                undo_import.Image = global::Financial_Journal.Properties.Resources.undo;
                undo_import.Enabled = true;
            }
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

        private List<Contact> Pre_Import_Contact_List = new List<Contact>();

        private void import_Click(object sender, EventArgs e)
        {
            parent_Contacts.Pre_Import_Contact_List = new List<Contact>();
            parent.Contact_List.ForEach(x => parent_Contacts.Pre_Import_Contact_List.Add(x.Clone_Contact()));

            string filePath = "";
            vCardParser contacts = new vCardParser();

            OpenFileDialog fbd = new OpenFileDialog();
            fbd.Filter = "vcf files (*.vcf)|*.vcf";
            fbd.Title = "Select the file you wish to import contact information from:";

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.FileName))
            {
                filePath = fbd.FileName;
                contacts.PopulateCardsFromVCF(filePath);
            }

            // Reset list
            parent_Contacts.CO_Filtered_List = new List<Contact>();

            // If file chosen
            if (filePath.Length > 0)
            {
                foreach (vCard c in contacts)
                {
                    Random OrderID_Gen = new Random();
                    string hashID = OrderID_Gen.Next(100000000, 999999999).ToString();

                    // Remove hash collision
                    while (parent.Contact_List.Any(x => x.Hash_Value == hashID))
                    {
                        hashID = OrderID_Gen.Next(100000000, 999999999).ToString();
                    }

                    Contact Contact = new Contact()
                    {
                        Hash_Value = hashID,
                        Association = "Friend"
                    };

                    Contact.First_Name = c.FirstName;
                    Contact.Last_Name = c.LastName;

                    if (c.Emails.Count > 0)
                    {
                        Contact.Email = c.Emails[0];
                    }
                    if (c.Emails.Count > 1)
                    {
                        Contact.Email = c.Emails[1];
                    }
                    if (c.PhoneNumbers.Count > 0)
                    {
                        Contact.Phone_No_Primary = c.PhoneNumbers[0];
                    }
                    if (c.PhoneNumbers.Count > 1)
                    {
                        Contact.Phone_No_Second = c.PhoneNumbers[1];
                    }

                    parent_Contacts.CO_Filtered_List.Add(Contact);
                }

                parent_Contacts.CO_Filtered_List = parent_Contacts.CO_Filtered_List.OrderBy(x => x.First_Name).ToList();

                Grey_Out();
                Contact_Filter CF = new Contact_Filter(parent, parent_Contacts, this.Location, this.Size);
                CF.ShowDialog();
                Grey_In();

                parent_Contacts.CO_Filtered_List.ForEach(x => parent.Contact_List.Add(x.Clone_Contact()));

                parent_Contacts.repaint_buttons = true;
                parent_Contacts.Invalidate();

                if (parent_Contacts.CO_Filtered_List.Count > 0)
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Import Successful!", true, -20, Location, Size);
                    FMB.ShowDialog();
                    undo_import.Image = global::Financial_Journal.Properties.Resources.undo;
                    undo_import.Enabled = true;
                    parent_Contacts.hasImported = true;
                    Grey_In();
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            parent_Contacts.Editing_Contacts = !parent_Contacts.Editing_Contacts;
            button1.Image = (parent_Contacts.Editing_Contacts ? global::Financial_Journal.Properties.Resources.edit_on : global::Financial_Journal.Properties.Resources.edit_off);
            parent_Contacts.repaint_buttons = true;
            parent_Contacts.Invalidate();
            parent_Contacts.Activate();
            this.Close();
        }

        private void export_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the directory you wish to export contact file to:";

            DialogResult result = fbd.ShowDialog();

            string exportFilePath = "";

            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    exportFilePath = fbd.SelectedPath;
                }

                exportFilePath += "\\" + Environment.UserName + "_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".vcf";

                parent_Contacts.CO_Filtered_List = new List<Contact>();
                parent.Contact_List.ForEach(x => parent_Contacts.CO_Filtered_List.Add(x.Clone_Contact()));

                parent_Contacts.CO_Filtered_List = parent_Contacts.CO_Filtered_List.OrderBy(x => x.First_Name).ToList();

                Contact_Filter CF = new Contact_Filter(parent, parent_Contacts, this.Location, this.Size, true);
                CF.ShowDialog();

                if (parent_Contacts.CO_Filtered_List.Count > 0)
                {
                    string line = "";
                    foreach (Contact c in parent_Contacts.CO_Filtered_List)
                    {
                        line += "BEGIN:VCARD" + Environment.NewLine;
                        line += "VERSION:3.0" + Environment.NewLine;
                        line += "PRODID:" + "PersonalBanker.exe_v_1.0.0.12" + Environment.NewLine;
                        line += "N:" + c.Last_Name + ";" + c.First_Name + Environment.NewLine;
                        line += "FN:" + c.First_Name + " " + c.Last_Name + Environment.NewLine;
                        if (c.Phone_No_Primary.Length > 0)
                            line += "TEL;TYPE=CELL;TYPE=pref;TYPE=VOICE:" + c.Phone_No_Primary + Environment.NewLine;
                        if (c.Phone_No_Primary.Length > 0)
                            line += "TEL;TYPE=CELL;TYPE=VOICE:" + c.Phone_No_Second + Environment.NewLine;
                        if (c.Email.Length > 0)
                            line += "EMAIL;TYPE=WORK;TYPE=pref;TYPE=INTERNET:" + c.Email + Environment.NewLine;
                        if (c.Email_Second.Length > 0)
                            line += "EMAIL;TYPE=WORK;TYPE=INTERNET:" + c.Email_Second + Environment.NewLine;
                        line += "REV:" + DateTime.Now.ToShortDateString() + "APersonalBanker" + Environment.NewLine;
                        line += "END:VCARD" + Environment.NewLine;
                    }

                    // Create new mapping file with new serial attached
                    using (StreamWriter sw = File.CreateText(exportFilePath)) //
                    {
                        sw.Write(line);
                        sw.Close();
                    }

                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Information export successfully to \"" + exportFilePath + "\"", true, 15, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
            }
        }

        private void undo_import_Click(object sender, EventArgs e)
        {
            parent.Contact_List = new List<Contact>();
            parent_Contacts.Pre_Import_Contact_List.ForEach(x => parent.Contact_List.Add(x.Clone_Contact()));
            parent_Contacts.Invalidate();
            this.Close();
        }
    }
}

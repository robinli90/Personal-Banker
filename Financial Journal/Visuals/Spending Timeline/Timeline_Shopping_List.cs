using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace Financial_Journal
{
    public partial class Timeline_Shopping_List : Form
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
        public Timeline_Shopping_List(Receipt _parent, Point g = new Point(), Size s = new Size())
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
            emailList.Enabled = parent.Settings_Dictionary["LOGIN_EMAIL"].Length > 3;

            #region Add checkboxes
            DataGridViewCheckBoxColumn cbc = new DataGridViewCheckBoxColumn();
            {
                cbc.HeaderText = "";
                cbc.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;
                cbc.Width = 23;
                cbc.FlatStyle = FlatStyle.Flat;
                cbc.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                cbc.CellTemplate.Style.ForeColor = Color.White;
                cbc.DisplayIndex = 0;
            }
            dataGridView1.Columns.Insert(0, cbc);
            #endregion

            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            dataGridView1.GridColor = Color.White;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(98, 110, 110);

            PopulateDGV();

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

        List<Item> Item_List;
        List<string> Extraneous_Item_List;
        int itemListCount = 0;

        private void PopulateDGV()
        {
            #region Add Master Item List items
            Item_List = new List<Item>();
            Extraneous_Item_List = new List<string>();

            // We want to filter the list for existing items already denoted as an asset
            foreach (Item item in parent.Master_Item_List.Where(x => x.consumedStatus == 1).ToList())
            {
                Item_List.Add(item);
            }

            itemListCount = Item_List.Count;

            dataGridView1.Rows.Clear();

            foreach (Item item in Item_List)
            {
                dataGridView1.Rows.Add(false, item.Name, item.Category);
            }
            #endregion

            #region Add extraneous shopping list items (manually entered)
            Extraneous_Item_List = parent.Settings_Dictionary["EXTRANEOUS_SHOPPING_ITEMS"].Trim('~').Split(new string[] { "~" }, StringSplitOptions.None).ToList();

            if (Extraneous_Item_List.Count > 1)
            {
                for (int i = 0; i < Extraneous_Item_List.Count; i += 2)
                {
                    dataGridView1.Rows.Add(false, Extraneous_Item_List[i], Extraneous_Item_List[i + 1]);
                }
            }
            #endregion
        }

        /// <summary>
        /// set extraneous list to master settings
        /// </summary>
        public void Save_Extraneous_List()
        {
            string exportList = "";

            if (Extraneous_Item_List.Count > 1)
            {
                for (int i = 0; i < Extraneous_Item_List.Count; i += 2)
                {
                    exportList += "~" + Extraneous_Item_List[i] + "~" + Extraneous_Item_List[i + 1];
                }
            }

            parent.Settings_Dictionary["EXTRANEOUS_SHOPPING_ITEMS"] = (exportList.Length > 0) ? exportList.Substring(1) : ""; //ignore first char
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

        private void UploadShoppingList()
        {
            string saveText = "";

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];

                // checked
                saveText += "[SI_NA_]=" + row.Cells[1].Value.ToString() + "||[SI_CA_]=" +
                            row.Cells[2].Value.ToString() + Environment.NewLine;
            }
        
            // Create temp sync file
            string tempShoppingListFile = parent.localSavePath + "\\" +
                                        parent.Settings_Dictionary["LOGIN_EMAIL"] + "_shop.pbf";
            using (StreamWriter sw = File.CreateText(tempShoppingListFile)) //
            {
                sw.Write(AESGCM.SimpleEncryptWithPassword(saveText, MobileSync.AESGCMKey));
                sw.Close();
            }

            // Create mapping file on FTP
            string ftpPath =
                @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/Sync/" +
                parent.Settings_Dictionary["LOGIN_EMAIL"] + "_shop.pbf";
            Cloud_Services.FTP_Upload_Synced(ftpPath, tempShoppingListFile);

            try
            {
                File.Delete(tempShoppingListFile);
            }
            catch (Exception e)
            {
                Diagnostics.WriteLine("Cannot delete temporary sync file");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView1.ClearSelection();
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                if (e.ColumnIndex == 0)
                {
                    if (Convert.ToBoolean(row.Cells[0].Value))
                    {
                        row.SetValues(false);
                    }
                    else
                    {
                        row.SetValues(true);
                    }
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }


        private void close_button_Click(object sender, EventArgs e)
        {
            List<int> removeExtraneousIndex = new List<int>();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];

                // If checked
                if (Convert.ToBoolean(row.Cells[0].Value))
                {
                    if (i < itemListCount) // if from master item list
                    {
                        Item_List[i].consumedStatus = 0;
                    }
                    else
                    {
                        removeExtraneousIndex.Add(i - itemListCount);                        
                    }
                }
            }

            for (int i = removeExtraneousIndex.Count - 1; i >= 0 ; i--)
            {
                Extraneous_Item_List.RemoveAt(removeExtraneousIndex[i] * 2); // remove name
                Extraneous_Item_List.RemoveAt(removeExtraneousIndex[i] * 2); // remove category
            }

            Save_Extraneous_List();

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

        private void addItem_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Add_Shopping_List_Item ASLI = new Add_Shopping_List_Item(parent, this.Location, this.Size);
            ASLI.ShowDialog();
            Grey_In();
            PopulateDGV();
        }

        private void clearList_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to clear shopping list?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                {
                    List<int> removeExtraneousIndex = new List<int>();

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        DataGridViewRow row = dataGridView1.Rows[i];

                        if (i < itemListCount) // if from master item list
                        {
                            Item_List[i].consumedStatus = 0;
                        }
                        else
                        {
                            removeExtraneousIndex.Add(i - itemListCount);
                        }
                    }

                    for (int i = removeExtraneousIndex.Count - 1; i >= 0; i--)
                    {
                        Extraneous_Item_List.RemoveAt(removeExtraneousIndex[i] * 2); // remove name
                        Extraneous_Item_List.RemoveAt(removeExtraneousIndex[i] * 2); // remove category
                    }

                    Save_Extraneous_List();
                    PopulateDGV();
                }
            }
            Grey_In();
        }

        private void exportList_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                // Create base agenda item
                Agenda_Item AI = new Agenda_Item()
                {
                    Name = "Shopping List - " + DateTime.Now.ToShortDateString(),
                    Check_State = false,
                    ID = parent.Agenda_Item_List.Count + 1,
                    Date = DateTime.Now,
                    Calendar_Date = new DateTime(1800, 1, 1),
                    Hash_Value = Return_Available_Hash_AI(),
                    Contact_Hash_Value = "",
                    Shopping_List = new List<Shopping_Item>()
                };

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewRow row = dataGridView1.Rows[i];

                    AI.Shopping_List.Add(new Shopping_Item
                    {
                        Name = row.Cells[1].Value.ToString(),
                        Check_State = false,
                        ID = AI.ID,
                        Calendar_Date = new DateTime(1800, 1, 1),
                        Hash_Value = Return_Available_Hash_SI(),
                        Contact_Hash_Value = ""
                    });
                }

                parent.Agenda_Item_List.Insert(0, AI);

                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Successfully created shopping list with " + AI.Shopping_List.Count + " item(s)", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: No items in shopping list", true, -10, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
        }

        // Get Random Order ID
        Random OrderID_Gen = new Random();

        // Return available hash value for Agenda Item
        public string Return_Available_Hash_AI()
        {
            string RandomHash = OrderID_Gen.Next(100000000, 999999999).ToString();

            while (parent.Agenda_Item_List.Any(x => x.Hash_Value == RandomHash))
            {
                RandomHash = OrderID_Gen.Next(100000000, 999999999).ToString();
            }

            return RandomHash;
        }

        // Return available hash value for Shopping Item
        public string Return_Available_Hash_SI()
        {
            string RandomHash = OrderID_Gen.Next(100000000, 999999999).ToString();

            List<Shopping_Item> Temp_SI_List = new List<Shopping_Item>();
            parent.Agenda_Item_List.ForEach(x => Temp_SI_List.AddRange(x.Shopping_List));

            while (Temp_SI_List.Any(x => x.Hash_Value == RandomHash))
            {
                RandomHash = OrderID_Gen.Next(100000000, 999999999).ToString();
            }

            return RandomHash;
        }

        private void emailList_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form2 = new Yes_No_Dialog(parent, "Are you sure you wish to email your current shopping list?", "Warning",
                "No", "Yes", 0, this.Location, this.Size))
            {
                var result22 = form2.ShowDialog();
                if (result22 == DialogResult.OK && form2.ReturnValue1 == "1")
                {
                    if (dataGridView1.Rows.Count > 0)
                    {
                        string shoppingListStr = "Here is your shopping list (generated on " +
                                                 DateTime.Now.ToShortDateString() + "):" + Environment.NewLine +
                                                 Environment.NewLine;

                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            DataGridViewRow row = dataGridView1.Rows[i];
                            shoppingListStr += "    " + (i + 1).ToString() + ") " + row.Cells[1].Value.ToString() +
                                               Environment.NewLine;
                        }

                        try
                        {
                            MailMessage mailmsg = new MailMessage();
                            MailAddress from = new MailAddress("automatedpersonalbanker@gmail.com");
                            mailmsg.From = from;
                            mailmsg.To.Add(parent.Settings_Dictionary["PERSONAL_EMAIL"]);
                            mailmsg.Subject = "Personal Banker Shopping List";
                            mailmsg.Body = "This is an automated message. Please do not reply to this email. " +
                                           Environment.NewLine + Environment.NewLine +

                                           shoppingListStr;

                            mailmsg.Body += "" + Environment.NewLine;

                            // smtp client
                            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                            client.EnableSsl = true;
                            NetworkCredential credential =
                                new NetworkCredential("automatedpersonalbanker@gmail.com", "R5o2b6i8R5o2b6i8");
                            client.Credentials = credential;
                            //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                            Task.Run(() => { client.Send(mailmsg); });

                            Form_Message_Box FMB = new Form_Message_Box(parent, "Shopping list has been emailed to you",
                                true, 0, this.Location, this.Size);
                            FMB.ShowDialog();
                        }
                        catch
                        {
                            Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Email is invalid", true, 0,
                                this.Location, this.Size);
                            FMB.ShowDialog();
                        }
                    }
                    else
                    {
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Error: No items in shopping list", true,
                            -10, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                }
            }
            Grey_In();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to sync your shopping list?", "Warning",
                "No", "Yes", 0, this.Location, this.Size))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                {
                    Grey_Out();
                    if (dataGridView1.Rows.Count > 0)
                    {
                        if (secondThreadFormHandle == IntPtr.Zero)
                        {

                            Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size,
                                "UPLOADING", "SHOPPING LIST", 11);
                            form.HandleCreated += SecondFormHandleCreated;
                            form.HandleDestroyed += SecondFormHandleDestroyed;
                            form.RunInNewThread(false);
                        }

                        UploadShoppingList();

                        if (secondThreadFormHandle != IntPtr.Zero)
                            PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);


                        Form_Message_Box FMB =
                            new Form_Message_Box(parent, "Shopping list synced to cloud successfully!", true, -10,
                                this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    else
                    {
                        Form_Message_Box FMB =
                            new Form_Message_Box(parent, "You have no items to sync to cloud", true, -10,
                                this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    Grey_In();
                }
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

        private void button2_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to remove selected items?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                {
                    List<int> removeExtraneousIndex = new List<int>();

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        DataGridViewRow row = dataGridView1.Rows[i];
                        // If checked
                        if (Convert.ToBoolean(row.Cells[0].Value))
                        {

                            if (i < itemListCount) // if from master item list
                            {
                                Item_List[i].consumedStatus = 0;
                            }
                            else
                            {
                                removeExtraneousIndex.Add(i - itemListCount);
                            }
                        }
                    }

                    for (int i = removeExtraneousIndex.Count - 1; i >= 0; i--)
                    {
                        Extraneous_Item_List.RemoveAt(removeExtraneousIndex[i] * 2); // remove name
                        Extraneous_Item_List.RemoveAt(removeExtraneousIndex[i] * 2); // remove category
                    }

                    Save_Extraneous_List();
                    PopulateDGV();
                }
            }
            Grey_In();
        }

    }
}

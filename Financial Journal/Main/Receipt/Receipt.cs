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
using System.Diagnostics;

using System.Runtime.InteropServices;
using System.Threading;

namespace Financial_Journal
{
    
    public partial class Receipt : Form
    {
        bool inTestMode = true;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;

            bool Quit = false;
            if (Item_List.Count > 0)
            {
                using (var form = new Yes_No_Dialog(this, "You have un-submitted items. Do you wish to terminate?", "Warning", new Point(this.Left + 84, this.Top + 100)))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        if (form.ReturnValue1 == "1")
                        {
                            Quit = true;
                        }
                    }
                }
            }
            else
            {
                Quit = true;
            }
            if (Quit)
            {
                #region Compare current version with the original loaded version and if changes, prompt a save
                // Load current journal info
                string Info_Path = Directory.GetCurrentDirectory() + "\\journal_info.txt";
                var text = File.ReadAllText(Info_Path); // current journal
                var text_temp = File.ReadAllText(Info_Path); // temporary mirror journal

                // force decrypt
                if (Enable_Encrypt && !text.Contains("[INFO_TYPE]=ITEM||[ITEM_DESC]"))
                {
                    text = AESGCM.SimpleDecryptWithPassword(text, "PASSWORDisHERE");
                }

                Enable_Encrypt = false;
                try
                {

                    // Try deleting temp file
                    File.Delete(Directory.GetCurrentDirectory() + "\\temp.txt");
                    Save("\\temp.txt");
                    File.SetAttributes(Directory.GetCurrentDirectory() + "\\temp.txt", File.GetAttributes(Directory.GetCurrentDirectory() + "\\temp.txt") | FileAttributes.Hidden);
                    text_temp = File.ReadAllText(Directory.GetCurrentDirectory() + "\\temp.txt");
                }
                catch
                {
                }
                Enable_Encrypt = true;
                #endregion

                if (text.Trim() != text_temp.Trim())
                {

                    using (var form = new Yes_No_Dialog(this, "You have made changes. Do you wish to save changes?", "Warning", new Point(this.Left + 84, this.Top + 70)))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            if (form.ReturnValue1 == "1")
                            {
                                Save();
                            }
                        }
                    }
                }
                try
                {
                    File.Delete(Directory.GetCurrentDirectory() + "\\temp.txt");
                }
                catch
                {
                }
                this.Dispose();
                this.Close();
            }
            base.OnFormClosing(e);
        }
        //bool inTestMode = false;

        /* Force mouse click
         * 
        [DllImport("user32.dll",CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public void DoMouseClick()
        {
            //Call the imported function with the cursor's current position
            uint X = Convert.ToUInt32(Cursor.Position.X);
            uint Y = Convert.ToUInt32(Cursor.Position.Y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        } 
        */

        protected override void OnPaint(PaintEventArgs e)
        {
            
            int data_height = 26;
            int start_height = Start_Size.Height;
            int start_margin = 15;              // Item
            int height_offset = 9;

            int margin1 = start_margin + 170;   //Price
            int margin2 = margin1 + 85;        //Quantity
            int margin3 = margin2 + 125;        //Category
            int margin4 = margin3 + 162;        //Actions

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
            Font f = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_italic = new Font("MS Reference Sans Serif", 9, FontStyle.Italic);

            // If has order
            if (paint && Item_List.Count > 0)
            {

                bufferedPanel2.Visible = true;

                location_box.Enabled = false;
                //payment_type.Enabled = false;
                dateTimePicker1.Enabled = false;
                location_box.ForeColor = Color.LightBlue;
                //payment_type.ForeColor = Color.LightBlue;


                // Draw gray header line
                e.Graphics.DrawLine(Grey_Pen, start_margin, start_height, this.Width - 15, start_height);

                height_offset += 1;
                // Header2   
                e.Graphics.DrawString("Item", f_header, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Unit Price", f_header, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Quantity", f_header, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Category", f_header, WritingBrush, margin3 - 10, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Actions", f_header, WritingBrush, margin4, start_height + height_offset + (row_count * data_height));
                row_count += 1;
                height_offset += 5;


                Delete_Item_Buttons.ForEach(button => button.Image.Dispose());
                Delete_Item_Buttons.ForEach(button => button.Dispose());
                Delete_Item_Buttons.ForEach(button => this.Controls.Remove(button));
                Delete_Item_Buttons = new List<Button>();
                Edit_Buttons.ForEach(button => button.Image.Dispose());
                Edit_Buttons.ForEach(button => button.Dispose());
                Edit_Buttons.ForEach(button => button.Image.Dispose());
                Edit_Buttons.ForEach(button => this.Controls.Remove(button));
                Edit_Buttons = new List<Button>();
                Discount_Buttons.ForEach(button => button.Image.Dispose());
                Discount_Buttons.ForEach(button => button.Dispose());
                Discount_Buttons.ForEach(button => this.Controls.Remove(button));
                Discount_Buttons = new List<Button>();

                bool has_exempt = false;
                int item_index = 0;
                double Running_Total = 0;
                double Tax_Total = 0;
                int Running_Quantity = 0;

                double total_discount = Item_List.Where(item => item.Discount_Amt > 0).Sum(item => item.Discount_Amt);
                
                foreach (Item item in Item_List)
                {
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button delete_button = new Button();
                    delete_button.BackColor = this.BackColor;
                    delete_button.ForeColor = this.BackColor;
                    delete_button.FlatStyle = FlatStyle.Flat;
                    delete_button.Image = item.Status != "0" ? global::Financial_Journal.Properties.Resources.na : global::Financial_Journal.Properties.Resources.delete;
                    delete_button.Enabled = item.Status == "0";
                    delete_button.Size = new Size(29, 29);
                    delete_button.Location = new Point(this.Width - 40, start_height + height_offset + (row_count * data_height) - 6);
                    delete_button.Name = "d" + item_index.ToString();
                    delete_button.Text = "";
                    delete_button.Click += new EventHandler(this.dynamic_button_click);
                    Delete_Item_Buttons.Add(delete_button);
                    ToolTip1.SetToolTip(delete_button, "Delete " + item.Name);
                    this.Controls.Add(delete_button);

                    Button discount_button = new Button();
                    discount_button.BackColor = this.BackColor;
                    discount_button.ForeColor = this.BackColor;
                    discount_button.FlatStyle = FlatStyle.Flat;
                    discount_button.Size = new Size(29, 29);
                    discount_button.Enabled = item.Status == "0";
                    discount_button.Image = item.Status != "0" ? global::Financial_Journal.Properties.Resources.na : global::Financial_Journal.Properties.Resources.discount;
                    discount_button.Location = new Point(this.Width - 82, start_height + height_offset + (row_count * data_height) - 6);
                    discount_button.Name = "s" + item_index.ToString();
                    discount_button.Text = "";
                    discount_button.Click += new EventHandler(this.dynamic_button_click);
                    Discount_Buttons.Add(discount_button);
                    ToolTip1.SetToolTip(discount_button, "Apply Discount to " + item.Name);
                    this.Controls.Add(discount_button);

                    Button edit_button = new Button();
                    edit_button.BackColor = this.BackColor;
                    edit_button.ForeColor = this.BackColor;
                    edit_button.FlatStyle = FlatStyle.Flat;
                    edit_button.Size = new Size(29, 29);
                    edit_button.Enabled = item.Status == "0";
                    edit_button.Image = (edit_index == item_index ? global::Financial_Journal.Properties.Resources.accept : item.Status != "0" ? global::Financial_Journal.Properties.Resources.na : global::Financial_Journal.Properties.Resources.edit);
                    edit_button.Location = new Point(this.Width - 124, start_height + height_offset + (row_count * data_height) - 6);
                    edit_button.Name = "e" + item_index.ToString();
                    edit_button.Text = "";
                    edit_button.Click += new EventHandler(this.dynamic_button_click);
                    Edit_Buttons.Add(edit_button);
                    ToolTip1.SetToolTip(edit_button, (edit_index == item_index ? "Apply " : "Edit " + item.Name));
                    this.Controls.Add(edit_button);
              

                    string temp = Tax_Rules_Dictionary.ContainsKey(item.Category) ? ((Tax_Rules_Dictionary[item.Category] == "0") ? "*" : "") : "";
                    if (temp.Length > 0) has_exempt = true;

                    e.Graphics.DrawString(item.Name, f, (item.Status == "0") ? WritingBrush : RedBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", item.Price) + temp, f, WritingBrush, margin1 + 10 - (item.Price >= 10 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString((item.Quantity - Convert.ToInt32(item.Status)).ToString(), f, WritingBrush, margin2 + 19 - (item.Quantity >= 10 ? 5 : 0), start_height + height_offset + (row_count * data_height));
                    using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1)))
                    {
                        SizeF size = graphics.MeasureString(item.Category, new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point));
                        e.Graphics.DrawString(item.Category, f, WritingBrush, margin3 + 15 - (size.Width)/2, start_height + height_offset + (row_count * data_height));
                    }

                    if (item.Discount_Amt > 0)
                    {
                        row_count++;
                        height_offset -= 5;
                        e.Graphics.DrawString("Discount", f_italic, WritingBrush, margin1 - 76, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString("-($" + String.Format("{0:0.00}", item.Discount_Amt) + ")", f, WritingBrush, margin1 + 2 - (item.Price >= 10 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                        height_offset += 2;
                    }
                    if (item.Status != "0")
                    {
                        row_count++;
                        height_offset -= 3;
                        if (item.Discount_Amt > 0) row_count--;
                        e.Graphics.DrawString(((item.Status == "0") ? "" : " (Refunded " + item.Status + ")"), f_italic, (item.Status == "0") ? WritingBrush : RedBrush, start_margin - 2, start_height + height_offset + (row_count * data_height));
                        height_offset += 2;
                    }

                    // Adjust for refunded items w/ discount
                    total_discount -= item.Discount_Amt - item.Get_Current_Discount();
                    Running_Total -= item.Price * (Convert.ToInt32(item.Status));
                    Tax_Total -= item.Price * item.Quantity * Get_Tax_Amount(item) - item.Get_Current_Tax_Amount(Get_Tax_Amount(item));

                    Running_Total += item.Price * item.Quantity;
                    Running_Quantity += item.Status.StartsWith("r") ? (item.Quantity - Convert.ToInt32(item.Status.Substring(1))) : (item.Quantity - Convert.ToInt32(item.Status));
                    Tax_Total += (Tax_Rules_Dictionary.ContainsKey(item.Category) ? Convert.ToDouble(Tax_Rules_Dictionary[item.Category]) * item.Price * item.Quantity : Tax_Rate * item.Price * item.Quantity);
                    row_count++;
                    item_index++;
                    height_offset += 2;
                }


                // Total line
                e.Graphics.DrawLine(p, start_margin, start_height + height_offset + (row_count * data_height), this.Width - 15, start_height + height_offset + (row_count * data_height));
                height_offset += 4;
                e.Graphics.DrawString("Subtotal", f, WritingBrush, margin1 - 75, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Running_Total), f, WritingBrush, margin1 + 10 - (Running_Total >= 100 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString(Running_Quantity.ToString(), f, WritingBrush, margin2 + 19 - (Running_Quantity >= 10 ? 5 : 0), start_height + height_offset + (row_count * data_height));
                row_count++;

                string note = "";
                List<Button> Temp = Edit_Buttons.Where(x => x.Enabled == false).ToList();
                if (Temp.Count() > 0)
                {
                    // Add note
                    height_offset -= 20;
                    e.Graphics.DrawString("*Note: Certain actions are disabled because" + note, f, WritingBrush, margin3 - 20, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("            you cannot modify refunded items" + note, f, WritingBrush, margin3 - 20, start_height + height_offset + (row_count * data_height) + 18);
                    height_offset += 20;
                }

                height_offset -= 2;
                e.Graphics.DrawString("Taxes", f, WritingBrush, margin1 - 60, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString(Tax_Exempt_Order ? "$0.00" : "$" + (Tax_Override_Amt > 0 ? String.Format("{0:0.00}", Tax_Override_Amt) : String.Format("{0:0.00}", Tax_Total)), f, WritingBrush, margin1 + 10 - (Tax_Total >= 10 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                row_count++;
                height_offset -= 2;
                if (total_discount > 0)
                {
                    e.Graphics.DrawString("Less Discounts", f, WritingBrush, margin1 - 110, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("-($" + String.Format("{0:0.00}", total_discount) + ")", f, WritingBrush, margin1  - (total_discount >= 10 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                    row_count++;
                }
                height_offset -= 6;
                e.Graphics.DrawLine(p, margin1 - 75, start_height + height_offset + (row_count * data_height), margin2, start_height + height_offset + (row_count * data_height));
                height_offset += 5;
                e.Graphics.DrawString("Total", f_total, WritingBrush, margin1 - 56, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", (Running_Total + (Tax_Exempt_Order ? 0 : (Tax_Override_Amt > 0 ? Tax_Override_Amt : Tax_Total)) - total_discount)), f_total, WritingBrush, margin1 + 10 - (Tax_Total >= 10 ? (Tax_Total >= 100 ? 15 : 8) : 0), start_height + height_offset + (row_count * data_height));

                row_count++;
                if (has_exempt) e.Graphics.DrawString("*Tax Exempt", f_asterisk, RedBrush, start_margin - 12, start_height + 12 + height_offset + 4 + (row_count * data_height));
                row_count++;


                //Draw accounting double lines
                //height_offset += 19;
                //e.Graphics.DrawLine(p, margin5, start_height + height_offset + (row_count * data_height), margin6, start_height + height_offset + (row_count * data_height));
                ///e.Graphics.DrawLine(p, margin5, start_height + height_offset + 2 + (row_count * data_height), margin6, start_height + height_offset + 2 + (row_count * data_height));
                //height_offset -= 17;
                
                 
                //row_count++;
                height_offset += 5;
                this.Height = start_height + height_offset + row_count * data_height;
            }
            else
            {
                bufferedPanel2.Visible = false;
                this.Height = Start_Size.Height;
                location_box.Enabled = true;
                payment_type.Enabled = true;
                location_box.ForeColor = Color.White;
                payment_type.ForeColor = Color.White;
            }

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
         
        private void dynamic_button_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            Expenses Ref_Expense = new Expenses();

            // Remove existing comboboxes
            List_Combos.ForEach(x => this.Controls.Remove(x));
            List_Combos = new List<AdvancedComboBox>();

            if (b.Name.StartsWith("d")) // delete
            {
                Item_List.RemoveAt(Convert.ToInt32(b.Name.Substring(1)));
                edit_index = -1;
                Invalidate();
                Update();
                Add_button.Visible = true;
            }
            else if (b.Name.StartsWith("s")) // discount
            {
                Input_Box IB = new Input_Box(this, "Set total discount amount", (Item_List[Convert.ToInt32(b.Name.Substring(1))].Discount_Amt).ToString());
                IB.ShowDialog();
                double Cost_w_tax = (Item_List[Convert.ToInt32(b.Name.Substring(1))].Quantity * Item_List[Convert.ToInt32(b.Name.Substring(1))].Price) * (1 + (Tax_Rules_Dictionary.ContainsKey(Item_List[Convert.ToInt32(b.Name.Substring(1))].Category) ? Convert.ToDouble(Tax_Rules_Dictionary[Item_List[Convert.ToInt32(b.Name.Substring(1))].Category]) : Tax_Rate));
                Item_List[Convert.ToInt32(b.Name.Substring(1))].Discount_Amt = (Discount_Transfer_Amount > Cost_w_tax ? (Cost_w_tax - Discount_Transfer_Amount < 0 ? Cost_w_tax : Cost_w_tax - Discount_Transfer_Amount) : Discount_Transfer_Amount); // If transfer amt is greater than unit price, set discount to unit price
                Discount_Transfer_Amount = 0;
                edit_index = -1;
                Invalidate();
                Update();
                Add_button.Visible = true;
            }
            else if (b.Name.StartsWith("e")) // edit mode on
            {
                
                if (edit_index.ToString() != b.Name.Substring(1)) // Switch edit index to new edit index
                {
                    edit_index = Convert.ToInt32(b.Name.Substring(1));
                    Add_button.Visible = false;
                    Item Ref_Item = Item_List[edit_index];
                    category_box.Text = Ref_Item.Category;
                    item_desc.Text = Ref_Item.Name;
                    item_price.Text = "$" + String.Format("{0:0.00}", Ref_Item.Price);
                    quantity.Text = Ref_Item.Quantity.ToString();
                }
                else // Accept changes
                {
                    Item_List[edit_index].Category = category_box.Text;
                    Item_List[edit_index].Name = item_desc.Text;
                    Item_List[edit_index].Price = Convert.ToDouble(item_price.Text.Substring(1));
                    Item_List[edit_index].Quantity = Convert.ToInt32(quantity.Text);
                    Item_List[edit_index].Category = category_box.Text;

                    edit_index = -1;
                    Add_button.Visible = true;

                    item_desc.Text = "";
                    item_price.Text = "$";

                }
                Invalidate();
                Update();
            }
        }

        public int edit_index = -1;

        public List<AdvancedComboBox> List_Combos = new List<AdvancedComboBox>();
        
        // Add a memo
        private void memo_button_Click(object sender, EventArgs e)
        {
            Input_Box IB = new Input_Box(this, "Attach a memo to item:", Temp_Memo);
            IB.ShowDialog();
        }

        private void submit_button_Click(object sender, EventArgs e)
        {
            if (Item_List.Count > 0)
            {

                if (Editing_Receipt)
                {

                    // delete existing items and order from Master_Item_List and Order_List and proceed with regular editing 

                    Editing_Receipt = false;
                    for (int i = Master_Item_List.Count - 1; i >= 0; i--)
                    {
                        if (Master_Item_List[i].OrderID == Editing_Order.OrderID)
                        {
                            Master_Item_List.RemoveAt(i);
                        }
                    }
                    Order_List.Remove(Editing_Order);
                }

                // Set order information to keep track of
                int Order_Quantity = 0;
                double Order_Total_Pre_Tax = 0;
                double Order_Taxes = 0;

                // Get Random Order ID
                Random OrderID_Gen = new Random();
                string randomID = OrderID_Gen.Next(100000000, 999999999).ToString();

                // Ensure no clashing of IDs
                List<Item> same_ID = Master_Item_List.Where(x => x.OrderID == randomID).ToList();
                while (same_ID.Count > 0)
                {
                    randomID = OrderID_Gen.Next(100000000, 999999999).ToString();
                    same_ID = Master_Item_List.Where(x => x.OrderID == randomID).ToList();
                }

                double total_discount = Item_List.Where(item => item.Discount_Amt > 0).Sum(item => item.Discount_Amt);

                foreach (Item item in Item_List)
                {
                // Adjust for refunded items w/ discount
                    total_discount -= item.Discount_Amt - item.Get_Current_Discount();
                    Order_Total_Pre_Tax -= item.Price * (Convert.ToInt32(item.Status));
                    Order_Taxes -= item.Price * item.Quantity * Get_Tax_Amount(item) - item.Get_Current_Tax_Amount(Get_Tax_Amount(item));
                }

                if (Alerts_On)
                {
                    // Check for categorical warnings
                    foreach (string category in Item_List.Select(x => x.Category).Distinct())
                    {
                        Check_Warnings(category);
                    }
                }


                for (int i = Item_List.Count - 1; i >= 0; i--)
                {
                    // Store order information
                    Order_Quantity += Item_List[i].Quantity;
                    Order_Total_Pre_Tax += Item_List[i].Price * Item_List[i].Quantity;
                    Order_Taxes += (Tax_Rules_Dictionary.ContainsKey(Item_List[i].Category) ? Convert.ToDouble(Tax_Rules_Dictionary[Item_List[i].Category]) * Item_List[i].Price * Item_List[i].Quantity : Tax_Rate * Item_List[i].Price * Item_List[i].Quantity);

                    // Transfer from current to master item list
                    Item_List[i].OrderID = randomID;
                    Item_List[i].Date = dateTimePicker1.Value;
                    Item_List[i].Payment_Type = payment_type.Text;
                    Master_Item_List.Add(Item_List[i]);
                    Item_List.RemoveAt(i);
                }

                if (Tax_Exempt_Order) Order_Taxes = 0;
                else if (Tax_Override_Amt > 0) Order_Taxes = Tax_Override_Amt;

                Order Current_Order = new Order();
                Current_Order.Location = location_box.Text;
                Current_Order.OrderMemo = Order_Memo;
                Current_Order.OrderID = randomID;
                Current_Order.Tax_Overridden = (Tax_Override_Amt > 0 || Tax_Exempt_Order);
                Current_Order.Payment_Type = payment_type.Text;
                Current_Order.Order_Total_Pre_Tax = Order_Total_Pre_Tax - total_discount;
                Current_Order.Order_Taxes = Order_Taxes;
                Current_Order.Order_Discount_Amt = total_discount;
                Current_Order.Order_Quantity = Order_Quantity;
                Current_Order.Date = dateTimePicker1.Value;

                Order_List.Add(Current_Order);

                // Update current expenditure
                Current_Month_Expenditure += Order_Total_Pre_Tax + Order_Taxes;

                // Check over-budget
                Check_Budget();

                dateTimePicker1.Value = DateTime.Now;
                Tax_Override_Amt = 0;
                Tax_Exempt_Order = false;
                Order_Memo = "";


                Invalidate();
                Update();

                if (MOMG_open)
                {

                    MOMG.Refresh_Window();
                }
            }

            item_desc.Text = "";
            item_price.Text = "$";

        }

        public bool Show_Calendar_On_Load = false;
        public bool Alerts_On = true;

        private bool Editing_Receipt = false;
        private Order Editing_Order = new Order();
        public Savings_Structure Savings = new Savings_Structure();
        bool Savings_Instantiated = false;

        public void Edit_Receipt(Order order)
        {
            if (Item_List.Count > 0)
            {
                using (var form = new Yes_No_Dialog(this, "You have un-submitted items. Continuing will reset your current receipt. Do you wish to continue?", "Warning", new Point(this.Left + 84, this.Top + 100)))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        if (form.ReturnValue1 == "1")
                        {
                        }
                        else
                        {
                            goto Finish;
                        }
                    }
                }
            }

            // Continue
            Editing_Receipt = true;

            Item_List = new List<Item>();
            Item_List = Master_Item_List.Where(x => x.OrderID == order.OrderID).ToList();
            Editing_Order = order;
            paint = true;
            Tax_Override_Amt = Editing_Order.Tax_Overridden ? Editing_Order.Order_Taxes : 0;
            dateTimePicker1.Value = Editing_Order.Date;
            location_box.Text = Editing_Order.Location;
            payment_type.Text = Editing_Order.Payment_Type;
            Invalidate();
            Update();

            Finish: ; // Do nothing
        }

        private void tax_override_button_Click(object sender, EventArgs e)
        {
            Input_Box IB = new Input_Box(this, "Override existing tax amount to:", Editing_Receipt ? Editing_Order.Tax_Overridden ? Editing_Order.Order_Taxes.ToString() : "0" : Tax_Override_Amt.ToString());
            IB.ShowDialog();
        }

        private void order_memo_button_Click(object sender, EventArgs e)
        {
            Input_Box IB = new Input_Box(this, "Attach a memo to this order:", Editing_Receipt ? Editing_Order.OrderMemo : Temp_Memo);
            IB.ShowDialog();
        }

        private void reset_button_Click(object sender, EventArgs e)
        {
            Item_List = new List<Item>();
            Tax_Exempt_Order = false;
            Order_Memo = "";
            Tax_Override_Amt = 0;
            quantity.Text = "1";
            Editing_Receipt = false;
            Invalidate();
            Update();
            Add_button.Visible = true;
        }



        private List<Button> Delete_Item_Buttons = new List<Button>();
        private List<Button> Edit_Buttons = new List<Button>();
        private List<Button> Discount_Buttons = new List<Button>();

        // Lists
        List<Item> Item_List = new List<Item>();
        public List<Item> Master_Item_List = new List<Item>();
        public List<Company> Company_List = new List<Company>();
        public List<string> Category_List = new List<string>();
        public List<Order> Order_List = new List<Order>();
        public List<Expenses> Expenses_List = new List<Expenses>();
        public List<Payment> Payment_List = new List<Payment>();
        public List<Account> Account_List = new List<Account>();
        public List<Calendar_Events> Calendar_Events_List = new List<Calendar_Events>();

        // Settings Variables
        public Dictionary<string, string> Link_Location = new Dictionary<string, string>(); //link source -> link destination
        public Dictionary<string, string> Tax_Rules_Dictionary = new Dictionary<string, string>();     //category -> tax rate %
        public Dictionary<string, string> Settings_Dictionary = new Dictionary<string, string>();
        public Dictionary<string, Warning> Warnings_Dictionary = new Dictionary<string, Warning>();

        // Temporary / Feedback variables
        public string Temp_Memo = "";
        public Color Frame_Color = SystemColors.HotTrack;
        public string Pass_Through_String = "";

        // Preset Variables 
        public double Tax_Rate = 0.13;
        public double Monthly_Income = 0;
        public double Tax_Override_Amt = 0;

        private bool paint = false;

        // Form Sizing Parameters
        Size Start_Size = new Size();

        public Receipt()
        {

            InitializeComponent();

            this.DoubleBuffered = true;
            Start_Size = this.Size;

            // Force redraw on resizing (required for borderless form resize
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            quantity.KeyPress += new KeyPressEventHandler(this.comboBox_KeyPress);
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            // Load from file the information
            //
            //
            //Company_List.Add(new Company() { Name = "Temp-Location", Location = "" });

            // Preset Categories
            /*
            Category_List.Add("Grocery");
            Category_List.Add("Dine-Out");
            Category_List.Add("Entertainment");
            Category_List.Add("Home & Living");
            Category_List.Add("Computer & Tech" 
            Category_List.Add("Cleaning Product");
            Category_List.Add("Drink");
            Category_List.Add("Gas");
            Category_List.Add("Furniture");
            */

            for (int i = 1; i <= 20; i++) quantity.Items.Add(i.ToString());
            quantity.Text = "1";

            // Add Enter-key auto add
            this.item_desc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxEnterKey_KeyPress);
            this.item_price.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxEnterKey_KeyPress);

            // Memo Tooltip (Hover)
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            ToolTip1.SetToolTip(memo_button, "Add a memo");
            ToolTip1.SetToolTip(Add_button, "Add item to receipt");
            ToolTip1.SetToolTip(button1, "Add a new location");
            ToolTip1.SetToolTip(button2, "Add a new category");
            ToolTip1.SetToolTip(button3, "Change Receipt Date");

            ToolTip1.SetToolTip(reset_button, "Clear Receipt");
            ToolTip1.SetToolTip(order_memo_button, "Add a memo to order");
            ToolTip1.SetToolTip(tax_override_button, "Override existing tax amount");
            ToolTip1.SetToolTip(submit_button, "Submit Receipt");

            // Render menustrip
            menuStrip1.ForeColor = Color.LightGray;

            Reload_Program();

            menuStrip1.Renderer = new ToolStripProfessionalRenderer(new TestColorTable() { Menu_Border_Color = Frame_Color });

            dateTimePicker1.Value = DateTime.Now;
            //menuStrip1.Renderer = new MyRenderer();

            close_button.FlatAppearance.BorderSize = 0;
            minimize_button.FlatAppearance.BorderSize = 0;

            //Price_Crawler PC = new Price_Crawler(this);
            //PC.ShowDialog();

            Get_Current_Month_Expenditure();

            if (Settings_Dictionary["LOGIN_EMAIL"].Contains("lirobin9@gmail.com"))
            {
                toolStripMenuItem4.Visible = true;
                fILE2ToolStripMenuItem1.Visible = true;
            }

            // If authentication set already
            if (Settings_Dictionary["AUTHENTICATION_REQ"] == "1")
            {
                using (var form = new Authentication_Form(this, "Please login using your personal email and password.", "Authentication", new Point(this.Left + 60, this.Top + 50), Settings_Dictionary["LOGIN_EMAIL"], Settings_Dictionary["LOGIN_PASSWORD"], true))
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
                        }
                        else
                        {
                            Environment.Exit(0);
                            this.Close();
                        }
                    }
                    else
                    {
                        Environment.Exit(0);
                        this.Close();
                    }
                }
            }

            if (Show_Calendar_On_Load)
            {
                Calendar c = new Calendar(this);
                c.ShowDialog();
            }
            this.TopMost = true;
            this.TopMost = false;
        }

        public void Set_Payment_Box()
        {
            payment_type.Items.Clear();
            foreach (Payment PT in Payment_List)
            {
                payment_type.Items.Add(PT.Company + " (xx-" + PT.Last_Four + ")");
            }

            // Preset Payment Types
            payment_type.Items.Add("Cash");
            payment_type.Items.Add("Other");
            payment_type.Text = payment_type.Items[0].ToString();
        }

        public void Reload_Program()
        {
            Reset_Parameters();
            Load_Information();

            location_box.Items.Clear();
            category_box.Items.Clear();

            Set_Payment_Box();

            payment_type.Text = payment_type.Items[0].ToString();

            // Load Combobox texts
            foreach (Company p in Company_List)
            {
                location_box.Items.Add(p.Name);

            }
            location_box.Sorted = true;

            // Load Combobox texts
            foreach (string p in Category_List)
            {
                category_box.Items.Add(p);
            }
            category_box.Sorted = true;

            // Preset text for combobox
            if (category_box.Items.Count > 0) category_box.Text = category_box.Items[0].ToString();
            if (location_box.Items.Count > 0) location_box.Text = location_box.Items[0].ToString();

            Invalidate();
            Update();
        }

        private void Reset_Parameters()
        {
            Item_List = new List<Item>();
            Master_Item_List = new List<Item>();
            Order_List = new List<Order>();
            Company_List = new List<Company>();
            Expenses_List = new List<Expenses>();
            Category_List = new List<string>();
            Account_List = new List<Account>();
            Link_Location = new Dictionary<string, string>();
            Tax_Rules_Dictionary = new Dictionary<string, string>();
            Settings_Dictionary = new Dictionary<string, string>();
            Warnings_Dictionary = new Dictionary<string, Warning>();
            Payment_List = new List<Payment>();
            Calendar_Events_List = new List<Calendar_Events>();
            Temp_Memo = "";
            Tax_Override_Amt = 0;
        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            base.Close();
        }


        double Current_Month_Expenditure = 0;

        private void Get_Current_Month_Expenditure(bool IncludeRecurringExpense = true)
        {
            Current_Month_Expenditure = (Order_List.Where(x => x.Date.Month == DateTime.Now.Month && x.Date.Year == DateTime.Now.Year).ToList()).Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes);
            Current_Month_Expenditure += (Expenses_List.Where(x => x.Expense_Status != "0").ToList()).Sum(x => x.Get_Amount_From_Weeks(Expenses.Weeks_In_Monthly));
        }



        // If press enter on length box, activate add (nmemonics)
        private void textboxEnterKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox g = (TextBox)sender;
            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0)
            {
                if (g.Name.Contains("desc"))
                {
                    item_price.Focus();
                }
                else
                {
                    Add_button.PerformClick();
                }
            }
        }

        // If press enter on length box, activate add (nmemonics)
        private void comboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            AdvancedComboBox g = (AdvancedComboBox)sender;
            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0)
            {
                Add_button.PerformClick();
            }
        }


        // Money text only
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (!(item_price.Text.StartsWith("$")))
            {
                if (Get_Char_Count(item_price.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = item_price.Text;
                    item_price.Text = temp.Substring(1) + temp[0];
                    item_price.SelectionStart = item_price.Text.Length;
                    item_price.SelectionLength = 0;
                }
                else
                {
                    item_price.Text = "$" + item_price.Text;
                }
            }
            else if ((item_price.Text.Length > 1) && ((Get_Char_Count(item_price.Text, Convert.ToChar(".")) > 1) || (item_price.Text[1].ToString() == ".") || (Get_Char_Count(item_price.Text, Convert.ToChar("$")) > 1) || (!((item_price.Text.Substring(item_price.Text.Length - 1).All(char.IsDigit))) && !(item_price.Text[item_price.Text.Length - 1].ToString() == "."))))
            {
                item_price.TextChanged -= new System.EventHandler(textBox6_TextChanged);
                item_price.Text = item_price.Text.Substring(0, item_price.Text.Length - 1);
                item_price.SelectionStart = item_price.Text.Length;
                item_price.SelectionLength = 0;
                item_price.TextChanged += new System.EventHandler(textBox6_TextChanged);
            }
        }

        // Return the token count within string given token
        public int Get_Char_Count(string comparison_text, char reference_char)
        {
            int count = 0;
            foreach (char c in comparison_text)
            {
                if (c == reference_char)
                {
                    count++;
                }
            }
            return count;
        }

        // Add Item Button
        private void start_button_Click(object sender, EventArgs e)
        {
            if (category_box.Text.Length > 0 && item_desc.Text.Length > 0 && location_box.Text.Length > 0 && item_price.Text.Length > 1)
            {
                Item New_Item = new Item();
                New_Item.Category = category_box.Text;
                New_Item.Name = item_desc.Text;
                New_Item.Status = "0";
                New_Item.Payment_Type = payment_type.Text;
                New_Item.Location = location_box.Text;
                New_Item.Price = Convert.ToDouble(item_price.Text.Substring(1));
                New_Item.Quantity = Convert.ToInt32(quantity.Text);
                New_Item.Date = DateTime.Now;
                New_Item.Refund_Date = DateTime.Now;
                New_Item.Memo = Temp_Memo;

                Item_List.Add(New_Item);

                item_desc.Text = "";
                item_price.Text = "";
                quantity.Text = "1";
                Temp_Memo = "";

                item_desc.Focus();

                paint = true;
                Invalidate();
                Update();

                foreach (Payment payment in Payment_List.Where(x => x.Payment_Type == payment_type.Text).ToList())
                {
                    string Alert_Message = payment.Check_Payment_Due();
                    if (Alert_Message.Length > 5)
                    {
                        Form_Message_Box FMB = new Form_Message_Box(this, Alert_Message);
                        FMB.ShowDialog();
                    }
                }
            }
            else
            {
                //Form_Message_Box FMB = new Form_Message_Box(this, "Missing item");
                //FMB.ShowDialog();
            }

            // Update expenditure when first inserting item (improves performance)
            if (Item_List.Count == 1) Get_Current_Month_Expenditure();

        }

        public bool Tax_Exempt_Order = false;
        public string Order_Memo = "";

        public double Discount_Transfer_Amount = 0;


        public void Set_Form_Color(Color chooseColor)
        {
            Frame_Color = chooseColor;
            menuStrip1.Renderer = new ToolStripProfessionalRenderer(new TestColorTable() { Menu_Border_Color = chooseColor });
            //minimize_button.ForeColor = chooseColor;
            //close_button.ForeColor = chooseColor;
            textBox1.BackColor = chooseColor;
            textBox2.BackColor = chooseColor;
            textBox3.BackColor = chooseColor;
            textBox4.BackColor = chooseColor;
            location_box.FrameColor = chooseColor;
            category_box.FrameColor = chooseColor;
            payment_type.FrameColor = chooseColor;
        }

        // Add a new location
        private void button1_Click(object sender, EventArgs e)
        {
            Input_Box IB = new Input_Box(this, "Add new location:");
            IB.ShowDialog();
        }

        // Add a new category
        private void button2_Click(object sender, EventArgs e)
        {
            Input_Box IB = new Input_Box(this, "Add new category:");
            IB.ShowDialog();
        }

        public void Check_Warnings(string Category)
        {
            if (Warnings_Dictionary.ContainsKey(Category))
            {
                // Get total spent
                List<Item> temp = Master_Item_List.Where(x => x.Category == Category && x.Date.Month == DateTime.Now.Month).ToList();
                temp.AddRange(Item_List.Where(x => x.Category == Category).ToList());
                double Category_Total = temp.Sum(x => x.Get_Current_Amount(Get_Tax_Amount(x)));

                if (Warnings_Dictionary[Category].Warning_Type == "Price")
                {
                    if (Category_Total > (Warnings_Dictionary[Category].Warning_Amt * (Warnings_Dictionary[Category].Final_Level / 100)))
                    {
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent $" + Category_Total + " of your '" + Category + "' monthly limit of your $" + Warnings_Dictionary[Category].Warning_Amt + " limit");
                        FMB.ShowDialog();
                    }
                    else if (Category_Total > (Warnings_Dictionary[Category].Warning_Amt * (Warnings_Dictionary[Category].Second_Level / 100)))
                    {
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent $" + Category_Total + " of your '" + Category + "' monthly limit of your $" + Warnings_Dictionary[Category].Warning_Amt + " limit");
                        FMB.ShowDialog();
                    }
                    else if (Category_Total > (Warnings_Dictionary[Category].Warning_Amt * (Warnings_Dictionary[Category].First_Level / 100)))
                    {
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent $" + Category_Total + " of your '" + Category + "' monthly limit of your $" + Warnings_Dictionary[Category].Warning_Amt + " limit");
                        FMB.ShowDialog();
                    }
                }
                else if (Warnings_Dictionary[Category].Warning_Type == "Percent")
                {
                    double Ref_Percent = (1 - (Monthly_Income - Category_Total) / Monthly_Income);

                    // Check final level first
                    if (Ref_Percent > (Warnings_Dictionary[Category].Final_Level / 100 * Warnings_Dictionary[Category].Warning_Amt / 100))
                    {
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent " + Math.Round(Ref_Percent * 10000, 2) + "% of your '" + Category + "' monthly limit of " + Warnings_Dictionary[Category].Warning_Amt + "% of monthly income");
                        FMB.ShowDialog();
                    }
                    else
                    if (Ref_Percent > (Warnings_Dictionary[Category].Second_Level / 100 * Warnings_Dictionary[Category].Warning_Amt / 100))
                    {
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent " + Math.Round(Ref_Percent * 10000, 2) + "% of your '" + Category + "' monthly limit of " + Warnings_Dictionary[Category].Warning_Amt + "% of monthly income");
                        FMB.ShowDialog();
                    }
                    else if (Ref_Percent > (Warnings_Dictionary[Category].First_Level / 100 * Warnings_Dictionary[Category].Warning_Amt / 100))
                    {
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent " + Math.Round(Ref_Percent * 10000, 2) + "% of your '" + Category + "' monthly limit of " + Warnings_Dictionary[Category].Warning_Amt + "% of monthly income");
                        FMB.ShowDialog();
                    }
                }
            }
            else
            {
                // Do nothing
            }
        }

        private void Check_Budget()
        {
            if (Monthly_Income > 0 && Current_Month_Expenditure > 0 && (Savings.Alert_1 || Savings.Alert_2 || Savings.Alert_3))
            {
                double Reference_Value = Savings.Structure == "Amount" ? Savings.Ref_Value : Monthly_Income * (Savings.Ref_Value / 100);

                // Fixed amount budget checking
                if (Savings.Alert_3 && (Monthly_Income - Current_Month_Expenditure == Reference_Value))
                {
                    Form_Message_Box FMB = new Form_Message_Box(this, "You have exactly no budget left");
                    FMB.ShowDialog();
                }
                if (Savings.Alert_3 && (Monthly_Income - Current_Month_Expenditure < Reference_Value))
                {
                    Form_Message_Box FMB = new Form_Message_Box(this, "You have spent $" + String.Format("{0:0.00}", Math.Abs(Monthly_Income - Current_Month_Expenditure - Reference_Value)) + " over budget");
                    FMB.ShowDialog();
                }
                else if (Savings.Alert_2 && ((Monthly_Income - Current_Month_Expenditure) * 0.9 < Reference_Value))
                {
                    Form_Message_Box FMB = new Form_Message_Box(this, "You have $" + String.Format("{0:0.00}", Math.Abs((Monthly_Income - Current_Month_Expenditure) - Reference_Value)) + " before going over-budget");
                    FMB.ShowDialog();
                }
                else if (Savings.Alert_1 && ((Monthly_Income - Current_Month_Expenditure) * 0.8 < Reference_Value))
                {
                    Form_Message_Box FMB = new Form_Message_Box(this, "You have $" + String.Format("{0:0.00}",Math.Abs((Monthly_Income - Current_Month_Expenditure) - Reference_Value)) + " before going over-budget");
                    FMB.ShowDialog();
                }
            }
        }

        public void Save(string text = "journal_info.txt")
        {
            string Info_Path = Directory.GetCurrentDirectory() + "\\" + text;

            try
            {
                File.Delete(Info_Path);
                using (StreamWriter sw = File.CreateText(Info_Path)) // Create translator file
                {
                    sw.Write(Get_Save_Lines() + Environment.NewLine);
                    sw.Close();
                }

            }
            catch
            {
            }
        }

        private string Get_Save_Lines()
        {
            string line = "";
            line += Save_Item_Information();
            line += Save_Orders();
            line += Save_Links();
            line += Save_Tax_Info();
            line += Save_Settings();
            line += Save_Expenses();
            line += Save_Warnings();
            line += Save_Payments();
            line += Save_Accounts();
            line += Save_Savings();
            line += Save_Calendar_Events();

            return (Enable_Encrypt && line.Contains("[INFO_TYPE]=ORDER||[ORDER_LOCATION]")) ? AESGCM.SimpleEncryptWithPassword(line, "PASSWORDisHERE") : line;
        }

        private string Save_Payments()
        {
            string line = "";
            foreach (Payment payment in Payment_List)
            {
                line += "[PAYMENT_TYPE]=" + payment.Payment_Type +
                        "||[LAST_FOUR]=" + payment.Last_Four +
                        "||[COMPANY]=" + payment.Company +
                        "||[BANK_NAME]=" + payment.Bank +
                        "||[CARD_LIMIT]=" + payment.Limit +
                        "||[BILLING_START]=" + payment.Billing_Start +
                        "||[EMERGENCY_NO]=" + payment.Emergency_No +
                        "||[LAST_UPDATE_DATE]=" + payment.Last_Reset_Date.ToShortDateString() + 
                        "||[ALERT_A]=" + (payment.Alerts[0].Active ? "1:" : "0:") + (payment.Alerts[0].Repeat ? "1" : "0") +
                        "||[ALERT_B]=" + (payment.Alerts[1].Active ? "1:" : "0:") + (payment.Alerts[1].Repeat ? "1" : "0") +
                        "||[ALERT_C]=" + (payment.Alerts[2].Active ? "1:" : "0:") + (payment.Alerts[2].Repeat ? "1" : "0") +
                        "||[ALERT_D]=" + (payment.Alerts[3].Active ? "1:" : "0:") + (payment.Alerts[3].Repeat ? "1" : "0") + Environment.NewLine;
            }

            return line;
        }

        // Save items from Master Item List
        private string Save_Item_Information()
        {
            string line = "";
            foreach (Item item in Master_Item_List.OrderBy(x => x.Date))
            {
                line += "[INFO_TYPE]=ITEM||[ITEM_DESC]=" + item.Name +
                        "||[ITEM_LOCATION]=" + item.Location +
                        "||[ITEM_STATUS]=" + item.Status +
                        "||[ITEM_CATEGORY]=" + item.Category +
                        "||[ITEM_QUANTITY]=" + item.Quantity +
                        "||[ITEM_PRICE]=" + item.Price +
                        "||[ITEM_DISCOUNT_AMT]=" + item.Discount_Amt +
                        "||[ITEM_DATE]=" + item.Date.ToString() +
                        "||[ITEM_REFUND_DATE]=" + item.Refund_Date.ToString() +
                        "||[ITEM_PAYMENT]=" + item.Payment_Type +
                        "||[ITEM_ORDERID]=" + item.OrderID +
                        "||[ITEM_MEMO]=" + item.Memo + Environment.NewLine;
            }
            return line;
        }

        private string Save_Links()
        {
            string line = "";
            foreach (KeyValuePair<string, string> Key in Link_Location)
            {
                line += "[LINK_SOURCE]=" + Key.Key + "||[LINK_DESTINATION]=" + Key.Value + Environment.NewLine;
            }
            return line;
        }

        private string Save_Tax_Info()
        {
            string line = "";
            foreach (KeyValuePair<string, string> Key in Tax_Rules_Dictionary)
            {
                line += "[TAX_CATEGORY]=" + Key.Key + "||[TAX_RATE]=" + Key.Value + Environment.NewLine;
            }
            return line;
        }

        private string Save_Settings()
        {
            string line = "[PERSONAL_SETTINGS]";
            foreach (KeyValuePair<string, string> Key in Settings_Dictionary)
            {
                line += "||[" + Key.Key + "]=" + Key.Value;
            }
            return line + Environment.NewLine;
        }

        private string Save_Savings()
        {
            string line = "[SAVINGS_SETTINGS]";
            line += "||[STRUCTURE]=" + Savings.Structure;
            line += "||[AMOUNT]=" + Savings.Ref_Value.ToString();
            line += "||[ALERT_1]=" + (Savings.Alert_1 ? "1" : "0");
            line += "||[ALERT_2]=" + (Savings.Alert_2 ? "1" : "0");
            line += "||[ALERT_3]=" + (Savings.Alert_3 ? "1" : "0");
            return line + Environment.NewLine;
        }

        private string Save_Calendar_Events()
        {
            string line = "";
            foreach (Calendar_Events CE in Calendar_Events_List)
            {   
                line += "[CALENDAR_EVENT]";
                line += "||[CALENDAR_TITLE]=" + CE.Title;
                line += "||[CALENDAR_DESC]=" + CE.Description.Replace(Environment.NewLine, "~~");
                line += "||[CALENDAR_IMPORTANCE]=" + CE.Importance.ToString();
                line += "||[CALENDAR_DATE]=" + CE.Date.ToShortDateString();
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
            return line + Environment.NewLine;
        }

        private string Save_Expenses()
        {
            string line = "";
            foreach (Expenses expense in Expenses_List)
            {

                line += "[EXPENSE_TYPE]=" + expense.Expense_Type +
                        "||[EXPENSE_NAME]=" + expense.Expense_Name +
                        "||[EXPENSE_PAYEE]=" + expense.Expense_Payee +
                        "||[EXPENSE_FREQUENCY]=" + expense.Expense_Frequency +
                        "||[EXPENSE_DATE_SEQUENCE]=" + String.Join(",", expense.Date_Sequence) +
                        "||[EXPENSE_STATUS]=" + expense.Expense_Status +
                        "||[EXPENSE_START_DATE]=" + expense.Expense_Start_Date.ToString() +
                        "||[EXPENSE_AMOUNT]=" + expense.Expense_Amount + Environment.NewLine;
            }
            return line;
        }

        private string Save_Orders()
        {
            string line = "";
            foreach (Order order in Order_List.OrderBy(x => x.Date))
            {
                line += "[INFO_TYPE]=ORDER" +
                        "||[ORDER_LOCATION]=" + order.Location +
                        "||[ORDER_QUANTITY]=" + order.Order_Quantity +
                        "||[ORDER_PRETAX_PRICE]=" + order.Order_Total_Pre_Tax +
                        "||[ORDER_DISCOUNT_AMT]=" + order.Order_Discount_Amt +
                        "||[ORDER_TAX]=" + order.Order_Taxes +
                        "||[ORDER_MEMO]=" + order.OrderMemo +
                        "||[ORDER_TAX_OVERRIDEN]=" + (order.Tax_Overridden ? "1" : "0") +
                        "||[ORDER_DATE]=" + order.Date.ToString() +
                        "||[ORDER_PAYMENT]=" + order.Payment_Type +
                        "||[ORDER_ORDERID]=" + order.OrderID + Environment.NewLine;
            }
            return line;
        }

        private string Save_Accounts()
        {
            string line = "";
            foreach (Account acc in Account_List)
            {
                line += "[ACCOUNT_TYPE]=" + acc.Type +
                        "||[ACCOUNT_PAYER]=" + acc.Payer +
                        "||[ACCOUNT_REMARK]=" + acc.Remark +
                        "||[ACCOUNT_AMOUNT]=" + acc.Amount +
                        "||[ACCOUNT_STATUS]=" + acc.Status.ToString() +
                        "||[ACCOUNT_INACTIVE]=" + acc.Inactive_Date.ToString() +
                        "||[ACCOUNT_START]=" + acc.Start_Date.ToString() + Environment.NewLine;
            }
            return line;
        }

        private string Save_Warnings()
        {
            string line = "";
            foreach (KeyValuePair<string, Warning> Key in Warnings_Dictionary)
            {
                line += "[WARNING_CATEGORY]=" + Key.Key +
                        "||[WARNING_FIRST]=" + Key.Value.First_Level +
                        "||[WARNING_SECOND]=" + Key.Value.Second_Level +
                        "||[WARNING_FINAL]=" + Key.Value.Final_Level +
                        "||[WARNING_TYPE]=" + Key.Value.Warning_Type +
                        "||[WARNING_AMOUNT]=" + Key.Value.Warning_Amt + Environment.NewLine;
            }
            return line;
        }

        public string Info_Path = Directory.GetCurrentDirectory() + "\\journal_info.txt";
        string[] lines;

        public bool Enable_Encrypt = true;

        public void Clear_Backups(int days = 30)
        {
            string Backup_Path = Directory.GetCurrentDirectory() + @"\Backups";

            string[] File_in_dir;
            
            File_in_dir = Directory.GetFiles(Backup_Path, "*cfg*");
            foreach (string file in File_in_dir)
            {
                if ((DateTime.Now - File.GetCreationTime(file)).TotalDays > days)
                {
                    File.Delete(file);
                }
            }
        }


        // Initialize existing information
        private void Load_Information()
        {
            if (File.Exists(Info_Path))
            {
                try
                {
                    File.Copy(Info_Path, Directory.GetCurrentDirectory() + "\\personal_banker_backup.cfg", true);
                    string backup_path_2 = Directory.GetCurrentDirectory() + "\\personal_banker_backup2.cfg";
                    if (File.Exists(backup_path_2))
                    {
                        File.Delete(backup_path_2);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    File.Copy(Info_Path, Directory.GetCurrentDirectory() + "\\personal_banker_backup2.cfg", true);
                }
                var text = File.ReadAllText(Info_Path); 
                try
                {
                    if (Enable_Encrypt && !text.Contains("[INFO_TYPE]=ITEM||[ITEM_DESC]"))  // force decrypt 
                    {
                        lines = AESGCM.SimpleDecryptWithPassword(text, "PASSWORDisHERE").Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    }
                    else if (!Enable_Encrypt || text.Contains("[INFO_TYPE]=ITEM||[ITEM_DESC]"))
                    {
                        lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    }
                }
                catch (Exception e)
                {
                    Form_Message_Box FMB = new Form_Message_Box(this, "Decryption failed! " + e.ToString());
                    FMB.Show();
                }

                foreach (string line in lines)
                {
                    // If Item
                    if (line.Contains("[INFO_TYPE]=ITEM"))
                    {
                        Item New_Item = new Item();
                        New_Item.Name = Parse_Line_Information(line, "ITEM_DESC");
                        New_Item.Status = Parse_Line_Information(line, "ITEM_STATUS") == "" ? "0" : Parse_Line_Information(line, "ITEM_STATUS");
                        New_Item.Location = Parse_Line_Information(line, "ITEM_LOCATION");
                        New_Item.Payment_Type = Parse_Line_Information(line, "ITEM_PAYMENT");
                        New_Item.Category = Parse_Line_Information(line, "ITEM_CATEGORY");
                        New_Item.Discount_Amt = Parse_Line_Information(line, "ITEM_DISCOUNT_AMT") == "" ? 0 : Convert.ToDouble(Parse_Line_Information(line, "ITEM_DISCOUNT_AMT"));
                        New_Item.Price = Convert.ToDouble(Parse_Line_Information(line, "ITEM_PRICE"));
                        New_Item.Quantity = Convert.ToInt32(Parse_Line_Information(line, "ITEM_QUANTITY"));
                        New_Item.Date = Convert.ToDateTime(Parse_Line_Information(line, "ITEM_DATE"));
                        New_Item.Refund_Date = Parse_Line_Information(line, "ITEM_REFUND_DATE").Length > 0 ? Convert.ToDateTime(Parse_Line_Information(line, "ITEM_REFUND_DATE")) : DateTime.Now;
                        New_Item.Memo = Parse_Line_Information(line, "ITEM_MEMO");
                        New_Item.OrderID = Parse_Line_Information(line, "ITEM_ORDERID");
                        Master_Item_List.Add(New_Item);

                        // Add pre-existing information to comboboxes
                        if (!Category_List.Contains(New_Item.Category)) Category_List.Add(New_Item.Category);

                        bool Contains_Location = false;
                        foreach (Company g in Company_List)
                        {
                            if (g.Name == New_Item.Location)
                            {
                                Contains_Location = true;
                            }
                        }

                        if (!Contains_Location) Company_List.Add(new Company() {Name = New_Item.Location} );
                    }

                    // If Link information, store link information
                    else if (line.Contains("[LINK_SOURCE]"))
                    {
                        Link_Location.Add(Parse_Line_Information(line, "LINK_SOURCE"), Parse_Line_Information(line, "LINK_DESTINATION"));
                    }

                    // If Tax information, store tax information
                    else if (line.Contains("[TAX_CATEGORY]"))
                    {
                        Tax_Rules_Dictionary.Add(Parse_Line_Information(line, "TAX_CATEGORY"), Parse_Line_Information(line, "TAX_RATE"));
                    }
                    else if (line.Contains("[PERSONAL_SETTINGS]"))
                    {
                        // App settings
                        Frame_Color = System.Drawing.ColorTranslator.FromHtml(Parse_Line_Information(line, "APP_SETTING_COLOR"));
                        Set_Form_Color(Frame_Color);
                        Settings_Dictionary.Add("APP_SETTING_COLOR", Parse_Line_Information(line, "APP_SETTING_COLOR"));
                        // Login Credentials and parameters
                        Settings_Dictionary.Add("LOGIN_PASSWORD", Parse_Line_Information(line, "LOGIN_PASSWORD") == "" ? "" : Parse_Line_Information(line, "LOGIN_PASSWORD"));
                        Settings_Dictionary.Add("AUTHENTICATION_REQ", Parse_Line_Information(line, "AUTHENTICATION_REQ") == "" ? "0" : Parse_Line_Information(line, "AUTHENTICATION_REQ"));
                        Settings_Dictionary.Add("REMEMBER_ME", Parse_Line_Information(line, "REMEMBER_ME") == "" ? "0" : Parse_Line_Information(line, "REMEMBER_ME"));
                        Settings_Dictionary.Add("LOGIN_EMAIL", Parse_Line_Information(line, "LOGIN_EMAIL") == "" ? "" : Parse_Line_Information(line, "LOGIN_EMAIL"));
                        // Personal Information
                        Settings_Dictionary.Add("PERSONAL_FIRST_NAME", Parse_Line_Information(line, "PERSONAL_FIRST_NAME") == "" ? "" : Parse_Line_Information(line, "PERSONAL_FIRST_NAME"));
                        Settings_Dictionary.Add("PERSONAL_LAST_NAME", Parse_Line_Information(line, "PERSONAL_LAST_NAME") == "" ? "" : Parse_Line_Information(line, "PERSONAL_LAST_NAME"));
                        Settings_Dictionary.Add("PERSONAL_EMAIL", Parse_Line_Information(line, "PERSONAL_EMAIL") == "" ? "" : Parse_Line_Information(line, "PERSONAL_EMAIL"));
                        // Alerts and Windows characteristics
                        Settings_Dictionary.Add("SHOW_CALENDAR_ON_LOAD", Parse_Line_Information(line, "SHOW_CALENDAR_ON_LOAD") == "" ? "0" : Parse_Line_Information(line, "SHOW_CALENDAR_ON_LOAD"));
                        Show_Calendar_On_Load = Settings_Dictionary["SHOW_CALENDAR_ON_LOAD"] == "1";
                        Settings_Dictionary.Add("ALERTS_ACTIVE", Parse_Line_Information(line, "ALERTS_ACTIVE") == "" ? "0" : Parse_Line_Information(line, "ALERTS_ACTIVE"));
                        Alerts_On = Settings_Dictionary["ALERTS_ACTIVE"] == "1";
                        // Backup settings
                        Settings_Dictionary.Add("BACKUP_REQ", Parse_Line_Information(line, "BACKUP_REQ") == "" ? "0" : Parse_Line_Information(line, "BACKUP_REQ"));
                        Settings_Dictionary.Add("BACKUP_DEL", Parse_Line_Information(line, "BACKUP_DEL") == "" ? "0" : Parse_Line_Information(line, "BACKUP_DEL"));
                        // Income information
                        Settings_Dictionary.Add("INCOME_MONTHLY", (Parse_Line_Information(line, "INCOME_MONTHLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_MONTHLY")));
                        Settings_Dictionary.Add("INCOME_HOURLY", (Parse_Line_Information(line, "INCOME_HOURLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_HOURLY")));
                        Settings_Dictionary.Add("INCOME_WEEKLY", (Parse_Line_Information(line, "INCOME_WEEKLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_WEEKLY")));
                        Settings_Dictionary.Add("INCOME_DAILY", (Parse_Line_Information(line, "INCOME_DAILY") == "" ? "0" : Parse_Line_Information(line, "INCOME_DAILY")));
                        Settings_Dictionary.Add("INCOME_YEARLY", (Parse_Line_Information(line, "INCOME_YEARLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_YEARLY")));
                        Settings_Dictionary.Add("WORK_HPD", (Parse_Line_Information(line, "WORK_HPD") == "" ? "0" : Parse_Line_Information(line, "WORK_HPD")));
                        Settings_Dictionary.Add("WORK_OHPD", (Parse_Line_Information(line, "WORK_OHPD") == "" ? "0" : Parse_Line_Information(line, "WORK_OHPD")));
                        Settings_Dictionary.Add("WORK_OMULTI", (Parse_Line_Information(line, "WORK_OMULTI") == "" ? "0" : Parse_Line_Information(line, "WORK_OMULTI")));
                        Settings_Dictionary.Add("INCOME_TAX_RATE", (Parse_Line_Information(line, "INCOME_TAX_RATE") == "" ? "0" : Parse_Line_Information(line, "INCOME_TAX_RATE")));
                        Settings_Dictionary.Add("GENERAL_TAX_RATE", (Parse_Line_Information(line, "GENERAL_TAX_RATE") == "" ? "0.13" : Parse_Line_Information(line, "GENERAL_TAX_RATE")));
                        Settings_Dictionary.Add("INCOME_CHANGE_LOG", (Parse_Line_Information(line, "INCOME_CHANGE_LOG") == "" ? "" : Parse_Line_Information(line, "INCOME_CHANGE_LOG")));

                        Monthly_Income = Convert.ToDouble(Settings_Dictionary["INCOME_MONTHLY"]);
                        Tax_Rate = Convert.ToDouble(Settings_Dictionary["GENERAL_TAX_RATE"]);
                    }
                    else if (line.Contains("[INFO_TYPE]=ORDER"))
                    {
                        Order New_Order = new Order();
                        New_Order.Location = Parse_Line_Information(line, "ORDER_LOCATION");
                        New_Order.OrderMemo = Parse_Line_Information(line, "ORDER_MEMO");
                        New_Order.Payment_Type = Parse_Line_Information(line, "ORDER_PAYMENT");
                        New_Order.Tax_Overridden = (Parse_Line_Information(line, "ORDER_TAX_OVERRIDEN") == "1");
                        New_Order.Order_Total_Pre_Tax = Convert.ToDouble(Parse_Line_Information(line, "ORDER_PRETAX_PRICE"));
                        New_Order.Order_Taxes = Convert.ToDouble(Parse_Line_Information(line, "ORDER_TAX"));
                        New_Order.Order_Discount_Amt = Parse_Line_Information(line, "ORDER_DISCOUNT_AMT") == "" ? 0 : Convert.ToDouble(Parse_Line_Information(line, "ORDER_DISCOUNT_AMT"));
                        New_Order.Order_Quantity = Convert.ToInt32(Parse_Line_Information(line, "ORDER_QUANTITY"));
                        New_Order.Date = Convert.ToDateTime(Parse_Line_Information(line, "ORDER_DATE"));
                        New_Order.OrderID = Parse_Line_Information(line, "ORDER_ORDERID");
                        Order_List.Add(New_Order);
                        editTest1ToolStripMenuItem.Enabled = true;
                        toolStripMenuItem6.Enabled = true;
                        toolStripMenuItem12.Enabled = true;
                        toolStripMenuItem18.Enabled = true;
                        toolStripMenuItem11.Enabled = true;
                        toolStripMenuItem16.Enabled = true;
                    }
                    else if (line.Contains("[EXPENSE_TYPE]="))
                    {
                        List<DateTime> Date_Sequence = new List<DateTime>();
                        if (Parse_Line_Information(line, "EXPENSE_DATE_SEQUENCE") != "")
                        {
                            string[] dates = Parse_Line_Information(line, "EXPENSE_DATE_SEQUENCE").Split(new string[] { "," }, StringSplitOptions.None);
                            foreach (string Date in dates)
                                Date_Sequence.Add(Convert.ToDateTime(Date));
                        }

                        Expenses New_Expense = new Expenses();
                        New_Expense.Expense_Type = Parse_Line_Information(line, "EXPENSE_TYPE");
                        New_Expense.Expense_Name = Parse_Line_Information(line, "EXPENSE_NAME");
                        New_Expense.Expense_Payee = Parse_Line_Information(line, "EXPENSE_PAYEE");
                        New_Expense.Expense_Frequency = Parse_Line_Information(line, "EXPENSE_FREQUENCY");
                        New_Expense.Date_Sequence = Date_Sequence;
                        New_Expense.Expense_Status = Parse_Line_Information(line, "EXPENSE_STATUS");
                        New_Expense.Expense_Amount = Convert.ToDouble(Parse_Line_Information(line, "EXPENSE_AMOUNT"));
                        New_Expense.Expense_Start_Date = Convert.ToDateTime(Parse_Line_Information(line, "EXPENSE_START_DATE"));
                        Expenses_List.Add(New_Expense);
                    }
                    else if (line.Contains("WARNING_FINAL"))
                    {
                        Warning warn = new Warning();
                        warn.Category = Parse_Line_Information(line, "WARNING_CATEGORY");
                        warn.First_Level = Convert.ToDouble(Parse_Line_Information(line, "WARNING_FIRST"));
                        warn.Second_Level = Convert.ToDouble(Parse_Line_Information(line, "WARNING_SECOND"));
                        warn.Final_Level = Convert.ToDouble(Parse_Line_Information(line, "WARNING_FINAL"));
                        warn.Warning_Type = Parse_Line_Information(line, "WARNING_TYPE");
                        warn.Warning_Amt = Convert.ToDouble(Parse_Line_Information(line, "WARNING_AMOUNT"));
                        Warnings_Dictionary.Add(warn.Category, warn);
                    }
                    else if (line.Contains("BILLING_START") && line.Contains("EMERGENCY_NO"))
                    {
                        Payment Payment = new Payment();
                        Payment.Payment_Type = Parse_Line_Information(line, "PAYMENT_TYPE");
                        Payment.Last_Four = Parse_Line_Information(line, "LAST_FOUR");
                        Payment.Company = Parse_Line_Information(line, "COMPANY");
                        Payment.Bank = Parse_Line_Information(line, "BANK_NAME");
                        Payment.Limit = Convert.ToDouble(Parse_Line_Information(line, "CARD_LIMIT"));
                        Payment.Billing_Start = Parse_Line_Information(line, "BILLING_START");
                        Payment.Emergency_No = Parse_Line_Information(line, "EMERGENCY_NO");
                        Payment.Last_Reset_Date = Parse_Line_Information(line, "LAST_UPDATE_DATE") == "" ? DateTime.Now.AddYears(-1) : Convert.ToDateTime(Parse_Line_Information(line, "LAST_UPDATE_DATE"));
                        Payment.Alerts[0].Active = Parse_Line_Information(line, "ALERT_A").Split(new string[] { ":" }, StringSplitOptions.None)[0] == "1";
                        Payment.Alerts[0].Repeat = Parse_Line_Information(line, "ALERT_A").Split(new string[] { ":" }, StringSplitOptions.None)[1] == "1";
                        Payment.Alerts[1].Active = Parse_Line_Information(line, "ALERT_B").Split(new string[] { ":" }, StringSplitOptions.None)[0] == "1";
                        Payment.Alerts[1].Repeat = Parse_Line_Information(line, "ALERT_B").Split(new string[] { ":" }, StringSplitOptions.None)[1] == "1";
                        Payment.Alerts[2].Active = Parse_Line_Information(line, "ALERT_C").Split(new string[] { ":" }, StringSplitOptions.None)[0] == "1";
                        Payment.Alerts[2].Repeat = Parse_Line_Information(line, "ALERT_C").Split(new string[] { ":" }, StringSplitOptions.None)[1] == "1";
                        Payment.Alerts[3].Active = Parse_Line_Information(line, "ALERT_D").Split(new string[] { ":" }, StringSplitOptions.None)[0] == "1";
                        Payment.Alerts[3].Repeat = Parse_Line_Information(line, "ALERT_D").Split(new string[] { ":" }, StringSplitOptions.None)[1] == "1";
                        Payment.Calendar_Toggle = 0;
                        Payment_List.Add(Payment);
                        Payment.Get_Total(Master_Item_List, Tax_Rules_Dictionary, Tax_Rate, Order_List);
                        string Alert_Message = Payment.Check_Payment_Due();
                        if (Alert_Message.Length > 5)
                        {
                            Form_Message_Box FMB = new Form_Message_Box(this, Alert_Message);
                            FMB.ShowDialog();
                        }
                    }
                    else if (line.Contains("ACCOUNT_TYPE") && line.Contains("ACCOUNT_PAYER"))
                    {
                        Account ACC = new Account();
                        ACC.Type = Parse_Line_Information(line, "ACCOUNT_TYPE");
                        ACC.Payer = Parse_Line_Information(line, "ACCOUNT_PAYER");
                        ACC.Remark = Parse_Line_Information(line, "ACCOUNT_REMARK");
                        ACC.Amount = Parse_Line_Information(line, "ACCOUNT_AMOUNT");
                        ACC.Status = Convert.ToInt32(Parse_Line_Information(line, "ACCOUNT_STATUS"));
                        ACC.Inactive_Date = Parse_Line_Information(line, "ACCOUNT_INACTIVE") == "" ? DateTime.Now : Convert.ToDateTime(Parse_Line_Information(line, "ACCOUNT_INACTIVE"));
                        ACC.Start_Date = Parse_Line_Information(line, "ACCOUNT_START") == "" ? DateTime.Now : Convert.ToDateTime(Parse_Line_Information(line, "ACCOUNT_START"));
                        Account_List.Add(ACC);
                    }
                    else if (line.Contains("SAVINGS_SETTINGS") && line.Contains("STRUCTURE"))
                    {
                        Savings.Structure = Parse_Line_Information(line, "STRUCTURE");
                        Savings.Ref_Value = Convert.ToDouble(Parse_Line_Information(line, "AMOUNT"));
                        Savings.Alert_1 = Parse_Line_Information(line, "ALERT_1") == "1" ? true : false;
                        Savings.Alert_2 = Parse_Line_Information(line, "ALERT_2") == "1" ? true : false;
                        Savings.Alert_3 = Parse_Line_Information(line, "ALERT_3") == "1" ? true : false;
                        Savings_Instantiated = true;
                    }
                    else if (line.Contains("CALENDAR_EVENT") && line.Contains("CALENDAR_IMPORTANCE"))
                    {
                        Calendar_Events CE = new Calendar_Events();
                        CE.Title = Parse_Line_Information(line, "CALENDAR_TITLE");
                        CE.Description = Parse_Line_Information(line, "CALENDAR_DESC").Replace("~~", Environment.NewLine);
                        CE.Importance = Convert.ToInt32(Parse_Line_Information(line, "CALENDAR_IMPORTANCE"));
                        CE.Date = Convert.ToDateTime(Parse_Line_Information(line, "CALENDAR_DATE"));
                        string[] date_Strings = (Parse_Line_Information(line, "CALENDAR_ALERT_SEQUENCE").Split(new string[] { "~" }, StringSplitOptions.None));
                        foreach (string dS in date_Strings)
                        {
                            if (dS.Length > 0)
                            {
                                DateTime temp = Convert.ToDateTime(dS);

                                // check if the date coincides with any event dates. If so, prompt an alert
                                if (temp.Date == DateTime.Now.Date)
                                {
                                    // get date difference
                                    double Date_Diff = (DateTime.Now.Date - CE.Date).TotalDays;
                                    Form_Message_Box FMB = new Form_Message_Box(this, CE.Title + " is in " + Math.Round(Math.Abs(Date_Diff), 1) + " days (on " + CE.Date.ToShortDateString() + ")");
                                    FMB.ShowDialog();
                                }
                                else
                                {
                                    // Do not remind more than once. If alert goes off, don't remind anymore
                                    CE.Alert_Dates.Add(temp);
                                }
                            }
                        }
                        Calendar_Events_List.Add(CE);
                    }
                }

                if (Settings_Dictionary["BACKUP_DEL"] == "1")
                {
                    Clear_Backups(30);
                }
                if (Settings_Dictionary["BACKUP_REQ"] == "1")
                {
                    if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\Backups"))
                    {
                        Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Backups");
                    }
                    try
                    {
                        File.Copy(Info_Path, Directory.GetCurrentDirectory() + "\\Backups\\personal_banker_backup_" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + ".cfg", true);
                    }
                    catch
                    { }
                } 
            }
            else
            {
                using (StreamWriter sw = File.CreateText(Directory.GetCurrentDirectory() + "\\journal_info.txt"))
                {
                    sw.Close();
                }
            }
            if (!Savings_Instantiated)
            {
                Savings.Structure = "";
                Savings.Ref_Value = 0;
                Savings.Alert_1 = false;
            }
        }
        

        // Return the tax rate for item
        public double Get_Tax_Amount(Item ref_Item)
        {
            return (Tax_Rules_Dictionary.ContainsKey(ref_Item.Category) ? Convert.ToDouble(Tax_Rules_Dictionary[ref_Item.Category]) : Tax_Rate);
        }



        /// <summary>
        /// Return the output line after [output].
        /// 
        /// For example, in line = [INFO_TYPE]=ITEM||[ITEM_NAME]=CLOTHING||[ITEM_PRICE]=49.22||....
        ///     Calling this program:
        /// 
        ///     
        ///     Parse_Line_Information(line, "ITEM_PRICE", parse_token = "||") returns "49.22"
        ///     
        /// </summary>
        private string Parse_Line_Information(string input, string output, string parse_token = "||")
        {
            string[] Split_Layer_1 =  input.Split(new string[] { parse_token }, StringSplitOptions.None);

            foreach (string Info_Pair in Split_Layer_1)
            {
                if (Info_Pair.Contains("[" + output + "]"))
                {
                    return Info_Pair.Split(new string[] { "=" }, StringSplitOptions.None)[1];
                }
            }

            return "";
        }

        private void location_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Link_Location.ContainsKey(location_box.Text))
            {
                category_box.Text = Link_Location[location_box.Text];
                item_desc.Focus();
            }
        }

        private void fILE2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void fILE2ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // reset button4.PerformClick();
            reset_button.PerformClick();
            Reload_Program();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Enable_Encrypt = false;
            try
            {
                // Try deleting temp file
                File.Delete(Directory.GetCurrentDirectory() + "\\temp.txt");
                Save("\\temp.txt");
                File.SetAttributes(Directory.GetCurrentDirectory() + "\\temp.txt", File.GetAttributes(Directory.GetCurrentDirectory() + "\\temp.txt") | FileAttributes.Hidden);
                Process.Start(Directory.GetCurrentDirectory() + "\\temp.txt");
            }
            catch
            {
                Form_Message_Box FMB = new Form_Message_Box(this, "Please close all opened confiuration files windows");
                FMB.ShowDialog();
            }
            Enable_Encrypt = true;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Tax_Rules TR = new Tax_Rules(this);
            TR.ShowDialog();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
        }


        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Link_Rules LR = new Link_Rules(this);
            LR.ShowDialog();
        }

        private void editTest1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refund_Report RP = new Refund_Report(this);
            RP.ShowDialog();
        }

        #region Function dump

        private void cALCULATORToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
        }
        private void eDITToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Master_Item_List.Count < 1)
            {
                editTest1ToolStripMenuItem.Enabled = false;
                toolStripMenuItem6.Enabled = false;
                toolStripMenuItem12.Enabled = false;
                toolStripMenuItem11.Enabled = false;
                toolStripMenuItem16.Enabled = false;
                toolStripMenuItem18.Enabled = false;
            }
            else
            {
                editTest1ToolStripMenuItem.Enabled = true;
                toolStripMenuItem6.Enabled = true;
                toolStripMenuItem12.Enabled = true;
                toolStripMenuItem11.Enabled = true;
                toolStripMenuItem16.Enabled = true;
                toolStripMenuItem18.Enabled = true;
            }
        }

        private void item_desc_TextChanged(object sender, EventArgs e)
        {

        }

        private void fILEToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        #endregion


        private void button7_Click_1(object sender, EventArgs e)
        {
            Receipt_Report RP = new Receipt_Report(this, Order_List[0]);
            RP.ShowDialog();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            Receipt_Report RP = new Receipt_Report(this);
            RP.ShowDialog();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            Customized_Settings CS = new Customized_Settings(this);
            CS.ShowDialog();
        }


        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            Salary_Calculator PF = new Salary_Calculator(this);
            PF.ShowDialog();
        }

        private void toolStripMenuItem10_Click_1(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            Purchases P = new Purchases(this);
            P.ShowDialog();
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the directory you wish to export information to:";

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string userName = Environment.UserName;
                string Info_Path = fbd.SelectedPath + "\\" + userName + "_info.cfg";
                string line = "";

                int check_index = 1;
                while (File.Exists(Info_Path))
                {
                    check_index++;
                    Info_Path = fbd.SelectedPath + "\\" + userName + "_" + check_index.ToString() + "_info.cfg";
                }

                // Save Item Information
                line += Get_Save_Lines();


                try
                {
                    File.Delete(Info_Path);
                    using (StreamWriter sw = File.CreateText(Info_Path)) // Create translator file
                    {
                        sw.Write(line + Environment.NewLine);
                        sw.Close();
                    }
                    Form_Message_Box FMB = new Form_Message_Box(this, "Information export successfully to \"" + Info_Path + "\"");
                    FMB.ShowDialog();
                }
                catch
                {
                }

            }
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Title = "Import information file";
            file.Multiselect = false;
            file.DefaultExt = ".cfg";
            file.Filter = "Config Files (*.cfg)|*.cfg";

            if (file.ShowDialog() == DialogResult.OK)
            {

                using (var form = new Yes_No_Dialog(this, "Your current information will be lost. Do you wish to export it?", "Warning", new Point(this.Left + 120, this.Top + 100)))
                {
                    var result2 = form.ShowDialog();
                    if (result2 == DialogResult.OK)
                    {
                        if (form.ReturnValue1 == "1")
                        {

                            FolderBrowserDialog fbd = new FolderBrowserDialog();
                            fbd.Description = "Select the directory you wish to export information to:";

                            DialogResult result = fbd.ShowDialog();

                            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                            {
                                string userName = Environment.UserName;
                                string Info_Path = fbd.SelectedPath + "\\" + userName + "_info.cfg";
                                int check_index = 1;
                                while (File.Exists(Info_Path))
                                {
                                    check_index++;
                                    Info_Path = fbd.SelectedPath + "\\" + userName + "_" + check_index.ToString() + "_info.cfg";
                                }
                                string line = "";

                                // Save Item Information
                                line += Get_Save_Lines();

                                try
                                {
                                    File.Delete(Info_Path);
                                    using (StreamWriter sw = File.CreateText(Info_Path)) // Create translator file
                                    {
                                        sw.Write(line + Environment.NewLine);
                                        sw.Close();
                                    }
                                    Form_Message_Box FMB = new Form_Message_Box(this, "Information export successfully to \"" + Info_Path + "\"");
                                    FMB.ShowDialog();
                                }
                                catch
                                {
                                }

                            }
                        }
                    }
                }
                
                string file_path = file.FileName;
                if (file_path.Contains(".cfg"))
                {
                    Info_Path = file_path;
                    reset_button.PerformClick();
                    Reload_Program();


                    // If authentication set already
                    if (Settings_Dictionary["AUTHENTICATION_REQ"] == "1")
                    {
                        using (var form = new Authentication_Form(this, "Profile is locked. Please unlock using appropriate email and password.", "Authentication", new Point(this.Left + 60, this.Top + 50), Settings_Dictionary["LOGIN_EMAIL"], Settings_Dictionary["LOGIN_PASSWORD"], true, false))
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
                                }
                                else
                                {
                                    Info_Path = Directory.GetCurrentDirectory() + "\\journal_info.txt";
                                    reset_button.PerformClick();
                                    Reload_Program();
                                }
                            }
                            else
                            {
                                Info_Path = Directory.GetCurrentDirectory() + "\\journal_info.txt";
                                reset_button.PerformClick();
                                Reload_Program();
                            }
                        }
                    }
                }
            }  
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void quantity_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = dateTimePicker1.Enabled ? false : true;
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem12_Click_1(object sender, EventArgs e)
        {
            Expenditures E = new Expenditures(this);
            E.ShowDialog();
        }

        Month_Over_Month_Graph MOMG;
        public bool MOMG_open = false;
        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            if (!MOMG_open)
            {
                MOMG_open = true;
                MOMG = new Month_Over_Month_Graph(this);
                MOMG.Show();
            }
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            Analysis_Report AR = new Analysis_Report(this);
            AR.Show();
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            Accounts_RP ARP = new Accounts_RP(this);
            ARP.ShowDialog();
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            Recurring_Expenses RE = new Recurring_Expenses(this);
            RE.ShowDialog();
        }

        private void toolStripMenuItem15_Click_1(object sender, EventArgs e)
        {
            Expenditure_Warnings EW = new Expenditure_Warnings(this);
            EW.ShowDialog();
        }

        private void toolStripMenuItem19_Click_1(object sender, EventArgs e)
        {
            foreach (Payment p in Payment_List)
            {
                p.Get_Total(Master_Item_List, Tax_Rules_Dictionary, Tax_Rate, Order_List);
            }
            Payment_Information PI = new Payment_Information(this);
            PI.Show();
        }

        private void toolStripMenuItem21_Click(object sender, EventArgs e)
        {
            using (var form1 = new Yes_No_Dialog(this, "Are you sure you wish to start a new profile?", "Warning", new Point(this.Left + 120, this.Top + 100)))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                {
                    using (var form = new Yes_No_Dialog(this, "Your current information will be lost. Do you wish to export it?", "Warning", new Point(this.Left + 120, this.Top + 100)))
                    {
                        var result2 = form.ShowDialog();
                        if (result2 == DialogResult.OK)
                        {
                            if (form.ReturnValue1 == "1")
                            {

                                FolderBrowserDialog fbd = new FolderBrowserDialog();
                                fbd.Description = "Select the directory you wish to export information to:";

                                DialogResult result = fbd.ShowDialog();

                                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                                {
                                    string userName = Environment.UserName;
                                    string Info_Path = fbd.SelectedPath + "\\" + userName + "_info.cfg";
                                    int check_index = 1;
                                    while (File.Exists(Info_Path))
                                    {
                                        check_index++;
                                        Info_Path = fbd.SelectedPath + "\\" + userName + "_" + check_index.ToString() + "_info.cfg";
                                    }
                                    string line = "";

                                    // Save Item Information
                                    line += Get_Save_Lines();

                                    try
                                    {
                                        File.Delete(Info_Path);
                                        using (StreamWriter sw = File.CreateText(Info_Path)) // Create translator file
                                        {
                                            sw.Write(line + Environment.NewLine);
                                            sw.Close();
                                        }
                                        Form_Message_Box FMB = new Form_Message_Box(this, "Information export successfully to \"" + Info_Path + "\"");
                                        FMB.ShowDialog();
                                    }
                                    catch
                                    {
                                    }

                                }
                            }
                        }
                    }
                    this.Info_Path = "";
                    reset_button.PerformClick();
                    Reload_Program();
                }
            }
        }

        private void toolStripMenuItem22_Click(object sender, EventArgs e)
        {
            if (Monthly_Income > 0)
            {
                Savings_Helper SH = new Savings_Helper(this);
                SH.Show();
            }
            else
            {
                Form_Message_Box FMB = new Form_Message_Box(this, "Please set a personal salary before setting up personal savings");
                FMB.ShowDialog();
                Salary_Calculator PF = new Salary_Calculator(this);
                PF.ShowDialog();
            }
        }

        private void toolStripMenuItem23_Click(object sender, EventArgs e)
        {
            Calendar c = new Calendar(this);
            c.Show();
        }

        private void toolStripMenuItem24_Click(object sender, EventArgs e)
        {
            Alerts_And_Windows AAW = new Alerts_And_Windows(this);
            AAW.ShowDialog();
        }

        private void toolStripMenuItem25_Click(object sender, EventArgs e)
        {
            Security S = new Security(this);
            S.ShowDialog();
        }

        private void toolStripMenuItem26_Click(object sender, EventArgs e)
        {
            Personal_Information PI = new Personal_Information(this);
            PI.ShowDialog();
        }

        private void toolStripMenuItem27_Click(object sender, EventArgs e)
        {
            Backup_Settings BS = new Backup_Settings(this);
            BS.ShowDialog();
        }


    }

    public class AdvancedComboBox : ComboBox
    {

        new public System.Windows.Forms.DrawMode DrawMode { get; set; }
        public Color HighlightColor { get; set; }
        public Color FrameColor { get; set; }


        public AdvancedComboBox()
        {
            //this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.FrameColor = SystemColors.HotTrack;
            base.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.HighlightColor = Color.Gray;
            this.DrawItem += new DrawItemEventHandler(AdvancedComboBox_DrawItem);
        }

        void AdvancedComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;


            ComboBox combo = sender as ComboBox;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e.Graphics.FillRectangle(new SolidBrush(HighlightColor),
                                         e.Bounds);
            else
                e.Graphics.FillRectangle(new SolidBrush(combo.BackColor),
                                         e.Bounds);

            e.Graphics.DrawString(combo.Items[e.Index].ToString(), e.Font,
                                  new SolidBrush(combo.ForeColor),
                                  new Point(e.Bounds.X, e.Bounds.Y));
            
            //e.Graphics.DrawString(combo.Items[e.Index].ToString(), e.Font, brush, e.Bounds, new StringFormat(StringFormatFlags.DirectionRightToLeft));

            //e.Graphics.DrawRectangle(new Pen(FrameColor, 2), 0, 0,
            //  this.Width - 2, this.Items.Count * 20);

            // Draw the rectangle around the drop-down list
            //if (combo.DroppedDown)
            if (false)
            {
                SolidBrush ArrowBrush = new SolidBrush(SystemColors.HighlightText);

                Rectangle dropDownBounds = new Rectangle(0, 0, combo.Width-2, combo.Items.Count*combo.ItemHeight);
                //ControlPaint.DrawBorder(g, dropDownBounds, _borderColor, _borderStyle);
                ControlPaint.DrawBorder(e.Graphics, dropDownBounds,
                    FrameColor, 1, ButtonBorderStyle.Solid,
                    FrameColor, 1, ButtonBorderStyle.Solid,
                    FrameColor, 1, ButtonBorderStyle.Solid,
                    FrameColor, 1, ButtonBorderStyle.Solid);
            }
            e.DrawFocusRectangle();
        }
    }


    // Custom ToolStrip 
    /*
    public class MyRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected) base.OnRenderMenuItemBackground(e);
            else
            {
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                e.Graphics.FillRectangle(Brushes.Gray, rc);
                //e.Graphics.DrawRectangle(Pens.Black, 1, 0, rc.Width - 2, rc.Height - 1);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
 	        //base.OnRenderToolStripBackground(e);
            ToolStripDropDown dr = e.ToolStrip as ToolStripDropDown;

            if (dr != null)
            {
                e.Graphics.FillRectangle(Brushes.Black, e.AffectedBounds);
            }
        }
    }
        */

    // Custom Menustrip 
    public class TestColorTable : ProfessionalColorTable
    {
        //private Color Menu_Bar_Color = Color.FromArgb(76, 76, 76);
        public Color Menu_Bar_Color = Color.FromArgb(64, 64, 64);
        public Color Highlight_Menu_Color = Color.FromArgb(80, 80, 80);
        public Color Menu_Border_Color = SystemColors.HotTrack;
        //public Color Menu_Border_Color = SystemColors.Control;

        public void Set_Menu_Color(Color color)
        {
            Menu_Border_Color = color;
        }

        public override Color MenuItemSelected
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color MenuItemBorder
        {
            get { return Menu_Bar_Color; }
        }

        public override Color MenuBorder  //added for changing the menu border
        {
            get { return Menu_Bar_Color; }
        }

        public override Color MenuStripGradientBegin
        {
            get { return Menu_Bar_Color; }
        }

        public override Color MenuStripGradientEnd
        {
            get { return Menu_Bar_Color; }
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color ToolStripDropDownBackground
        {
            get { return Menu_Border_Color; }
        }

        public override Color ImageMarginGradientBegin
        {
            get { return Menu_Border_Color; }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color CheckSelectedBackground
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color ButtonSelectedHighlight
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color ButtonSelectedHighlightBorder
        {
            get { return Highlight_Menu_Color; }
        }
    }

    public class NoFocusCueButton : Button
    {
        public NoFocusCueButton() : base()
        {
            //InitializeComponent();

            this.SetStyle(ControlStyles.Selectable, false);
        }

        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
    }

    public class Payment
    {
        public string Emergency_No { get; set; }
        public string Payment_Type { get; set; }
        public string Last_Four { get; set; }
        public string Company { get; set; }
        public string Bank { get; set; }
        public double Limit { get; set; }
        public string Billing_Start { get; set; }
        public double Total { get; set; }
        public DateTime Last_Reset_Date { get; set; }
        public List<Payment_Alert> Alerts { get; set; }
        public int Calendar_Toggle { get; set; }

        public Payment()
        {
            Alerts = new List<Payment_Alert>() 
            {
                new Payment_Alert() { ID = 0, Active = false, Repeat = false, Desc = "Upcoming payment due alert"
                },
                new Payment_Alert() { ID = 1, Active = false, Repeat = false, Desc = "20% spending alert"
                },
                new Payment_Alert() { ID = 2, Active = false, Repeat = false, Desc = "50% spending alert"
                },
                new Payment_Alert() { ID = 3, Active = false, Repeat = false, Desc = "90% spending alert"
                }
            };
        }

        public string Check_Alerts()
        {
            if (Alerts[3].Active && Alerts[3].Repeat && (Limit * 0.9 < Total))
            {
                Alerts[3].Repeat = false;
                return "You have spent over 90% of your limit ($" + String.Format("{0:0.00}", Limit) + ") on your " + Payment_Type + " ending in " + Last_Four;
            }
            else if (Alerts[2].Active && Alerts[2].Repeat && (Limit * 0.5 < Total))
            {
                Alerts[2].Repeat = false;
                return "You have spent over 50% of your limit ($" + String.Format("{0:0.00}", Limit) + ") on your " + Payment_Type + " ending in " + Last_Four;
            }
            else if (Alerts[1].Active && Alerts[1].Repeat && (Limit * 0.2 < Total))
            {
                Alerts[1].Repeat = false;
                return "You have spent over 20% of your limit ($" + String.Format("{0:0.00}", Limit) + ") on your " + Payment_Type + " ending in " + Last_Four;
            }
            return "";
        }

        public string Check_Payment_Due()
        {
            string[] Day_Name = new string[] {"", "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", 
                                                        "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th", 
                                                        "21st", "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th", "30th", "31st"};

            if (Alerts[0].Active && Alerts[0].Repeat && Total > 0)
            {
                if (DateTime.Now.Day + 5 > Convert.ToInt32(Billing_Start) && DateTime.Now.Day < Convert.ToInt32(Billing_Start))
                { 
                    Alerts[0].Repeat = false;
                    return "Your " + Payment_Type + " ending in " + Last_Four + " is due on the " + Day_Name[Convert.ToInt32(Billing_Start)] + " ($" + String.Format("{0:0.00}", Total) + ")";
                }
            }
            return "";
        }

        public void Get_Total(List<Item> Item_List, Dictionary<string, string> Tax_Rules, double base_tax, List<Order> Order_List)
        {
            // Check reset
            if (DateTime.Now > Last_Reset_Date.AddDays(29) || (DateTime.Now > Last_Reset_Date.AddDays(25) && Convert.ToInt32(Billing_Start) <= DateTime.Now.Day))
            {
                Alerts.ForEach(x => x.Repeat = true);
                Last_Reset_Date = DateTime.Now;
            }


            string Date_String = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString("D2") + "-" + (Convert.ToInt32(DateTime.Now.Month == 2 && Convert.ToInt32(Billing_Start) > 28 ? "28" : (Billing_Start == "31" ? DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString() : Billing_Start))).ToString("D2");
            DateTime End_Date = DateTime.ParseExact(Date_String, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
            if (DateTime.Now.Day > Convert.ToInt32(Billing_Start)) End_Date = End_Date.AddMonths(1);

            DateTime Start_Date = End_Date.AddMonths(-1);

            List<Order> Filtered_Item_List = Order_List.Where(x => x.Payment_Type == (this.Company + " (xx-" + this.Last_Four + ")") && x.Date > Start_Date && x.Date < End_Date).ToList();
            Total = Filtered_Item_List.Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes);
            //Total = Filtered_Item_List.Sum(x => x.Get_Full_Amount((Tax_Rules.ContainsKey(x.Category) ? Convert.ToDouble(Tax_Rules[x.Category]) : base_tax)));
        }

    }

    public class Payment_Alert
    {
        public string Desc { get; set; }
        public int ID { get; set; }
        public bool Active { get; set; }
        public bool Repeat { get; set; }
    }

    public class Account
    {
        public string Type { get; set; }
        public string Payer { get; set; }
        public string Remark { get; set; }
        public string Amount { get; set; }
        public int Status { get; set; }
        public DateTime Inactive_Date { get; set; }
        public DateTime Start_Date { get; set; }
    }
}

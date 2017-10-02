using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Financial_Journal
{
    public partial class Payment_Information : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            parent.Focus();
            base.OnFormClosing(e);
        }

        private string[] Day_Name = new string[] {"", "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", 
                                                        "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th", 
                                                        "21st", "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th", "30th", "31st"};

        private List<Button> Interactive_Button_List = new List<Button>();
        private List<Button> View_Payment_History_Button = new List<Button>();
        private List<Button> Delete_Item_Buttons = new List<Button>();
        private List<Button> Edit_Item_Button = new List<Button>();
        private List<Button> Alert1_Buttons = new List<Button>();
        private List<Button> Alert2_Buttons = new List<Button>();
        private List<Button> Alert3_Buttons = new List<Button>();
        private List<Button> Alert4_Buttons = new List<Button>();
        ToolTip ToolTip1 = new ToolTip();

        protected override void OnPaint(PaintEventArgs e)
        {
            // Update payment values
            parent.Payment_List.ForEach(x => x.Get_Total(parent.Master_Item_List, parent.Tax_Rules_Dictionary, parent.Tax_Rate, parent.Order_List));

            int data_height = 30;
            int start_height = Start_Size.Height;
            int start_margin = 15;              // Item
            int height_offset = 9;

            int margin1 = start_margin + 190;   //Price
            int margin2 = margin1 + 95;        //Quantity
            int margin3 = margin2 + 145;        //Category
            int margin4 = margin3 + 135;        //Actions
            int margin5 = margin4 + 135;        //Actions

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
            Font f_italic = new Font("MS Reference Sans Serif", 8, FontStyle.Italic);

            // Draw gray header line
            e.Graphics.DrawLine(Grey_Pen, start_margin, start_height, this.Width - 15, start_height);

            height_offset += 1;
            // Header2   
            e.Graphics.DrawString("Payment", f_header, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("Cycle Day", f_header, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("Amt. this cycle", f_header, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("Balance", f_header, WritingBrush, margin3 - 7, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("Alerts", f_header, WritingBrush, margin4, start_height + height_offset + (row_count * data_height));
            row_count += 1;
            height_offset += 5;

            Delete_Item_Buttons.ForEach(button => button.Image.Dispose());
            Delete_Item_Buttons.ForEach(button => button.Dispose());
            Delete_Item_Buttons.ForEach(button => this.Controls.Remove(button));
            Delete_Item_Buttons = new List<Button>();
            Edit_Item_Button.ForEach(button => button.Image.Dispose());
            Edit_Item_Button.ForEach(button => button.Dispose());
            Edit_Item_Button.ForEach(button => this.Controls.Remove(button));
            Edit_Item_Button = new List<Button>();
            Interactive_Button_List.ForEach(button => button.Image.Dispose());
            Interactive_Button_List.ForEach(button => button.Dispose());
            Interactive_Button_List.ForEach(button => this.Controls.Remove(button));
            Interactive_Button_List = new List<Button>();
            View_Payment_History_Button.ForEach(button => button.Image.Dispose());
            View_Payment_History_Button.ForEach(button => button.Dispose());
            View_Payment_History_Button.ForEach(button => this.Controls.Remove(button));
            View_Payment_History_Button = new List<Button>();
            Alert1_Buttons.ForEach(button => button.Image.Dispose());
            Alert1_Buttons.ForEach(button => button.Dispose());
            Alert1_Buttons.ForEach(button => this.Controls.Remove(button));
            Alert1_Buttons = new List<Button>();
            Alert2_Buttons.ForEach(button => button.Image.Dispose());
            Alert2_Buttons.ForEach(button => button.Dispose());
            Alert2_Buttons.ForEach(button => this.Controls.Remove(button));
            Alert2_Buttons = new List<Button>();
            Alert3_Buttons.ForEach(button => button.Image.Dispose());
            Alert3_Buttons.ForEach(button => button.Dispose());
            Alert3_Buttons.ForEach(button => this.Controls.Remove(button));
            Alert3_Buttons = new List<Button>();
            Alert4_Buttons.ForEach(button => button.Image.Dispose());
            Alert4_Buttons.ForEach(button => button.Dispose());
            Alert4_Buttons.ForEach(button => this.Controls.Remove(button));
            Alert4_Buttons = new List<Button>();

            int payment_index = 0;


            foreach (Payment payment in parent.Payment_List)
            {
                ToolTip ToolTip1 = new ToolTip();
                ToolTip1.InitialDelay = 1;
                ToolTip1.ReshowDelay = 1;

                Button delete_button = new Button();
                delete_button.BackColor = this.BackColor;
                delete_button.ForeColor = this.BackColor;
                delete_button.FlatStyle = FlatStyle.Flat;
                delete_button.Image = global::Financial_Journal.Properties.Resources.delete;
                delete_button.Enabled = true;
                delete_button.Size = new Size(29, 29);
                delete_button.Location = new Point(this.Width - 40, start_height + height_offset + (row_count * data_height) - 6);
                delete_button.Name = "del" + payment_index.ToString();
                delete_button.Text = "";
                delete_button.Click += new EventHandler(this.dynamic_button_click);
                //if (payment.Total > 0) delete_button.Enabled = false;
                Delete_Item_Buttons.Add(delete_button);
                ToolTip1.SetToolTip(delete_button, "Delete " + payment.Company);
                this.Controls.Add(delete_button);

                Button Interactive_Button = new Button();
                Interactive_Button.BackColor = this.BackColor;
                Interactive_Button.ForeColor = this.BackColor;
                Interactive_Button.FlatStyle = FlatStyle.Flat;
                Interactive_Button.Image = global::Financial_Journal.Properties.Resources.wallet;
                Interactive_Button.Enabled = true;
                Interactive_Button.Size = new Size(29, 29);
                Interactive_Button.Location = new Point(this.Width - 70, start_height + height_offset + (row_count * data_height) - 6);
                Interactive_Button.Name = "man" + payment_index.ToString();
                Interactive_Button.Text = "";
                Interactive_Button.Click += new EventHandler(this.dynamic_button_click);
                Interactive_Button_List.Add(Interactive_Button);
                ToolTip1.SetToolTip(Interactive_Button, "Manage " + payment.Company);
                this.Controls.Add(Interactive_Button);

                Button Edit_Button = new Button();
                Edit_Button.BackColor = this.BackColor;
                Edit_Button.ForeColor = this.BackColor;
                Edit_Button.FlatStyle = FlatStyle.Flat;
                Edit_Button.Image = global::Financial_Journal.Properties.Resources.edit;
                Edit_Button.Enabled = true;
                Edit_Button.Size = new Size(29, 29);
                Edit_Button.Location = new Point(this.Width - 100, start_height + height_offset + (row_count * data_height) - 6);
                Edit_Button.Name = "edi" + payment_index.ToString();
                Edit_Button.Text = "";
                Edit_Button.Click += new EventHandler(this.dynamic_button_click);
                //if (payment.Total > 0) Edit_Button.Enabled = false;
                Edit_Item_Button.Add(Edit_Button);
                ToolTip1.SetToolTip(Edit_Button, "Edit " + payment.Company);
                this.Controls.Add(Edit_Button);

                //if (payment.Total > 0)
                if (parent.Order_List.Where(x => x.Payment_Type == (payment.Company + " (xx-" + payment.Last_Four + ")")).ToList().Count > 0)
                {
                    Button view_button = new Button();
                    view_button.BackColor = this.BackColor;
                    view_button.ForeColor = this.BackColor;
                    view_button.FlatStyle = FlatStyle.Flat;
                    view_button.Image = global::Financial_Journal.Properties.Resources.eye;
                    view_button.Enabled = true;
                    view_button.Size = new Size(29, 29);
                    view_button.Location = new Point(this.Width - 130, start_height + height_offset + (row_count * data_height) - 6);
                    view_button.Name = "see" + payment_index.ToString();
                    view_button.Text = "";
                    view_button.Click += new EventHandler(this.dynamic_button_click);
                    View_Payment_History_Button.Add(view_button);
                    ToolTip1.SetToolTip(view_button, "View purchases for " + payment.Company);
                    this.Controls.Add(view_button);
                }


                Button alert1_button = new Button();
                alert1_button.BackColor = this.BackColor;
                alert1_button.ForeColor = this.BackColor;
                alert1_button.FlatStyle = FlatStyle.Flat;
                alert1_button.Image = payment.Alerts[0].Active ? global::Financial_Journal.Properties.Resources.onegreen : global::Financial_Journal.Properties.Resources.onewhite;
                alert1_button.Enabled = true;
                alert1_button.Size = new Size(29, 29);
                alert1_button.Location = new Point(margin4 - 30, start_height + height_offset + (row_count * data_height) - 6);
                alert1_button.Name = "a" + payment_index.ToString();
                alert1_button.Text = "";
                alert1_button.Click += new EventHandler(this.dynamic_button_click);
                Alert1_Buttons.Add(alert1_button);
                ToolTip1.SetToolTip(alert1_button, payment.Alerts[0].Active ? "Stop Alert #1" : "Start Alert #1");
                this.Controls.Add(alert1_button);

                Button alert2_button = new Button();
                alert2_button.BackColor = this.BackColor;
                alert2_button.ForeColor = this.BackColor;
                alert2_button.FlatStyle = FlatStyle.Flat;
                alert2_button.Image = payment.Alerts[1].Active ? global::Financial_Journal.Properties.Resources.twogreen : global::Financial_Journal.Properties.Resources.twowhite;
                alert2_button.Enabled = true;
                alert2_button.Size = new Size(29, 29);
                alert2_button.Location = new Point(margin4 - 5, start_height + height_offset + (row_count * data_height) - 6);
                alert2_button.Name = "b" + payment_index.ToString();
                alert2_button.Text = "";
                alert2_button.Click += new EventHandler(this.dynamic_button_click);
                Alert2_Buttons.Add(alert2_button);
                ToolTip1.SetToolTip(alert2_button, payment.Alerts[1].Active ? "Stop Alert #2" : "Start Alert #2");
                this.Controls.Add(alert2_button);

                Button alert3_button = new Button();
                alert3_button.BackColor = this.BackColor;
                alert3_button.ForeColor = this.BackColor;
                alert3_button.FlatStyle = FlatStyle.Flat;
                alert3_button.Image = payment.Alerts[2].Active ? global::Financial_Journal.Properties.Resources.threegreen : global::Financial_Journal.Properties.Resources.threewhite;
                alert3_button.Enabled = true;
                alert3_button.Size = new Size(29, 29);
                alert3_button.Location = new Point(margin4 + 20, start_height + height_offset + (row_count * data_height) - 6);
                alert3_button.Name = "c" + payment_index.ToString();
                alert3_button.Text = "";
                alert3_button.Click += new EventHandler(this.dynamic_button_click);
                Alert3_Buttons.Add(alert3_button);
                ToolTip1.SetToolTip(alert3_button, payment.Alerts[2].Active ? "Stop Alert #3" : "Start Alert #3");
                this.Controls.Add(alert3_button);

                Button alert4_button = new Button();
                alert4_button.BackColor = this.BackColor;
                alert4_button.ForeColor = this.BackColor;
                alert4_button.FlatStyle = FlatStyle.Flat;
                alert4_button.Image = payment.Alerts[3].Active ? global::Financial_Journal.Properties.Resources.fourgreen : global::Financial_Journal.Properties.Resources.fourwhite;
                alert4_button.Enabled = true;
                alert4_button.Size = new Size(29, 29);
                alert4_button.Location = new Point(margin4 + 45, start_height + height_offset + (row_count * data_height) - 6);
                alert4_button.Name = "d" + payment_index.ToString();
                alert4_button.Text = "";
                alert4_button.Click += new EventHandler(this.dynamic_button_click);
                Alert4_Buttons.Add(alert4_button);
                ToolTip1.SetToolTip(alert4_button, payment.Alerts[3].Active ? "Stop Alert #4" : "Start Alert #4");
                this.Controls.Add(alert4_button);


                e.Graphics.DrawString(payment.Company + " (" + payment.Bank + ")", f, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString(Day_Name[Convert.ToInt32(payment.Billing_Start)] + " of month", f, WritingBrush, margin1 - 6 + (payment.Billing_Start.Length == 1 ? 4 : 0), start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", payment.Total), f, WritingBrush, margin2 + 20 + (payment.Total < 1000 ? payment.Total < 100 ? 8 : 4 : 0), start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", payment.Balance), f, WritingBrush, margin3 - 10 + (payment.Balance < 1000 ? payment.Balance < 100 ? 8 : 4 : 0), start_height + height_offset + (row_count * data_height));
                //e.Graphics.DrawString("Alerts", f_header, WritingBrush, margin4, start_height + height_offset + (row_count * data_height));
                height_offset += 15;
                e.Graphics.DrawString(payment.Payment_Type + " ending in xx-" + payment.Last_Four, f_italic, WritingBrush, start_margin + 1, start_height + height_offset + (row_count * data_height));

                row_count++;
                payment_index++;
            }

            height_offset += 5;
            this.Height = start_height + height_offset + row_count * data_height;

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

        int Edit_Index = -1; 
        double Temp_Payment_Total = 0;


        private void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        private void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
        }

        private void dynamic_button_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            Expenses Ref_Expense = new Expenses();

            if (b.Name.StartsWith("del")) // delete
            {
                Grey_Out();
                using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to delete " + parent.Payment_List[Convert.ToInt32(b.Name.Substring(3))].Company + "?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result21 = form1.ShowDialog();
                    if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                    {
                        Payment ref_pay = parent.Payment_List[Convert.ToInt32(b.Name.Substring(3))];

                        // Delete all payment_options with same payment
                        for (int i = parent.Payment_Options_List.Count - 1; i >= 0; i--)
                        {
                            if (parent.Payment_Options_List[i].Payment_Bank == ref_pay.Bank && parent.Payment_Options_List[i].Payment_Last_Four == ref_pay.Last_Four && parent.Payment_Options_List[i].Payment_Company == ref_pay.Company)
                            {
                                parent.Payment_Options_List.RemoveAt(i);
                            }
                        }

                        // Toggle autodebit to false for recurring expenses
                        foreach (Expenses exp in parent.Expenses_List)
                        {
                            if (exp.Payment_Company == ref_pay.Company && exp.Payment_Last_Four == ref_pay.Last_Four)
                            {
                                exp.AutoDebit = "0";
                                exp.Payment_Company = "";
                                exp.Payment_Last_Four = "";
                            }
                        }

                        // Toggle auto deposit to false for manual income
                        foreach (CustomIncome CI in parent.Income_Company_List)
                        {
                            if (CI.Deposit_Account == ref_pay.Get_Long_String())
                            {
                                CI.Deposit_Account = "";
                            }
                        }

                        parent.Payment_Options_List = parent.Payment_Options_List.Where(x => x.Payment_Bank != ref_pay.Bank && x.Payment_Company != ref_pay.Company && x.Payment_Last_Four != ref_pay.Last_Four).ToList(); ;
                        parent.Payment_List.RemoveAt(Convert.ToInt32(b.Name.Substring(3)));
                        parent.Set_Payment_Box();
                    }

                }
                Grey_In();
            }
            else if (b.Name.StartsWith("see")) // delete
            {
                Grey_Out();
                Payment ref_payment = parent.Payment_List[Convert.ToInt32(b.Name.Substring(3))];
                Purchases PI = new Purchases(parent, true, ref_payment, this.Location, this.Size);
                PI.ShowDialog();
                Grey_In();
            }
            else if (b.Name.StartsWith("edi")) // edit
            {
                if (button1.Visible) button1.PerformClick();
                Edit_Index = Convert.ToInt32(b.Name.Substring(3));
                Payment pay = parent.Payment_List[Edit_Index];
                type_box.Text = pay.Payment_Type;
                Temp_Payment_Total = pay.Total;
                digit_box.Text = pay.Last_Four ;
                company_box.Text = pay.Company;
                bank_box.Text = pay.Bank;
                textBox6.Text = "$" + pay.Balance.ToString();
                limit_box.Text = pay.Limit.ToString();
                billing_date_box.Text = pay.Billing_Start;
                phone_box.Text = pay.Emergency_No;
                check0.Checked = pay.Alerts[0].Active;
                check1.Checked = pay.Alerts[1].Active;
                check2.Checked = pay.Alerts[2].Active;
                check3.Checked = pay.Alerts[3].Active;
                //Purchases PI = new Purchases(parent, true, ref_payment);
                //PI.ShowDialog();
                ToolTip1.SetToolTip(Add_button, "Save payment");

                digit_box.Enabled = Edit_Index < 0;
                bank_box.Enabled = Edit_Index < 0;
                company_box.Enabled = Edit_Index < 0;
                billing_date_box.Enabled = Edit_Index < 0;
            }
            else if (b.Name.StartsWith("man"))
            {
                Grey_Out();
                Payment ref_payment = parent.Payment_List[Convert.ToInt32(b.Name.Substring(3))];
                Manage_Payment MP = new Manage_Payment(this, parent, ref ref_payment, this.Location, this.Size);
                MP.ShowDialog();
                Grey_In();
            }
            else if (b.Name.StartsWith("a")) // delete
            {
                parent.Payment_List[Convert.ToInt32(b.Name.Substring(1))].Alerts[0].Active = !parent.Payment_List[Convert.ToInt32(b.Name.Substring(1))].Alerts[0].Active;
            }
            else if (b.Name.StartsWith("b")) // delete
            {
                parent.Payment_List[Convert.ToInt32(b.Name.Substring(1))].Alerts[1].Active = !parent.Payment_List[Convert.ToInt32(b.Name.Substring(1))].Alerts[1].Active;
            }
            else if (b.Name.StartsWith("c")) // delete
            {
                parent.Payment_List[Convert.ToInt32(b.Name.Substring(1))].Alerts[2].Active = !parent.Payment_List[Convert.ToInt32(b.Name.Substring(1))].Alerts[2].Active;
            }
            else if (b.Name.StartsWith("d")) // delete
            {
                parent.Payment_List[Convert.ToInt32(b.Name.Substring(1))].Alerts[3].Active = !parent.Payment_List[Convert.ToInt32(b.Name.Substring(1))].Alerts[3].Active;
            }
            Invalidate();
            Update();
        }


        Receipt parent;
        Size Start_Size = new Size();
        Size Collapsed_Size = new Size();
        int Start_Location_Offset = 45;

        public Payment_Information(Receipt _parent)
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Collapsed_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
        }

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

            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.company_box.Enter += new EventHandler(this.company_box_Enter);
            this.company_box.Leave += new EventHandler(this.company_box_Leave);
            this.bank_box.Enter += new EventHandler(this.bank_box_Enter);
            this.bank_box.Leave += new EventHandler(this.bank_box_Leave);

            for (int i = 1; i < 32; i++)
            {
                billing_date_box.Items.Add(i.ToString());
            }

            type_box.Items.Add("Credit Card");
            type_box.Items.Add("Debit Card");
            label10.Text = "Add a new payment method";

            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;
            
            ToolTip1.SetToolTip(Add_button, "Add payment");

        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            parent.Background_Save();
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

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (type_box.Text.Length > 0 && company_box.Text.Length > 0 && digit_box.Text.Length == 4 &&
                bank_box.Text.Length > 0 && billing_date_box.Text.Length > 0 && limit_box.Text.Length > 0)
            {
                Payment Orig_Payment = new Payment();
                //edit
                if (Edit_Index >= 0)
                {
                    Orig_Payment = parent.Payment_List[Edit_Index];
                    parent.Payment_List.RemoveAt(Edit_Index);
                }
                Payment pay = new Payment();
                pay.Payment_Type = type_box.Text;
                pay.Last_Four = digit_box.Text;
                pay.Company = company_box.Text;
                pay.Bank = bank_box.Text;
                pay.Balance = (textBox6.Text.Length > 1) ? Convert.ToDouble(textBox6.Text.Substring(1)) : 0;
                try
                {
                    pay.Limit = Convert.ToDouble(limit_box.Text.Substring(1));
                }
                catch
                {
                    pay.Limit = 0;
                }
                pay.Billing_Start = billing_date_box.Text;
                pay.Emergency_No = phone_box.Text;
                pay.Total = Temp_Payment_Total;
                pay.Alerts[0].Active = check0.Checked;
                pay.Alerts[1].Active = check1.Checked;
                pay.Alerts[2].Active = check2.Checked;
                pay.Alerts[3].Active = check3.Checked;
                pay.Alerts[0].Repeat = check0.Checked;
                pay.Alerts[1].Repeat = check1.Checked;
                pay.Alerts[2].Repeat = check2.Checked;
                pay.Alerts[3].Repeat = check3.Checked;

                // If edit
                if (Edit_Index >= 0)
                {
                    if (Orig_Payment.Balance != pay.Balance)
                    {
                        // balance altered

                        // Add record of balance adjustment to history
                        parent.Payment_Options_List.Add(new Payment_Options()
                        {
                            Type = "Adjustment",
                            Amount = pay.Balance - Orig_Payment.Balance,
                            Date = DateTime.Now,
                            Note = "Balance adjustment (" + "$" + String.Format("{0:0.00}", Orig_Payment.Balance) + " -> " + "$" + String.Format("{0:0.00}", pay.Balance) + ")",
                            Ending_Balance = pay.Balance,// Adjust balance
                            Payment_Bank = pay.Bank,
                            Hidden_Note = "Balance Adj.", // hidden so program can identify this is recurring expense
                            Payment_Company = pay.Company,
                            Payment_Last_Four = pay.Last_Four
                        });
                    }
                        
                    parent.Payment_List.Insert(Edit_Index, pay);

                    // Modifying existing items/orders/payment_options with current bank/company/last_four to the new values

                    // Modify items
                    foreach (Item item in parent.Master_Item_List)
                    {
                        if (item.Payment_Type == (Orig_Payment.Company + " (xx-" + Orig_Payment.Last_Four + ")"))
                        {
                            item.Payment_Type = (pay.Company + " (xx-" + pay.Last_Four + ")");
                        }
                    }

                    // Modify Orders
                    foreach (Order order in parent.Order_List)
                    {
                        if (order.Payment_Type == (Orig_Payment.Company + " (xx-" + Orig_Payment.Last_Four + ")"))
                        {
                            order.Payment_Type = (pay.Company + " (xx-" + pay.Last_Four + ")");
                        }
                    }

                    // Modifying Accounts
                    foreach (Payment_Options PO in parent.Payment_Options_List)
                    {
                        if (PO.Payment_Bank == Orig_Payment.Bank && PO.Payment_Last_Four == Orig_Payment.Last_Four && PO.Payment_Company == Orig_Payment.Company)
                        {
                            PO.Payment_Bank = pay.Bank;
                            PO.Payment_Last_Four = pay.Last_Four;
                            PO.Payment_Company = pay.Company;
                        }
                    }

                    // Modify expenses with auto debit to changed accounts
                    foreach (Expenses exp in parent.Expenses_List)
                    {
                        if (exp.Payment_Last_Four == Orig_Payment.Last_Four && exp.Payment_Company == Orig_Payment.Company)
                        {
                            exp.Payment_Last_Four = pay.Last_Four;
                            exp.Payment_Company = pay.Company;
                        }
                    }
                    Edit_Index = -1;
                    Temp_Payment_Total = 0;
                    Invalidate();
                    phone_box.Text = "";
                    billing_date_box.Text = "";
                    limit_box.Text = "";
                    bank_box.Text = "";
                    digit_box.Text = "";
                    type_box.Text = "";
                    textBox6.Text = "";
                    bufferedPanel1.Visible = false;
                    label10.Visible = true;
                    button1.Visible = true;
                    Start_Size.Height -= 150;
                    this.Height -= 150;
                    parent.Set_Payment_Box();
                    ToolTip1.SetToolTip(Add_button, "Add payment");
                }
                else
                {
                    List<string> Payment_Check_List = new List<string>();
                    parent.Payment_List.ForEach(x => Payment_Check_List.Add(x.ToString()));

                    if (Payment_Check_List.Contains(pay.ToString()))
                    {
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(parent, "A payment with same company and four digits exists already", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        Grey_In();
                    }
                    else
                    {
                        parent.Payment_List.Add(pay);
                        Edit_Index = -1;
                        Temp_Payment_Total = 0;

                        Invalidate();
                        phone_box.Text = "";
                        billing_date_box.Text = "";
                        limit_box.Text = "";
                        bank_box.Text = "";
                        digit_box.Text = "";
                        type_box.Text = "";
                        textBox6.Text = "";
                        bufferedPanel1.Visible = false;
                        label10.Visible = true;
                        button1.Visible = true;
                        Start_Size.Height -= 150;
                        this.Height -= 150;
                        parent.Set_Payment_Box();
                        ToolTip1.SetToolTip(Add_button, "Add payment");
                    }
                }

            }

            digit_box.Enabled = Edit_Index < 0;
            bank_box.Enabled = Edit_Index < 0;
            company_box.Enabled = Edit_Index < 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bufferedPanel1.Visible = true;
            label10.Visible = false;
            button1.Visible = false;
            Start_Size.Height += 150;
            this.Height += 150;

            phone_box.Text = "";
            billing_date_box.Text = "";
            limit_box.Text = "";
            bank_box.Text = "";
            digit_box.Text = "";
            type_box.Text = "";
            textBox6.Text = "";
        }
        
        //close
        private void search_desc_button_Click(object sender, EventArgs e)
        {
            bufferedPanel1.Visible = false;
            label10.Visible = true;
            button1.Visible = true;
            Edit_Index = -1;
            Temp_Payment_Total = 0;
            Start_Size.Height -= 150;
            this.Height -= 150;
            ToolTip1.SetToolTip(Add_button, "Add payment");
        }

        private void digit_box_TextChanged(object sender, EventArgs e)
        {

            if (digit_box.Text.All(char.IsDigit) && digit_box.Text.Length > 0)
            {
            }
            else if (digit_box.Text.Length > 0)
            {
                // If letter in SO_number box, do not output and move CARET to end
                try
                {
                    digit_box.Text = digit_box.Text.Substring(0, digit_box.Text.Length - 1);
                    digit_box.SelectionStart = digit_box.Text.Length;
                    digit_box.SelectionLength = 0;
                }
                catch
                { }
            }
        }

        private void company_box_TextChanged(object sender, EventArgs e)
        {
            company_box.Text = parent.Remove_Character(company_box.Text, ',');
            company_box.ForeColor = company_box.Text.Length > 0 ? Color.White : Color.LightGray;
        }

        private void company_box_Leave(object sender, EventArgs e)
        {
            if (company_box.Text.Length == 0)
            {
                company_box.Text = "Mastercard";
                company_box.ForeColor = Color.LightGray;
            }
        }

        private void company_box_Enter(object sender, EventArgs e)
        {
            if (company_box.Text == "Mastercard")
            {
                company_box.Text = "";
                company_box.ForeColor = company_box.ForeColor;
            }
        }

        private void bank_box_Leave(object sender, EventArgs e)
        {
            if (bank_box.Text.Length == 0)
            {
                bank_box.Text = "Mastercard";
                bank_box.ForeColor = Color.LightGray;
            }
        }

        private void bank_box_Enter(object sender, EventArgs e)
        {
            if (bank_box.Text == "Mastercard")
            {
                bank_box.Text = "";
                bank_box.ForeColor = bank_box.ForeColor;
            }
        }

        private void bank_box_TextChanged(object sender, EventArgs e)
        {
            bank_box.ForeColor = bank_box.Text.Length > 0 ? Color.White : Color.LightGray;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (!(textBox6.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(textBox6.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = textBox6.Text;
                    textBox6.Text = temp.Substring(1) + temp[0];
                    textBox6.SelectionStart = textBox6.Text.Length;
                    textBox6.SelectionLength = 0;
                }
                else
                {
                    textBox6.Text = "$" + textBox6.Text;
                }
            }
            else if ((textBox6.Text.Length > 1) && ((parent.Get_Char_Count(textBox6.Text, Convert.ToChar(".")) > 1) || (textBox6.Text[1].ToString() == ".") || (parent.Get_Char_Count(textBox6.Text, Convert.ToChar("$")) > 1) || (!((textBox6.Text.Substring(textBox6.Text.Length - 1).All(char.IsDigit))) && !(textBox6.Text[textBox6.Text.Length - 1].ToString() == "."))))
            {
                textBox6.TextChanged -= new System.EventHandler(textBox6_TextChanged);
                textBox6.Text = textBox6.Text.Substring(0, textBox6.Text.Length - 1);
                textBox6.SelectionStart = textBox6.Text.Length;
                textBox6.SelectionLength = 0;
                textBox6.TextChanged += new System.EventHandler(textBox6_TextChanged);
            }
            else if (textBox6.Text[textBox6.Text.Length - 1] != '.' && textBox6.Text.Length > 1 || (textBox6.Text.Length == textBox6.MaxLength && textBox6.Text[textBox6.Text.Length - 1] == '.'))
            {
                if (textBox6.Text.Contains("."))
                {
                    textBox6.MaxLength = 8;
                }
                else
                {
                    textBox6.MaxLength = 7;
                }
            }
        }

        private void limit_box_TextChanged(object sender, EventArgs e)
        {
            if (!(limit_box.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(limit_box.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = limit_box.Text;
                    limit_box.Text = temp.Substring(1) + temp[0];
                    limit_box.SelectionStart = limit_box.Text.Length;
                    limit_box.SelectionLength = 0;
                }
                else
                {
                    limit_box.Text = "$" + limit_box.Text;
                }
            }
            else if ((limit_box.Text.Length > 1) && ((parent.Get_Char_Count(limit_box.Text, Convert.ToChar(".")) > 1) || (limit_box.Text[1].ToString() == ".") || (parent.Get_Char_Count(limit_box.Text, Convert.ToChar("$")) > 1) || (!((limit_box.Text.Substring(limit_box.Text.Length - 1).All(char.IsDigit))) && !(limit_box.Text[limit_box.Text.Length - 1].ToString() == "."))))
            {
                limit_box.TextChanged -= new System.EventHandler(limit_box_TextChanged);
                limit_box.Text = limit_box.Text.Substring(0, limit_box.Text.Length - 1);
                limit_box.SelectionStart = limit_box.Text.Length;
                limit_box.SelectionLength = 0;
                limit_box.TextChanged += new System.EventHandler(limit_box_TextChanged);
            }
            else if (limit_box.Text[limit_box.Text.Length - 1] != '.' && limit_box.Text.Length > 1 || (limit_box.Text.Length == limit_box.MaxLength && limit_box.Text[limit_box.Text.Length - 1] == '.'))
            {
                if (limit_box.Text.Contains("."))
                {
                    limit_box.MaxLength = 8;
                }
                else
                {
                    limit_box.MaxLength = 6;
                }
            }
        }

        private void type_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            //label11.Visible = type_box.Text.Contains("Debit");
            //label12.Visible = type_box.Text.Contains("Debit");
            //textBox6.Visible = type_box.Text.Contains("Debit");
        }

        private void bufferedPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

    }


}

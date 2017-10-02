using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Financial_Journal
{
    public partial class Manage_Payment : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            base.OnFormClosing(e);
        }

        int SH0 = 0;
        int SH1 = 0;
        int SH2 = 0;
        int SH3 = 0;
        int SH4 = 0;
        int Margin0 = 15;
        int Margin1 = 0;
        int Buffer1_Height = 0;
        int Buffer2_Height = 0;
        int Buffer3_Height = 0;
        int Buffer4_Height = 0;
        int Buffer5_Height = 0;
        int Data_Height = 22;

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        Payment Ref_Payment = new Payment();

        List<Button> Open_Buttons = new List<Button>();
        List<Button> Close_Buttons = new List<Button>();
        List<Label> Label_List = new List<Label>();
        List<int> Height_List = new List<int>();

        private List<Payment_Options> Current_Payment_Options_List = new List<Payment_Options>();

        private int Entries_Per_Page = 6;
        int Pages_Required = 0;
        int Current_Page = 0;

        private void Populate_History()
        {
            Current_Payment_Options_List = new List<Payment_Options>();


            Current_Payment_Options_List = parent.Payment_Options_List.Where(x => x.Payment_Bank == Ref_Payment.Bank &&
                x.Payment_Last_Four == Ref_Payment.Last_Four &&
                x.Payment_Company == Ref_Payment.Company
                ).ToList();

            // Quantify billing end balance values (aka balance them accordingly to transactions in and out) - from start balance ONLY (unless balance adjustment)
            Recalculate_Balances(ref Current_Payment_Options_List);

            //Current_Payment_Options_List = Current_Payment_Options_List.OrderByDescending(x => x.Date).ToList();

            Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Current_Payment_Options_List.Count() / (decimal)Entries_Per_Page));
            next_page_button.Visible = Pages_Required > 1;
        }

        private void Recalculate_Balances(ref List<Payment_Options> Ref_PO_List)
        {
            Ref_PO_List = Ref_PO_List.OrderBy(x => x.Date).ToList();

            if (Ref_PO_List.Count > 0)
            {
                double referenceBalance = Ref_PO_List[0].Ending_Balance;

                for (int i = 1; i < Ref_PO_List.Count; i++)
                {
                    Payment_Options Ref_PO = Ref_PO_List[i];

                    if (Ref_PO.Type != "Adjustment")
                    {
                        switch (Ref_PO.Type)
                        {
                            case "Deposit":
                                referenceBalance += Ref_PO.Amount;
                                break;
                            case "Withdrawal":
                                referenceBalance -= Ref_PO.Amount;
                                break;
                            case "Payment":
                                referenceBalance -= Ref_PO.Amount;
                                break;
                        }
                        Ref_PO.Ending_Balance = referenceBalance;
                    }
                    else
                    {
                        Ref_PO.Ending_Balance = Ref_PO.Ending_Balance;
                        referenceBalance = Ref_PO.Ending_Balance;
                    }
                }

                Ref_PO_List.Reverse();
            }
        }

        // Painting history panel
        private void panel_Paint(object sender, PaintEventArgs e)
        {

            SolidBrush WritingBrush = new SolidBrush(Color.White);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(180, 180, 180));
            SolidBrush LightGreyBrush = new SolidBrush(Color.FromArgb(210, 210, 210));
            SolidBrush BlueBrush = new SolidBrush(Color.LightBlue);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            SolidBrush PurpleBrush = new SolidBrush(Color.MediumPurple);
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush OrangeBrush = new SolidBrush(Color.Orange);
            SolidBrush LightOrangeBrush = new SolidBrush(Color.FromArgb(255, 200, 0));

            Pen p = new Pen(WritingBrush, 1);
            Pen p_dash = new Pen(GreyBrush, 1);
            p_dash.DashPattern = new float[] { 2, 2, 2, 2, 2, 2, 2 };
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Blue_Pen = new Pen(BlueBrush, 1);
            Pen Green_Pen = new Pen(GreenBrush, 1);
            Pen Red_Pen = new Pen(RedBrush, 1);
            Pen Orange_Pen = new Pen(OrangeBrush, 1);
            Pen Purple_Pen = new Pen(PurpleBrush, 1);

            Font f_9_bold = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_10_bold = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_10 = new Font("MS Reference Sans Serif", 8, FontStyle.Italic);
            Font f_8 = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font f_14_bold = new Font("MS Reference Sans Serif", 14, FontStyle.Bold);
            Font f_12_bold = new Font("MS Reference Sans Serif", 11, FontStyle.Bold);

            var g = e.Graphics;

            int start_height = 9;
            int height_offset = 9;
            int row_count = 0;
            int data_height = 20;

            int margin1 = 15;
            int margin2 = margin1 + 70;
            int margin3 = margin2 + 70;
            int margin4 = margin3 + 70;

            if (Current_Payment_Options_List.Count > 0)
            {
                e.Graphics.DrawString("Date", f_10_bold, WritingBrush, margin1, start_height + (row_count * data_height) + height_offset);
                e.Graphics.DrawString("Type", f_10_bold, WritingBrush, margin2, start_height + (row_count * data_height) + height_offset);
                e.Graphics.DrawString("Amount", f_10_bold, WritingBrush, margin3, start_height + (row_count * data_height) + height_offset);
                e.Graphics.DrawString("Ending Balance", f_10_bold, WritingBrush, margin4, start_height + (row_count * data_height) + height_offset);
                row_count++;
                height_offset += 5;


                foreach (Payment_Options PO in Current_Payment_Options_List.GetRange(Current_Page * Entries_Per_Page, (Current_Payment_Options_List.Count - Entries_Per_Page * Current_Page) >= Entries_Per_Page ? Entries_Per_Page : (Current_Payment_Options_List.Count % Entries_Per_Page)))
                {
                    WritingBrush = PO.Type == "Deposit" ? GreenBrush : PO.Type == "Adjustment" ? new SolidBrush(Color.White) : RedBrush;
                    e.Graphics.DrawString(PO.Date.ToShortDateString(), f_8, WritingBrush, margin1 - 10, start_height + (row_count * data_height) + height_offset);
                    e.Graphics.DrawString(PO.Type, f_8, WritingBrush, margin2 - 5, start_height + (row_count * data_height) + height_offset);
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", PO.Amount), f_8, WritingBrush, margin3 + (PO.Amount < 10 ? 15 : PO.Amount < 100 ? 7 : 0), start_height + (row_count * data_height) + height_offset);
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", PO.Ending_Balance), f_8, WritingBrush, margin4 + 20, start_height + (row_count * data_height) + height_offset);
                    height_offset += 16;
                    if (PO.Note.Length > 0)
                    {
                        e.Graphics.DrawString(PO.Note, f_10, WritingBrush, margin1 + 10, start_height + (row_count * data_height) + height_offset);
                        row_count++;
                    }
                }

                WritingBrush = new SolidBrush(Color.White);
            }
            else
            {
                row_count += 5;
                e.Graphics.DrawString("No history available", f_14_bold, WritingBrush, margin2 - 25, start_height + (row_count * data_height) + height_offset);
            }

            TFLP.Size = new Size(this.Width - 2, this.Height - 2);

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            LightGreyBrush.Dispose();
            GreyBrush.Dispose();
            BlueBrush.Dispose();
            RedBrush.Dispose();
            GreenBrush.Dispose();
            PurpleBrush.Dispose();
            OrangeBrush.Dispose();
            f_14_bold.Dispose();
            LightOrangeBrush.Dispose();
            Blue_Pen.Dispose();
            Green_Pen.Dispose();
            Red_Pen.Dispose();
            f_12_bold.Dispose();
            f_10.Dispose();
            f_9_bold.Dispose();
            f_10_bold.Dispose();
            f_8.Dispose();
            f_14_bold.Dispose();
            Purple_Pen.Dispose();
            Orange_Pen.Dispose();
            WritingBrush.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        public Manage_Payment(Payment_Information PI_parent, Receipt _parent, ref Payment Ref_Payment_, Point g = new Point(), Size s = new Size())
        {

            Ref_Payment = Ref_Payment_;
            //this.Location = new Point(PI_parent.Location.X + Start_Location_Offset, PI_parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            this.Location = new Point(g.X + s.Width / 2 - this.Width / 2, g.Y + s.Height / 3 - this.Height / 2);
            Set_Form_Color(parent.Frame_Color);

            // Preset dimensions

            SH0 = open1.Top;
            SH1 = open2.Top;
            SH2 = open3.Top;
            SH3 = open4.Top;
            SH4 = open5.Top;
            Margin1 = this.Width / 2;


            Buffer1_Height = bufferedPanel1.Height;
            Buffer2_Height = bufferedPanel2.Height;
            Buffer3_Height = bufferedPanel3.Height;
            Buffer4_Height = bufferedPanel4.Height;
            Buffer5_Height = bufferedPanel5.Height;

            Height_List.Add(SH0);
            Height_List.Add(SH1);
            Height_List.Add(SH2);
            Height_List.Add(SH3);
            Height_List.Add(SH4);
            Open_Buttons.Add(open1);
            Open_Buttons.Add(open2);
            Open_Buttons.Add(open3);
            Open_Buttons.Add(open4);
            Open_Buttons.Add(open5);
            Close_Buttons.Add(close1);
            Close_Buttons.Add(close2);
            Close_Buttons.Add(close3);
            Close_Buttons.Add(close4);
            Close_Buttons.Add(close5);
            Label_List.Add(label11);
            Label_List.Add(label12);
            Label_List.Add(label13);
            Label_List.Add(label14);
            Label_List.Add(label15);

            Populate_History();

        }
        
        ToolTip ToolTip1 = new ToolTip();

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            label5.Text = "Manage " + Ref_Payment.Company + " (" + Ref_Payment.Bank + ")";

            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            ToolTip1.SetToolTip(deposit, "Make a Deposit");
            ToolTip1.SetToolTip(withdraw, "Make a Withdrawal");
            ToolTip1.SetToolTip(pay, "Make a Payment");

            // Preset dates 
            dtpDeposit.Value = DateTime.Now;
            dtpWithdraw.Value = DateTime.Now;
            dtpPay.Value = DateTime.Now;

            bufferedPanel4.Paint += new PaintEventHandler(panel_Paint);

            if (Ref_Payment.Payment_Type == "Credit Card")
            {
                //open1.Enabled = false;
                //open2.Enabled = false;
                //label11.Text = "You cannot deposit into Credit Card";
                //label12.Text = "You cannot withdraw from Credit Card";
            }

            Populate_Combo_Boxes();

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


        List<Account> Payable_List = new List<Account>();
        List<Account> Receivable_List = new List<Account>();

        private void Populate_Combo_Boxes()
        {
            // Populate Payables
            payAR.Items.Clear();
            depositAR.Items.Clear();
            payAR.Items.Add("None");
            depositAR.Items.Add("None");
            Payable_List = new List<Account>();
            Receivable_List = new List<Account>();

            foreach (Account AR in parent.Account_List.Where(x => x.Type == "Payable" && x.Status > 0))
            {
                Payable_List.Add(AR);
                payAR.Items.Add(AR.Payer + (AR.Remark.Length > 1 ? "-" + AR.Remark + "" : "") + " (" + AR.Amount + ")");
            }

            foreach (Account AR in parent.Account_List.Where(x => x.Type == "Receivable" && x.Status > 0))
            {
                Receivable_List.Add(AR);
                depositAR.Items.Add(AR.Payer + (AR.Remark.Length > 1 ? "-" + AR.Remark + "" : "") + " (" + AR.Amount + ")");
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


        private void Reset_Panel()
        {
            bufferedPanel1.Left = bufferedPanel2.Left = bufferedPanel3.Left = bufferedPanel4.Left = bufferedPanel5.Left = Margin1;
            bufferedPanel1.Visible = bufferedPanel2.Visible = bufferedPanel3.Visible = bufferedPanel4.Visible = bufferedPanel5.Visible = false;
            open1.Visible = open2.Visible = open3.Visible = open4.Visible = open5.Visible = true;
            label11.Visible = label12.Visible = label13.Visible = label14.Visible = label15.Visible = true;
            label11.ForeColor = label12.ForeColor = label13.ForeColor = label14.ForeColor = label15.ForeColor = close_button.ForeColor;
            close1.Visible = close2.Visible = close3.Visible = close4.Visible = close5.Visible = false;
            this.Height = Start_Size.Height;

            // Reset Heights
            for (int i = 1; i < Open_Buttons.Count; i++)
            {
                Open_Buttons[i].Top = Height_List[i];
                Close_Buttons[i].Top = Height_List[i];
                Label_List[i].Top = Height_List[i] + 5;
            }

            Populate_History();
        }

        private Color Highlight_Color = Color.LightGreen;

        private void open1_Click(object sender, EventArgs e)
        {
            Reset_Panel();
            open1.Visible = false;
            close1.Visible = true;
            bufferedPanel1.Left = Margin0;
            bufferedPanel1.Top = open1.Top + Data_Height;
            bufferedPanel1.Visible = true;
            label11.ForeColor = Highlight_Color;
            this.Height = Start_Size.Height + Buffer1_Height;

            // Move rest of the items down
            for (int i = 1; i < Open_Buttons.Count; i++)
            {
                Open_Buttons[i].Top += Buffer1_Height;
                Close_Buttons[i].Top += Buffer1_Height;
                Label_List[i].Top += Buffer1_Height;
            }
        }

        private void open4_Click(object sender, EventArgs e)
        {
            Populate_History();
            Current_Page = 0;
            Reset_Panel();
            open4.Visible = false;
            close4.Visible = true;
            bufferedPanel4.Left = Margin0;
            bufferedPanel4.Top = open4.Top + Data_Height;
            bufferedPanel4.Visible = true;
            label14.ForeColor = Highlight_Color;
            this.Height = Start_Size.Height + Buffer4_Height;

            // Move rest of the items down
            for (int i = 4; i < Open_Buttons.Count; i++)
            {
                Open_Buttons[i].Top += Buffer4_Height;
                Close_Buttons[i].Top += Buffer4_Height;
                Label_List[i].Top += Buffer4_Height;
            }
        }

        private void open3_Click(object sender, EventArgs e)
        {
            Reset_Panel();
            open3.Visible = false;
            close3.Visible = true;
            bufferedPanel3.Left = Margin0;
            bufferedPanel3.Top = open3.Top + Data_Height;
            bufferedPanel3.Visible = true;
            label13.ForeColor = Highlight_Color;
            this.Height = Start_Size.Height + Buffer3_Height;

            // Move rest of the items down
            for (int i = 3; i < Open_Buttons.Count; i++)
            {
                Open_Buttons[i].Top += Buffer3_Height;
                Close_Buttons[i].Top += Buffer3_Height;
                Label_List[i].Top += Buffer3_Height;
            }
        }

        private void open2_Click(object sender, EventArgs e)
        {
            Reset_Panel();
            open2.Visible = false;
            close2.Visible = true;
            bufferedPanel2.Left = Margin0;
            bufferedPanel2.Top = open2.Top + Data_Height;
            bufferedPanel2.Visible = true;
            label12.ForeColor = Highlight_Color;
            this.Height = Start_Size.Height + Buffer2_Height;

            // Move rest of the items down
            for (int i = 2; i < Open_Buttons.Count; i++)
            {
                Open_Buttons[i].Top += Buffer2_Height;
                Close_Buttons[i].Top += Buffer2_Height;
                Label_List[i].Top += Buffer2_Height;
            }
        }

        private void open5_Click(object sender, EventArgs e)
        {
            Reset_Panel();
            open5.Visible = false;
            close5.Visible = true;
            bufferedPanel5.Left = Margin0;
            bufferedPanel5.Top = open5.Top + Data_Height;
            bufferedPanel5.Visible = true;
            label15.ForeColor = Highlight_Color;
            this.Height = Start_Size.Height + Buffer5_Height;

            // Move rest of the items down
            for (int i = 5; i < Open_Buttons.Count; i++)
            {
                Open_Buttons[i].Top += Buffer5_Height;
                Close_Buttons[i].Top += Buffer5_Height;
                Label_List[i].Top += Buffer5_Height;
            }
        }

        private void close3_Click(object sender, EventArgs e)
        {
            Reset_Panel();
            bufferedPanel3.Top = open3.Top + Data_Height;
        }

        private void close2_Click(object sender, EventArgs e)
        {
            Reset_Panel();
            bufferedPanel2.Top = open2.Top + Data_Height;
        }

        private void close1_Click(object sender, EventArgs e)
        {
            Reset_Panel();
            bufferedPanel1.Top = open1.Top + Data_Height;
        }

        private void close4_Click(object sender, EventArgs e)
        {
            Reset_Panel();
            bufferedPanel4.Top = open4.Top + Data_Height;
        }

        private void close5_Click(object sender, EventArgs e)
        {
            Reset_Panel();
            bufferedPanel5.Top = open5.Top + Data_Height;
        }

        private void deposit_Click(object sender, EventArgs e)
        {
            if (depositAMT.Text.Length > 1)
            {
                Create_Payment_Options("Deposit", Convert.ToDouble(depositAMT.Text.Substring(1)), dtpDeposit.Enabled ? dtpDeposit.Value : DateTime.Now, depositMemo.Text);

                Payment Reference_Pay = parent.Payment_List.FirstOrDefault(x => x.Bank == Ref_Payment.Bank &&
                    x.Last_Four == Ref_Payment.Last_Four &&
                    x.Company == Ref_Payment.Company);

                Reference_Pay.Balance += Convert.ToDouble(depositAMT.Text.Substring(1));
                if (depositAR.Text.Length > 4) parent.Account_List.FirstOrDefault(x => x == Temp_Acc).Status = 0;

                depositAMT.Text = "";
                depositMemo.Text = "";
                depositAR.Text = "";
                payAR.Text = "";
                close1.PerformClick();
                Populate_Combo_Boxes();
            }
        }

        private void withdraw_Click(object sender, EventArgs e)
        {
            if (withdrawAMT.Text.Length > 1)
            {
                Payment Reference_Pay = parent.Payment_List.FirstOrDefault(x => x.Bank == Ref_Payment.Bank &&
                    x.Last_Four == Ref_Payment.Last_Four &&
                    x.Company == Ref_Payment.Company);

                if (Reference_Pay.Balance >= Convert.ToDouble(withdrawAMT.Text.Substring(1)))
                {
                    Create_Payment_Options("Withdrawal", Convert.ToDouble(withdrawAMT.Text.Substring(1)), dtpWithdraw.Enabled ? dtpWithdraw.Value : DateTime.Now, withdrawMemo.Text);
                    Reference_Pay.Balance -= Convert.ToDouble(withdrawAMT.Text.Substring(1));
                }
                else
                {
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Insufficient funds to withdraw", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                }

                withdrawAMT.Text = "";
                withdrawMemo.Text = "";
                depositAR.Text = "";
                payAR.Text = "";
                close2.PerformClick();
                Populate_Combo_Boxes();
            }
        }

        private void pay_Click(object sender, EventArgs e)
        {
            if (payAMT.Text.Length > 1)
            {
                Payment Reference_Pay = parent.Payment_List.FirstOrDefault(x => x.Bank == Ref_Payment.Bank &&
                    x.Last_Four == Ref_Payment.Last_Four &&
                    x.Company == Ref_Payment.Company);

                if (Reference_Pay.Balance >= Convert.ToDouble(payAMT.Text.Substring(1)))
                {
                    Create_Payment_Options("Payment", Convert.ToDouble(payAMT.Text.Substring(1)), dtpPay.Enabled ? dtpPay.Value : DateTime.Now, payMemo.Text);
                    Reference_Pay.Balance -= Convert.ToDouble(payAMT.Text.Substring(1));
                    if (payAR.Text.Length > 4) parent.Account_List.FirstOrDefault(x => x == Temp_Acc).Status = 0;
                }
                else
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Insufficient funds to withdraw", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }

                payAMT.Text = "";
                payMemo.Text = "";
                depositAR.Text = "";
                payAR.Text = "";
                close3.PerformClick();
                Populate_Combo_Boxes();
            }
        }

        private void dateDeposit_Click(object sender, EventArgs e)
        {
            dtpDeposit.Enabled = !dtpDeposit.Enabled;
        }

        private void dateWithdraw_Click(object sender, EventArgs e)
        {
            dtpWithdraw.Enabled = !dtpWithdraw.Enabled;
        }

        private void datePay_Click(object sender, EventArgs e)
        {
            dtpPay.Enabled = !dtpPay.Enabled;
        }

        private void payAMT_TextChanged(object sender, EventArgs e)
        {
            if (!(payAMT.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(payAMT.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = payAMT.Text;
                    payAMT.Text = temp.Substring(1) + temp[0];
                    payAMT.SelectionStart = payAMT.Text.Length;
                    payAMT.SelectionLength = 0;
                }
                else
                {
                    payAMT.Text = "$" + payAMT.Text;
                }
            }
            else if ((payAMT.Text.Length > 1) && ((parent.Get_Char_Count(payAMT.Text, Convert.ToChar(".")) > 1) || (payAMT.Text[1].ToString() == ".") || (parent.Get_Char_Count(payAMT.Text, Convert.ToChar("$")) > 1) || (!((payAMT.Text.Substring(payAMT.Text.Length - 1).All(char.IsDigit))) && !(payAMT.Text[payAMT.Text.Length - 1].ToString() == "."))))
            {
                payAMT.TextChanged -= new System.EventHandler(payAMT_TextChanged);
                payAMT.Text = payAMT.Text.Substring(0, payAMT.Text.Length - 1);
                payAMT.SelectionStart = payAMT.Text.Length;
                payAMT.SelectionLength = 0;
                payAMT.TextChanged += new System.EventHandler(payAMT_TextChanged);
            }
        }

        private void withdrawAMT_TextChanged(object sender, EventArgs e)
        {
            if (!(withdrawAMT.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(withdrawAMT.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = withdrawAMT.Text;
                    withdrawAMT.Text = temp.Substring(1) + temp[0];
                    withdrawAMT.SelectionStart = withdrawAMT.Text.Length;
                    withdrawAMT.SelectionLength = 0;
                }
                else
                {
                    withdrawAMT.Text = "$" + withdrawAMT.Text;
                }
            }
            else if ((withdrawAMT.Text.Length > 1) && ((parent.Get_Char_Count(withdrawAMT.Text, Convert.ToChar(".")) > 1) || (withdrawAMT.Text[1].ToString() == ".") || (parent.Get_Char_Count(withdrawAMT.Text, Convert.ToChar("$")) > 1) || (!((withdrawAMT.Text.Substring(withdrawAMT.Text.Length - 1).All(char.IsDigit))) && !(withdrawAMT.Text[withdrawAMT.Text.Length - 1].ToString() == "."))))
            {
                withdrawAMT.TextChanged -= new System.EventHandler(withdrawAMT_TextChanged);
                withdrawAMT.Text = withdrawAMT.Text.Substring(0, withdrawAMT.Text.Length - 1);
                withdrawAMT.SelectionStart = withdrawAMT.Text.Length;
                withdrawAMT.SelectionLength = 0;
                withdrawAMT.TextChanged += new System.EventHandler(withdrawAMT_TextChanged);
            }
        }

        private void depositAMT_TextChanged(object sender, EventArgs e)
        {
            if (!(depositAMT.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(depositAMT.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = depositAMT.Text;
                    depositAMT.Text = temp.Substring(1) + temp[0];
                    depositAMT.SelectionStart = depositAMT.Text.Length;
                    depositAMT.SelectionLength = 0;
                }
                else
                {
                    depositAMT.Text = "$" + depositAMT.Text;
                }
            }
            else if ((depositAMT.Text.Length > 1) && ((parent.Get_Char_Count(depositAMT.Text, Convert.ToChar(".")) > 1) || (depositAMT.Text[1].ToString() == ".") || (parent.Get_Char_Count(depositAMT.Text, Convert.ToChar("$")) > 1) || (!((depositAMT.Text.Substring(depositAMT.Text.Length - 1).All(char.IsDigit))) && !(depositAMT.Text[depositAMT.Text.Length - 1].ToString() == "."))))
            {
                depositAMT.TextChanged -= new System.EventHandler(depositAMT_TextChanged);
                depositAMT.Text = depositAMT.Text.Substring(0, depositAMT.Text.Length - 1);
                depositAMT.SelectionStart = depositAMT.Text.Length;
                depositAMT.SelectionLength = 0;
                depositAMT.TextChanged += new System.EventHandler(depositAMT_TextChanged);
            }
        }

        public void Create_Payment_Options(string Type_, double Amount_, DateTime Date_, string Note_)
        {
            parent.Payment_Options_List.Add(new Payment_Options() {  Type = Type_, 
                                            Amount = Amount_, 
                                            Date = Date_, 
                                            Note = Note_, 
                                            Hidden_Note = "",
                                            Ending_Balance = Ref_Payment.Balance + Amount_ * (Type_ == "Deposit" ? 1 : -1), // Adjust balance
                                            Payment_Bank = Ref_Payment.Bank, 
                                            Payment_Company = Ref_Payment.Company, 
                                            Payment_Last_Four = Ref_Payment.Last_Four 
                                         });
        }

        private void next_page_button_Click(object sender, EventArgs e)
        {
            if (Current_Page + 1 < Pages_Required)
            {
                Current_Page++;
                back_page_button.Visible = true;
                bufferedPanel4.Invalidate();
                if (Pages_Required == Current_Page + 1) next_page_button.Visible = false;
            }
        }

        private void back_page_button_Click(object sender, EventArgs e)
        {
            if (Current_Page >= 1)
            {
                Current_Page--;
                next_page_button.Visible = true;
                bufferedPanel4.Invalidate();
                if (0 == Current_Page) back_page_button.Visible = false;
            }
        }

        Account Temp_Acc = new Account();

        private void payment_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (payAR.Text.Length > 4)
            {
                payAMT.Enabled = false;
                Temp_Acc = Payable_List[payAR.Items.IndexOf(payAR.Text) - 1]; // -1 to ignore "none" entry
                payAMT.Text = Temp_Acc.Amount;
                payMemo.Text = Temp_Acc.Payer + " - " + Temp_Acc.Remark;
            }
            else
            {
                payAMT.Enabled = true;
                payAMT.Text = "";
            }
        }

        private void depositAR_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (depositAR.Text.Length > 4)
            {
                depositAMT.Enabled = false;
                Temp_Acc = Receivable_List[depositAR.Items.IndexOf(depositAR.Text) - 1];
                depositAMT.Text = Temp_Acc.Amount;
                depositMemo.Text = Temp_Acc.Payer + " - " + Temp_Acc.Remark;
            }
            else
            {
                depositAMT.Enabled = true;
                depositAMT.Text = "";
            }
        }

    }

    public class Payment_Options
    { 
        public string Type { get; set; } // Deposit, Withdraw, Payment
        public string Note { get; set; }
        public string Hidden_Note { get; set; }
        public double Amount { get; set; }
        public double Ending_Balance { get; set; }
        public DateTime Date { get; set; }

        // Identifiers
        public string Payment_Last_Four { get; set; }
        public string Payment_Company { get; set; }
        public string Payment_Bank { get; set; }

        public Payment_Options Clone_PO()
        {
            return System.MemberwiseClone.Copy(this);
        }
    }
}

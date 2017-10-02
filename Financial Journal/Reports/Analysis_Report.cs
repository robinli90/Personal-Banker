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
    public partial class Analysis_Report : Form
    {

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            int data_height = 26;
            int start_height = 30;
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
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(122, 122, 122));
            SolidBrush BlueBrush = new SolidBrush(Color.LightBlue);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            SolidBrush PurpleBrush = new SolidBrush(Color.MediumPurple);
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush OrangeBrush = new SolidBrush(Color.Orange);
            SolidBrush LightOrangeBrush = new SolidBrush(Color.FromArgb(255, 200, 0));

            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Blue_Pen = new Pen(BlueBrush, 1);
            Pen Green_Pen = new Pen(GreenBrush, 1);
            Pen Red_Pen = new Pen(RedBrush, 1);
            Pen Orange_Pen = new Pen(OrangeBrush, 1);
            Pen Purple_Pen = new Pen(PurpleBrush, 1);

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_italic = new Font("MS Reference Sans Serif", 9, FontStyle.Italic);
            


            #region Top 3 Expenditures

            List<Item> Current_Item_List = new List<Item>();
            Current_Item_List = parent.Master_Item_List.Where(px => px.Date.Month == DateTime.Now.Month && px.Date.Year == DateTime.Now.Year).ToList();

            double Total_Monthly_Spending = 0;

            Dictionary<string, double> Category_Values = new Dictionary<string, double>();
            foreach (Item item in Current_Item_List)
            {
                double price = (item.Price * (1 + parent.Get_Tax_Amount(item)) - item.Discount_Amt / item.Quantity) * (item.Quantity - Convert.ToInt32(item.Status));

                if (Category_Values.ContainsKey(item.Category))
                {
                    Category_Values[item.Category] += price;
                    Total_Monthly_Spending += price;
                }
                else
                {
                    Category_Values.Add(item.Category, price);
                    Total_Monthly_Spending += price;
                }
            }

            e.Graphics.DrawString("Your top 3 expenditures this month are:", f, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
            KeyValuePair<string, double> Top_Value = Category_Values.FirstOrDefault(x => x.Value == Category_Values.Values.Max());
            e.Graphics.DrawString("  1) " + Top_Value.Key + " = $" + String.Format("{0:0.00}", Top_Value.Value) + " (" + Math.Round(Top_Value.Value / Total_Monthly_Spending * 100, 2) + "%)", f, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
            Category_Values.Remove(Top_Value.Key);
            row_count++;
            Top_Value = Category_Values.FirstOrDefault(x => x.Value == Category_Values.Values.Max());
            e.Graphics.DrawString("  2) " + Top_Value.Key + " = $" + String.Format("{0:0.00}", Top_Value.Value) + " (" + Math.Round(Top_Value.Value / Total_Monthly_Spending * 100, 2) + "%)", f, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
            Category_Values.Remove(Top_Value.Key);
            row_count++;
            Top_Value = Category_Values.FirstOrDefault(x => x.Value == Category_Values.Values.Max());
            e.Graphics.DrawString("  3) " + Top_Value.Key + " = $" + String.Format("{0:0.00}", Top_Value.Value) + " (" + Math.Round(Top_Value.Value / Total_Monthly_Spending * 100, 2) + "%)", f, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
            Category_Values.Remove(Top_Value.Key);
            #endregion

            row_count++;
            e.Graphics.DrawLine(Grey_Pen, start_margin - 5, start_height + height_offset + (row_count * data_height) - 5, Start_Size.Width - 10, start_height + height_offset + (row_count * data_height) - 5);

            #region Top 3 locations/destinations

            List<Order> Current_Order_List = new List<Order>();
            Current_Order_List = parent.Order_List.Where(px => px.Date.Month == DateTime.Now.Month && px.Date.Year == DateTime.Now.Year).ToList();
            Dictionary<string, double> Location_Count = new Dictionary<string, double>();
            int Total_Orders = Current_Order_List.Count;

            foreach (Order o in Current_Order_List)
            {
                if (Location_Count.ContainsKey(o.Location))
                {
                    Location_Count[o.Location]++;
                }
                else
                {
                    Location_Count.Add(o.Location, 1);
                }
            }

            e.Graphics.DrawString("Your top 3 destinations this month are:", f, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
            Top_Value = Location_Count.FirstOrDefault(x => x.Value == Location_Count.Values.Max());
            e.Graphics.DrawString("  1) " + Top_Value.Key + " @ " + Top_Value.Value + " visits this month (" + Math.Round(Top_Value.Value / Total_Orders * 100, 2) + "%)", f, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
            Location_Count.Remove(Top_Value.Key);
            row_count++;
            Top_Value = Location_Count.FirstOrDefault(x => x.Value == Location_Count.Values.Max());
            e.Graphics.DrawString("  2) " + Top_Value.Key + " @ " + Top_Value.Value + " visits this month (" + Math.Round(Top_Value.Value / Total_Orders * 100, 2) + "%)", f, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
            Location_Count.Remove(Top_Value.Key);
            row_count++;
            Top_Value = Location_Count.FirstOrDefault(x => x.Value == Location_Count.Values.Max());
            e.Graphics.DrawString("  3) " + Top_Value.Key + " @ " + Top_Value.Value + " visits this month (" + Math.Round(Top_Value.Value / Total_Orders * 100, 2) + "%)", f, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
            Location_Count.Remove(Top_Value.Key);

            #endregion

            row_count++;
            e.Graphics.DrawLine(Grey_Pen, start_margin - 5, start_height + height_offset + (row_count * data_height) - 5, Start_Size.Width - 10, start_height + height_offset + (row_count * data_height) - 5);

            #region Refund ratio

            double refund_count = parent.Master_Item_List.Count(x => x.Status != "0");
            e.Graphics.DrawString("You refund " + Math.Round(refund_count / parent.Master_Item_List.Count * 100, 2) + "% of your purchases", f, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));

            #endregion

            row_count++;
            e.Graphics.DrawLine(Grey_Pen, start_margin - 5, start_height + height_offset + (row_count * data_height) - 5, Start_Size.Width - 10, start_height + height_offset + (row_count * data_height) - 5);

            #region Credit Card warning

            List<string> Credit_Card_Names = new List<string>();
            parent.Payment_List.Where(x => x.Payment_Type == "Credit Card").ToList().ForEach(x => Credit_Card_Names.Add(x.Company + " (xx-" + x.Last_Four + ")"));

            List<Item> Credit_Card_Items = parent.Master_Item_List.Where(x => Credit_Card_Names.Contains(x.Payment_Type)).ToList();
            double credit_total = Credit_Card_Items.Sum(x => x.Price * x.Quantity - Convert.ToInt32(x.Status) * x.Price);
            e.Graphics.DrawString("You use a credit card for " + Math.Round((double)Credit_Card_Items.Count / parent.Master_Item_List.Count * 100, 2) + "% of your purchases totalling $" + String.Format("{0:0.00}", credit_total) + " (exc. tax). Using a card with", f, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
            height_offset += 16;
            e.Graphics.DrawString("1% cash back will result in an extra $" + String.Format("{0:0.00}", credit_total/100), f, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));

            #endregion

            //row_count++;
            //e.Graphics.DrawLine(Grey_Pen, start_margin - 5, start_height + height_offset + (row_count * data_height) - 5, Start_Size.Width - 10, start_height + height_offset + (row_count * data_height) - 5);

            #region Resource Disposal

            p.Dispose();
            Grey_Pen.Dispose();
            GreyBrush.Dispose();
            BlueBrush.Dispose();
            RedBrush.Dispose();
            GreenBrush.Dispose();
            PurpleBrush.Dispose();
            OrangeBrush.Dispose();
            LightOrangeBrush.Dispose();
            Blue_Pen.Dispose();
            Green_Pen.Dispose();
            Red_Pen.Dispose();
            Purple_Pen.Dispose();
            Orange_Pen.Dispose();
            WritingBrush.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);

            #endregion
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Analysis_Report(Receipt _parent)
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.Invalidate();
            this.Update();
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
    }
}

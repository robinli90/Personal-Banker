using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace Financial_Journal
{
    public partial class Category_Summary : Form
    {
        private Button Ref_Sort_Button = new Button();

        private void sort_item_name_button_Click(object sender, EventArgs e)
        {
            Ref_Sort_Button = (Button)sender;
            Invalidate();
            Update();
        }

        private void sort_item_location_button_Click(object sender, EventArgs e)
        {
            Ref_Sort_Button = (Button)sender;
            Invalidate();
            Update();
        }

        private void sort_item_date_button_Click(object sender, EventArgs e)
        {
            Ref_Sort_Button = (Button)sender;
            Invalidate();
            Update();
        }

        private void sort_item_price_button_Click(object sender, EventArgs e)
        {
            Ref_Sort_Button = (Button)sender;
            Invalidate();
            Update();
        }

        private int Entries_Per_Page = 20;
        int Pages_Required = 0;
        int Current_Page = 0;


        protected override void OnPaint(PaintEventArgs e)
        {
            int data_height = 19;
            int start_height = 30;
            int start_margin = 15;
            int height_offset = 9;

            int margin1 = start_margin + 240;   //Location
            int margin2 = margin1 + 150;        //Date
            int margin3 = margin2 + 140;        //Quantity
            int margin4 = margin3 + 80;         //Unit Price
            int margin5 = margin4 + 90;         //Total Amount
            int margin6 = margin5 + 92;         //Payment

            int row_count = 0;

            Color DrawForeColor = Color.White;
            Color BackColor = Color.FromArgb(64, 64, 64);
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(180, 180, 180));
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 2);

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);


            // If has order (always true)
            if (true)
            {

                // Header
                e.Graphics.DrawString("Item", f_header, WritingBrush, start_margin + 15, start_height + (row_count * data_height));
                e.Graphics.DrawString("Location", f_header, WritingBrush, margin1, start_height + (row_count * data_height));
                e.Graphics.DrawString("Date", f_header, WritingBrush, margin2, start_height + (row_count * data_height));
                e.Graphics.DrawString("Unit Price", f_header, WritingBrush, margin3, start_height + (row_count * data_height));
                e.Graphics.DrawString("Quantity", f_header, WritingBrush, margin4, start_height + (row_count * data_height));
                row_count += 1;

                int item_index = 0;
                bool has_exempt = false;
                bool has_overwritten = false;
                double Ongoing_Total = 0;
                double Total_Tax = 0;

                List<Item_View> Ref_List = Item_List;

                // Sort ascending
                if (Ref_Sort_Button.Name.StartsWith("2"))
                {
                    Ref_Sort_Button.Name = Ref_Sort_Button.Name.Substring(1);
                    if (Ref_Sort_Button.Name.Contains("name")) Ref_List = Item_List.OrderByDescending(x => x.Name).ToList();
                    else if (Ref_Sort_Button.Name.Contains("location")) Ref_List = Item_List.OrderByDescending(x => x.Location).ToList();
                    else if (Ref_Sort_Button.Name.Contains("date")) Ref_List = Item_List.OrderByDescending(x => x.Date).ToList();
                    else if (Ref_Sort_Button.Name.Contains("price")) Ref_List = Item_List.OrderByDescending(x => x.Price).ToList();
                }
                else // sort descending
                {
                    Ref_Sort_Button.Name = "2" + Ref_Sort_Button.Name;
                    if (Ref_Sort_Button.Name.Contains("name")) Ref_List = Item_List.OrderBy(x => x.Name).ToList();
                    else if (Ref_Sort_Button.Name.Contains("location")) Ref_List = Item_List.OrderBy(x => x.Location).ToList();
                    else if (Ref_Sort_Button.Name.Contains("date")) Ref_List = Item_List.OrderBy(x => x.Date).ToList();
                    else if (Ref_Sort_Button.Name.Contains("price")) Ref_List = Item_List.OrderBy(x => x.Price).ToList();
                }

                // Get all prices first
                foreach (Item_View item in Ref_List)
                {
                    double Total_Price = (item.Quantity - Convert.ToDouble(item.Status)) * (item.Price) * Get_Tax_Rate(item.Category) - (item.Discount_Amt / Convert.ToInt32(item.Quantity)) * (item.Quantity);
                    Total_Tax += (item.Quantity - Convert.ToDouble(item.Status)) * (item.Price) * (Get_Tax_Rate(item.Category) - 1);
                    Ongoing_Total += Total_Price;
                }

                // For each refund item
                foreach (Item_View item in Ref_List.GetRange(Current_Page * Entries_Per_Page, (Ref_List.Count - Entries_Per_Page * Current_Page) >= Entries_Per_Page ? Entries_Per_Page : (Ref_List.Count % Entries_Per_Page)))
                {
                    if (item_index <= Entries_Per_Page)
                    {
                        ToolTip ToolTip1 = new ToolTip();
                        ToolTip1.InitialDelay = 1;
                        ToolTip1.ReshowDelay = 1;

                        // Find corresponding order based on orderID

                        e.Graphics.DrawString(item.Name, f, item_index % 2 == 0 ? WritingBrush : GreyBrush, start_margin + 6, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString(item.Location, f, item_index % 2 == 0 ? WritingBrush : GreyBrush, margin1, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString(item.Date.ToShortDateString(), f, item_index % 2 == 0 ? WritingBrush : GreyBrush, margin2 - 6, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString("$" + String.Format("{0:0.00}", item.Price), f, item_index % 2 == 0 ? WritingBrush : GreyBrush, margin3 + 7, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString((item.Quantity - Convert.ToDouble(item.Status)).ToString(), f, item_index % 2 == 0 ? WritingBrush : GreyBrush, margin4 + 24, start_height + height_offset + (row_count * data_height));
                        
                        row_count++;

                        item_index++;
                    }
                }

                height_offset += 4; 
                e.Graphics.DrawLine(p, margin3 - 50, start_height + height_offset + (row_count * data_height), margin4 - 15, start_height + height_offset + (row_count * data_height));
                height_offset += 4;
                e.Graphics.DrawString("Subtotal", f, WritingBrush, margin3 - 50, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Ongoing_Total-Total_Tax), f, WritingBrush, margin3 + 7, start_height + height_offset + (row_count * data_height));
                row_count++;
                e.Graphics.DrawString("Taxes", f, WritingBrush, margin3 - 50, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Total_Tax), f, WritingBrush, margin3 + 7, start_height + height_offset + (row_count * data_height));
                row_count++;

                // Total line
                height_offset += 4;
                e.Graphics.DrawLine(p, margin3 - 50, start_height + height_offset + (row_count * data_height), margin4 - 15, start_height + height_offset + (row_count * data_height));
                height_offset += 4;
                e.Graphics.DrawString("Total", f_total, WritingBrush, margin3 - 50, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Ongoing_Total), f_total, GreenBrush, margin3 + 7, start_height + height_offset + (row_count * data_height));

                // Draw accounting double lines
                height_offset += 19;
                e.Graphics.DrawLine(p, margin3 - 50, start_height + height_offset + (row_count * data_height), margin4 - 15, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawLine(p, margin3 - 50, start_height + height_offset + 2 + (row_count * data_height), margin4 - 15, start_height + height_offset + 2 + (row_count * data_height));
                height_offset -= 17;

                row_count++;
                if (has_exempt) e.Graphics.DrawString("*Tax Exempt" + (has_overwritten ? "           **Tax amount has been overrided" : ""), f_asterisk, RedBrush, margin4 + 4, start_height + height_offset + 4 + (row_count * data_height));
                row_count++;
                this.Height = start_height + height_offset + row_count * data_height;
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
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        Size Start_Size = new Size();
        List<Item_View> Item_List = new List<Item_View>();
        Dictionary<string, string> Tax_Dict = new Dictionary<string,string>();
        bool exit_on_leave = false;

        public Category_Summary(string form_title, Point start_location, string info_string, bool exit_on_leave_form = false, Point g = new Point(), Size s = new Size())
        {
            this.Location = start_location;
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            Start_Size = this.Size;
            this.label5.Text = form_title;
            string[] item_line = info_string.Split(new string[] { "||" }, StringSplitOptions.None);
            exit_on_leave = exit_on_leave_form;

            if (start_location == new Point())
            {
                this.Location = new Point(g.X + s.Width / 2 - this.Width / 2, g.Y + s.Height / 3 - this.Height / 2);
                minimize_button.Visible = false;
            }
            else
            {
                    this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            }

            foreach (string line in item_line)
            {
                if (line.Contains("ITEM_"))
                {
                    Item_View New_Item = new Item_View();
                    New_Item.Name = Parse_Line_Information(line, "ITEM_DESC");
                    New_Item.Status = Parse_Line_Information(line, "ITEM_STATUS") == "" ? "0" : Parse_Line_Information(line, "ITEM_STATUS");
                    New_Item.Location = Parse_Line_Information(line, "ITEM_LOCATION");
                    New_Item.Category = Parse_Line_Information(line, "ITEM_CATEGORY");
                    New_Item.Discount_Amt = Parse_Line_Information(line, "ITEM_DISCOUNT_AMT") == "" ? 0 : Convert.ToDouble(Parse_Line_Information(line, "ITEM_DISCOUNT_AMT"));
                    New_Item.Price = Convert.ToDouble(Parse_Line_Information(line, "ITEM_PRICE"));
                    New_Item.Quantity = Convert.ToInt32(Parse_Line_Information(line, "ITEM_QUANTITY"));
                    New_Item.Date = Convert.ToDateTime(Parse_Line_Information(line, "ITEM_DATE")); 
                    Item_List.Add(New_Item);
                }
                else if (line.Contains("APP_COLOR"))
                {
                    Set_Form_Color(System.Drawing.ColorTranslator.FromHtml(Parse_Line_Information(line, "APP_COLOR")));
                }
                else if (line.Contains("TAX_RATE"))
                {
                    string[] tax_lines = line.Split(new string[] { "|" }, StringSplitOptions.None);
                    foreach (string tax_line in tax_lines)
                    {
                        Tax_Dict.Add(Parse_Line_Information(tax_line, "TAX_RULE", ","), Parse_Line_Information(tax_line, "TAX_RATE", ","));
                    }
                }

            }

            Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Item_List.Count() / (decimal)Entries_Per_Page));
            next_page_button.Visible = Pages_Required > 1;
        }

        private double Get_Tax_Rate(string Category)
        {
            return 1 + (Tax_Dict.ContainsKey(Category) ? Convert.ToDouble(Tax_Dict[Category]) : Convert.ToDouble(Tax_Dict["DEFAULT_RATE123"]));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.Invalidate();
            this.Update();

            if (exit_on_leave)
            {
                this.MouseLeave += new EventHandler(form_MouseLeave);
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

        private void form_MouseLeave(object sender, EventArgs e)
        {
            if (this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition)))
                return;
            else
            {
                this.Close();
                this.Dispose();
            }
        }

        public void Set_Form_Color(Color randomColor)
        {
            //minimize_button.ForeColor = randomColor;
            //close_button.ForeColor = randomColor;
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor;
        }


        /// <summary>
        /// Return the output line after [output].
        /// 
        /// For example, in line = [INFO_TYPE]=ITEM||[ITEM_NAME]=CLOTHING||[ITEM_PRICE]=49.22||....
        /// 
        ///     Calling this program:
        ///     
        ///     Parse_Line_Information(line, "ITEM_PRICE", parse_token = "||") returns "49.22"
        ///     
        /// </summary>
        private string Parse_Line_Information(string input, string output, string parse_token = "|")
        {
            string[] Split_Layer_1 = input.Split(new string[] { parse_token }, StringSplitOptions.None);

            foreach (string Info_Pair in Split_Layer_1)
            {
                if (Info_Pair.Contains("[" + output + "]"))
                {
                    return Info_Pair.Split(new string[] { "=" }, StringSplitOptions.None)[1];
                }
            }

            return "";
        }

        private void next_page_button_Click(object sender, EventArgs e)
        {
            if (Current_Page + 1 < Pages_Required) 
            {
                Current_Page++;
                back_page_button.Visible = true;
                this.Invalidate();
                this.Update();
                if (Pages_Required == Current_Page + 1) next_page_button.Visible = false;
            }
        }

        private void back_page_button_Click(object sender, EventArgs e)
        {
            if (Current_Page >= 1)
            {
                Current_Page--;
                next_page_button.Visible = true;
                this.Invalidate();
                this.Update();
                if (0 == Current_Page) back_page_button.Visible = false;
            }
        }

    }

    public class Item_View
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string OrderID { get; set; }
        public string Payment_Type { get; set; }
        public string Memo { get; set; }
        public double Price { get; set; }
        public double Discount_Amt { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public DateTime Refund_Date { get; set; }

        public Item_View()
        {

        }
    }
}

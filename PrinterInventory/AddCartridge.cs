using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrinterInventory
{
    public partial class AddCartridge : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.RefreshMain();
            parent.Grey_In();
            base.OnFormClosing(e);
            
        }

        Main parent;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public AddCartridge(Main _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
            parent.Grey_Out();
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            // Populate brand drop down
            foreach (string brand in parent.printerBrandList)
            {
                brandBox.Items.Add(brand);
            }

            // Populate quantity
            for (int i = 0; i < 11; i++)
            {
                quantityBox.Items.Add(i);
            }

            brandBox.KeyPress += keypressTabNext;
            modelBox.KeyPress += keypressTabNext;
            quantityBox.KeyPress += keypressTabNext;
            priceBox.KeyPress += keypressTabNext;
            memoBox.KeyPress += keypressTabNext;
            requisitionerBox.KeyPress += keypressTabNext;

            brandBox.SelectedIndex = 0;
            quantityBox.SelectedIndex = 1;

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

        // Form mnemonics
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                {
                    addCartridgeButton.PerformClick();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
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

        // If Enter, tab
        private void keypressTabNext(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        public void Set_Form_Color(Color randomColor)
        {
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void priceBox_TextChanged(object sender, EventArgs e)
        {
            TextBox Ref_Box = (TextBox)sender;

            if (!(Ref_Box.Text.StartsWith("$")))
            {
                if (Get_Char_Count(Ref_Box.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = Ref_Box.Text;
                    Ref_Box.Text = temp.Substring(1) + temp[0];
                    Ref_Box.SelectionStart = Ref_Box.Text.Length;
                    Ref_Box.SelectionLength = 0;
                }
                else
                {
                    Ref_Box.Text = "$" + Ref_Box.Text;
                }
            }
            else if ((Ref_Box.Text.Length > 1) && ((Get_Char_Count(Ref_Box.Text, Convert.ToChar(".")) > 1) || (Ref_Box.Text[1].ToString() == ".") || (Get_Char_Count(Ref_Box.Text, Convert.ToChar("$")) > 1) || (!((Ref_Box.Text.Substring(Ref_Box.Text.Length - 1).All(char.IsDigit))) && !(Ref_Box.Text[Ref_Box.Text.Length - 1].ToString() == "."))))
            {
                Ref_Box.TextChanged -= new System.EventHandler(priceBox_TextChanged);
                Ref_Box.Text = Ref_Box.Text.Substring(0, Ref_Box.Text.Length - 1);
                Ref_Box.SelectionStart = Ref_Box.Text.Length;
                Ref_Box.SelectionLength = 0;
                Ref_Box.TextChanged += new System.EventHandler(priceBox_TextChanged);
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

        private void addCartridgeButton_Click(object sender, EventArgs e)
        {
            Grey_Out();
            if (modelBox.Text.Length > 0 && priceBox.Text.Length > 1 && memoBox.Text.Length > 0 &&
                requisitionerBox.Text.Length > 0)
            {
                if (!parent.CartridgeList.Any(x => x.Model.ToLower() == modelBox.Text.ToLower() &&
                                                   x.Brand.ToLower() == brandBox.Text.ToLower() &&
                                                   x.Memo.ToLower() == memoBox.Text.ToLower()))
                {
                    try
                    {
                        // Add a cartridge for each quantity
                        for (int i = 0; i < Convert.ToInt32(quantityBox.Text == "0" ? "1" : quantityBox.Text); i++)
                        {
                            parent.AddCartridge(new Cartridge()
                            {
                                Brand = brandBox.Text,
                                Model = modelBox.Text,
                                Memo = memoBox.Text,
                                Quantity = quantityBox.Text == "0" ? 0 : 1,
                                Price = Convert.ToDouble(priceBox.Text.Substring(1)),
                                ReceiveDate = DateTime.Now,
                                RemoveDate = new DateTime(),
                                RemoveMemo = "",
                                Requisitioner = requisitionerBox.Text,
                                InternalNote = "",
                                CartQuantity = 0
                            });
                        }
                        Close();
                    }
                    catch
                    {
                        Form_Message_Box FMB =
                            new Form_Message_Box(parent, "Error: Price is invalid", true, -20, Location, Size);
                        FMB.ShowDialog();
                        return;
                    }
                }
                else
                {
                    Form_Message_Box FMB =
                        new Form_Message_Box(parent, "Error: This cartridge already exists", true, -10, Location, Size);
                    FMB.ShowDialog();
                }
            }
            else
            {
                Form_Message_Box FMB = new Form_Message_Box(parent, "Missing a required field", true, -20, Location, Size);
                FMB.ShowDialog();
            }
            Grey_In();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Convert.ToInt32(quantityBox.Text); i++)
            {
                parent.AddCartridge(new Cartridge()
                {
                    Brand = brandBox.Text,
                    Model = parent.GetNewHashID(),
                    Memo = parent.GetNewHashID(),
                    Quantity = 1,
                    Price = 50,// Convert.ToDouble(priceBox.Text.Substring(1)),
                    ReceiveDate = DateTime.Now,
                    RemoveDate = new DateTime(),
                    RemoveMemo = "",
                    Requisitioner = parent.GetNewHashID(),
                    InternalNote = "",
                });
            }
        }
    }
}

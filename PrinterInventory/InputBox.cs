using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PrinterInventory
{
    public enum InputType
    {
        String,
        Numeric,
        Currency
    }

    public partial class InputBox : Form
    {

        Main parent;
        bool Allow_Close_ = true;
        bool requireAnswer = true;
        string LabelString = "";

        private InputType inputType;

        public string returnValue = "";

       // public Form_Message_Box(Receipt _parent, string Label_Text, bool Allow_Close = true, int grow_height = 0, Point g = new Point(), Size s = new Size())
        public InputBox(Main _parent, InputType _inputType, string Label_Text, bool Allow_Close, int grow_height, bool _requireAnswer = false, Point g = new Point(), Size s = new Size())
        {
            LabelString = Label_Text;
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            label1.Text = Label_Text;
            parent = _parent;
            Allow_Close_ = Allow_Close;
            this.Height += grow_height;
            parent.Grey_Out();
            requireAnswer = _requireAnswer;
            inputType = _inputType;
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));

            if (_inputType == InputType.Currency)
            {
                inputValueBox.Text = "$";
                inputValueBox.KeyPress += currencyHandler;
            }
        }

        private void currencyHandler(object sender, EventArgs e)
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
                Ref_Box.TextChanged -= new System.EventHandler(currencyHandler);
                Ref_Box.Text = Ref_Box.Text.Substring(0, Ref_Box.Text.Length - 1);
                Ref_Box.SelectionStart = Ref_Box.Text.Length;
                Ref_Box.SelectionLength = 0;
                Ref_Box.TextChanged += new System.EventHandler(currencyHandler);
            }
        }

        // Form mnemonics
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    {
                        yesbutton.PerformClick();
                        return true;
                    }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {

            if (!Allow_Close_) close_button.Visible = false;

            //inputValueBox.KeyPress += keypressTabNext;
        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        private void close_button_Click(object sender, EventArgs e)
        {
            Close_Form();
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

        public void Close_Form()
        {
            parent.Grey_In();
            Dispose();
            Close();
        }

        private void yesbutton_Click(object sender, EventArgs e)
        {
            bool responseValid = !requireAnswer;

            if (requireAnswer && inputValueBox.Text.Length > 0 && inputType == InputType.String)
            {
                responseValid = true;
            }
            else if (requireAnswer && inputValueBox.Text.Length > 1 &&
                                        inputType == InputType.Currency)
            {
                try
                {
                    Convert.ToDouble(inputValueBox.Text.Substring(1));
                    responseValid = true;
                }
                catch { }
            }

            if (responseValid)
            {
                returnValue = inputValueBox.Text;
                DialogResult = DialogResult.OK;
                Close_Form();
            }
        }

        private void inputValueBox_TextChanged(object sender, EventArgs e)
        {

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
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Opulos.Core.UI
{
    public partial class TimePickerPanel : UserControl
    {
        public TimePicker timePicker = new TimePicker(0, true, true);
        int numEvents = 0;

        public TimePickerPanel()
        {
            InitializeComponent();
            HFLP p = new HFLP(timePicker) { Padding = new Padding(1) };
            Controls.Add(p);
        }
    }

    internal class HFLP : FlowLayoutPanel
    {
        public HFLP(params Control[] controls)
        {
            FlowDirection = FlowDirection.LeftToRight;
            WrapContents = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            foreach (Control c in controls)
                Controls.Add(c);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size s = Size.Empty;
            foreach (Control c in Controls)
            {
                Size ps = c.PreferredSize;
                Padding m = c.Margin;
                s.Width += ps.Width + m.Horizontal;
                int h = ps.Height + m.Vertical;
                if (h > s.Height)
                    s.Height = h;
            }
            return s;
        }
    }
}

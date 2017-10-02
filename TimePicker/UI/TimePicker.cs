using System;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;

namespace Opulos.Core.UI {

public class TimePicker : MaskedTextBox<DateTime> {

	private String dateTimeFormat = null;

	public ClockControl ClockMenu = null;
	public ToolStripDropDownAttacher attacher = null;

	///<summary>
	///<para>Creates a time picker with the specified number of millisecond digits. The base format
	///will be HH:mm:ss when the use24HourClock parameter is true, otherwise the base format
	///will be hh:mm:ss. The milliseconds are appended at the end, e.g. HH:mm:ss.ffff
	///</para>
	///<para>Call the MimicDateTimePicker() to mimic the same style of input.</para>
	///<para>Use the Value property to access the current DateTime value.</para>
	///<para>Use the ValueChanged event to listen for DateTime changes.</para>
	///<para>Use the TimeFormat and Mask in conjuction to change the format.</para>
	///<para>The Arrays MinValues, MaxValues, PageUpDownDeltas and ByValues must
	///contain at least the number of tokens as the TimeFormat contains.</para>
	///</summary>
	///<param name="addUpDownSpinner">An option to display up-down buttons.</param>
	///<param name="immediateValueChange">An option if changing the clock menu value requires clicking the OK button, or if the change is instant.</param>
	///<param name="numMilliDigits">The number of milliseconds to show.</param>
	///<param name="showClockMenu">An option to show a clock menu when this control gets the focus or is clicked.</param>
	///<param name="use24HourClock">An option to show hours 0 to 23 or 1 to 12.</param>
	public TimePicker(int numMilliDigits = 3, bool use24HourClock = true, bool addUpDownSpinner = true,
                      bool showClockMenu = true, bool immediateValueChange = true) : base(addUpDownSpinner)
    {
        this.BackColor = Color.Black;

		//Using 9s produces more natural usability than 0s.
		String mask = "99:99:99";
		String timeFormat = (use24HourClock ? "HH:mm:ss" : "hh:mm:ss");
		int maxValue = 1;
		if (numMilliDigits > 0) {
			mask += "." + new String('9', numMilliDigits);
			timeFormat += "." + new String('f', numMilliDigits);
			for (int i = 0; i < numMilliDigits; i++)
				maxValue *= 10;
		}
		dateTimeFormat = timeFormat;
		this.Mask = mask;
		if (use24HourClock) {
			Tokens[0].MaxValue = 24;
		}
		else {
			Tokens[0].MinValue = 1;
			Tokens[0].MaxValue = 13;
		}
		Tokens[2].MaxValue = 60;
		Tokens[4].MaxValue = 60;
		//--
		Tokens[0].BigIncrement = 4;
		Tokens[2].BigIncrement = 10;
		Tokens[4].BigIncrement = 10;
		//--

		MimicDateTimePicker();
		Value = DateTime.Now;

		ClockMenu = new ClockControl(); //false, false, false);
		if (showClockMenu)
			attacher = new ToolStripDropDownAttacher(ClockMenu, this);

		ClockMenu.ButtonClicked += ClockMenu_ButtonClicked;

		this.ValueChanged += delegate {
			ClockMenu.Value = this.Value;
		};

		DateTime origValue = DateTime.MinValue;
		ClockMenu.VisibleChanged += delegate {
			if (ClockMenu.Visible) {
				origValue = this.Value;
				ClockMenu.Value = this.Value;
			}
		};

		ClockMenu.Closed += (o, e) => {
			// escape key is used
			if (e.CloseReason == ToolStripDropDownCloseReason.Keyboard) {
				this.Value = origValue;
			}
			else {
				this.Value = ClockMenu.Value;
			}
		};

        /*
		ClockMenu.ValueChanged += delegate {
			if (immediateValueChange)
				this.Value = ClockMenu.Value;
		}; 
        */
        
		attacher.MenuShowing += delegate {
			Token t = this.TokenAt(this.SelectionStart);
			if (t.SeqNo == 0)
				ClockMenu.ClockFace = ClockFace.Hours;
			else if (t.SeqNo == 2)
				ClockMenu.ClockFace = ClockFace.Minutes;
		};
	}

	void ClockMenu_ButtonClicked(object sender, EventArgs e) {
		// if Cancel button, then send Keyboard, which is used to indicate keyboard escape
		ToolStripDropDownCloseReason r = (sender == ClockMenu.ClockButtonOK ? ToolStripDropDownCloseReason.ItemClicked : ToolStripDropDownCloseReason.Keyboard);
		attacher.CloseMenu(r);
	}

	public String DateTimeFormat {
		get {
			return dateTimeFormat;
		}
		set {
			dateTimeFormat = value;
		}
	}

	public override DateTime TextToValue(String text) {
		DateTime d = DateTime.MinValue;
		DateTime.TryParseExact(text, dateTimeFormat, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out d);
		return d;
	}

	public override String ValueToText(DateTime value) {
		return value.ToString(dateTimeFormat);
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		if (disposing) {
			if (ClockMenu != null)
				ClockMenu.Dispose();

			ClockMenu = null;
		}
	}
}
}
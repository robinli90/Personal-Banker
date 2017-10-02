using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

using Opulos.Core.Localization;

namespace Opulos.Core.UI {

///<summary>A drop down user interface that displays an analogue clock.</summary>
public class ClockControl : ToolStripDropDown {

	///<summary>An event is fired after the value has changed.</summary>
	public event EventHandler ValueChanged = null;

	///<summary>An event is fired when either the OK button or Cancel button is clicked.</summary>
	public event EventHandler ButtonClicked = null;

	///<summary>The item that displays the currently selected hour at the top.
	///Clicking the item displays the hour clock face numbers.</summary>
	public ClockHeaderItem ClockItemHour = new ClockHeaderItem("12") { IsSelected = true, TextAlign = ContentAlignment.MiddleRight };

	///<summary>The item that separates the hour and minute items at the top.
	///Clicking the item has no effect.</summary>
	public ClockHeaderItem ClockItemColon = new ClockHeaderItem(DateTimeFormatInfo.CurrentInfo.TimeSeparator, false);

	///<summary>The item that displays the currently selected minute at the top.
	///Clicking the item displays the minute clock face numbers.</summary>
	public ClockHeaderItem ClockItemMinute = new ClockHeaderItem("59");

	///<summary>The item that displays the currently selected localized meridiem value (e.g. AM or PM) at the top.
	///Clicking the item toggles the current value.</summary>
	public ClockHeaderItem ClockItemAMPM = new ClockHeaderItem(DateTimeFormatInfo.CurrentInfo.AMDesignator) { TextAlign = ContentAlignment.MiddleRight };

	///<summary>The item that displays the localized meridiem 'AM' value at the bottom left of the clock.
	///Clicking the item changes the current value to AM.</summary>
	public ClockItem ClockItemAM = new ClockItem(DateTimeFormatInfo.CurrentInfo.AMDesignator);

	///<summary>The item that displays the localized merdiem 'PM' value at the bottom right of the clock.
	///Clicking the item changes the current value to PM.</summary>
	public ClockItem ClockItemPM = new ClockItem(DateTimeFormatInfo.CurrentInfo.PMDesignator);

	///<summary>An OK button that appears at the bottom of the control. By default,
	///no behavior is assigned to the button.</summary>
	public ClockButton ClockButtonOK = new ClockButton(Strings.OK);

	///<summary>A Cancel button that appears at the bottom of the control. By default,
	///no behavior is assigned to the button.</summary>
	public ClockButton ClockButtonCancel = new ClockButton(Strings.Cancel);

	///<summary>A flag that indicates if user is currently selecting a number clicking and dragging the mouse.</summary>
	public bool IsDragging { get; private set; }

	///<summary>A parameter that controls the diameter of the clock face. The default value is 1.586. If the
	///maximum diameter of all the hour circles and minute circles is d, where d is controlled by the font size and text,
	///then the circumference of the clock face is 1.586 * d * 12, and dividing by PI gives the diameter of the clock face.
	///</summary>
	public double ClockFaceSizeFactor { get; set; }

	///<summary>An option to automatically display the minute face after the user has selected the hour value.
	///The default value is true.</summary>
	public bool ShowMinuteFaceAfterHourValueSelected { get; set; }

	///<summary>The size of the solid blue focus dot relative to the diameter of the clock number.
	///The default value is 0.3, which renders the blue focus dot slightly less than one-third the size.</summary>
	public double FocusDotFactor { get; set; }

	///<summary>Determines where the separator line above the buttons is rendered. The
	///line is drawn relative to the AM button. The default value is 1.2. The separator
	///line Y location is calculated by starting at the AM button top-left Y location,
	///and adding the height of the AM button multiplied by the gap factor 1.2.</summary>
	public double ButtonGapFactor { get; set; }

	///<summary>Gets the current center point of the clock face.</summary>
	public Point ClockFaceCenter { get; private set; }

	///<summary>Gets the current clock face diameter.</summary>
	public Size ClockFaceSize { get; private set; }

	///<summary>The distance from the bottom of the header to the top of the clock face.
	///The default value is 20.</summary>
	public int HeaderClockFaceGap { get; set; }

	///<summary>An option to allow the escape key to hide the drop down. The default value is true.
	///This value is used when AutoClose is false, otherwise the control won't hide.</summary>
	//public bool CloseOnEscapeKey { get; set; }

	///<summary>An option to turn off rendering the dotted focus rectangles.</summary>
	public new bool ShowFocusCues {
		get {
			return showFocusCues && !mouseHideFocusCues;
		}
		set {
			showFocusCues = value;
		}
	}

	///<summary>The horizontal position of the OK button. The default value is 0.0, which means the OK button is left aligned.</summary>
	public double ClockButtonOKAlignX = 0;

	///<summary>The horizontal position of the Cancel button. The default value is 1.0, which means the Cancel button is right aligned.</summary>
	public double ClockButtonCancelAlignX = 1;

	///<summary>The width of the OK button relative to the width of the clock control. The default value is 0.5, which means half the width.</summary>
	public double ClockButtonOKWidthFactor = 0.5;

	///<summary>The width of the Cancel button relative to the width of the clock control. The default value is 0.5, which means half the width.</summary>
	public double ClockButtonCancelWidthFactor = 0.5;

	// maybe make these public later if needed
	private double ClockItemAMAngleDegrees { get; set; }
	private double ClockItemPMAngleDegrees { get; set; }
	private double ClockItemAMDistanceFactor { get; set; }
	private double ClockItemPMDistanceFactor { get; set; }

	//---
	private Font font = null;
	internal List<ClockNumberItem> minuteItems = new List<ClockNumberItem>();
	internal List<ClockNumberItem> hourItems = new List<ClockNumberItem>();
	internal List<ClockHeaderItem> headerItems = new List<ClockHeaderItem>();
	private ClockLayoutEngine layoutEngine = new ClockLayoutEngine();
	private DateTime value = DateTime.MinValue;
	private ClockFace clockFace = ClockFace.Hours;
	private bool numberClicked = false;
	private int tabIndex = 0;
	private bool topMost = false;
	private bool valueSelected = false;
	//private bool allowClose = false;
	private bool mouseHideFocusCues = false;
	private bool showFocusCues = false;
	//private bool noActivate = true;
	//---

	///<summary>
	///Create a clock control that allows the user to select a time.
	///<param name="topMost">An option to keep this window topmost.</param>
	///<param name="showOKCancelButtons"></param>
	///<param name="showAMPMButtons"></param>
	///</summary>
	public ClockControl(bool topMost = false, bool showOKCancelButtons = true, bool showAMPMButtons = true) {//, bool noActivate = true) {
		this.topMost = topMost;
		//this.noActivate = noActivate;
		SuspendLayout();
		Padding = new Padding(20, 0, 20, 0);
		Margin = Padding.Empty;
		Dock = DockStyle.Fill;
		GripStyle = ToolStripGripStyle.Hidden;
		CanOverflow = false;
		TabStop = true; // allows key interaction
		showFocusCues = base.ShowFocusCues;
		//CloseOnEscapeKey = true;
        this.BackColor = Color.FromArgb(64, 64, 64);
        this.ForeColor = Color.White;
        //this.ForeColor = Color.FromArgb(64, 64, 64);

		ClockFaceSizeFactor = 1.586;
		ButtonGapFactor = 1.2;
		FocusDotFactor = 0.3;
		ClockItemAMAngleDegrees = 130;
		ClockItemPMAngleDegrees = 50;
		HeaderClockFaceGap = 20;
		ClockItemAMDistanceFactor = 1.3;
		ClockItemPMDistanceFactor = 1.3;
		ShowMinuteFaceAfterHourValueSelected = true;
		Renderer = new ClockRenderer { RoundedEdges = false };

		headerItems.AddRange(new [] { ClockItemHour, ClockItemColon, ClockItemMinute, ClockItemAMPM });

		//String family = "Lucida Sans";
		//String family = "Century Gothic";
		//String family = "Calibri";
		//String family = "Arial";
		//String family = "Tahoma";
		String family = "Segoe UI";
		ClockItemHour.Font = new Font(family, 10f, FontStyle.Regular, GraphicsUnit.Point);
		ClockItemColon.Font = new Font(family, 10f, FontStyle.Regular, GraphicsUnit.Point);
		ClockItemMinute.Font = new Font(family, 10f, FontStyle.Regular, GraphicsUnit.Point);
		ClockItemAMPM.Font = new Font("Segoe UI Semibold", 10f, FontStyle.Regular, GraphicsUnit.Point);
		ClockItemAM.Font = new Font(family, 10f, FontStyle.Regular, GraphicsUnit.Point);
		ClockItemPM.Font = new Font(family, 10f, FontStyle.Regular, GraphicsUnit.Point);
		//Font = new Font("Lucida Sans Unicode", 10f, FontStyle.Regular, GraphicsUnit.Point);

		ClockButtonOK.Margin = new Padding(2, 1, 2, 2);
		ClockButtonCancel.Margin = new Padding(2, 1, 2, 2);

		Items.Add(ClockItemHour);
		Items.Add(ClockItemColon);
		Items.Add(ClockItemMinute);
		Items.Add(ClockItemAMPM);

		for (int i = 0; i < 60; i += 1) {
			int value = (i + 15) % 60;
			String text = (value % 5 == 0 ? value.ToString("0#") : "");
			ClockNumberItem item = new ClockNumberItem(value, text, i * 360d / 60, value % 5 != 0);
			item.Visible = false;
			Items.Add(item);
			minuteItems.Add(item);
		}

		for (int i = 9; i < 21; i++) {
			int value = (i + 2) % 12 + 1;
			String text = value.ToString();
			ClockNumberItem item = new ClockNumberItem(value, text, i * 360d / 12, false);
			//item.Visible = false;
			Items.Add(item);
			hourItems.Add(item);
		}

		if (showAMPMButtons) {
			Items.Add(ClockItemAM);
			Items.Add(ClockItemPM);
		}

		if (showOKCancelButtons) {
			Items.Add(ClockButtonOK);
			Items.Add(ClockButtonCancel);
		}

		UpdateClockItemFonts();
		Value = DateTime.Now;
		ResumeLayout(false);

		ClockButtonOK.Click += btn_Click;
		ClockButtonCancel.Click += btn_Click;
	}

	// Even if NOACTIVATE is set, this drop down window can steal the non-client title bar focus
	//protected override CreateParams CreateParams {
	//	get {
	//		var p = base.CreateParams;
	//		if (noActivate)
	//			p.ExStyle = p.ExStyle | 0x08000000;
	//		return p;
	//	}
	//}

	///<summary>If TopMost is true then the DropDown will appear overtop of the taskbar
	///when the window is moved around. An alternative approach is to set the z-order
	///of this dropdown by calling: SetWindowPos(ClockMenu.Handle, (IntPtr) 0, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE)
	///after the Show(...) method is called.</summary>
	protected override bool TopMost { get { return topMost; }}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
		if (!TabStop)
			return base.ProcessCmdKey(ref msg, keyData);

		bool b = ProcessCmdKeyInternal(ref msg, keyData);
		if (b && mouseHideFocusCues) {
			// if user types a key then unhide the focus cues
			mouseHideFocusCues = false;
			Refresh();
		}
		else if (keyData == (Keys.Alt | Keys.C) && ClockButtonCancel.Owner != null && ClockButtonCancel.Text.IndexOf('&') < 0) {
			// if there is no hotkey assigned to the cancel button, then listen for Alt+C
			ClockButtonCancel.PerformClick();
		}
		return b;
	}

	protected override void OnKeyUp(KeyEventArgs e) {
		base.OnKeyUp(e);
		if (e.KeyData == Keys.Tab) {
			// if this control was tabbed into, then show the focus cues
			mouseHideFocusCues = false;
			Refresh();
		}
	}

	// handle the keyboard events
	private bool ProcessCmdKeyInternal(ref Message msg, Keys keyData) {
		var fi = FocusedItem;
		if (IsUp(keyData) || IsDown(keyData)) {
			int dir = IsUp(keyData) ? 1 : -1;
			var cf = ClockFace;
			if (fi == ClockItemHour || fi == ClockItemMinute || fi is ClockNumberItem) {
				if (fi == ClockItemHour)
					cf = ClockFace.Hours;
				else if (fi == ClockItemMinute)
					cf = ClockFace.Minutes;

				if (cf == ClockFace.Hours) {
					int h = ClockHour + dir;
					if (h > 12)
						h = 1;
					else if (h < 1)
						h = 12;

					ClockHour = h;
					Refresh();
					return true;
				}
				else if (cf == ClockFace.Minutes) {
					int m = ClockMinute + dir;
					if (m > 59)
						m = 0;
					else if (m < 0)
						m = 59;

					ClockMinute = m;
					Refresh();
					return true;
				}
			}
			else if (fi == ClockItemAMPM || fi == ClockItemAM || fi == ClockItemPM) {
				ToggleClockMeridiem();
				return true;
			}
			else if (fi == ClockButtonOK || fi == ClockButtonCancel) {
				if (fi == ClockButtonOK)
					tabIndex = Items.IndexOf(ClockButtonCancel);
				else
					tabIndex = Items.IndexOf(ClockButtonOK);
				Refresh();
				return true;
			}
		}
		else if (keyData == Keys.H) {
			SetClockFace(ClockFace.Hours);
			return true;
		}
		else if (keyData == Keys.M) {
			SetClockFace(ClockFace.Minutes);
			return true;
		}
		else if (keyData == Keys.A) {
			ClockMeridiem = ClockMeridiem.AM;
			Refresh();
			return true;
		}
		else if (keyData == Keys.P) {
			ClockMeridiem = ClockMeridiem.PM;
			Refresh();
			return true;
		}
		else if (keyData == Keys.Space) {
			if (fi is ClockNumberItem) {
				ClockNumberItem cni = (ClockNumberItem) fi;
				if (ClockFace == ClockFace.Hours)
					ClockHour = cni.Value;
				else if (ClockFace == ClockFace.Minutes)
					ClockMinute = cni.Value;
				Refresh();
				return true;
			}
			else if (fi == ClockItemAM) {
				ClockMeridiem = ClockMeridiem.AM;
				Refresh();
				return true;
			}
			else if (fi == ClockItemPM) {
				ClockMeridiem = ClockMeridiem.PM;
				Refresh();
				return true;
			}
			else if (fi == ClockItemAMPM) {
				ToggleClockMeridiem();
				return true;
			}
		}
		else if (keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift)) {
			SelectNextItem(keyData == Keys.Tab);
			return true;
		}
		else {
			var cf = ClockFace;
			if (fi == ClockItemHour)
				cf = ClockFace.Hours;
			else if (fi == ClockItemMinute)
				cf = ClockFace.Minutes;

			int? keyValue = IntValue(keyData, cf);
			if (keyValue.HasValue) {
				if (cf == ClockFace.Hours) {
					ClockHour = keyValue.Value;
					Refresh();
				}
				else if (cf == ClockFace.Minutes) {
					int m = keyValue.Value;
					int cm = ClockMinute;
					if (cm / 10 == m / 10) {
						m = cm + 1;
						if (m % 10 == 0)
							m = keyValue.Value;
					}
					ClockMinute = m;
					Refresh();
				}
				return true;	
			}
		}

		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void ToggleClockMeridiem() {
		var cm = ClockMeridiem;
		cm = (cm == ClockMeridiem.AM ? ClockMeridiem.PM : ClockMeridiem.AM);
		ClockMeridiem = cm;
		Refresh();
	}

	private static bool IsUp(Keys keyData) {
		return keyData == Keys.Up || keyData == Keys.Oemplus || keyData == Keys.Right;
	}

	private static bool IsDown(Keys keyData) {
		return keyData == Keys.Down || keyData == Keys.OemMinus || keyData == Keys.Left;
	}

	public static int? IntValue(Keys keyData, ClockFace clockFace) {
		if (clockFace == ClockFace.Hours) {
			if (keyData >= Keys.D1 && keyData <= Keys.D9)
				return (int) keyData - (int) Keys.D0;
			if (keyData == Keys.D0)
				return 10;

			if (keyData >= Keys.NumPad1 && keyData <= Keys.NumPad9)
				return (int) keyData - (int) Keys.D0;
			if (keyData == Keys.NumPad0)
				return 10;
		}
		else if (clockFace == ClockFace.Minutes) {
			if (keyData >= Keys.D0 && keyData <= Keys.D5)
				return 10 * ((int) keyData - (int) Keys.D0);
			if (keyData >= Keys.NumPad0 && keyData <= Keys.NumPad5)
				return 10 * ((int) keyData - (int) Keys.NumPad0);
		}
		return null;
	}

	//protected override void OnClosing(ToolStripDropDownClosingEventArgs e) {
	//	// when AutoClose is false, then Escape key is disabled and e.Cancel is true
	//	if (allowClose)
	//		e.Cancel = false;

	//	base.OnClosing(e);
	//}

	protected override void OnLostFocus(EventArgs e) {
		base.OnLostFocus(e);
		Refresh(); // clears the Focus rectangle
	}

	//protected override void OnKeyDown(KeyEventArgs e) {
	//	if (e.KeyCode == Keys.Escape && !AutoClose && CloseOnEscapeKey) {
	//		allowClose = true;
	//		Close(ToolStripDropDownCloseReason.Keyboard);
	//		allowClose = false;
	//		e.Handled = true;
	//		//e.SuppressKeyPress = true;
	//	}
	//	base.OnKeyDown(e);
	//}

	private void btn_Click(object sender, EventArgs e) {
		if (ButtonClicked != null)
			ButtonClicked(sender, e);
	}

	///<summary>Gets or sets the currently visible clock face.</summary>
	public ClockFace ClockFace {
		get {
			return clockFace;
		}
		set {
			SetClockFace(value);
		}
	}

	///<summary>Gets or sets the currently selected hour value. The hour value must be in the range [1, 12] inclusive.</summary>
	public int ClockHour {
		get {
			return int.Parse(ClockItemHour.Text);
		}
		set {
			int hour = value;
			if (hour <= 0 || hour > 12)
				throw new ArgumentException("hour must be between [1, 12] inclusive");

			SetValue(ClockFace.Hours, hour);
		}
	}

	///<summary>Gets or sets the currently selected minute value. The minute value must be in the range [0, 59] inclusive.</summary>
	public int ClockMinute {
		get {
			return int.Parse(ClockItemMinute.Text);
		}
		set {
			int minute = value;
			if (minute < 0 || minute >= 60)
				throw new ArgumentException("minute must be between [0, 59] inclusive");

			SetValue(ClockFace.Minutes, minute);
		}
	}

	///<summary>Gets or sets the currently selected AM or PM value.</summary>
	public ClockMeridiem ClockMeridiem {
		get {
			String am = DateTimeFormatInfo.CurrentInfo.AMDesignator;
			return ClockItemAMPM.Text == am ? ClockMeridiem.AM : ClockMeridiem.PM;
		}
		set {
			String am = DateTimeFormatInfo.CurrentInfo.AMDesignator;
			String pm = DateTimeFormatInfo.CurrentInfo.PMDesignator;
			SetAMPM(value == ClockMeridiem.AM ? am : pm);
		}
	}

	public ToolStripItem FocusedItem {
		get {
			return Items[tabIndex];
		}
	}

	public void SelectNextItem(bool forward) {
		GetNextItem(forward, out tabIndex);
		if (FocusedItem == ClockButtonOK)
			ClockButtonCancel.IsHot = false;
		else if (FocusedItem == ClockButtonCancel)
			ClockButtonOK.IsHot = false;
		valueSelected = false;
		Refresh();
	}

	public ToolStripItem GetNextItem(bool forward) {
		int index = 0;
		return GetNextItem(forward, out index);
	}

	// the logic is that if a number item was just selected, then select
	// the 'AM' item if tabbing forward. Otherwise the focus will do one loop
	// of the clock face, starting at the current value. Then the focus
	// moves to the 'AM' item if tabbing forward.
	private ToolStripItem GetNextItem(bool forward, out int itemIndex) {
		if (valueSelected) {
			// hour or minute was just selected
			if (forward) {
				itemIndex = -1; // required for compile
				foreach (var item in new ToolStripItem[] { ClockItemAM, ClockItemPM, ClockButtonOK, ClockButtonCancel, ClockItemHour }) {
					// the first four items are optional and might not be added
					itemIndex = Items.IndexOf(item);
					if (itemIndex != -1)
						break;
				}
			}
			else {
				itemIndex = Items.IndexOf(ClockItemAMPM);
			}

			return Items[itemIndex];
		}

		var fi = FocusedItem;
		int currentValue = 0;
		var cf = ClockFace;
		if (cf == ClockFace.Hours)
			currentValue = ClockHour;
		else if (cf == ClockFace.Minutes)
			currentValue = ClockMinute;

		// because there may not be any items after the last number, then if focused on the header hour,
		// shift+tab should select the current value
		ToolStripItem firstItemAfterLastNumber = null;
		for (int i = Items.Count - 1; i > 0; i--) {
			if (Items[i] is ClockNumberItem) {
				if (i + 1 < Items.Count) {
					firstItemAfterLastNumber = Items[i + 1];
				}
				break;
			}
		}

		bool selectCurrentValue = (fi == ClockItemAMPM && forward || (fi == firstItemAfterLastNumber || fi == ClockItemHour && firstItemAfterLastNumber == null) && !forward);
		bool mustBeNumber = (fi is ClockNumberItem);

		int x = (forward ? 1 : -1);
		int k = tabIndex;
		while (true) {
			k += x;
			if (k < 0)
				k = Items.Count - 1;
			else if (k >= Items.Count)
				k = 0;

			var item = Items[k];

			if (item.Visible && item.CanSelect) {
				if (!selectCurrentValue && !mustBeNumber)
					break;

				if (item is ClockNumberItem) {
					if (selectCurrentValue) {
						var cni = (ClockNumberItem) item;
						if (cni.Value == currentValue)
							break;
					}
					else {
						var cni = (ClockNumberItem) item;
						if (cni.Value == currentValue) {
							for (k = 0; k < Items.Count; k++)
								if (Items[k] == firstItemAfterLastNumber && forward || Items[k] == ClockItemAMPM && !forward)
									break;
						}
						if (k == Items.Count)
							k = 0;
						break;
					}
				}
			}
		}
		itemIndex = k;
		return Items[k];
	}

	///<summary>Gets or sets the current DateTime value. When getting the value, the hour and minute are applied.
	///The year, month day, milliseconds and kind are preserved from the previous set value.</summary>
	public DateTime Value {
		get {
			int hour = int.Parse(ClockItemHour.Text);
			int minute = int.Parse(ClockItemMinute.Text);
			ClockMeridiem mer = ClockMeridiem;
			if (mer == ClockMeridiem.AM) {
				if (hour == 12)
					hour = 0;
			}
			else {
				if (hour != 12)
					hour += 12;
			}
			return new DateTime(value.Year, value.Month, value.Day, hour, minute, value.Second, value.Millisecond, value.Kind);
		}
		set {
			if (value == this.value)
				return;

			this.value = value;
			int h = value.Hour;
			String ampm = null;
			if (h >= 12) {
				if (h > 12)
					h -= 12;
				ampm = DateTimeFormatInfo.CurrentInfo.PMDesignator;
				ClockItemAM.IsSelected = false;
				ClockItemPM.IsSelected = true;
			}
			else {
				if (h == 0)
					h = 12;
				ampm = DateTimeFormatInfo.CurrentInfo.AMDesignator;
				ClockItemAM.IsSelected = true;
				ClockItemPM.IsSelected = false;
			}

			int minute = value.Minute;
			//String ampm = value.ToString("tt");

			SuspendLayout();
			ClockItemHour.Text = h.ToString();
			ClockItemMinute.Text = minute.ToString("00");
			ClockItemAMPM.Text = ampm;

			foreach (var cni in minuteItems)
				cni.IsSelected = (cni.Value == minute);

			foreach (var cni in hourItems)
				cni.IsSelected = (cni.Value == h);

			if (ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);

			ResumeLayout(false);
			Refresh();
		}
	}

	private void SetClockFace(ClockFace clockFace) {
		if (clockFace == this.clockFace)
			return;

		SuspendLayout();

		if (clockFace == ClockFace.Minutes) {
			this.clockFace = ClockFace.Minutes;
			foreach (var item in hourItems)
				item.Visible = false;

			foreach (var item in minuteItems)
				item.Visible = true;

			ClockItemHour.IsSelected = false;
			ClockItemMinute.IsSelected = true;

			if (FocusedItem is ClockNumberItem)
				tabIndex = Items.IndexOf(GetMinuteItem(ClockMinute));
		}
		else {
			this.clockFace = ClockFace.Hours;
			foreach (var item in minuteItems)
				item.Visible = false;

			foreach (var item in hourItems)
				item.Visible = true;

			ClockItemHour.IsSelected = true;
			ClockItemMinute.IsSelected = false;

			if (FocusedItem is ClockNumberItem)
				tabIndex = Items.IndexOf(GetHourItem(ClockHour));
		}
		ResumeLayout(true);
		Refresh();
	}

	private ClockNumberItem GetMinuteItem(int value) {
		return minuteItems.Find(m => m.Value == value);
	}

	private ClockNumberItem GetHourItem(int value) {
		return hourItems.Find(h => h.Value == value);
	}

	protected override void OnFontChanged(EventArgs e) {
		UpdateClockItemFonts();
		base.OnFontChanged(e);
	}

	protected override void OnItemClicked(ToolStripItemClickedEventArgs e) {
		base.OnItemClicked(e);
		if (e.ClickedItem == ClockItemAMPM) {
			ToggleClockMeridiem();
		}
	}

	protected override void OnMouseLeave(EventArgs e) {
		base.OnMouseLeave(e);
		ClockButtonOK.IsHot = false;
		ClockButtonCancel.IsHot = false;
		Refresh();
	}

	protected override void OnMouseMove(MouseEventArgs e) {
		base.OnMouseMove(e);

		if (FocusedItem == ClockButtonOK || FocusedItem == ClockButtonCancel) {
			if (FocusedItem == ClockButtonCancel && ClockButtonOK.Bounds.Contains(e.Location)) {
				tabIndex = Items.IndexOf(ClockButtonOK);
				Refresh();
			}
			else if (FocusedItem == ClockButtonOK && ClockButtonCancel.Bounds.Contains(e.Location)) {
				tabIndex = Items.IndexOf(ClockButtonCancel);
				Refresh();
			}
		}

		if (e.Button == MouseButtons.None) {
			ClockNumberItem cni = FindClosest(e.Location, false);
			if (cni != null)
				cni.IsHot = true; 

			foreach (ClockItem ci in headerItems) {
				if (!ci.CanSelect)
					continue;
				var r = ci.Bounds;
				ci.IsHot = r.Contains(e.Location);
			}

			FindAMPM(e.Location);

			ClockButtonOK.IsHot = (ClockButtonOK.Owner != null && ClockButtonOK.Bounds.Contains(e.Location));
			ClockButtonCancel.IsHot = (ClockButtonCancel.Owner != null && ClockButtonCancel.Bounds.Contains(e.Location));

			Refresh();
		}
		else if (e.Button == MouseButtons.Left) {
			mouseHideFocusCues = true;
			if (!IsDragging) {
				Point pt = ClockFaceCenter;
				Size sz = ClockFaceSize;
				int r2 = sz.Width / 2;
				int dx = e.X - pt.X;
				int dy = e.Y - pt.Y;
				int dist = dx * dx + dy * dy;
				if (dist > r2 * r2)
					return;
			}

			IsDragging = true;
			ClockNumberItem cni = FindClosest(e.Location, true, false);
			if (cni != null && !cni.IsSelected) {
				SetValue(clockFace, cni.Value);
			}
		}
	}

	protected override void OnMouseWheel(MouseEventArgs e) {
		base.OnMouseWheel(e);
		if (Control.ModifierKeys == Keys.Control) {
			Font f = Font;
			int df = (e.Delta > 0 ? 1 : -1);
			float sz = f.Size + df;
			if (sz < 6f)
				sz = 6f;
			Font f2 = new Font(f.FontFamily, sz, f.Style, f.Unit, f.GdiCharSet, f.GdiVerticalFont);
			Font = f2;
			if (font != null)
				font.Dispose();
			font = f2;
		}
		else if (Control.ModifierKeys == Keys.None) {
			if (!Enabled) // window still receives mouse wheel if disabled
				return;   // other mouse events are blocked

			mouseHideFocusCues = true;

			if (ClockItemAMPM.IsHot) {
				ToggleClockMeridiem();
				return;
			}

			ClockFace cf = ClockFace;
			if (ClockItemHour.IsHot)
				cf = ClockFace.Hours;
			else if (ClockItemMinute.IsHot)
				cf = ClockFace.Minutes;

			int df = (e.Delta > 0 ? 1 : -1);
			int value = 0;
			if (cf == ClockFace.Hours) {
				value = ClockHour + df;
				if (value < 1)
					value = 12;
				else if (value > 12)
					value = 1;
			}
			else {
				value = ClockMinute + df;
				if (value < 0)
					value = 59;
				else if (value > 59)
					value = 0;
			}
			SetValue(cf, value);
		}
	}

	protected override void OnMouseDown(MouseEventArgs e) {
		base.OnMouseDown(e);
		numberClicked = false;
		ClockNumberItem cni = FindClosest(e.Location, true, true, true);
		ToolStripItem newTabItem = null;
		mouseHideFocusCues = true;

		if (cni != null) {
			// looks bad to set IsDragging to true right away
			//IsDragging = (e.Button == MouseButtons.Left);
			numberClicked = true;
			SetValue(clockFace, cni.Value);
			newTabItem = cni;
		}
		else {
			ClockRenderer ren = (ClockRenderer) this.Renderer;
			ProfessionalColorTable t = ren.ColorTable;

			ClockItem header = null;
			foreach (ClockItem ci in headerItems) {
				if (!ci.CanSelect)
					continue;

				var r = ci.Bounds;
				if (r.Contains(e.Location)) {
					header = ci;
					newTabItem = ci;
					break;
				}
			}

			if (header != null) {
				if (header == ClockItemHour || header == ClockItemMinute)
					SetClockFace(header == ClockItemHour ? ClockFace.Hours : ClockFace.Minutes);
			}
			else {
				var ampm = FindAMPM(e.Location);
				newTabItem = ampm;
				if (ampm != null)
					ClockMeridiem = (ampm == ClockItemAM ? ClockMeridiem.AM : ClockMeridiem.PM);
			}
		}

		if (newTabItem != null) {
			for (int i = 0; i < Items.Count; i++) {
				if (newTabItem == Items[i]) {
					tabIndex = i;
					break;
				}
			}
			Refresh();
		}
	}

    /// <summary>
    /// Set clock face to hours 
    /// </summary>
    public void Set_To_Hour()
    {
        SetClockFace(ClockFace.Hours);
    }

	protected override void OnMouseUp(MouseEventArgs mea) {
		base.OnMouseUp(mea);
		if (ShowMinuteFaceAfterHourValueSelected && clockFace == ClockFace.Hours && (IsDragging || numberClicked)) // && FindClosest(mea.Location, true) != null)
			SetClockFace(ClockFace.Minutes);

		IsDragging = false;
		Refresh();
	}

	private void SetValue(ClockFace cf, int value) {
		valueSelected = (FocusedItem is ClockNumberItem);
		String t = null;
		ClockItem header = null;
		if (cf == ClockFace.Hours) {
			header = ClockItemHour;
			t = value.ToString();
		}
		else {
			header = ClockItemMinute;
			t = value.ToString("00");
		}
		if (t == header.Text.Trim())
			return;

		// only update the focused item if cf is the current clock face
		if (TabStop && ClockFace == cf && FocusedItem is ClockNumberItem) {
			if (cf == ClockFace.Hours)
				tabIndex = Items.IndexOf(GetHourItem(value));
			else
				tabIndex = Items.IndexOf(GetMinuteItem(value));
		}

		SuspendLayout();
		header.Text = t;
		ResumeLayout(false);
		var v = Value;
		Value = v;
	}

	private void SetAMPM(String amPM) {
		SuspendLayout();
		ClockItemAMPM.Text = amPM;
		ResumeLayout(false);
		var v = Value;
		Value = v; // fires event
	}

	protected void UpdateClockItemFonts() {
		List<Font> oldFonts = new List<Font>();
		foreach (var item in new [] { ClockItemAMPM, ClockItemColon, ClockItemHour, ClockItemMinute, ClockItemAM, ClockItemPM })
			oldFonts.Add(item.Font);

		Font f = Font;
		UpdateFont(ClockItemAMPM, f.Size);
		UpdateFont(ClockItemColon, 2.5f * f.Size);
		UpdateFont(ClockItemHour, 2.5f * f.Size);
		UpdateFont(ClockItemMinute, 2.5f * f.Size);
		UpdateFont(ClockItemAM, 1.1f * f.Size);
		UpdateFont(ClockItemPM, 1.1f * f.Size);

		using (var g = CreateGraphics()) {
			g.SmoothingMode = SmoothingMode.HighQuality;
			MeasureText(ClockItemHour, "12", g);
			MeasureText(ClockItemColon, DateTimeFormatInfo.CurrentInfo.TimeSeparator, g);
			MeasureText(ClockItemMinute, "44", g); // 44 tends to be the widest number
			MeasureText(ClockItemAMPM, new [] { DateTimeFormatInfo.CurrentInfo.AMDesignator, DateTimeFormatInfo.CurrentInfo.PMDesignator }, g);
			//---
			// AM / PM
			MeasureText(ClockItemAM, ClockItemAM.Text, g, true, 1.4);
			ClockItemPM.PreferredSize = ClockItemAM.PreferredSize;
			//---
			UpdateButtonSizes(g, f, minuteItems);
			UpdateButtonSizes(g, f, hourItems);
		}

		Size s1 = ClockButtonOK.GetPreferredSize(Size.Empty);
		Size s2 = ClockButtonCancel.GetPreferredSize(Size.Empty);
		s1.Height = (int) (1.5 * s1.Height);
		s2.Height = (int) (1.5 * s2.Height);
		ClockButtonOK.PreferredSize = s1;
		ClockButtonCancel.PreferredSize = s2;

		foreach (Font f2 in oldFonts)
			f2.Dispose();
	}

	private static void UpdateFont(ToolStripMenuItem mi, float size) {
		Font f = mi.Font;
		mi.Font = new Font(mi.Font.FontFamily, size, f.Style, f.Unit, f.GdiCharSet, f.GdiVerticalFont);
	}

	private static Size UpdateButtonSizes(Graphics g, Font f, List<ClockNumberItem> items) {
		Size s1 = Size.Empty;
		foreach (var item in items) {
			Size ps = TextRenderer.MeasureText(g, item.Text, f);
			var p = item.Padding;
			ps.Width += p.Horizontal;
			ps.Height += p.Vertical;
			if (ps.Width > s1.Width)
				s1.Width = ps.Width;
			if (ps.Height > s1.Height)
				s1.Height = ps.Height;
		}
		s1.Width = Math.Max(s1.Width, s1.Height);
		s1.Height = Math.Max(s1.Width, s1.Height);
		foreach (var item in items) {
			item.PreferredSize = s1;
		}
		return s1;
	}

	private static void MeasureText(ClockItem mi, String text, Graphics g, bool square = false, double extraWFactor = 1.0) {
		MeasureText(mi, new [] { text }, g, square, extraWFactor);
	}

	private static void MeasureText(ClockItem mi, String[] texts, Graphics g, bool square = false, double extraWFactor = 1.0) {
		g.TextRenderingHint = mi.Font.Size < 20 ? TextRenderingHint.SystemDefault : TextRenderingHint.AntiAlias;
		TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding;

		Size ps = Size.Empty;
		String text = null;
		foreach (String t in texts) {	
			Size ps2 = TextRenderer.MeasureText(g, t, mi.Font, Size.Empty, flags);
			ps.Width = (int) (extraWFactor * ps.Width);
			if (ps2.Width > ps.Width) {
				text = t;
				ps.Width = ps2.Width;
			}
			if (ps2.Height > ps.Height)
				ps.Height = ps2.Height;
		}

		ps.Width = (int) Math.Ceiling(extraWFactor * ps.Width);

		if (square) {
			ps.Width = Math.Max(ps.Width, ps.Height);
			ps.Height = ps.Width;
		}

		mi.PreferredSize = ps;
		mi.TextBounds = MeasureString.Measure(text, g, mi.Font, DrawMethod.TextRenderer, flags, new Rectangle(0, 0, ps.Width, ps.Height));
	}

	private ClockNumberItem FindClosest(Point pt, bool includeAll, bool limitDistance = true, bool getHot = false) {
		List<ClockNumberItem> items = (clockFace == ClockFace.Hours ? hourItems : minuteItems);

		int min = int.MaxValue;
		ClockNumberItem closest = null;
		foreach (ClockNumberItem cni in items) {
			cni.IsHot = false;

			if (!includeAll && cni.ShowFocusDot)
				continue;

			Rectangle r = cni.Bounds;
			int x = r.X + r.Width / 2;
			int y = r.Y + r.Height / 2;
			int dx = pt.X - x;
			int dy = pt.Y - y;
			int dist = dx * dx + dy * dy;
			if (dist < min) {
				closest = cni;
				min = dist;
			}

			if (getHot && !cni.ShowFocusDot) {
				int w2 = r.Width / 2;
				int ww = w2 * w2;
				if (dist <= ww)
					return cni;
			}
		}

		if (!limitDistance)
			return closest;

		int rr = items[0].Size.Width / 2;
		if (min > rr * rr)
			closest = null;

		return closest;
	}

	private ClockItem FindAMPM(Point pt) {
		ClockItem ci = null;
		foreach (var item in new [] { ClockItemAM, ClockItemPM }) {
			if (item.Owner == null)
				continue;
			var r = item.Bounds;
			int rad = r.Width / 2;
			int cx = r.X + rad;
			int cy = r.Y + rad;
			int dx = pt.X - cx;
			int dy = pt.Y - cy;
			item.IsHot = (dx * dx + dy * dy <= rad * rad);
			if (item.IsHot)
				ci = item;
		}
		return ci;
	}

	public override Size GetPreferredSize(Size proposedSize) {
		Size ps = layoutEngine.LayoutInternal(false, this);
		return ps;
	}

	///<summary>Returns the height at the top of the clock that displays the current time value.</summary>
	public virtual int GetHeaderHeight() {
		// adding 10 looks better at smaller font sizes. For large fonts, the extra 10 pixels aren't noticeable.
		return 2 * ClockItemHour.TextBounds.Height + 10;
	}

	///<summary>Returns the distance from the bottom of the header to the top of the clock face.
	public virtual int GetHeaderClockFaceGap() {
		//double ClockFaceGapFactor = 0;
		int gap = Math.Max(0, HeaderClockFaceGap - (int) (30 / Font.Size));
		return gap;
	}

	public override LayoutEngine LayoutEngine {
		get {
			return layoutEngine;
		}
	}

	private class ClockLayoutEngine : LayoutEngine {
		public override void InitLayout(object child, BoundsSpecified specified) {}

		public override bool Layout(object container, LayoutEventArgs layoutEventArgs) {
			//var c = layoutEventArgs.AffectedControl;
			//sb.AppendLine((c == null ? "null" : c.GetType().FullName + "  " + layoutEventArgs.AffectedProperty));
			LayoutInternal(true, (ClockControl) container);
			return false;
		}

		public int GetPreferredClockDiameter(ClockControl clock) {
			Size s = Size.Empty;
			foreach (var cni in clock.minuteItems) {
				Size ps = cni.PreferredSize;
				if (ps.Width > s.Width)
					s.Width = ps.Width;
				if (ps.Height > s.Height)
					s.Height = ps.Height;
			}
			foreach (var cni in clock.hourItems) {
				Size ps = cni.PreferredSize;
				if (ps.Width > s.Width)
					s.Width = ps.Width;
				if (ps.Height > s.Height)
					s.Height = ps.Height;
			}

			int r = Math.Max(s.Width, s.Height);
			double circumference = 12 * clock.ClockFaceSizeFactor * r;
			double diameter = circumference / Math.PI;
			int w = (int) Math.Ceiling(diameter);
			return w;
		}

		public Size LayoutInternal(bool doLayout, ClockControl clock) {
			Size ps = Size.Empty;
			Padding p = clock.Padding;

			int width = 0;
			Size clockFaceSize = Size.Empty;
			if (doLayout) {
				clockFaceSize = clock.ClockFaceSize;
				width = clock.Width;
			}
			else {
				width = GetPreferredClockDiameter(clock);
				clockFaceSize = new Size(width, width);
				clock.ClockFaceSize = clockFaceSize;
				width += p.Horizontal;
			}

			Size szAM = clock.ClockItemAM.PreferredSize;
			Size szPM = clock.ClockItemPM.PreferredSize;
			
			Point ptAM = Point.Empty;
			if (clock.ClockItemAM.Owner != null)
				ptAM = GetLocation(Point.Empty, clockFaceSize, clock.ClockItemAMAngleDegrees, szAM, (int) (-clock.ClockItemAMDistanceFactor * szAM.Width));

			Point ptPM = Point.Empty;
			if (clock.ClockItemPM.Owner != null)
				ptPM = GetLocation(Point.Empty, clockFaceSize, clock.ClockItemPMAngleDegrees, szPM, (int) (-clock.ClockItemPMDistanceFactor * szPM.Width));
			int offsetX = 0;

			if (ptAM.X < -clockFaceSize.Width / 2) {
				offsetX = -(clockFaceSize.Width / 2) - ptAM.X;
				width = p.Horizontal + (ptPM.X + szPM.Width - ptAM.X);
			}

			ps.Width = width;
			int h2 = clock.GetHeaderHeight();
			int gap = clock.GetHeaderClockFaceGap();
			int ww = clockFaceSize.Width;
			clock.ClockFaceCenter = new Point(offsetX + p.Left + ww / 2, h2 + gap + ww / 2);

			if (doLayout) {
				var headers = new [] { clock.ClockItemHour, clock.ClockItemColon, clock.ClockItemMinute, clock.ClockItemAMPM };
				double[] alignY = new [] { 0.5, 1.0, 0.5, 1.0 };
				int gap0 = clock.hourItems[0].PreferredSize.Width / 3;
				int[] gaps = new [] { gap0, gap0, (int) (1.4 * gap0), 0 };

				int totalWidth = 0;
				for (int i = 0; i < headers.Length; i++) {
					var h = headers[i];
					var s = h.PreferredSize;
					totalWidth += s.Width + gaps[i];
				}

				int m1 = clock.ClockItemHour.TextBounds.X;
				int m2 = clock.ClockItemAMPM.PreferredSize.Width - clock.ClockItemAMPM.TextBounds.Right;
				totalWidth -= (m1 + m2);
				int x = Math.Max(0, width - totalWidth) / 2 - m1;

				for (int i = 0; i < alignY.Length; i++) {
					var h = headers[i];
					int idealy = (int) Math.Ceiling(0.5 * (h2 - h.TextBounds.Height));
					int dy = idealy - h.TextBounds.Y;
					int dy2 = (int) Math.Ceiling((alignY[i] - 0.5) * (headers[0].TextBounds.Height - h.TextBounds.Height));
					int y = p.Top + dy + dy2;
					var s = h.PreferredSize;
					h.SetBounds(new Point(x, y + 1), s);
					x += s.Width + gaps[i];
				}

				var items = (clock.clockFace == ClockFace.Hours ? clock.hourItems : clock.minuteItems);
				foreach (var item in items) {
					Size s = item.PreferredSize;
					Point pt = GetLocation(clock, item);
					item.SetBounds(pt, s);
				}
			}

			ptAM = GetLocation(clock.ClockFaceCenter, clockFaceSize, clock.ClockItemAMAngleDegrees, szAM, (int) (-clock.ClockItemAMDistanceFactor * szAM.Width));
			ptPM = GetLocation(clock.ClockFaceCenter, clockFaceSize, clock.ClockItemPMAngleDegrees, szPM, (int) (-clock.ClockItemPMDistanceFactor * szPM.Width));
			ptPM.X++; // pixel adjustment

			Padding mOK = clock.ClockButtonOK.Margin;
			Padding mCancel = clock.ClockButtonCancel.Margin;

			//var r = clock.miAM.Bounds;
			int y2a = 0;
			if (clock.ClockItemAM.Owner != null)
				y2a = ptAM.Y + (int) (clock.ButtonGapFactor * szAM.Height) + 1; // +1 otherwise the starting y-loc
			int y2b = 0;
			if (clock.ClockItemPM.Owner != null)
				y2b = ptPM.Y + (int) (clock.ButtonGapFactor * szPM.Height) + 1; // would be on the separator line

			int y2 = Math.Max(y2a, y2b);
			if (y2 == 0) {
				// use the same gap above the clock as below the clock when no AM button and no PM button are visible
				y2 = clock.ClockFaceCenter.Y + clockFaceSize.Height / 2 + gap;
			}

			if (doLayout) {
				if (clock.ClockItemAM.Owner != null)
					clock.ClockItemAM.SetBounds(ptAM, szAM);
				if (clock.ClockItemPM.Owner != null)
					clock.ClockItemPM.SetBounds(ptPM, szPM);
			}

			if (clock.ClockButtonOK.Owner != null || clock.ClockButtonCancel.Owner != null) {
				//---
				int wOK = (int) Math.Ceiling(clock.ClockButtonOKWidthFactor * width);
				int xOK = (int) ((width - wOK) * clock.ClockButtonOKAlignX) + mOK.Left;
				wOK = wOK - mOK.Horizontal;
				Point ptOK = new Point(xOK, y2 + mOK.Top);
				Size szOK = new Size(wOK, clock.ClockButtonOK.PreferredSize.Height);
				//---
				int wCancel = (int) Math.Ceiling(clock.ClockButtonCancelWidthFactor * width);
				int xCancel = (int) ((width - wCancel) * clock.ClockButtonCancelAlignX) + mCancel.Left;
				wCancel = wCancel - mCancel.Horizontal;
				Point ptCancel = new Point(xCancel, y2 + mCancel.Top);			
				Size szCancel = new Size(wCancel, clock.ClockButtonCancel.PreferredSize.Height);

				if (doLayout) {
					if (clock.ClockButtonOK.Owner != null)					
						clock.ClockButtonOK.SetBounds(ptOK, szOK);
					if (clock.ClockButtonCancel.Owner != null)
						clock.ClockButtonCancel.SetBounds(ptCancel, szCancel);
				}

				int yz1 = ptOK.Y + szOK.Height + mOK.Bottom + p.Bottom;
				int yz2 = ptCancel.Y + szCancel.Height + mCancel.Bottom + p.Bottom;
				ps.Height = Math.Max(yz1, yz2); // expected to be identical
			}
			else {
				ps.Height = y2 + p.Bottom;
			}
			return ps;
		}

		private static Point GetLocation(ClockControl clock, ClockNumberItem item, int gap = 3) {
			return GetLocation(clock.ClockFaceCenter, clock.ClockFaceSize, item.AngleDegrees, item.PreferredSize, gap);
		}

		private static Point GetLocation(Point ptCenter, Size s, double angleDegrees, Size ps, int gap = 3) {
			int ox = ptCenter.X;
			int oy = ptCenter.Y;

			double rx = (s.Width - ps.Width) / 2d;
			double ry = (s.Height - ps.Height) / 2d;
			rx -= gap;
			ry -= gap;

			int x = ox + (int) Math.Round(rx * Math.Cos(angleDegrees * Math.PI / 180));
			int y = oy + (int) Math.Round(ry * Math.Sin(angleDegrees * Math.PI / 180));
			// translate to the top left corner
			x = x - ps.Width / 2;
			y = y - ps.Height / 2;
			return new Point(x, y);
		}
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		if (disposing) {
			foreach (var item in new [] { ClockItemAMPM, ClockItemColon, ClockItemHour, ClockItemMinute })
				item.Font.Dispose();

			if (font != null)
				font.Dispose();
		}
	}
}

public class ClockRenderer : ToolStripProfessionalRenderer {

    public Color Form_BackColor = Color.FromArgb(64, 64, 64);
    public Color Form_ForeColor = Color.White;
    public Color Highlight_Color = Color.LightBlue;

	// Border renders top-most, so draw the solid focus dot size here.
	protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
		base.OnRenderToolStripBorder(e);

		ProfessionalColorTable t = this.ColorTable;
		ClockControl clock = (ClockControl) e.ToolStrip;
		var g = e.Graphics;
		g.SmoothingMode = SmoothingMode.HighQuality;

		// always want the small solid blue focus dot to appear on top
		using (var b = new SolidBrush(t.MenuItemBorder)) {
			List<ClockNumberItem> items = (clock.ClockFace == ClockFace.Hours ? clock.hourItems : clock.minuteItems);
			foreach (var cni in items) {
				bool dot = cni.IsSelected && (cni.ShowFocusDot || clock.IsDragging);
				if (dot) {
					var r = cni.Bounds;
					int w = (int) (clock.FocusDotFactor * r.Width);
					int x = (r.Width - w) / 2;
					int	y = x;
					g.FillEllipse(b, r.X + x, r.Y + y, w, w);
				}
			}

			ToolStripItem fi = (clock.TabStop ? clock.FocusedItem : null);
			// draw focus rectangles
			if (clock.ShowFocusCues && clock.TabStop && clock.Focused && !clock.IsDragging) {
				if (fi is ClockNumberItem) {
					var r = fi.Bounds;
					ClockNumberItem cni = (ClockNumberItem) fi;
					if (cni.ShowFocusDot && !cni.IsSelected) {
						int w = (int) (clock.FocusDotFactor * r.Width);
						int x = (r.Width - w) / 2;
						int	y = x;
						r = new Rectangle(r.X + x - 1, r.Y + y - 1, w + 2, w + 2);
					}	
					ControlPaint.DrawFocusRectangle(g, r);
				}
				else if (fi == clock.ClockItemAM || fi == clock.ClockItemPM) {
					if (fi.Owner != null) {
						ClockItem ci = (ClockItem) fi;
						var r = fi.Bounds;
						ControlPaint.DrawFocusRectangle(g, r);
					}
				}
				else if (fi is ClockHeaderItem) {
					ClockItem ci = (ClockItem) fi;
					int inflate = 6;
					var r = fi.Bounds;
					var r2 = ci.TextBounds;
					r.Width = r2.Width;
					r.Height = r2.Height;
					r.Y = r.Y + r2.Y;
					r.X = r.X + r2.X;
					r.Inflate(inflate, inflate);
					ControlPaint.DrawFocusRectangle(g, r);
				}
				else {
					// the default blue background is used for the OK and Cancel buttons, so a dotted
					// focus rectangle is not drawn.
					//var r = fi.Bounds;
					//ControlPaint.DrawFocusRectangle(g, r);
				}
			}
		}
	}

	protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) {

		base.OnRenderToolStripBackground(e);
		var g = e.Graphics;
        g.Clear(Form_BackColor);
		//g.Clear(ColorTable.MenuStripGradientBegin);
		//g.Clear(ColorTable.ToolStripGradientMiddle);
		//g.Clear(Color.White);
		g.SmoothingMode = SmoothingMode.HighQuality;

		var t = ColorTable;
		ClockControl clock = (ClockControl) e.ToolStrip;
		Point pt = clock.ClockFaceCenter;
		Size sz = clock.ClockFaceSize;

        SolidBrush bb = new SolidBrush(Color.FromArgb(76, 76, 76));

		int rr = sz.Width / 2;
		g.FillEllipse(bb, pt.X - rr, pt.Y - rr, 2 * rr, 2 * rr);

		Padding p = clock.Padding;
		int h = clock.GetHeaderHeight();
		g.FillRectangle(bb, 0, 0, clock.Width, h);

        Brush b = new SolidBrush(Highlight_Color);
        using (Pen pen = new Pen(Highlight_Color))
        {
			List<ClockNumberItem> items = (clock.ClockFace == ClockFace.Hours ? clock.hourItems : clock.minuteItems);
			for (int i = 0; i < 2; i++) {
				foreach (var item in items) {
					bool b1 = item.IsSelected && i == 1;
					bool b2 = item.IsHot && i == 0;
					if (b1 || b2) {
						Rectangle r = item.Bounds;
						bool drawDot = item.IsSelected && (item.ShowFocusDot || clock.IsDragging);
						if (item.IsSelected && !drawDot && !b2)
							g.DrawLine(pen, pt, new Point(r.X + r.Width / 2, r.Y + r.Height / 2));

                        g.FillEllipse(new SolidBrush(t.MenuItemSelected), r);

						if (item.IsSelected && drawDot && !b2)
							g.DrawLine(pen, pt, new Point(r.X + r.Width / 2, r.Y + r.Height / 2));
					}
				}
			}
		}

		int k = 0; // pixel
		foreach (var item in new [] { clock.ClockItemAM, clock.ClockItemPM }) {
			if (item.Owner != null) {
				var r1 = item.Bounds;
				Brush br = (item.IsHot || item.IsSelected ? new SolidBrush(t.MenuItemBorder) : new SolidBrush(Color.FromArgb(76, 76, 76)));
				r1.X += k;
				g.FillEllipse(br, r1);
			}
			k--;
		}

		if (clock.ClockButtonOK.Owner != null || clock.ClockButtonCancel.Owner != null) {
			var r2 = clock.ClockItemAM.Bounds;
			using (Pen pen = new Pen(Color.FromArgb(197, 197, 197))) {
				int y2 = 0; //(int) (r2.Y + clock.ButtonGapFactor * r2.Height);
				if (clock.ClockButtonOK.Owner != null)
					y2 = clock.ClockButtonOK.Bounds.Y - clock.ClockButtonOK.Margin.Top;
				else
					y2 = clock.ClockButtonCancel.Bounds.Y - clock.ClockButtonCancel.Margin.Top;

				y2--;
				g.DrawLine(pen, 0, y2, clock.Width, y2);
			}
		}

		g.FillEllipse(new SolidBrush(Color.LightBlue), pt.X - 1, pt.Y - 1, 2, 2); // the clock center dot
		b.Dispose();
	}

	// japanese characters start looking noticeably jagged starting at font size 14
	// however, below 14 using anti-aliasing looks too thin. The numbers look OK up
	// to font size 20, and below 20 they look thin if anti-aliased.
	private static TextRenderingHint GetTextRenderingHint(ToolStripItem item) {
		Font f = item.Font;
		if (item is ClockNumberItem)
			return (f.Size > 20 ? TextRenderingHint.AntiAlias : TextRenderingHint.SystemDefault);

		return (f.Size >= 14 ? TextRenderingHint.AntiAlias : TextRenderingHint.SystemDefault);
	}

	protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
		var g = e.Graphics;

		g.TextRenderingHint = GetTextRenderingHint(e.Item);
		g.SmoothingMode = SmoothingMode.HighQuality;

		ClockControl clock = (ClockControl) e.ToolStrip;
		if (e.Item is ClockHeaderItem) {
			ClockItem ci = (ClockItem) e.Item;
			e.TextColor = (ci.IsHot || ci.IsSelected ? ColorTable.MenuItemBorder : ci.ForeColor);
			e.TextRectangle = new Rectangle(Point.Empty, ci.PreferredSize);
			e.TextFormat = e.TextFormat | TextFormatFlags.NoPadding;
		}
		else if (e.Item is ClockItem) { //e.Item == clock.ClockItemAM || e.Item == clock.ClockItemPM) {
			ClockItem ci = (ClockItem) e.Item;
			e.TextRectangle = new Rectangle(Point.Empty, ci.PreferredSize);
			e.TextFormat = e.TextFormat | TextFormatFlags.NoPadding;
		}

		base.OnRenderItemText(e);
	}

	protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
		if (e.Item is ClockButton)
			base.OnRenderMenuItemBackground(e);
	}
}

public enum ClockFace {
	Hours,
	Minutes,
}

public enum ClockMeridiem {
	AM,
	PM,
}

public class ClockButton : ToolStripButton {

	///<summary>The preferred width and height of the item. This value
	///is calculated based on the item's font and the item's text.</summary>
	public Size PreferredSize { get; set; }

	public ClockButton(String text) : base(text) {
		AutoToolTip = false;
	}

	public void SetBounds(Point p, Size s) {
		SetBounds(new Rectangle(p, s));
	}

	public bool IsHot { get; set; } // true if the mouse is over

	public override bool Selected {
		get {
			ClockControl clock = (ClockControl) Parent;
			return IsHot || clock.Focused && clock.FocusedItem == this; // && clock.ShowFocusCues 
		}
	}
}

public class ClockItem : ToolStripMenuItem {

	///<summary>The preferred width and height of the item. This value
	///is calculated based on the item's font and the item's text.</summary>
	public Size PreferredSize { get; set; }

	///<summary>The true rendered width and height of the item's text in pixels.</summary>
	public Rectangle TextBounds { get; set; }

	///<summary>A flag that indicates if the item is currently selected. The clock renderer
	///will use this flag to highlight the item.</summary>
	public bool IsSelected { get; set; }

	///<summary>A flag that indicates if the mouse is currently over top of the item.
	///The clock renderer will use this flag to highlight the item.</summary>
	public bool IsHot { get; set; }

	private bool canSelect = true;

	public ClockItem(String text, bool canSelect = true) : base(text) {
		this.canSelect = canSelect;
		this.AutoSize = false;
		this.DropDown = null;
		this.Overflow = ToolStripItemOverflow.Never;
		this.TextAlign = ContentAlignment.MiddleCenter;
        this.ForeColor = Color.White;
        this.BackColor = Color.FromArgb(64, 64, 64);
	}

	public override bool CanSelect {
		get {
			return canSelect;
		}
	}

	public void SetBounds(Point p, Size s) {
		SetBounds(new Rectangle(p, s));
	}
}

public class ClockHeaderItem : ClockItem {
	public ClockHeaderItem(String text, bool canSelect = true) : base(text, canSelect) {
	}
}

public class ClockNumberItem : ClockItem {

	///<summary>The angle with respect to the center of the clock face. For example, the 3rd hour
	///and 15th minute are both 0 degrees. The 6th hour and 30th minute and both 90 degrees.
	///The 9th hour and 45th minute are both 180 degrees. The 12th hour and 00 minute are
	///both 270 degrees.</summary>
	public double AngleDegrees { get; private set; }

	///<summary>The minute value or hour value of the number.</summary>
	public int Value { get; private set; }

	///<summary>A flag that indicates to display a small solid blue circle in the middle
	///of the number when number is selected but not being dragged.</summary>
	public bool ShowFocusDot { get; set; }

	public ClockNumberItem(int value, String text, double angleDegrees, bool showFocusDot) : base(text) {
		this.Value = value;
		this.AngleDegrees = angleDegrees;
		this.ShowFocusDot = showFocusDot;
		Padding = new Padding(4);
	}
}

}

/*


public class ClockRenderer : ToolStripProfessionalRenderer {

	// Border renders top-most, so draw the solid focus dot size here.
	protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
		base.OnRenderToolStripBorder(e);

		ProfessionalColorTable t = this.ColorTable;
		ClockControl clock = (ClockControl) e.ToolStrip;
		var g = e.Graphics;
		g.SmoothingMode = SmoothingMode.HighQuality;

		// always want the small solid blue focus dot to appear on top
		using (var b = new SolidBrush(t.MenuItemBorder)) {
			List<ClockNumberItem> items = (clock.ClockFace == ClockFace.Hours ? clock.hourItems : clock.minuteItems);
			foreach (var cni in items) {
				bool dot = cni.IsSelected && (cni.ShowFocusDot || clock.IsDragging);
				if (dot) {
					var r = cni.Bounds;
					int w = (int) (clock.FocusDotFactor * r.Width);
					int x = (r.Width - w) / 2;
					int	y = x;
					g.FillEllipse(b, r.X + x, r.Y + y, w, w);
				}
			}

			ToolStripItem fi = (clock.TabStop ? clock.FocusedItem : null);
			// draw focus rectangles
			if (clock.ShowFocusCues && clock.TabStop && clock.Focused && !clock.IsDragging) {
				if (fi is ClockNumberItem) {
					var r = fi.Bounds;
					ClockNumberItem cni = (ClockNumberItem) fi;
					if (cni.ShowFocusDot && !cni.IsSelected) {
						int w = (int) (clock.FocusDotFactor * r.Width);
						int x = (r.Width - w) / 2;
						int	y = x;
						r = new Rectangle(r.X + x - 1, r.Y + y - 1, w + 2, w + 2);
					}	
					ControlPaint.DrawFocusRectangle(g, r);
				}
				else if (fi == clock.ClockItemAM || fi == clock.ClockItemPM) {
					if (fi.Owner != null) {
						ClockItem ci = (ClockItem) fi;
						var r = fi.Bounds;
						ControlPaint.DrawFocusRectangle(g, r);
					}
				}
				else if (fi is ClockHeaderItem) {
					ClockItem ci = (ClockItem) fi;
					int inflate = 6;
					var r = fi.Bounds;
					var r2 = ci.TextBounds;
					r.Width = r2.Width;
					r.Height = r2.Height;
					r.Y = r.Y + r2.Y;
					r.X = r.X + r2.X;
					r.Inflate(inflate, inflate);
					ControlPaint.DrawFocusRectangle(g, r);
				}
				else {
					// the default blue background is used for the OK and Cancel buttons, so a dotted
					// focus rectangle is not drawn.
					//var r = fi.Bounds;
					//ControlPaint.DrawFocusRectangle(g, r);
				}
			}
		}
	}

	protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) {
		base.OnRenderToolStripBackground(e);
		var g = e.Graphics;
		g.Clear(ColorTable.MenuItemPressedGradientMiddle);
		//g.Clear(ColorTable.MenuStripGradientBegin);
		//g.Clear(ColorTable.ToolStripGradientMiddle);
		//g.Clear(Color.White);
		g.SmoothingMode = SmoothingMode.HighQuality;

		var t = ColorTable;
		ClockControl clock = (ClockControl) e.ToolStrip;
		Point pt = clock.ClockFaceCenter;
		Size sz = clock.ClockFaceSize;

		int rr = sz.Width / 2;
		g.FillEllipse(Brushes.White, pt.X - rr, pt.Y - rr, 2 * rr, 2 * rr);

		Padding p = clock.Padding;
		int h = clock.GetHeaderHeight();
		g.FillRectangle(Brushes.White, 0, 0, clock.Width, h);

		Brush b = new SolidBrush(t.MenuItemSelected);
		using (Pen pen = new Pen(t.MenuItemBorder)) {
			List<ClockNumberItem> items = (clock.ClockFace == ClockFace.Hours ? clock.hourItems : clock.minuteItems);
			for (int i = 0; i < 2; i++) {
				foreach (var item in items) {
					bool b1 = item.IsSelected && i == 1;
					bool b2 = item.IsHot && i == 0;
					if (b1 || b2) {
						Rectangle r = item.Bounds;
						bool drawDot = item.IsSelected && (item.ShowFocusDot || clock.IsDragging);
						if (item.IsSelected && !drawDot && !b2)
							g.DrawLine(pen, pt, new Point(r.X + r.Width / 2, r.Y + r.Height / 2));

						g.FillEllipse(b, r);

						if (item.IsSelected && drawDot && !b2)
							g.DrawLine(pen, pt, new Point(r.X + r.Width / 2, r.Y + r.Height / 2));
					}
				}
			}
		}

		int k = 0; // pixel
		foreach (var item in new [] { clock.ClockItemAM, clock.ClockItemPM }) {
			if (item.Owner != null) {
				var r1 = item.Bounds;
				Brush br = (item.IsHot || item.IsSelected ? b : Brushes.White);
				r1.X += k;
				g.FillEllipse(br, r1);
			}
			k--;
		}

		if (clock.ClockButtonOK.Owner != null || clock.ClockButtonCancel.Owner != null) {
			var r2 = clock.ClockItemAM.Bounds;
			using (Pen pen = new Pen(Color.FromArgb(197, 197, 197))) {
				int y2 = 0; //(int) (r2.Y + clock.ButtonGapFactor * r2.Height);
				if (clock.ClockButtonOK.Owner != null)
					y2 = clock.ClockButtonOK.Bounds.Y - clock.ClockButtonOK.Margin.Top;
				else
					y2 = clock.ClockButtonCancel.Bounds.Y - clock.ClockButtonCancel.Margin.Top;

				y2--;
				g.DrawLine(pen, 0, y2, clock.Width, y2);
			}
		}

		g.FillEllipse(Brushes.Black, pt.X - 1, pt.Y - 1, 2, 2); // the clock center dot
		b.Dispose();
	}

	// japanese characters start looking noticeably jagged starting at font size 14
	// however, below 14 using anti-aliasing looks too thin. The numbers look OK up
	// to font size 20, and below 20 they look thin if anti-aliased.
	private static TextRenderingHint GetTextRenderingHint(ToolStripItem item) {
		Font f = item.Font;
		if (item is ClockNumberItem)
			return (f.Size > 20 ? TextRenderingHint.AntiAlias : TextRenderingHint.SystemDefault);

		return (f.Size >= 14 ? TextRenderingHint.AntiAlias : TextRenderingHint.SystemDefault);
	}

	protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
		var g = e.Graphics;

		g.TextRenderingHint = GetTextRenderingHint(e.Item);
		g.SmoothingMode = SmoothingMode.HighQuality;

		ClockControl clock = (ClockControl) e.ToolStrip;
		if (e.Item is ClockHeaderItem) {
			ClockItem ci = (ClockItem) e.Item;
			e.TextColor = (ci.IsHot || ci.IsSelected ? ColorTable.MenuItemBorder : ci.ForeColor);
			e.TextRectangle = new Rectangle(Point.Empty, ci.PreferredSize);
			e.TextFormat = e.TextFormat | TextFormatFlags.NoPadding;
		}
		else if (e.Item is ClockItem) { //e.Item == clock.ClockItemAM || e.Item == clock.ClockItemPM) {
			ClockItem ci = (ClockItem) e.Item;
			e.TextRectangle = new Rectangle(Point.Empty, ci.PreferredSize);
			e.TextFormat = e.TextFormat | TextFormatFlags.NoPadding;
		}

		base.OnRenderItemText(e);
	}

	protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
		if (e.Item is ClockButton)
			base.OnRenderMenuItemBackground(e);
	}
}

*/
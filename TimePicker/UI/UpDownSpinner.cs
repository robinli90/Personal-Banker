using System;
using System.Windows.Forms;
using System.Drawing;

namespace Opulos.Core.UI {
///<summary>It's possible to leverage the UpDownBase to represent a spinner control. The main disadvantage is that
///it's not possible to increase the width of the buttons, e.g. when the font is made larger. The width will
///always be 16 pixels.</summary>
public class UpDownSpinner : UpDownBase, IUpDown {

	public event EventHandler UpClicked;
	public event EventHandler DownClicked;
	public bool FocusParentOnClick { get; set; }

	public UpDownSpinner() {
		Dock = DockStyle.Right;
		BorderStyle = BorderStyle.None;
		TabStop = false; // otherwise textbox gets the focus when tabbing
		SetStyle(ControlStyles.Selectable, false);
		Cursor = Cursors.Default;
	}

	public override void DownButton() {
		if (FocusParentOnClick)
			Parent.Focus();

		if (DownClicked != null)
			DownClicked(this, EventArgs.Empty);
	}

	//Owner.KeyUpDown(1);
	public override void UpButton() {
		if (FocusParentOnClick)
			Parent.Focus();

		if (UpClicked != null)
			UpClicked(this, EventArgs.Empty);
	}

	protected override void OnFontChanged(EventArgs e) {
		base.OnFontChanged(e);
		this.Size = GetPreferredSize(Size.Empty);
	}

	protected override void UpdateEditText() {}

	public override Size GetPreferredSize(Size proposedSize) {
		Size s = Size.Empty;
		// UpDownBase is a TextBox combined with buttons.
		// Only return the size of the buttons as the preferred size
		foreach (Control c in this.Controls) {
			if (c is TextBox)
				continue;
			s = c.PreferredSize;
			break;
		}
		return s;
	}
}


}
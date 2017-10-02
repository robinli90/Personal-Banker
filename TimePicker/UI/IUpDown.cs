using System;
using System.Drawing;

namespace Opulos.Core.UI {

///<summary>This interface allows any custom control to be used as the MaskedTextBox&lt;T&gt;'s Spinner control.</summary>
public interface IUpDown {

	///<summary>Event occurs when the up button is clicked.</summary>
	event EventHandler UpClicked;

	///<summary>Event occurs when the down button is clicked.</summary>
	event EventHandler DownClicked;

	///<summary>An option to automatically focus on the parent after a button is clicked.</summary>
	bool FocusParentOnClick { get; set; }

	///<summary>Gets or sets the size of the control.</summary>
	Size Size { get; set; }

	///<summary>Gets the preferred size of the control.</summary>
	Size PreferredSize { get; }

	///<summary>Causes the control to repaint itself.</summary>
	void Refresh();
}


}
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Opulos.Core.UI {

///<summary>
///Attaches a drop down menu to a control, like the drop down window of a combo box.
///</summary>
public class ToolStripDropDownAttacher : IMessageFilter {

	public event CancelEventHandler MenuShowing;

	public ToolStripDropDown Menu { get; private set; }
	public Control Control { get; private set; }

	private bool isFocusing = false;
	private bool isClosing = false;

	public bool KeepMenuOpen { get; set; }

	///<summary></summary>
	///<param name="keepMenuOpen">An open to keep the drop down menu open if the main window is no longer the active window.</param>
	public ToolStripDropDownAttacher(ToolStripDropDown menu, Control c, bool keepMenuOpen = true) {
		Menu = menu;
		Control = c;
		KeepMenuOpen = keepMenuOpen;
		menu.AutoClose = false;

		menu.Click += delegate {
			isFocusing = true;
			menu.Focus();
			IntPtr h = GetTopWindow(c.Handle);
			// after the main window is minimized and restored, clicking on
			// clock will deactivate the main window title bar unless this is called:
			SendMessage(h, WM_NCACTIVATE, (IntPtr) 1, (IntPtr) 0);
			isFocusing = false;
		};

		menu.Closing += (o, e) => {
			if (isClosing)
				e.Cancel = false;
		};

		menu.KeyDown += (o, e) => {
			if (e.KeyCode == Keys.Escape && !menu.AutoClose) {
				CloseMenu(ToolStripDropDownCloseReason.Keyboard);
				e.Handled = true;
				//e.SuppressKeyPress = true;
			}
		};

		menu.GotFocus += delegate {
			// click on the control, then tab to focus on the drop down
			// then minimize the main window, then restore the main window
			// tabbing into the drop down deactivates the main window
			var tl = GetTopWindow(c.Handle);
			SendMessage(tl, WM_NCACTIVATE, (IntPtr) 1, (IntPtr) 0);
		};

		menu.LostFocus += delegate {
			// if another window is clicked, then two issues occur:
			// 1) The top level window doesn't deactivate, so two windows on the
			// desktop both appear active.
			// 2) When the focus returns to this program's main window, clicking
			// on thedrop down shows this window's caption as deactivated.

			if (!c.Focused && !isFocusing && !isClosing) {
				if (KeepMenuOpen) {
					IntPtr fg = GetForegroundWindow();
					IntPtr tl = GetTopWindow(c.Handle);
					bool b = fg == tl;
					if (!b) {
						SendMessage(tl, WM_NCACTIVATE, (IntPtr) 0, (IntPtr) 0);
						return;
					}
				}

				CloseMenu(ToolStripDropDownCloseReason.CloseCalled);
			}
		};

		c.MouseDown += delegate {
			ShowMenu();
		};

		c.GotFocus += delegate {
			if (!menu.Visible && !isClosing)
				ShowMenu();
		};

		c.LostFocus += delegate {
			// if the focus was transferred to another window, then keep
			// the drop down open. Otherwise if the focus was transfered to
			// another control in the same window then close the menu
			bool b = true;
			if (KeepMenuOpen) {
				IntPtr fg = GetForegroundWindow();
				IntPtr tl = GetTopWindow(c.Handle);
				b = tl == fg;
			}

			if (!menu.Focused && b) {
				CloseMenu(ToolStripDropDownCloseReason.CloseCalled);
			}
		};

		c.KeyDown += (o, e) => {
			if (e.KeyData == Keys.Escape) {
				e.Handled = true;
				e.SuppressKeyPress = true; // stops the beep
				if (menu.Visible) {
					CloseMenu(ToolStripDropDownCloseReason.Keyboard);
				}
			}
		};

		c.EnabledChanged += delegate {
			menu.Enabled = c.Enabled;
		};

		Application.AddMessageFilter(this);
	}

	public bool PreFilterMessage(ref Message m) {
		// seems like PreFilterMessage is the only way to prevent the focus from transferring to the next control
		// on the same main window. The TAB key is never fired in the c.KeyDown event, and trying to use
		// PreviewKeyDown doesn't give an option to block the key (unlike overriding the ProcessCmdKey method).
        if (!Menu.IsDisposed)
		if (m.HWnd == Control.Handle && m.Msg == WM_KEYDOWN && m.WParam.ToInt32() == VK_TAB && Menu.Visible && Menu.TabStop) {
			isFocusing = true;
			Menu.Focus();
			isFocusing = false;
			return true;
		}
		return false;
	}

	private static IntPtr GetTopWindow(IntPtr hWnd) {
		while (true) {
			IntPtr p2 = GetParent(hWnd);
			// some windows return their own handle if they are topmost
			if (p2 == hWnd || p2 == IntPtr.Zero)
				break;
			hWnd = p2;
		}
		return hWnd;
	}

	public void CloseMenu(ToolStripDropDownCloseReason reason) {
		isClosing = true;
		Menu.Close(reason);
		isClosing = false;
	}

	public void ShowMenu() {
		if (MenuShowing != null) {
			CancelEventArgs e = new CancelEventArgs();
			MenuShowing(this, e);
			if (e.Cancel)
				return;
		}

		IntPtr hWnd = GetTopWindow(Control.Handle);
		Menu.SnapWindow(Control.Handle, hWnd, new SnapPoint { ParentHeightFactor = 1, OffsetConstantY = 1 });
		RECT r = new RECT();
		GetWindowRect(Control.Handle, ref r);
		// Show(Control, Point) is indented by the control's border, e.g. a TextBox has an internal
		// border of 2, so the control needs to be shifted left. This inconsistency makes using
		// Show(Control, Point) no good, so GetWindowRect is used instead.
		// Menu.Show(this, new Point(-2, this.Height)); <-- no good
		Menu.Show(new Point(r.Left, r.Bottom + 1));
		// Required to set the control back on top (e.g. click off the window, then click back on the
		// window, the ClockMenu will appear underneath).
		// if SWP_NOACTIVATE is not used, then the main window will lose the focus when this drop down is closed.
		// this is similar to Control.BringToFront(), but BringToFront doesn't use the SWP_NOACTIVATE flag.
		SetWindowPos(Menu.Handle, (IntPtr) 0, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
	}

	private const int WM_NCACTIVATE = 0x86;
	private const int WM_KEYDOWN = 0x100;
	private const int VK_TAB = 0x09;
	private const int SWP_NOSIZE = 0x0001;
	private const int SWP_NOACTIVATE = 0x0010;
	private const int SWP_NOMOVE = 0x0002;

	[DllImport("user32.dll", SetLastError=true)]
	private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll", SetLastError=true)]
	private static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);

	[DllImport("user32.dll")]
	private static extern void SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll")]
	private static extern IntPtr GetParent(IntPtr hWnd);

	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();
}

}
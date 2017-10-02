using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Opulos.Core.UI {

///<summary>When a token is changed, by the up/down arrows, page up/page down keys, the spinner buttons, or by typing, then
///a token changing event is fired.</summary>
public class TokenChangeArgs : CancelEventArgs {

	public TokenChangeArgs(MaskedTextBox source, Token token) {
		this.Source = source;
		this.Token = token;
		this.IsTextValid = true;
	}

	///<summary>The token's owning MaskedTextBox control.</summary>
	public MaskedTextBox Source { get; private set; }

	///<summary>This field is assigned the value of what the MaskTextBox Text property will be
	///(as calculated by the MaskedTextBox<T> implementation). Lisetners can change this Text value
	///if the Text would not be valid. For example, if the month token was changed to Feb, but the
	///day token was still at 30, then the listener could change the text to Feb 28.
	///</summary>
	public String Text { get; set; }

	///<summary>A flag that indicates if the Text is valid. The Text should always be
	///valid, however it is possible to specify a custom values list on a non-literal token
	///that does not pass validation.</summary>
	public bool IsTextValid { get; internal set; }

	///<summary>The source Token object that was changed.</summary>
	public Token Token { get; internal set; }

	///<summary>The current value of the token text with the padding characters removed.</summary>
	public String CurrentTokenText { get; internal set; }

	///<summary>The value of the object's text from the custom values list by calling the object's ToString() method.</summary> 
	public String NewTokenText { get; internal set; }

	///<summary>The text of the current token as it currently appears in the MaskedTextBox. Note: the visual represenation
	///of the text might not be the same, as the MaskedTextBox may display spaces as the prompt character.</summary>
	public String CurrentTokenTextPadded { get; internal set; }
	
	///<summary>The new token's text with padding. The padding only applies to tokens that have a
	///custom values list. For non-literal tokens, the number of characters in the original mask represents
	///maximum allowable length. If the selected value from the list is longer, then it is truncated,
	///and this field contains the truncated value.</summary>
	public String NewTokenTextPadded { get; internal set; }

	///<summary>The new mask text using the NewLiteralEscaped text. It's possible for event listeners
	///to set this value, which will set as the new Mask property after the event. Only Tokens that are
	///literals that have a CustomValues list will change the mask. Literal tokens have to change the
	///mask because the new mask contains the escaped text (ToString) of selected item.</summary>
	public String Mask { get; set; }
}

public class TokenDeleteArgs : CancelEventArgs {

	public MaskedTextBox Owner;
	public String NewText;
	public String CurrentText;
	public Token[] Tokens;
	public String[] CurrentTokenTexts;
	public String[] NewTokenTexts;
	public String[] Substrings;
	public int KeyCode;

	///<summary>Be default, when text is deleted, the same selection start is maintained. This value can be changed to
	///change which text is selected after the delete.</summary>
	public int SelectionStart = 0;

	///</summary>By default, when text is deleted, the same selection length is maintained. This value can be changed to
	///change which text is selected after the delete.</summary>
	public int SelectionLength = 0;
}

public delegate void TokenChangeEventHandler(Object sender, TokenChangeArgs e);
public delegate void TokenDeleteEventHandler(Object sender, TokenDeleteArgs e);


public delegate void ValueChangedEventHandler<T>(Object sender, ValueChangedEventArgs<T> e);
public delegate void ValueChangingEventHandler<T>(Object sender, ValueChangingEventArgs<T> e);

public class ValueChangedEventArgs<T> : EventArgs {
	public T OldValue { get; private set; }
	public T NewValue { get; private set; }

	public ValueChangedEventArgs(T oldValue, T newValue) {
		OldValue = oldValue;
		NewValue = newValue;
	}
}

public class ValueChangingEventArgs<T> : CancelEventArgs {
	public T OldValue { get; private set; }
	public T NewValue { get; set; }

	public ValueChangingEventArgs(T oldValue, T newValue) {
		OldValue = oldValue;
		NewValue = newValue;
	}
}


}
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Opulos.Core.UI {


///<summary>
///A token is sequence of characters within a mask that are either all editable or all literals.
///There is an additional restriction that literal tokens are not allowed to contain shifting characters
///(the &lt; &gt; and | characters that change to uppercase, lowercase or no change). So if a sequence
///of literal characters contains a shifting character within it, then there will be one token for each.
///
///
///</summary>
public class Token {

	internal Token(int seqNo, String text, String mask) {
		this.SeqNo = seqNo;
		this.Text = text;
		this.SmallIncrement = 1;
		decimal max = 1;
		for (int i = 0; i < mask.Length; i++) {
			char c = mask[i];
			if (c == '.')
				break;
			if (c == '0' || c == '9' || c == '#')
				max *= 10;
		}
		this.MaxValue = max;
		this.BigIncrement = Math.Max(2, max / 20);
		this.CarryOverScope = int.MaxValue;
	}

	///<summary>The default value is null, which inherits from the parent ValuesWrapAround property.</summary>
	public bool? ValueWrapsAround { get; set; }

	///<summary>The position of this token in the Tokens collection.</summary>
	public int SeqNo { get; private set; }

	///<summary>A continuous sequence of characters that are all editiable or all literals. The text is a substring of the
	///MaskedTextBox's Text property (including prompts and literals) at the time when the Mask was changed.</summary>
	public String Text { get; internal set; }

	///<summary>The index position of the first character of the raw text in the overall Text value at the time the Mask was changed.</summary>
	public int StartIndex { get; internal set; }

	///<summary>The length of the RawText.</summary>
	public int Length { get { return Text.Length; } }

	///<summary>True means that the characters in RawText are literals, as determined by the MaskedTextBox's MaskedTextProvider at the time the Mask was changed.</summary>
	public bool IsLiteral { get; internal set; }

	///<summary>The minimum value this token can take when the user changes the value by typing a key, the mouse wheel or spinner buttons.
	///The default value is 0.</summary>
	public decimal MinValue { get; set; }

	///<summary>The maximum value this token can take when the user changes the value by typing a key, the mouse wheel or spinner buttons.
	///The default value is the power of 10 to the length of the RawText.</summary>
	public decimal MaxValue { get; set; }

	///<summary>The amount the value changes by the up/down arrows, or spinner buttons. The default value is 1.</summary>
	public decimal SmallIncrement { get; set; }

	///<summary>The amount the value changes by the page up/page down keys. The default value is 1/20th the MaxValue.</summary> 
	public decimal BigIncrement { get; set; }

	///<summary>Specifies if the token to the left increments or decrements if this token's value is wrapped. If this value is null,
	///then the MaskedTextBox ValuesCarryOver property is used. Note: CarryOver is ignored if ByDigit is true.</summary>
	public bool? CarryOver { get; set; }

	///<summary>
	///Specifies how many editable tokens to the left will increment or decrement if this token's value is wrapped.
	///The ValuesWrapAround property must be set to true. The default value is set to int.MaxValue, which means the
	///scope is essentially infinite.
	///</summary>
	public int CarryOverScope { get; set; }

	///<summary>There are two different behaviors for changing a token's value. ByDigit true means that the digit
	///at the location of the caret changes independently of the other digits in the token. ByDigit cannot be
	///used when KeepTokenSelected is true. Also, CarryOver is ignored if ByDigit is true. ByDigit true is useful
	///for tokens than can take a value of 100 or more, because at that point the user is more likely to type the
	///number rather than scroll to the desired number. However, a year token might not want to use ByDigit since
	///it's likely the user will pick a number close to the current year. The default value of ByDigit is false,
	///which means the KeyUpDown amount is added to the current value of the token. Both input styles obey the
	///MinValue and MaxValue constraints. ByDigit null means it inherits from the ByDigit of the owning MaskedTextBox.
	///</summary>
	public bool? ByDigit { get; set; }

	///<summary>Returns the unescaped version of the mask, i.e. the backslashes are removed from the literals
	///and the &gt; &lt; and | special characters are skipped.</summary>
	public String Mask { get; internal set; }

	///<summary>Returns the Mask text that contains the literals, backslashes, and control characters, etc.</summary>
	public String MaskRaw { get; internal set; }

	///<summary>The index position of the first character in the Mask property, which contains the control characters.</summary>
	public int MaskRawStartIndex { get; internal set; }

	///<summary>The length of the corresponding characters in the mask, which could be longer than Length. For example, if the Mask contains
	///special characters that change to uppercase or lowercase.</summary>
	public int MaskRawLength { get { return MaskRaw.Length; } }

	///<summary>Returns an array that is the same length as the Mask property, where a true value means the
	///character is converted to uppercase, false to lowercase, and null means no conversion.</summary>
	public bool?[] Shifts { get; internal set; }

	///<summary>A custom list of characters that the MaskedTextBox will cycle through. If null, then the
	///a default list is used based on the allowable characters for the mask position, and whether a shift
	///control character is used to convert to uppercase or lowercase characters.</summary>
	public IList<char> ByDigitChars { get; set; }

	///<summary>If specified, then the custom value is padded with this character. The property applies for
	///tokens that have a CustomValues list, and for tokens that allow user keyboard input to delete
	///one or more characters.</summary>
	public char? PadChar { get; set; }

	///<summary>The default value is to pad the characters on the right side. This this property to true to
	///have the characters pad on the left side. The PadChar must be set. This property applies to tokens
	///that have a CustomValues list, as well as when for tokens that allow user keyboard input to delete
	///one or more characters.</summary> 
	public TokenValuePadRule PadRule { get; set; }

	///<summary>An option to switch the up arrow or page up with the down arrow or page down. For example, a
	///custom values list of month names is [January, ... , December]. It is more natural that down scrolls
	///from smaller to larger.</summary>
	public bool ReverseUpDown { get; set; }

	///<summary>An option to override the default value set in the MaskedTextBox parent. Only applies when
	///KeepTokenSelected is true.</summary>
	public bool? KeepSelectedIncludesWhitespace { get; set; }

	///<summary>Returns a bitwise enum value that indicates which characters can be typed in this token.
	///Multiple flags might be set if a token mixes multiple different wildcard characters in a row.</summary>
	public TokenInputFlags AllowedChars {
		get {
			if (this.IsLiteral)
				return TokenInputFlags.Literals;

			TokenInputFlags f = (TokenInputFlags) 0;
			foreach (char c in Mask) {
				if (c == '$' || c == '.' || c == ',' || c == '/' || c == ':')
					f = f | TokenInputFlags.Placeholder;
				else if (c == '0')
					f = f | TokenInputFlags.Digits;
				else if (c == '9' || c == '#')
					f = f | TokenInputFlags.Digits | TokenInputFlags.Spaces;
				else if (c == 'L' || c == '&' || c == 'A' || c == 'a')
					f = f | TokenInputFlags.Letters;
				else if (c == '?' || c == 'C')
					f = f | TokenInputFlags.Letters | TokenInputFlags.Spaces;
				else {
				}
			}
			return f;
		}
	}

	///<summary>A custom list of values can be assigned to a token, which defines the allowable values that
	///can be selected. The selectable values from one token, may depend on the current value of another token,
	///so it is possible to listen for the TokenChanging event and assign the CustomValues dynamically.</summary>
	public IList<Object> CustomValues { get; set; }

	///<summary>The MaskedTextProvider for the entire MaskedTextBox as provided by the MaskedTextBox after the Mask property is set.</summary>
	public MaskedTextProvider OwnerTextMaskProvider { get; internal set; }

	///<summary>Returns true if this token has non-empty display text and is either not a literal or has a non-null CustomValues list.</summary>
	public bool CanEdit {
		get {
			// Text.Length == 0 if the token only contain shifting characters
			return this.Text.Length > 0 && (!IsLiteral || CustomValues != null);
		}
	}

	///<summary>If null, then the MaskedTextBox's ValueTooSmallFixMode is used.</summary>
	public ValueFixMode? ValueTooSmallFixMode { get; set; }

	///<summary>If null, then the MaskedTextBox's ValueTooSmallFixMode is used.</summary>
	public ValueFixMode? ValueTooLargeFixMode { get; set; }

	///<summary>Returns the SeqNo concatenated with the RawText from the mask.</summary>
	public override String ToString() {
		return SeqNo + "  " + Text;
	}
}



}
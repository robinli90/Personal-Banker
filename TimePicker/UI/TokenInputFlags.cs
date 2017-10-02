using System;

namespace Opulos.Core.UI {

///<summary>For a token object, the input flags indicate what types of characters its corresponding Mask accepts.
[Flags]
public enum TokenInputFlags {
	///<summary>The token contains at least one wildcard digit characters 0, 9 or #.</summary>
	Digits = 1,
	///<summary>The token contains at least one wildcard character that can accept spaces.</summary>
	Spaces = 2,
	///<summary>The token contains at least one wildcard character that can accepts letters.</summary>
	Letters = 4,
	///<summary>The token contains one of the reserved special characters for currency, time, or numeric characters $ : / . ,</summary>
	Placeholder = 8,
	///<summary>If this flag is set, then the token is a literal token. This flag is not used with the other flags. A literal token
	///can accept any kind of character.</summary>
	Literals = 16
}


}
using System;

namespace Opulos.Core.UI {

public enum TokenValuePadRule {

	///<summary>The token uses the default behavior depending on the type of token. A token that contains
	///a CustomValues list will pad right. A token that is a numeric type will pad left.</summary>
	Default = 0,

	///<summary>Specifies that if the token value is not long enough, then characters are padded on the left side.</summary>
	Left = 1,

	///<summary>Specifies that if the token value is not long enough, then characters are padded on the right side.</summary>
	Right = 2,
}



}
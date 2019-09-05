using System.Collections.Generic;

namespace Umbrella.Utilities.Encryption.Options
{
	/// <summary>
	/// Options for the <see cref="SecureStringGenerator"/> class.
	/// </summary>
	public class SecureStringGeneratorOptions
	{
		/// <summary>
		/// Gets or sets the special characters to choose from. The default characters are: !@#$&amp;%
		/// </summary>
		/// <value>
		/// The special characters.
		/// </value>
		public IReadOnlyList<char> SpecialCharacters { get; set; } = new[] { '!', '@', '#', '$', '&', '%' };
	}
}
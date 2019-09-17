using System.Collections.Generic;
using Umbrella.Utilities.Abstractions;

namespace Umbrella.Utilities.Encryption.Options
{
	/// <summary>
	/// Options for the <see cref="SecureRandomStringGenerator"/> class.
	/// </summary>
	public class SecureRandomStringGeneratorOptions : IValidatableUmbrellaOptions
	{
		/// <summary>
		/// Gets or sets the special characters to choose from. The default characters are: !@#$&amp;%
		/// </summary>
		/// <value>
		/// The special characters.
		/// </value>
		public IReadOnlyList<char> SpecialCharacters { get; set; } = new[] { '!', '@', '#', '$', '&', '%' };

		/// <summary>
		/// Validates this instance.
		/// </summary>
		public void Validate()
		{
			Guard.ArgumentNotNull(SpecialCharacters, nameof(SpecialCharacters));
		}
	}
}
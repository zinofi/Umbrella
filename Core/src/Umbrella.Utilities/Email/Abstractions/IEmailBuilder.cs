using System;
using System.Collections.Generic;

namespace Umbrella.Utilities.Email.Abstractions
{
	/// <summary>
	/// A generic class to aid in building emails based on HTML generated templates stored as static files on disk.
	/// </summary>
	public interface IEmailBuilder
	{
		/// <summary>
		/// Specify either an email template filename, or supply a raw html string to use instead in conjunction with the <paramref name="isRawHtml"/> parameter.
		/// </summary>
		/// <param name="source">The source template file or raw html to use.</param>
		/// <param name="isRawHtml">Indicates whether the source is a file or raw html.</param>
		/// <returns>The <see cref="EmailBuilder"/>.</returns>
		EmailBuilder UsingTemplate(string source = "GenericTemplate", bool isRawHtml = false);

		/// <summary>
		/// Appends a data row to the email data rows builder. The data rows builder will ultimately be substituted for all instances of {rows} in the email template.
		/// </summary>
		/// <param name="name">The name of the data item, e.g. First Name</param>
		/// <param name="value">The value of the data item, e.g. Rich</param>
		/// <returns>The <see cref="EmailBuilder"/>.</returns>
		EmailBuilder AppendRow(string name, string value);

		/// <summary>
		/// Appends the data rows specified in the <paramref name="source"/> collection.
		/// </summary>
		/// <typeparam name="T">The type of the data item being appended.</typeparam>
		/// <param name="source">The source collection containing instances of <typeparamref name="T"/>.</param>
		/// <param name="rowsTokenName">Name of the rows token in the email template to be replaced by these data rows.</param>
		/// <param name="rowFormat">The row format.</param>
		/// <param name="keySelector">The key selector.</param>
		/// <param name="valueSelector">The value selector.</param>
		/// <returns>The <see cref="EmailBuilder"/>.</returns>
		EmailBuilder AppendDataRows<T>(IEnumerable<T> source, string rowsTokenName, string rowFormat, Func<T, string> keySelector, Func<T, string> valueSelector);

		/// <summary>
		/// Replaces the specified <paramref name="tokenName"/> with the specified <paramref name="value"/>.
		/// </summary>
		/// <param name="tokenName">Name of the token.</param>
		/// <param name="value">The value to replace the token.</param>
		/// <returns>The <see cref="EmailBuilder"/>.</returns>
		EmailBuilder ReplaceToken(string tokenName, string value);
	}
}
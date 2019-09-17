using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Email.Abstractions;
using Umbrella.Utilities.Email.Options;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Hosting.Abstractions;

namespace Umbrella.Utilities.Email
{
	/// <summary>
	/// A generic class to aid in building emails based on HTML generated templates stored as static files on disk.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Email.Abstractions.IEmailBuilder" />
	public class EmailBuilder : IEmailBuilder
	{
		#region Private Static Members
		private static readonly CultureInfo _cultureInfo = new CultureInfo("en-GB");
		private static volatile Dictionary<string, string> _emailTemplateDictionary;
		private static volatile bool _isInitialized;
		private static readonly object _syncRoot = new object();
		#endregion

		#region Private Members
		private readonly ILogger _log;
		private readonly EmailBuilderOptions _options;
		private readonly IUmbrellaHostingEnvironment _hostingEnvironment;
		private StringBuilder _builder;
		private readonly StringBuilder _rowsBuilder = new StringBuilder();
		#endregion

		#region Constructors				
		/// <summary>
		/// Initializes a new instance of the <see cref="EmailBuilder"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="options">The options.</param>
		/// <param name="hostingEnvironment">The hosting environment.</param>
		/// <exception cref="UmbrellaException">There has been a problem initializing the email builder instance.</exception>
		public EmailBuilder(
			ILogger<EmailBuilder> logger,
			EmailBuilderOptions options,
			IUmbrellaHostingEnvironment hostingEnvironment)
		{
			_log = logger;
			_options = options;
			_hostingEnvironment = hostingEnvironment;

			try
			{
				if (!_isInitialized)
				{
					lock (_syncRoot)
					{
						if (!_isInitialized)
						{
							options.Validate();

							string absolutePath = _hostingEnvironment.MapPath(options.TemplatesVirtualPath);

							var dicItems = new Dictionary<string, string>();

							foreach (string filename in Directory.EnumerateFiles(absolutePath, "*.html", SearchOption.TopDirectoryOnly))
							{
								//Read all template files into memory and store in the dictionary
								using (var fileStream = new FileStream(filename, FileMode.Open))
								{
									using (var reader = new StreamReader(fileStream))
									{
										string template = reader.ReadToEnd();

										dicItems.Add(Path.GetFileNameWithoutExtension(filename), template);
									}
								}
							}

							_emailTemplateDictionary = dicItems;
							_isInitialized = true;
						}
					}
				}
			}
			catch (Exception exc) when (_log.WriteError(exc, new { options }, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem initializing the email builder instance.", exc);
			}
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// Specify either an email template filename, or supply a raw html string to use instead in conjunction with the <paramref name="isRawHtml"/> parameter.
		/// </summary>
		/// <param name="source">The source template file or raw html to use.</param>
		/// <param name="isRawHtml">Indicates whether the source is a file or raw html.</param>
		/// <returns>The <see cref="EmailBuilder"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="source"/> is empty or whitespace.</exception>
		/// <exception cref="UmbrellaException">There was a problem initializing the builder using the specified options.</exception>
		public EmailBuilder UsingTemplate(string source = "GenericTemplate", bool isRawHtml = false)
		{
			Guard.ArgumentNotNullOrWhiteSpace(source, nameof(source));

			try
			{
				_builder = isRawHtml
						? new StringBuilder(source)
						: new StringBuilder(_emailTemplateDictionary[source]);

				// Make sure the date is shown in the correct format
				_builder.Replace("{datetime}", DateTime.Now.ToString(_cultureInfo));

				return this;
			}
			catch (Exception exc) when (_log.WriteError(exc, new { source, isRawHtml }, returnValue: true))
			{
				throw new UmbrellaException("There was a problem initializing the builder using the specified options.", exc);
			}
		}

		/// <summary>
		/// Appends a data row to the email data rows builder. The data rows builder will ultimately be substituted for all instances of {rows} in the email template.
		/// </summary>
		/// <param name="name">The name of the data item, e.g. First Name</param>
		/// <param name="value">The value of the data item, e.g. Rich</param>
		/// <returns>The <see cref="EmailBuilder"/>.</returns>
		/// <exception cref="UmbrellaException">There was a problem appending the data row with the specified name and value.</exception>
		public EmailBuilder AppendRow(string name, string value)
		{
			try
			{
				_rowsBuilder.AppendFormat(_options.DataRowFormat, name, !string.IsNullOrEmpty(value) ? value.Replace(Environment.NewLine, "<br />") : string.Empty);

				return this;
			}
			catch (Exception exc) when (_log.WriteError(exc, new { name, value }, returnValue: true))
			{
				throw new UmbrellaException("There was a problem appending the data row with the specified name and value.", exc);
			}
		}

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
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="rowsTokenName"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="rowsTokenName"/> is empty or whitespace.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="rowFormat"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="rowFormat"/> is empty or whitespace.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="keySelector"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="valueSelector"/> is null.</exception>
		/// <exception cref="UmbrellaException">There was a problem appending the specified data rows.</exception>
		public EmailBuilder AppendDataRows<T>(IEnumerable<T> source, string rowsTokenName, string rowFormat, Func<T, string> keySelector, Func<T, string> valueSelector)
		{
			Guard.ArgumentNotNull(source, nameof(source));
			Guard.ArgumentNotNullOrWhiteSpace(rowsTokenName, nameof(rowsTokenName));
			Guard.ArgumentNotNullOrWhiteSpace(rowFormat, nameof(rowFormat));
			Guard.ArgumentNotNull(keySelector, nameof(keySelector));
			Guard.ArgumentNotNull(valueSelector, nameof(valueSelector));

			try
			{
				var builder = new StringBuilder();

				foreach (T item in source)
				{
					builder.AppendFormat(rowFormat, keySelector(item), valueSelector != null ? valueSelector(item) : string.Empty);
				}

				ReplaceToken(rowsTokenName, builder.ToString());

				return this;
			}
			catch (Exception exc) when (_log.WriteError(exc, new { rowsTokenName, rowFormat }, returnValue: true))
			{
				throw new UmbrellaException("There was a problem appending the specified data rows.", exc);
			}
		}

		/// <summary>
		/// Replaces the specified <paramref name="tokenName"/> with the specified <paramref name="value"/>.
		/// </summary>
		/// <param name="tokenName">Name of the token.</param>
		/// <param name="value">The value to replace the token.</param>
		/// <returns>The <see cref="EmailBuilder"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="tokenName"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="tokenName"/> is empty or whitespace.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is empty or whitespace.</exception>
		/// <exception cref="UmbrellaException">There has been a problem replacing the specified token with the specified value.</exception>
		public EmailBuilder ReplaceToken(string tokenName, string value)
		{
			Guard.ArgumentNotNullOrWhiteSpace(tokenName, nameof(tokenName));
			Guard.ArgumentNotNullOrWhiteSpace(value, nameof(value));

			try
			{
				ThrowIfNotInitialized();
				_builder.Replace("{" + tokenName + "}", value);

				return this;
			}
			catch (Exception exc) when (_log.WriteError(exc, new { tokenName }, returnValue: true))
			{
				throw new UmbrellaException("There has been a problem replacing the specified token with the specified value.", exc);
			}
		}
		#endregion

		#region Overridden Methods		
		/// <summary>
		/// Converts to string.
		/// </summary>
		/// <returns>
		/// A <see cref="string" /> that represents this instance.
		/// </returns>
		/// <exception cref="UmbrellaException">There was a problem outputting this instance to a string.</exception>
		public override string ToString()
		{
			try
			{
				ThrowIfNotInitialized();

				return _builder.Replace("{rows}", _rowsBuilder.ToString()).ToString();
			}
			catch (Exception exc) when (_log.WriteError(exc, returnValue: true))
			{
				throw new UmbrellaException("There was a problem outputting this instance to a string.", exc);
			}
		}
		#endregion

		#region Private Methods
		private void ThrowIfNotInitialized()
		{
			if (_builder == null)
				throw new InvalidOperationException($"This builder has not been initialized properly. You need to call the {nameof(UsingTemplate)} method before using it.");
		}
		#endregion
	}
}
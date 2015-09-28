using Umbrella.Utilities.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace Umbrella.Utilities.Email
{
	public class EmailBuilder
	{
		#region Constants
		private const string c_RowFormat = "<tr><td>{0}:</td><td>{1}</td></tr>";
		#endregion

		#region Private Static Members
		private static readonly CultureInfo s_CultureInfo = new CultureInfo("en-GB");
		private static Dictionary<string, string> s_EmailTemplateDictionary = new Dictionary<string, string>();
		private static readonly IHostingEnvironment s_HostingEnvironment;
		#endregion

		#region Private Instance Members
		private StringBuilder m_Builder;
		private StringBuilder m_RowsBuilder = new StringBuilder();
		#endregion

		#region Static Constructor
		static EmailBuilder()
		{
			s_HostingEnvironment = LibraryBindings.DependencyResolver.Get<IHostingEnvironment>();

			string absolutePath = s_HostingEnvironment.MapPath("~/Content/EmailTemplates/");

			foreach (string filename in Directory.EnumerateFiles(absolutePath, "*.html", SearchOption.TopDirectoryOnly))
			{
				//Read all template files into memory and store in the dictionary
				using (StreamReader reader = new StreamReader(filename))
				{
					string template = reader.ReadToEnd();

					s_EmailTemplateDictionary.Add(Path.GetFileNameWithoutExtension(filename), template);
				}
			}
		}
		#endregion

		#region Constructors
		public EmailBuilder()
			: this("GenericTemplate")
		{
		}

		/// <summary>
		/// Specify either an email template filename, or supply a raw html string to use instead
		/// </summary>
		/// <param name="source">The source template file or raw html to use</param>
		/// <param name="isRawHtml">Indicates whether the source is a file or raw html</param>
		public EmailBuilder(string source, bool isRawHtml = false)
		{
			if (isRawHtml)
				m_Builder = new StringBuilder(source);
			else
				m_Builder = new StringBuilder(s_EmailTemplateDictionary[source]);

			//Make sure the date is shown in the correct format
			m_Builder.Replace("{datetime}", DateTime.Now.ToString(s_CultureInfo));
		}
		#endregion

		#region Public Methods
		public EmailBuilder AppendRow(string name, string value)
		{
			m_RowsBuilder.AppendFormat(c_RowFormat, name, !string.IsNullOrEmpty(value) ? value.Replace(Environment.NewLine, "<br />") : string.Empty);

			return this;
		}

		public EmailBuilder AppendDataRows<T>(string rowsTokenName, string rowFormat, IEnumerable<T> source, Func<T, string> keySelector, Func<T, string> valueSelector)
		{
			StringBuilder builder = new StringBuilder();

			foreach (T item in source)
			{
				builder.AppendFormat(rowFormat, keySelector(item), valueSelector != null ? valueSelector(item) : string.Empty);
			}

			ReplaceToken(rowsTokenName, builder.ToString());

			return this;
		}

		public EmailBuilder ReplaceToken(string tokenName, string value)
		{
			m_Builder.Replace("{" + tokenName + "}", value);

			return this;
		}
		#endregion

		#region Overrides
		public override string ToString()
		{
			return m_Builder.Replace("{rows}", m_RowsBuilder.ToString()).ToString();
		}
		#endregion
	}
}
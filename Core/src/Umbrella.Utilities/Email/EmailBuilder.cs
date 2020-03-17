using Umbrella.Utilities.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Email.Interfaces;

namespace Umbrella.Utilities.Email
{
	// TODO: The folder path containing the email templates should be configurable.
	[Obsolete("This will be removed in the next v3 major release.")]
	public class EmailBuilder : IEmailBuilder
	{
		#region Constants
		private const string c_RowFormat = "<tr><td>{0}:</td><td>{1}</td></tr>";
		#endregion

		#region Private Static Members
		private static readonly CultureInfo s_CultureInfo = new CultureInfo("en-GB");
        private static volatile Dictionary<string, string> s_EmailTemplateDictionary;
        private static volatile bool s_IsInitialized;
        private static readonly object s_Lock = new object();
        #endregion

        #region Private Members
        private readonly IUmbrellaHostingEnvironment m_HostingEnvironment;
		private StringBuilder m_Builder;
		private StringBuilder m_RowsBuilder = new StringBuilder();
		#endregion

		#region Constructors
        public EmailBuilder(IUmbrellaHostingEnvironment hostingEnvironment)
        {
            m_HostingEnvironment = hostingEnvironment;

            if(!s_IsInitialized)
            {
                lock(s_Lock)
                {
                    if(!s_IsInitialized)
                    {
                        string absolutePath = m_HostingEnvironment.MapPath("~/Content/EmailTemplates/");

                        Dictionary<string, string> dicItems = new Dictionary<string, string>();

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

                        s_EmailTemplateDictionary = dicItems;
                        s_IsInitialized = true;
                    }
                }
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Specify either an email template filename, or supply a raw html string to use instead
        /// </summary>
        /// <param name="source">The source template file or raw html to use</param>
        /// <param name="isRawHtml">Indicates whether the source is a file or raw html</param>
        public EmailBuilder UsingTemplate(string source = "GenericTemplate", bool isRawHtml = false)
        {
            m_Builder = isRawHtml
                ? new StringBuilder(source)
                : new StringBuilder(s_EmailTemplateDictionary[source]);

            //Make sure the date is shown in the correct format
            m_Builder.Replace("{datetime}", DateTime.Now.ToString(s_CultureInfo));

            return this;
        }

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
            ThrowIfNotInitialized();

			m_Builder.Replace("{" + tokenName + "}", value);

			return this;
		}
        #endregion

        #region Overridden Methods
        public override string ToString()
        {
            ThrowIfNotInitialized();

            return m_Builder.Replace("{rows}", m_RowsBuilder.ToString()).ToString();
        }
        #endregion

        #region Private Methods
        private void ThrowIfNotInitialized()
        {
            if (m_Builder == null)
                throw new InvalidOperationException($"This builder has not been initialized properly. You need to call the {nameof(UsingTemplate)} method before using it.");
        }
        #endregion
    }
}
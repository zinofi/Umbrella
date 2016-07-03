using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net.Mail;
using System.IO;
using Umbrella.Utilities.Email.Interfaces;

namespace Umbrella.Utilities.Email
{
    public class EmailHelper : IEmailHelper, IDisposable
    {
        #region Private Members
        private SmtpClient m_SmtpClient;
        private NameValueCollection m_Substitutions;
        private NameValueCollection m_RawSubstitutions;
        #endregion Private Members

        #region Constructors
        public EmailHelper()
        {
            m_SmtpClient = null;
            m_Substitutions = null;
            m_RawSubstitutions = null;  // Arguments that should not be xml escaped
            IsBodyHtml = true;
        }
        #endregion Constructors

        #region Public Properties
        public string Error { get; private set; }
        public bool IsBodyHtml { get; set; }
        #endregion Properties

        #region Public Methods
        /// <summary>
        /// Adds placeholder key (without braces) and its replacement value
        /// </summary>
        /// <param name="keyExcludingCurlyBraces"></param>
        /// <param name="replacementValue"></param>
        public void AddSubstitution(string keyExcludingCurlyBraces, string replacementValue)
        {
            if (m_Substitutions == null)
                m_Substitutions = new NameValueCollection();

            m_Substitutions.Add(string.Concat("{", keyExcludingCurlyBraces.Trim(), "}"), replacementValue);
        }

        /// <summary>
        /// Adds placeholder key (without braces) and its replacement value for a value that should not be xml escaped
        /// </summary>
        /// <param name="keyExcludingCurlyBraces"></param>
        /// <param name="replacementValue"></param>
        public void AddRawSubstitution(string keyExcludingCurlyBraces, string replacementValue)
        {
            if (m_RawSubstitutions == null)
                m_RawSubstitutions = new NameValueCollection();

            m_RawSubstitutions.Add(string.Concat("{", keyExcludingCurlyBraces.Trim(), "}"), replacementValue);
        }

        public void ClearSubstitutions()
        {
            if (m_Substitutions != null)
                m_Substitutions.Clear();
            if (m_RawSubstitutions != null)
                m_RawSubstitutions.Clear();
        }

        public bool SendEmail(string from, string to, string subject, string body)
        {
            return SendEmail(from, to, null, null, subject, body, null, null);
        }

        public bool SendEmail(string from, string to, string cc, string subject, string body)
        {
            return SendEmail(from, to, cc, null, subject, body, null, null);
        }

        public bool SendEmail(string from, string to, List<string> ccList, string subject, string body)
        {
            StringBuilder ccBuilder = new StringBuilder();
            if (ccList != null)
            {
                for (int i = 0; i < ccList.Count; i++)
                    ccBuilder.AppendFormat("{0}{1}", ccList[i], i < ccList.Count - 1 ? "," : string.Empty);
            }

            return SendEmail(from, to, ccBuilder.ToString(), null, subject, body, null, null);
        }

        public bool SendEmail(string from, string to, string cc, string bcc, string subject, string body)
        {
            return SendEmail(from, to, cc, bcc, subject, body, null, null);
        }

        public bool SendEmail(string from, string to, List<string> ccList, List<string> bccList, string subject, string body)
        {
            StringBuilder ccBuilder = new StringBuilder();
            if (ccList != null)
            {
                for (int i = 0; i < ccList.Count; i++)
                    ccBuilder.AppendFormat("{0}{1}", ccList[i], i < ccList.Count - 1 ? "," : string.Empty);
            }

            StringBuilder bccBuilder = new StringBuilder();
            if (bccList != null)
            {
                for (int i = 0; i < bccList.Count; i++)
                    bccBuilder.AppendFormat("{0}{1}", bccList[i], i < bccList.Count - 1 ? "," : string.Empty);

            }

            return SendEmail(from, to, ccBuilder.ToString(), bccBuilder.ToString(), subject, body, null, null);
        }

        public bool SendEmail(string from, string to, string cc, string bcc, string subject, string body, string smtpServer)
        {
            return SendEmail(from, to, cc, bcc, subject, body, null, smtpServer);
        }

        public bool SendEmail(string from, string to, string subject, string body, string[] attachments)
        {
            return SendEmail(from, to, null, null, subject, body, attachments, null);   
        }

        public bool SendEmail(string from, string to, string subject, string body, string[] attachments, string smtpServer)
        {
            return SendEmail(from, to, null, null, subject, body, attachments, smtpServer);
        }

        /// <summary>
        /// Allows an email message to be sent using various supplied parameters
        /// </summary>
        /// <param name="from">The from address</param>
        /// <param name="to">The to address</param>
        /// <param name="cc">A comma delimited string of cc addresses</param>
        /// <param name="bcc">A comma delimited string of bcc addresses</param>
        /// <param name="subject">The email subject</param>
        /// <param name="body">The email body</param>
        /// <param name="attchments">
        /// A string array of email file attachments. Each string must represent
        /// the full file path to the attachment.
        /// </param>
        /// <param name="smtpServer">The IP Address of the SMTP server</param>
        /// <returns>An empty string to indicate success or the message of any exception thrown on failure.</returns>
        public bool SendEmail(string from,
            string to,
            string cc,
            string bcc,
            string subject,
            string body,
            string[] attchments,
            string smtpServer)
        {
            Error = string.Empty;

            using (MailMessage message = new MailMessage(from, to))
            {
                try
                {
                    //Add any cc addresses
                    if(!string.IsNullOrEmpty(cc))
                        message.CC.Add(cc);

                    //Add any bcc addresses
                    if (!string.IsNullOrEmpty(bcc))
                        message.Bcc.Add(bcc);

                    message.Subject = DoSubjectSubstitutions(subject);
                    message.IsBodyHtml = IsBodyHtml;
                    message.Body = DoBodySubstitutions(body);

                    if (attchments != null)
                    {
                        foreach (string att in attchments)
                        {
                            if (File.Exists(att))
                            {
                                message.Attachments.Add(new Attachment(att));
                            }
                            else
                            {
                                throw new Exception(string.Format("The attachment \"{0}\" cannot be found, not attached.", att));
                            }
                        }
                    }

                    EnsureSmtpClient(smtpServer);
                    m_SmtpClient.Send(message);

                    message.Attachments.Dispose();
                    message.Dispose();
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                    return false;
                }
            }

            return true;
        }

        public void Dispose()
        {
			Dispose(true);
			GC.SuppressFinalize(this);
        }

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(m_SmtpClient != null)
				{
					m_SmtpClient.Dispose();
					m_SmtpClient = null;
				}
			}
		}

        #endregion Public Methods

        #region Private Methods
        private void EnsureSmtpClient(string smtpServer)
        {
            if (m_SmtpClient == null)
                m_SmtpClient = !string.IsNullOrEmpty(smtpServer) ? new SmtpClient(smtpServer) : new SmtpClient();
        }

        private string DoSubjectSubstitutions(string template)
        {
            string result = template;

            if (m_Substitutions != null)
            {
                foreach (string key in m_Substitutions.AllKeys)
                {
                    result = result.Replace(key, m_Substitutions[key]);
                }
            }

            if (m_RawSubstitutions != null)
            {
                foreach (string key in m_RawSubstitutions.AllKeys)
                {
                    result = result.Replace(key, m_RawSubstitutions[key]);
                }
            }
            return result;
        }

        private string DoBodySubstitutions(string template)
        {
            string result = template;

            if (m_Substitutions != null)
            {
                foreach (string key in m_Substitutions.AllKeys)
                {
                    string substitution = IsBodyHtml ? EscapeSpecialChars(m_Substitutions[key]) : m_Substitutions[key];
                    result = result.Replace(key, substitution);
                }
            }

            if (m_RawSubstitutions != null)
            {
                foreach (string key in m_RawSubstitutions.AllKeys)
                {
                    result = result.Replace(key, m_RawSubstitutions[key]);
                }
            }
            return result;
        }

        private string EscapeSpecialChars(string oldValue)
        {
            string newValue;
            if (!string.IsNullOrEmpty(oldValue))
            {
                newValue = oldValue.Replace("&", "&amp;");
                newValue = newValue.Replace("\"", "&quot;");
                newValue = newValue.Replace("'", "&apos;");
                newValue = newValue.Replace("<", "&lt;");
                newValue = newValue.Replace(">", "&gt;");
                newValue = newValue.Replace("\r\n", "<br/>");
            }
            else
            {
                newValue = string.Empty;
            }
            return newValue;
        }
        #endregion Private Methods
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.Utilities.Email.Interfaces
{
    public interface IEmailHelper
    {
        void AddSubstitution(string keyExcludingCurlyBraces, string replacementValue);
        void ClearSubstitutions();
        bool SendEmail(string from, string to, string subject, string body);
        bool SendEmail(string from, string to, string cc, string subject, string body);
        bool SendEmail(string from, string to, List<string> ccList, string subject, string body);
        bool SendEmail(string from, string to, string cc, string bcc, string subject, string body);
        bool SendEmail(string from, string to, List<string> ccList, List<string> bccList, string subject, string body);
        bool SendEmail(string from, string to, string cc, string bcc, string subject, string body, string smtpServer);
        bool SendEmail(string from, string to, string subject, string body, string[] attachments);
        bool SendEmail(string from, string to, string subject, string body, string[] attachments, string smtpServer);
        bool SendEmail(string from, string to, string cc, string bcc, string subject, string body, string[] attchments, string smtpServer);
        string Error { get; }
    }
}

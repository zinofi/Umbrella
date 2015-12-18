using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Email.Interfaces
{
    public interface IEmailBuilder
    {
        EmailBuilder UsingTemplate(string source = "GenericTemplate", bool isRawHtml = false);
        EmailBuilder AppendRow(string name, string value);
        EmailBuilder AppendDataRows<T>(string rowsTokenName, string rowFormat, IEnumerable<T> source, Func<T, string> keySelector, Func<T, string> valueSelector);
        EmailBuilder ReplaceToken(string tokenName, string value);
    }
}

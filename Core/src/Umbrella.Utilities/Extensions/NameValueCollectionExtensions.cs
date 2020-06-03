using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// Extensions for use with the <see cref="NameValueCollection"/> type.
	/// </summary>
	public static class NameValueCollectionExtensions
    {
        public static string SerializeToString(this NameValueCollection nvc, string keyValueSeparator = "=", string pairSeparator = "&")
            => SerializeToString(nvc, WebUtility.UrlEncode, keyValueSeparator, pairSeparator);

        public static string SerializeToString(this NameValueCollection nvc, Func<string, string> urlEncoder, string keyValueSeparator = "=", string pairSeparator = "&")
        {
            Guard.ArgumentNotNull(nvc, nameof(nvc));
            Guard.ArgumentNotNull(urlEncoder, nameof(urlEncoder));
            Guard.ArgumentNotNullOrWhiteSpace(keyValueSeparator, nameof(keyValueSeparator));
            Guard.ArgumentNotNullOrWhiteSpace(pairSeparator, nameof(pairSeparator));

            return string.Join(pairSeparator,
                        nvc.AllKeys.Where(key => !string.IsNullOrWhiteSpace(nvc[key]))
                            .Select(
                                key => string.Join(pairSeparator, nvc.GetValues(key).Select(val => $"{urlEncoder(key)}{keyValueSeparator}{urlEncoder(val)}"))));
        }
    }
}
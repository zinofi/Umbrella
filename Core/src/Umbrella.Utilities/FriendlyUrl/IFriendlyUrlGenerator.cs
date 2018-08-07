using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.FriendlyUrl
{
    /// <summary>
    /// A utility for generating friendly URLs.
    /// </summary>
    public interface IFriendlyUrlGenerator
    {
        /// <summary>
        /// Generates a friendly URL segment.
        /// </summary>
        /// <param name="text">The text to use to generate the URL segment.</param>
        /// <param name="maxLength">The maximum length of the genrated URL segment.</param>
        /// <returns>The URL segment.</returns>
        string GenerateUrl(string text, int maxLength = 0);
    }
}
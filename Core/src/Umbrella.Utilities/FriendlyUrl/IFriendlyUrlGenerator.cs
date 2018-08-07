using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.FriendlyUrl
{
    public interface IFriendlyUrlGenerator
    {
        string GenerateUrl(string text, int maxLength = 0);
    }
}
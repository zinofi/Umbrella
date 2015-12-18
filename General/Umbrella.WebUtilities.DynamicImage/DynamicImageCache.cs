using Umbrella.WebUtilities.DynamicImage.Enumerations;
using Umbrella.WebUtilities.DynamicImage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.WebUtilities.DynamicImage
{
    public abstract class DynamicImageCache
    {
        public string GenerateCacheKey(DynamicImageOptions options)
        {
            string key = string.Format("{0}-W-{1}-H-{2}-M-{3}-F-{4}-P", options.Width, options.Height, options.Mode, options.Format, options.OriginalVirtualPath);

            byte[] bytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));

            StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);

            foreach (byte num in bytes)
                stringBuilder.Append(num.ToString("x").PadLeft(2, '0'));

            return stringBuilder.ToString();
        }
    }
}

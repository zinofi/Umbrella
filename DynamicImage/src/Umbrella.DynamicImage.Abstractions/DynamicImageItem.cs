using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions
{
    public class DynamicImageItem
    {
        private Func<Task<byte[]>> m_ContentResolver;
        private byte[] m_Content;
        
        public DateTime LastModified { get; set; }
        public DynamicImageOptions ImageOptions { get; set; }
        /// <summary>
        /// This will either be a physical file path or a URL.
        /// </summary>
        //public string CachedPath { get; set; } //TODO: Do we need this?? - Yes, this is needed to lazily serve files from the disk cache. Need some way

        public void SetContent(byte[] bytes)
            => m_Content = bytes;

        public async Task<byte[]> GetContentAsync()
        {
            if (m_Content != null)
                return m_Content;

            if (m_ContentResolver != null)
                m_Content = await m_ContentResolver();

            return m_Content;
        }

        public void SetContentResolver(Func<Task<byte[]>> contentResolver)
            => m_ContentResolver = contentResolver;
    }
}
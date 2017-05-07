using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions
{
    public class DynamicImageItem
    {
        private Func<CancellationToken, Task<byte[]>> m_ContentResolver;
        private byte[] m_Content;
        
        public DateTimeOffset LastModified { get; set; }
        public long Length { get; set; }
        public DynamicImageOptions ImageOptions { get; set; }

        public void SetContent(byte[] bytes)
        {
            m_Content = bytes;

            //Ensure the length is in sync with the content
            Length = bytes.Length;
        }

        public async Task<byte[]> GetContentAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (m_Content != null)
                return m_Content;

            if (m_ContentResolver != null)
                m_Content = await m_ContentResolver(cancellationToken);

            return m_Content;
        }

        public void SetContentResolver(Func<CancellationToken, Task<byte[]>> contentResolver)
            => m_ContentResolver = contentResolver;
    }
}
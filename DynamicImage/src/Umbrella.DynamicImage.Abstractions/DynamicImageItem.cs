using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;

namespace Umbrella.DynamicImage.Abstractions
{
    public class DynamicImageItem
    {
        private DateTimeOffset m_LastModified;
        private byte[] m_Content;

        public DateTimeOffset LastModified
        {
            get => UmbrellaFileInfo != null ? UmbrellaFileInfo.LastModified.Value : m_LastModified;
            set => m_LastModified = value;
        }
        public long Length => UmbrellaFileInfo?.Length ?? m_Content?.Length ?? -1;
        public DynamicImageOptions ImageOptions { get; set; }
        public byte[] Content
        {
            set => m_Content = value;
        }
        public IUmbrellaFileInfo UmbrellaFileInfo { get; set; }

        public async Task<byte[]> GetContentAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (m_Content == null && UmbrellaFileInfo != null)
                m_Content = await UmbrellaFileInfo.ReadAsByteArrayAsync(cancellationToken);

            return m_Content;
        }

        public Task WriteContentToStreamAsync(Stream target, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Guard.ArgumentNotNull(target, nameof(target));

            if (m_Content == null && UmbrellaFileInfo != null)
                return UmbrellaFileInfo.WriteToStreamAsync(target, cancellationToken);

            return target.WriteAsync(m_Content, 0, m_Content.Length);
        }
    }
}
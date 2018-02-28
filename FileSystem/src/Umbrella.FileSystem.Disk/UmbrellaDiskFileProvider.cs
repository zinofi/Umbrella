using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Mime;

namespace Umbrella.FileSystem.Disk
{
    public class UmbrellaDiskFileProvider : UmbrellaFileProvider<UmbrellaDiskFileInfo, UmbrellaDiskFileProviderOptions>, IUmbrellaDiskFileProvider
    {
        #region Constructors
        public UmbrellaDiskFileProvider(ILoggerFactory loggerFactory,
            IMimeTypeUtility mimeTypeUtility,
            UmbrellaDiskFileProviderOptions options)
            : base(loggerFactory.CreateLogger<UmbrellaDiskFileProvider>(), loggerFactory, mimeTypeUtility, options)
        {
            Guard.ArgumentNotNullOrWhiteSpace(options.RootPhysicalPath, nameof(options.RootPhysicalPath));

            //Sanitize the root path
            options.RootPhysicalPath = options.RootPhysicalPath.Trim().TrimEnd('\\');
        }
        #endregion

        #region Overridden Methods
        protected override async Task<IUmbrellaFileInfo> GetFileAsync(string subpath, bool isNew, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //Sanitize subpath
            StringBuilder pathBuilder = new StringBuilder(subpath)
                .Trim(' ', '~', '\\', '/', ' ')
                .Replace('/', '\\')
                .Insert(0, Options.RootPhysicalPath + @"\");

            string physicalPath = pathBuilder.ToString();

            if (Log.IsEnabled(LogLevel.Debug))
                Log.WriteDebug(new { subpath, physicalPath });

            FileInfo physicalFileInfo = new FileInfo(physicalPath);

            if (!isNew && !physicalFileInfo.Exists)
                return null;

            if (!await CheckFileAccessAsync(physicalFileInfo, cancellationToken))
                throw new UmbrellaFileAccessDeniedException(subpath);

            return new UmbrellaDiskFileInfo(FileInfoLoggerInstance, MimeTypeUtility, subpath, this, physicalFileInfo, isNew);
        }
        #endregion

        #region Protected Methods
        protected Task<bool> CheckFileAccessAsync(FileInfo fileInfo, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(true);
        }
        #endregion
    }
}
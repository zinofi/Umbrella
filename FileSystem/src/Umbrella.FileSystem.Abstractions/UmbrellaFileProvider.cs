using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Utilities;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.Abstractions
{
	public abstract class UmbrellaFileProvider<TFileInfo, TOptions>
		where TFileInfo : IUmbrellaFileInfo
		where TOptions : IUmbrellaFileProviderOptions
	{
		#region Protected Properties
		protected ILogger Log { get; }
		protected ILoggerFactory LoggerFactory { get; }
		protected IMimeTypeUtility MimeTypeUtility { get; }
		protected IGenericTypeConverter GenericTypeConverter { get; }
		protected ILogger<TFileInfo> FileInfoLoggerInstance { get; }
		protected TOptions Options { get; private set; }
		#endregion

		#region Constructors
		public UmbrellaFileProvider(
			ILogger logger,
			ILoggerFactory loggerFactory,
			IMimeTypeUtility mimeTypeUtility,
			IGenericTypeConverter genericTypeConverter)
		{
			Log = logger;
			LoggerFactory = loggerFactory;
			MimeTypeUtility = mimeTypeUtility;
			GenericTypeConverter = genericTypeConverter;
			FileInfoLoggerInstance = LoggerFactory.CreateLogger<TFileInfo>();
		}
		#endregion

		#region Public Methods
		public virtual void InitializeOptions(IUmbrellaFileProviderOptions options)
		{
			if (Options != null)
				throw new UmbrellaFileSystemException("The options have already been initialized for this instance.");

			Options = (TOptions)options;
		}

		public virtual async Task<IUmbrellaFileInfo> CreateAsync(string subpath, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

				return await GetFileAsync(subpath, true, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> GetAsync(string subpath, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

				return await GetFileAsync(subpath, false, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<bool> DeleteAsync(string subpath, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

				IUmbrellaFileInfo fileInfo = await GetAsync(subpath, cancellationToken).ConfigureAwait(false);

				if (fileInfo != null)
					return await fileInfo.DeleteAsync(cancellationToken).ConfigureAwait(false);

				return true;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<bool> DeleteAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNull(fileInfo, nameof(fileInfo));

				return await fileInfo.DeleteAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { fileInfo }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> CopyAsync(string sourceSubpath, string destinationSubpath, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNullOrWhiteSpace(sourceSubpath, nameof(sourceSubpath));
				Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

				IUmbrellaFileInfo sourceFile = await GetAsync(sourceSubpath, cancellationToken).ConfigureAwait(false);

				if (sourceFile == null)
					throw new UmbrellaFileNotFoundException(sourceSubpath);

				return await sourceFile.CopyAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { sourceSubpath, destinationSubpath }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, string destinationSubpath, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentOfType<TFileInfo>(sourceFile, nameof(sourceFile));
				Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

				IUmbrellaFileInfo destinationFile = await CreateAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);

				return await sourceFile.CopyAsync(destinationFile, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { sourceFile, destinationSubpath }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentOfType<TFileInfo>(sourceFile, nameof(sourceFile));
				Guard.ArgumentOfType<TFileInfo>(destinationFile, nameof(destinationFile));

				return await sourceFile.CopyAsync(destinationFile, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { sourceFile, destinationFile }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> SaveAsync(string subpath, byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));
				Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));

				IUmbrellaFileInfo file = await CreateAsync(subpath, cancellationToken).ConfigureAwait(false);
				await file.WriteFromByteArrayAsync(bytes, cacheContents, cancellationToken, bufferSizeOverride).ConfigureAwait(false);

				return file;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath, bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<IUmbrellaFileInfo> SaveAsync(string subpath, Stream stream, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));
				Guard.ArgumentNotNull(stream, nameof(stream));

				IUmbrellaFileInfo file = await CreateAsync(subpath, cancellationToken).ConfigureAwait(false);
				await file.WriteFromStreamAsync(stream, cancellationToken, bufferSizeOverride).ConfigureAwait(false);

				return file;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath, bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		public virtual async Task<bool> ExistsAsync(string subpath, CancellationToken cancellationToken = default)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

				IUmbrellaFileInfo file = await GetFileAsync(subpath, false, cancellationToken).ConfigureAwait(false);

				return file != null;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}
		#endregion

		#region Abstract Methods
		protected abstract Task<IUmbrellaFileInfo> GetFileAsync(string subpath, bool isNew, CancellationToken cancellationToken);
		#endregion
	}
}
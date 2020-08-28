using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.Abstractions
{
	/// <summary>
	/// The abstract base class upon which file provider implementations are built.
	/// </summary>
	/// <typeparam name="TFileInfo">The type of the file information.</typeparam>
	/// <typeparam name="TOptions">The type of the options.</typeparam>
	public abstract class UmbrellaFileProvider<TFileInfo, TOptions>
		where TFileInfo : IUmbrellaFileInfo
		where TOptions : IUmbrellaFileProviderOptions
	{
		#region Private Static Members
		private static readonly char[] _subpathTrimCharacters = new[] { ' ', '\\', '/', '~', ' ' };
		private static readonly Regex _multipleSlashSelector = new Regex("/+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		#endregion

		#region Protected Properties		
		/// <summary>
		/// Gets the log.
		/// </summary>
		protected ILogger Log { get; }

		/// <summary>
		/// Gets the logger factory.
		/// </summary>
		protected ILoggerFactory LoggerFactory { get; }

		/// <summary>
		/// Gets the MIME type utility.
		/// </summary>
		protected IMimeTypeUtility MimeTypeUtility { get; }

		/// <summary>
		/// Gets the generic type converter.
		/// </summary>
		protected IGenericTypeConverter GenericTypeConverter { get; }

		/// <summary>
		/// Gets the file information logger instance.
		/// </summary>
		protected ILogger<TFileInfo> FileInfoLoggerInstance { get; }

		/// <summary>
		/// Gets the options.
		/// </summary>
		protected TOptions Options { get; private set; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaFileProvider{TFileInfo, TOptions}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="loggerFactory">The logger factory.</param>
		/// <param name="mimeTypeUtility">The MIME type utility.</param>
		/// <param name="genericTypeConverter">The generic type converter.</param>
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

		/// <inheritdoc />
		public virtual void InitializeOptions(IUmbrellaFileProviderOptions options)
		{
			if (Options != null)
				throw new UmbrellaFileSystemException("The options have already been initialized for this instance.");

			Options = (TOptions)options;
		}

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> CreateAsync(string subpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

			try
			{
				return await GetFileAsync(subpath, true, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> GetAsync(string subpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

			try
			{
				return await GetFileAsync(subpath, false, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<bool> DeleteAsync(string subpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

			try
			{
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

		/// <inheritdoc />
		public virtual async Task<bool> DeleteAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNull(fileInfo, nameof(fileInfo));

			try
			{
				return await fileInfo.DeleteAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { fileInfo }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> CopyAsync(string sourceSubpath, string destinationSubpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(sourceSubpath, nameof(sourceSubpath));
			Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

			try
			{
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

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, string destinationSubpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentOfType<TFileInfo>(sourceFile, nameof(sourceFile));
			Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

			try
			{
				IUmbrellaFileInfo destinationFile = await CreateAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);

				return await sourceFile.CopyAsync(destinationFile, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { sourceFile, destinationSubpath }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> CopyAsync(IUmbrellaFileInfo sourceFile, IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentOfType<TFileInfo>(sourceFile, nameof(sourceFile));
			Guard.ArgumentOfType<TFileInfo>(destinationFile, nameof(destinationFile));

			try
			{
				return await sourceFile.CopyAsync(destinationFile, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { sourceFile, destinationFile }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> MoveAsync(string sourceSubpath, string destinationSubpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(sourceSubpath, nameof(sourceSubpath));
			Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

			try
			{
				IUmbrellaFileInfo sourceFile = await GetAsync(sourceSubpath, cancellationToken).ConfigureAwait(false);

				if (sourceFile == null)
					throw new UmbrellaFileNotFoundException(sourceSubpath);

				return await sourceFile.MoveAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { sourceSubpath, destinationSubpath }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> MoveAsync(IUmbrellaFileInfo sourceFile, string destinationSubpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentOfType<TFileInfo>(sourceFile, nameof(sourceFile));
			Guard.ArgumentNotNullOrWhiteSpace(destinationSubpath, nameof(destinationSubpath));

			try
			{
				IUmbrellaFileInfo destinationFile = await CreateAsync(destinationSubpath, cancellationToken).ConfigureAwait(false);

				return await sourceFile.MoveAsync(destinationFile, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { sourceFile, destinationSubpath }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> MoveAsync(IUmbrellaFileInfo sourceFile, IUmbrellaFileInfo destinationFile, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentOfType<TFileInfo>(sourceFile, nameof(sourceFile));
			Guard.ArgumentOfType<TFileInfo>(destinationFile, nameof(destinationFile));

			try
			{
				return await sourceFile.MoveAsync(destinationFile, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception exc) when (Log.WriteError(exc, new { sourceFile, destinationFile }, returnValue: true) && exc is UmbrellaFileNotFoundException == false)
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> SaveAsync(string subpath, byte[] bytes, bool cacheContents = true, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));
			Guard.ArgumentNotNullOrEmpty(bytes, nameof(bytes));

			try
			{
				IUmbrellaFileInfo file = await CreateAsync(subpath, cancellationToken).ConfigureAwait(false);
				await file.WriteFromByteArrayAsync(bytes, cacheContents, cancellationToken, bufferSizeOverride).ConfigureAwait(false);

				return file;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath, bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<IUmbrellaFileInfo> SaveAsync(string subpath, Stream stream, CancellationToken cancellationToken = default, int? bufferSizeOverride = null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));
			Guard.ArgumentNotNull(stream, nameof(stream));

			try
			{
				IUmbrellaFileInfo file = await CreateAsync(subpath, cancellationToken).ConfigureAwait(false);
				await file.WriteFromStreamAsync(stream, cancellationToken, bufferSizeOverride).ConfigureAwait(false);

				return file;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath, bufferSizeOverride }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}

		/// <inheritdoc />
		public virtual async Task<bool> ExistsAsync(string subpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

			try
			{
				IUmbrellaFileInfo file = await GetFileAsync(subpath, false, cancellationToken).ConfigureAwait(false);
				
				return file != null;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException(exc.Message, exc);
			}
		}
		#endregion

		#region Protected Methods		
		/// <summary>
		/// Performs core sanitization of the subpath.
		/// </summary>
		/// <param name="subpath">The subpath.</param>
		/// <returns>The sanitized subpath.</returns>
		protected string SanitizeSubPathCore(string subpath)
		{
			StringBuilder pathBuilder = new StringBuilder(subpath)
				.Trim(_subpathTrimCharacters)
				.Replace('\\', '/');

			// Force all files to be stored and read in lowercase to avoid issues with Blob Storage
			// and Linux which both use case-sensitive file systems.
			return _multipleSlashSelector.Replace(pathBuilder.ToString(), "/").ToLowerInvariant();
		}
		#endregion

		#region Abstract Methods		
		/// <summary>
		/// Gets the file at the specified <paramref name="subpath"/>.
		/// </summary>
		/// <param name="subpath">The subpath.</param>
		/// <param name="isNew">Specifies if the file is new.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An awaitable Task that returns the file.</returns>
		protected abstract Task<IUmbrellaFileInfo> GetFileAsync(string subpath, bool isNew, CancellationToken cancellationToken);
		#endregion
	}
}
using System;

namespace Umbrella.Utilities.Threading.Abstractions;

/// <summary>
/// A synchronization primitive used in conjunction with the <see cref="ISynchronizationManager"/>.
/// </summary>
/// <seealso cref="IDisposable" />
/// <seealso cref="IAsyncDisposable" />
public interface ISynchronizationRoot : IDisposable, IAsyncDisposable
{
}
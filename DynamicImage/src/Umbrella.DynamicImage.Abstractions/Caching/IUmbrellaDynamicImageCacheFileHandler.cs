using Umbrella.FileSystem.Abstractions;

namespace Umbrella.DynamicImage.Abstractions.Caching;

/// <summary>
/// A file handler for accessing files stored in the Dynamic Image cache directory.
/// </summary>
/// <seealso cref="IUmbrellaFileHandler{Int32}" />
public interface IUmbrellaDynamicImageCacheFileHandler : IUmbrellaFileHandler<int>
{
}
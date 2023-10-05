namespace Umbrella.FileSystem.Abstractions;

/// <summary>
/// A file handler for accessing files stored in the temporary files directory.
/// </summary>
/// <seealso cref="IUmbrellaFileHandler{Int32}" />
public interface IUmbrellaTempFileHandler : IUmbrellaFileHandler<int>
{
}
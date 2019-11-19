namespace Umbrella.FileSystem.Abstractions
{
	/// <summary>
	/// This is a marker interface for more specific options types.
	/// It is used by the <see cref="UmbrellaFileProvider{TFileInfo, TOptions}"/> type as a way of generically specifying options without having to resort to generics.
	/// </summary>
	public interface IUmbrellaFileProviderOptions
	{
	}
}
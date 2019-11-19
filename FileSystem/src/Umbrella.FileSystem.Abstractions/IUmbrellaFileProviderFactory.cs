namespace Umbrella.FileSystem.Abstractions
{
	public interface IUmbrellaFileProviderFactory
	{
		TProvider CreateProvider<TProvider, TOptions>(TOptions options)
			where TProvider : IUmbrellaFileProvider
			where TOptions : IUmbrellaFileProviderOptions;
	}
}
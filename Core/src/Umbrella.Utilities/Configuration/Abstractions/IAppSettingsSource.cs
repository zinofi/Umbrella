namespace Umbrella.Utilities.Configuration.Abstractions
{
	public interface IAppSettingsSource : IReadOnlyAppSettingsSource
	{
		void SetValue(string key, string value);
	}
}
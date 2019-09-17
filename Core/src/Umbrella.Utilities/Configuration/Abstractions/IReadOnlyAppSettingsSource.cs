namespace Umbrella.Utilities.Configuration.Abstractions
{
	public interface IReadOnlyAppSettingsSource
	{
		string GetValue(string key);
	}
}
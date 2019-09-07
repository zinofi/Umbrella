namespace Umbrella.DataAccess.Abstractions
{
	public interface IDataAccessLookupNormalizer
	{
		string Normalize(string value, bool trim = true);
	}
}
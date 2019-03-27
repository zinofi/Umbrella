namespace Umbrella.Kentico.Utilities.Users.Abstractions
{
	public interface IKenticoUserNameNormalizer
    {
		string Normalize(string userName);
	}
}
namespace Umbrella.DataAccess.Abstractions
{
	public interface IConcurrencyStamp
	{
		string ConcurrencyStamp { get; set; }
	}
}
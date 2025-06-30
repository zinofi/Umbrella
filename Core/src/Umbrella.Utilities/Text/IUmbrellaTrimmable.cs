namespace Umbrella.Utilities.Text;

/// <summary>
/// Interface for types that support automatic trimming of string properties.
/// </summary>
public interface IUmbrellaTrimmable
{
	/// <summary>
	/// Trims all string properties in this object and its nested properties.
	/// </summary>
	void TrimAllStringProperties();
}
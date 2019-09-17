namespace Umbrella.Utilities.Abstractions
{
	/// <summary>
	/// An interface used to allow Umbrella Options types to be validated.
	/// </summary>
	public interface IValidatableUmbrellaOptions
	{
		/// <summary>
		/// Validates this instance.
		/// </summary>
		void Validate();
	}
}
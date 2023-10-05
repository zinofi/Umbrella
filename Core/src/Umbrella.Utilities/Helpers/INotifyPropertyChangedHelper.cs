using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Umbrella.Utilities.Helpers;

/// <summary>
/// A helper class containing useful methods for use by classes that implement the <see cref="INotifyPropertyChanged"/> interface.
/// </summary>
public static class INotifyPropertyChangedHelper
{
	/// <summary>
	/// Sets the property.
	/// </summary>
	/// <typeparam name="T">The type of the property <paramref name="value"/>.</typeparam>
	/// <param name="backingStore">The backing store.</param>
	/// <param name="value">The value.</param>
	/// <param name="sender">The sender.</param>
	/// <param name="handler">The handler.</param>
	/// <param name="propertyName">Name of the property.</param>
	/// <returns><see langword="true"/> if the <paramref name="backingStore"/> has been updated because the <paramref name="value"/> has changed; otherwise <see langword="false"/>.</returns>
	public static bool SetProperty<T>(ref T backingStore, T value, object sender, PropertyChangedEventHandler? handler, [CallerMemberName] string propertyName = "")
	{
		if (EqualityComparer<T>.Default.Equals(backingStore, value))
			return false;

		backingStore = value;
		handler?.Invoke(sender, new PropertyChangedEventArgs(propertyName));

		return true;
	}
}
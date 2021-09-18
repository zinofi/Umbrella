using System;

namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Extensions for the <see cref="IConcurrencyStamp"/> type.
	/// </summary>
	public static class IConcurrencyStampExtensions
	{
		/// <summary>
		/// Updated the <see cref="IConcurrencyStamp.ConcurrencyStamp" /> value.
		/// </summary>
		/// <param name="target">The entity whose concurrency stamp will be updated.</param>
		public static void UpdateConcurrencyStamp(this IConcurrencyStamp target) => target.ConcurrencyStamp = Guid.NewGuid().ToString();
	}
}
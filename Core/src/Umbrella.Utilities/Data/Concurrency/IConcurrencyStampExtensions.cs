// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;

namespace Umbrella.Utilities.Data.Concurrency
{
	/// <summary>
	/// Extensions for the <see cref="IConcurrencyStamp"/> type.
	/// </summary>
	public static class IConcurrencyStampExtensions
	{
		/// <summary>
		/// Updated the <see cref="IConcurrencyStamp.ConcurrencyStamp" /> value.
		/// </summary>
		/// <param name="target">The item whose concurrency stamp will be updated.</param>
		public static void UpdateConcurrencyStamp(this IConcurrencyStamp target) => target.ConcurrencyStamp = Guid.NewGuid().ToString();
	}
}
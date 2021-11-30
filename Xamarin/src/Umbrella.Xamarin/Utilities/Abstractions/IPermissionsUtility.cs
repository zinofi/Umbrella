// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Umbrella.Xamarin.Utilities.Abstractions
{
	/// <summary>
	/// A utility that is a wrapper around the <see cref="Permissions"/> functionality
	/// for checking if a requested device permission has been granted using a workflow that assists the end user
	/// in enabling such permissions where they have been previously denied.
	/// </summary>
	public interface IPermissionsUtility
	{
		/// <summary>
		/// Checks the current status of the specfied <typeparamref name="TPermission"/> and requests access.
		/// </summary>
		/// <typeparam name="TPermission">The type of the permission.</typeparam>
		/// <returns><see langword="true" /> if the permission has been granted; otherwise <see langword="false" />.</returns>
		/// <remarks>
		/// If a permission has been previously denied an attempt will be made on Android devices to re-request the permission.
		/// On iOS, this is not possible and the user needs to be directed to enable the relevant permissions from the native iOS Settings app.
		/// </remarks>
		Task<bool> CheckAndRequestPermissionAsync<TPermission>() where TPermission : Permissions.BasePermission, new();
	}
}
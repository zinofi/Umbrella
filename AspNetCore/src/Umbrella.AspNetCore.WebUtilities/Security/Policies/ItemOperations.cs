// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Umbrella.AspNetCore.WebUtilities.Security.Policies
{
	/// <summary>
	/// Specifies authorization requirements for common operations that can be performed on resources.
	/// </summary>
	public static class ItemOperations
	{
		/// <summary>
		/// The Create operation.
		/// </summary>
		public static OperationAuthorizationRequirement Create = new OperationAuthorizationRequirement { Name = nameof(Create) };

		/// <summary>
		/// The Read operation.
		/// </summary>
		public static OperationAuthorizationRequirement Read = new OperationAuthorizationRequirement { Name = nameof(Read) };

		/// <summary>
		/// The Update operation.
		/// </summary>
		public static OperationAuthorizationRequirement Update = new OperationAuthorizationRequirement { Name = nameof(Update) };

		/// <summary>
		/// The Delete operation.
		/// </summary>
		public static OperationAuthorizationRequirement Delete = new OperationAuthorizationRequirement { Name = nameof(Delete) };
	}
}
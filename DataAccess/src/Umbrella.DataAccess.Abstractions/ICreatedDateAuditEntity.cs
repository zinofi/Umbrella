// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.DataAccess.Abstractions;

/// <summary>
/// Adds support to an entity type for storing the date it was created.
/// </summary>
public interface ICreatedDateAuditEntity
{
	/// <summary>
	/// Gets or sets the date the entity was created.
	/// </summary>
	DateTime CreatedDateUtc { get; set; }
}
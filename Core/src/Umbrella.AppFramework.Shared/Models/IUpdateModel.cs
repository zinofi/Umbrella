// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.Utilities.Data.Concurrency;

namespace Umbrella.AppFramework.Shared.Models
{
	public interface IUpdateModel<TKey> : IConcurrencyStamp
	{
		TKey Id { get; }
	}
}
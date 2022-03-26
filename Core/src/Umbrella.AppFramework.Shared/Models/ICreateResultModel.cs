// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AppFramework.Shared.Models
{
	public interface ICreateResultModel<TKey>
	{
		TKey Id { get; set; }
	}
}
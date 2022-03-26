// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;

namespace Umbrella.AppFramework.Shared.Models
{
	public class NoopUpdateModel<TKey> : IUpdateModel<TKey>
	{
		public TKey Id => throw new NotImplementedException();
		public string ConcurrencyStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
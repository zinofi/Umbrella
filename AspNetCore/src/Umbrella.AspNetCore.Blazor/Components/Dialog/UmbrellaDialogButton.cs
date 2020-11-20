// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AspNetCore.Blazor.Components.Dialog
{
	public class UmbrellaDialogButton
	{
		public UmbrellaDialogButton(string text, UmbrellaDialogButtonType buttonType, bool isCancel = false, string? navigateUrl = null, bool flexibleWidth = false)
		{
			Text = text;
			ButtonType = buttonType;
			IsCancel = isCancel;
			NavigateUrl = navigateUrl;
			FlexibleWidth = flexibleWidth;
		}

		public string Text { get; set; }
		public UmbrellaDialogButtonType ButtonType { get; set; }
		public bool IsCancel { get; set; }
		public string? NavigateUrl { get; set; }
		public bool FlexibleWidth { get; set; }

		public string CssClass => ButtonType switch
		{
			UmbrellaDialogButtonType.Danger => "danger",
			UmbrellaDialogButtonType.Default => "secondary",
			UmbrellaDialogButtonType.Info => "info",
			UmbrellaDialogButtonType.Primary => "primary",
			UmbrellaDialogButtonType.Success => "success",
			UmbrellaDialogButtonType.Warning => "warning",
			_ => "secondary"
		};
	}

	public enum UmbrellaDialogButtonType
	{
		Default,
		Success,
		Info,
		Warning,
		Danger,
		Primary
	}
}
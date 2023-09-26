using Blazored.Modal.Services;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.Blazor.Components.Grid.Dialogs.Models;
using Umbrella.AspNetCore.Blazor.Infrastructure;

namespace Umbrella.AspNetCore.Blazor.Components.Grid.Dialogs;

public abstract class DateRangeDialogBase : UmbrellaDialogComponentBase
{
	public DateRangeDialogModel Model { get; set; } = new();

	public async Task SubmitFormAsync()
	{
		try
		{
			await ModalInstance.CloseAsync(ModalResult.Ok(Model));
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { Model }))
		{
			await DialogUtility.ShowDangerMessageAsync();
		}
	}
}
// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Razor.Abstractions;
using Umbrella.Utilities.Email.Abstractions;
using Umbrella.WebUtilities.Exceptions;

namespace Umbrella.AspNetCore.WebUtilities.Emails
{
	public abstract class UmbrellaRazorEmailSender
	{
		protected ILogger Logger { get; }
		protected IEmailSender EmailSender { get; }
		protected IRazorViewToStringRenderer ViewToStringRenderer { get; }

		public UmbrellaRazorEmailSender(
			ILogger logger,
			IEmailSender emailSender,
			IRazorViewToStringRenderer viewToStringRenderer)
		{
			Logger = logger;
			EmailSender = emailSender;
			ViewToStringRenderer = viewToStringRenderer;
		}

		protected abstract string GetFullViewPath(string viewName);

		protected async Task SendEmailAsync<T>(T model, string email, string subject, string viewNameOrPath, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				string viewPath = viewNameOrPath.StartsWith('~') ? viewNameOrPath : GetFullViewPath(viewNameOrPath);

				string content = await ViewToStringRenderer.RenderViewToStringAsync(viewPath, model);

				await EmailSender.SendEmailAsync(email, subject, content, cancellationToken);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { model, email, subject, viewNameOrPath }, returnValue: true))
			{
				throw new UmbrellaWebException($"There has been an error sending the '{subject}' email.", exc);
			}
		}
	}
}
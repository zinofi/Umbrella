// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.Extensions.Logging.Azure.Configuration;

/// <summary>
/// The type of the Azure Table Storage log appender.
/// </summary>
public enum AzureTableStorageLogAppenderType
{
	/// <summary>
	/// The client appender. This is used to log client side messages, e.g. web browser error, mobile app errors.
	/// </summary>
	Client,

	/// <summary>
	/// The server appender. This is used to log server messages.
	/// </summary>
	Server
}
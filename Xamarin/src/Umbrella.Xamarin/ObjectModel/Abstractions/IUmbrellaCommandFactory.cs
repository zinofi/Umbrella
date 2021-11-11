// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Umbrella.Xamarin.ObjectModel.Abstractions
{
	public interface IUmbrellaCommandFactory
	{
		AsyncCommand CreateAsyncCommand(Func<Task> execute, bool checkNetworkConnection = false, Func<bool>? canExecute = null);
		AsyncCommand<T> CreateAsyncCommand<T>(Func<T, Task> execute, bool checkNetworkConnection = false, Func<bool>? canExecute = null);
		AsyncValueCommand CreateAsyncValueCommand(Func<ValueTask> execute, bool checkNetworkConnection = false, Func<bool>? canExecute = null);
		AsyncValueCommand<T> CreateAsyncValueCommand<T>(Func<T, ValueTask> execute, bool checkNetworkConnection = false, Func<bool>? canExecute = null);
		Command CreateCommand(Action execute, Func<bool>? canExecute = null);
		Command<T> CreateCommand<T>(Action<T> execute, Func<T, bool>? canExecute = null);
	}
}
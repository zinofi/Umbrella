﻿using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Behaviors
{
	public class ConditionalTapGestureRecognizerBehavior : Behavior<View>
	{
		public static BindableProperty IsEnabledProperty = BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(ConditionalTapGestureRecognizerBehavior), false);
		public static BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ConditionalTapGestureRecognizerBehavior), null);
		public static BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ConditionalTapGestureRecognizerBehavior), null);

		private View? _view;
		private TapGestureRecognizer? _gestureRecognizer;

		/// <summary>
		/// Gets or sets a value that determines if this behavior is enabled.
		/// </summary>
		public bool IsEnabled
		{
			get => (bool)GetValue(IsEnabledProperty);
			set => SetValue(IsEnabledProperty, value);
		}

		/// <summary>
		/// Gets or sets the command.
		/// </summary>
		public ICommand? Command
		{
			get => (ICommand?)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		/// <summary>
		/// Gets or sets the parameter passed to the <see cref="Command" />.
		/// </summary>
		public object? CommandParameter
		{
			get => GetValue(CommandParameterProperty);
			set => SetValue(CommandParameterProperty, value);
		}

		/// <inheritdoc />
		protected override void OnAttachedTo(View bindable)
		{
			base.OnAttachedTo(bindable);

			_view = bindable;

			if (IsEnabled && Command != null)
			{
				AddGestureRecognizer(bindable);
			}
		}

		/// <inheritdoc />
		protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (_view is null)
				return;

			if (propertyName == nameof(IsEnabled))
			{
				if (IsEnabled && Command != null)
				{
					AddGestureRecognizer(_view);
				}
				else
				{
					RemoveGestureRecognizer(_view);
				}
			}
			else if (propertyName == nameof(Command) || propertyName == nameof(CommandParameter))
			{
				if (IsEnabled && Command != null)
				{
					if (_gestureRecognizer is null)
					{
						AddGestureRecognizer(_view);
					}
					else
					{
						UpdateGestureRecognizer();
					}
				}
				else
				{
					RemoveGestureRecognizer(_view);
				}
			}
		}

		/// <inheritdoc />
		protected override void OnDetachingFrom(View bindable)
		{
			base.OnDetachingFrom(bindable);
			RemoveGestureRecognizer(bindable);
		}

		private void AddGestureRecognizer(View bindable)
		{
			_gestureRecognizer = new TapGestureRecognizer
			{
				Command = Command,
				CommandParameter = CommandParameter
			};

			bindable.GestureRecognizers.Add(_gestureRecognizer);
		}

		private void UpdateGestureRecognizer()
		{
			if (_gestureRecognizer != null)
			{
				_gestureRecognizer.Command = Command;
				_gestureRecognizer.CommandParameter = CommandParameter;
			}
		}

		private void RemoveGestureRecognizer(View bindable)
		{
			if (_gestureRecognizer != null)
			{
				_ = bindable.GestureRecognizers.Remove(_gestureRecognizer);
				_gestureRecognizer = null;
			}
		}
	}
}
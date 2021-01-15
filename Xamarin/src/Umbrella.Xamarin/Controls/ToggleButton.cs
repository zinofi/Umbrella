using System;
using System.Collections.Generic;
using System.Linq;
using Umbrella.Utilities.Extensions;
using Umbrella.Xamarin.Extensions;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Controls
{
	/// <summary>
	/// A control that extends the <see cref="Button"/> control to provide checkbox and radiobutton behaviour.
	/// </summary>
	/// <seealso cref="Button" />
	public class ToggleButton : Button
	{
		/// <summary>
		/// Occurs when the toggle state of the button changes.
		/// </summary>
		public event EventHandler<ToggledEventArgs>? Toggled;

		/// <summary>
		/// The labelled by property
		/// </summary>
		public static BindableProperty LabelledByProperty = BindableProperty.Create(nameof(LabelledBy), typeof(Label), typeof(ToggleButton));

		/// <summary>
		/// The is toggled property
		/// </summary>
		public static BindableProperty IsToggledProperty = BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(ToggleButton), false, BindingMode.TwoWay, propertyChanged: OnIsToggledChanged);

		/// <summary>
		/// The group name property
		/// </summary>
		public static BindableProperty GroupNameProperty = BindableProperty.Create(nameof(GroupName), typeof(string), typeof(ToggleButton));

		/// <summary>
		/// Initializes a new instance of the <see cref="ToggleButton"/> class.
		/// </summary>
		public ToggleButton()
		{
			Clicked += (sender, args) => ToggleState();
		}

		/// <summary>
		/// Gets or sets the <see cref="Label"/> that is associated with this control.
		/// </summary>
		public Label? LabelledBy
		{
			set => SetValue(LabelledByProperty, value);
			get => (Label?)GetValue(LabelledByProperty);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is toggled.
		/// </summary>
		public bool IsToggled
		{
			set => SetValue(IsToggledProperty, value);
			get => (bool)GetValue(IsToggledProperty);
		}

		/// <summary>
		/// Gets or sets the name of the group. This is used to provide radio button behaviour.
		/// </summary>
		public string? GroupName
		{
			get => (string?)GetValue(GroupNameProperty);
			set => SetValue(GroupNameProperty, value);
		}

		/// <inheritdoc />
		protected override void OnParentSet()
		{
			base.OnParentSet();

			VisualStateManager.GoToState(this, "ToggledOff");

			if (LabelledBy != null)
			{
				VisualStateManager.GoToState(LabelledBy, "ToggledOff");

				var grTap = new TapGestureRecognizer
				{
					Command = new Command(ToggleState)
				};

				LabelledBy.GestureRecognizers.Add(grTap);
			}
		}

		private void ToggleState()
		{
			if (!string.IsNullOrEmpty(GroupName))
			{
				IReadOnlyCollection<ToggleButton> lstToggleButton = this.FindPageControls<ToggleButton>(x => x.GroupName == GroupName);

				bool newValue = !IsToggled;

				if (newValue)
				{
					// Ensure others in the group are deselected
					lstToggleButton.Where(x => x != this).ForEach(x => x.IsToggled = false);
					IsToggled = newValue;
				}
				else
				{
					// Never allow deselection for grouped items as we need to ensure 1 is always selected
					// once an initial selection has been made.
				}
			}
			else
			{
				IsToggled = !IsToggled;
			}
		}

		private static void OnIsToggledChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var toggleButton = (ToggleButton)bindable;
			bool isToggled = (bool)newValue;

			// Fire event
			toggleButton.Toggled?.Invoke(toggleButton, new ToggledEventArgs(isToggled));

			// Set the visual state
			VisualStateManager.GoToState(toggleButton, isToggled ? "ToggledOn" : "ToggledOff");

			if (toggleButton.LabelledBy != null)
				VisualStateManager.GoToState(toggleButton, isToggled ? "ToggledOn" : "ToggledOff");
		}
	}
}
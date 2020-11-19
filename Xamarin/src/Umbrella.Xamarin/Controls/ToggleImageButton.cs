using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.Xamarin.Extensions;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Controls
{
	public class ToggleImageButton : ImageButton
	{
		public event EventHandler<ToggledEventArgs>? Toggled;

		public static BindableProperty IsToggledProperty = BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(ToggleButton), false, BindingMode.TwoWay, propertyChanged: OnIsToggledChanged);
		public static BindableProperty GroupNameProperty = BindableProperty.Create(nameof(GroupName), typeof(string), typeof(ToggleButton));

		public ToggleImageButton()
		{
			Clicked += (sender, args) =>
			{
				if (!string.IsNullOrEmpty(GroupName))
				{
					IReadOnlyCollection<ToggleImageButton> lstToggleButton = this.FindPageControls<ToggleImageButton>(x => x.GroupName == GroupName);

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
			};
		}

		public bool IsToggled
		{
			set => SetValue(IsToggledProperty, value);
			get => (bool)GetValue(IsToggledProperty);
		}

		public string? GroupName
		{
			get => (string?)GetValue(GroupNameProperty);
			set => SetValue(GroupNameProperty, value);
		}

		protected override void OnParentSet()
		{
			base.OnParentSet();
			VisualStateManager.GoToState(this, "ToggledOff");
		}

		private static async void OnIsToggledChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var toggleButton = (ToggleImageButton)bindable;
			bool isToggled = (bool)newValue;

			// Fire event
			toggleButton.Toggled?.Invoke(toggleButton, new ToggledEventArgs(isToggled));

			// Small delay to ensure the visual state change is picked up properly.
			await Task.Delay(50);

			// Set the visual state
			VisualStateManager.GoToState(toggleButton, isToggled ? "ToggledOn" : "ToggledOff");
		}
	}
}

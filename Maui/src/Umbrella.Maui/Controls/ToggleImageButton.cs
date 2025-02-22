using System.Diagnostics.CodeAnalysis;
using Microsoft.Maui.Controls;

namespace Umbrella.Maui.Controls;

/// <summary>
/// A control that extends the <see cref="ImageButton"/> control to provide checkbox and radiobutton behaviour.
/// </summary>
/// <seealso cref="ImageButton" />
[SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "Required by Xamarin's conventions.")]
public class ToggleImageButton : ImageButton
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
	/// Initializes a new instance of the <see cref="ToggleImageButton"/> class.
	/// </summary>
	public ToggleImageButton() => Clicked += (sender, args) => ToggleState();

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

	/// <inheritdoc />
	protected override void OnParentSet()
	{
		base.OnParentSet();

		_ = VisualStateManager.GoToState(this, IsToggled ? "ToggledOn" : "ToggledOff");

		if (LabelledBy is not null)
		{
			_ = VisualStateManager.GoToState(LabelledBy, IsToggled ? "ToggledOn" : "ToggledOff");

			var grTap = new TapGestureRecognizer
			{
				Command = new Command(ToggleState)
			};

			LabelledBy.GestureRecognizers.Add(grTap);
			AutomationProperties.SetLabeledBy(this, LabelledBy);
		}
	}

	private void ToggleState() => IsToggled = !IsToggled;

	private static void OnIsToggledChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var toggleButton = (ToggleImageButton)bindable;
		bool isToggled = (bool)newValue;

		// Fire event
		toggleButton.Toggled?.Invoke(toggleButton, new ToggledEventArgs(isToggled));

		// Set the visual state
		_ = VisualStateManager.GoToState(toggleButton, isToggled ? "ToggledOn" : "ToggledOff");

		if (toggleButton.LabelledBy is not null)
			_ = VisualStateManager.GoToState(toggleButton.LabelledBy, isToggled ? "ToggledOn" : "ToggledOff");
	}
}
using Umbrella.AppFramework.Utilities.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Umbrella.Xamarin.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UmbrellaActivityInidicatorView : ContentView
	{
		public static BindableProperty IsLoadingProperty = BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(UmbrellaActivityInidicatorView));
		public static BindableProperty LoadingBackgroundColorProperty = BindableProperty.Create(nameof(LoadingBackgroundColor), typeof(Color), typeof(UmbrellaActivityInidicatorView));
		public static BindableProperty LoadingForegroundColorProperty = BindableProperty.Create(nameof(LoadingForegroundColor), typeof(Color), typeof(UmbrellaActivityInidicatorView));

		public bool IsLoading
		{
			get => (bool)GetValue(IsLoadingProperty);
			set => SetValue(IsLoadingProperty, value);
		}

		public Color LoadingBackgroundColor
		{
			get => (Color)GetValue(LoadingBackgroundColorProperty);
			set => SetValue(LoadingBackgroundColorProperty, value);
		}

		public Color LoadingForegroundColor
		{
			get => (Color)GetValue(LoadingForegroundColorProperty);
			set => SetValue(LoadingForegroundColorProperty, value);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaActivityInidicatorView"/> class.
		/// </summary>
		public UmbrellaActivityInidicatorView()
		{
			InitializeComponent();

			ILoadingScreenUtility loadingScreenUtility = UmbrellaXamarinServices.GetService<ILoadingScreenUtility>();

			loadingScreenUtility.OnShow += () => IsLoading = true;
			loadingScreenUtility.OnHide += () => IsLoading = false;
		}
	}
}
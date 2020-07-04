namespace Umbrella.DynamicImage.Abstractions
{
	/// <summary>
	/// The mode to use when resizing images.
	/// </summary>
	public enum DynamicResizeMode
	{
		/// <summary>
		/// Resizes based on the target width. If the target width is greater than the original width, the original width is used.
		/// The height is then calculated using the target width to maintain aspect ratio.
		/// </summary>
		UseWidth = 0,

		/// <summary>
		/// Resizes based on the target height. If the target height is greater than the original height, the original height is used.
		/// The width is then calculated using the target height to maintain aspect ratio.
		/// </summary>
		UseHeight = 1,

		/// <summary>
		/// Resizes the image to the exact values specified for width and height. Aspect ratio is not maintained and the image will be
		/// stretched or sqauashed as necessary to meet the size requirements.
		/// </summary>
		Fill = 2,

		/// <summary>
		/// Resizes the image uniformly (without squashing or stretching) without exceeding the original width or height. Resizing is attempted based
		/// on the target width first. If that would result in the height exceeding the target height, resizing is done using the target height instead.
		/// This effectively combines the <see cref="UseWidth"/> and <see cref="UseHeight"/> modes.
		/// </summary>
		Uniform = 3,

		/// <summary>
		/// Resizes the image uniformly and tries to meet both the target height and width exactly by centrally cropping the image to maintain the aspect ratio.
		/// Resizing is attempted using the target width first.
		/// If that would result in the height exceeding the target height, resizing is done using the target height instead. The width is then cropped either side of the horizonal image center.
		/// If not, the resizing is done using the target width with the height then being cropped either side of the vertical image center.
		/// </summary>
		UniformFill = 4
	}
}
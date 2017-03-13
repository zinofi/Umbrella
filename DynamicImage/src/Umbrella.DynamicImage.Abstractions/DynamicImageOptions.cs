namespace Umbrella.DynamicImage.Abstractions
{
    public struct DynamicImageOptions
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public DynamicResizeMode Mode { get; set; }
        public DynamicImageFormat Format { get; set; }
        public string OriginalVirtualPath { get; set; }
    }
}
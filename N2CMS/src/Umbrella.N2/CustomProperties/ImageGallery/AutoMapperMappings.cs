using AutoMapper;
using N2.Plugin;
using Umbrella.N2.CustomProperties.LinkEditor.Items;
using Umbrella.WebUtilities.DynamicImage;
using Umbrella.WebUtilities.DynamicImage.Enumerations;

namespace Umbrella.N2.CustomProperties.ImageGallery
{
    [AutoInitialize]
    public class AutoMapperMappings : IPluginInitializer
    {
        public void Initialize(global::N2.Engine.IEngine engine)
        {
            Mapper.CreateMap<ImageItem, ImageGalleryItemEditDTO>().AfterMap((item, dto) =>
                {
                    dto.ThumbnailUrl = DynamicImageUtility.GetResizedUrl(dto.Url, 150, 150, DynamicResizeMode.UniformFill, toAbsolutePath: true);
                });

            Mapper.CreateMap<ImageGalleryItemEditDTO, ImageItem>();
        }
    }
}
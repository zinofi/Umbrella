using AutoMapper;
using N2.Plugin;
using Ninject;
using Umbrella.Legacy.Utilities;
using Umbrella.N2.CustomProperties.LinkEditor.Items;
using Umbrella.WebUtilities.DynamicImage;
using Umbrella.WebUtilities.DynamicImage.Enumerations;
using Umbrella.WebUtilities.DynamicImage.Interfaces;

namespace Umbrella.N2.CustomProperties.ImageGallery
{
    [AutoInitialize]
    public class AutoMapperMappings : IPluginInitializer
    {
        public void Initialize(global::N2.Engine.IEngine engine)
        {
            IDynamicImageUtility dynamicImageUtility = LibraryBindings.DependencyResolver.Get<IDynamicImageUtility>();

            Mapper.CreateMap<ImageItem, ImageGalleryItemEditDTO>().AfterMap((item, dto) =>
                {
                    dto.ThumbnailUrl = dynamicImageUtility.GetResizedUrl(dto.Url, 150, 150, DynamicResizeMode.UniformFill, toAbsolutePath: true);
                });

            Mapper.CreateMap<ImageGalleryItemEditDTO, ImageItem>();
        }
    }
}
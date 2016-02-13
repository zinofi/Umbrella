using AutoMapper;
using Umbrella.N2.CustomProperties.LinkEditor.Items;

namespace Umbrella.N2.CustomProperties.ImageGallery
{
    public static class ImageGalleryAutoMapperMappings
    {
        #region Public Properties
        public static IMapper Instance { get; private set; }
        #endregion

        #region Constructors
        static ImageGalleryAutoMapperMappings()
        {
            MapperConfiguration config = new MapperConfiguration(x =>
            {
                x.CreateMap<ImageItem, ImageGalleryItemEditDTO>();
                x.CreateMap<ImageGalleryItemEditDTO, ImageItem>();
            });

            Instance = config.CreateMapper();
        }
        #endregion
    }
}
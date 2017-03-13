using N2.Details;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Legacy.WebUtilities.DynamicImage.Configuration;
using Umbrella.N2.CustomProperties.ImageGallery.Controls;
using Umbrella.N2.CustomProperties.LinkEditor;
using Umbrella.N2.CustomProperties.LinkEditor.Items;

namespace Umbrella.N2.CustomProperties.ImageGallery
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EditableImageGalleryAttribute : AbstractEditableAttribute, IRelativityTransformer
    {
        #region Private Static Members
        private static readonly Type s_IDynamicImageUtilityResolverType = typeof(IDynamicImageUtilityResolver);
        private static readonly ConcurrentDictionary<Type, IDynamicImageUtilityResolver> s_ResolverDictionary = new ConcurrentDictionary<Type, IDynamicImageUtilityResolver>();
        #endregion

        #region Private Members
        private readonly Type m_IDynamicImageUtilityResolverType;
        #endregion

        #region Constructors
        static EditableImageGalleryAttribute()
		{
            DynamicImageMappingsConfig mappingsConfig = new DynamicImageMappingsConfig(WebConfigurationManager.OpenWebConfiguration("~/web.config"));

            //Add in the Dynamic Image mappings we need to allow the images to be
            //generated without being blocked
            DynamicImageMapping thumbnailMapping = new DynamicImageMapping
			{
				Width = 150,
				Height = 150,
				ResizeMode = DynamicResizeMode.UniformFill,
				Format = DynamicImageFormat.Jpeg
			};

			DynamicImageMapping previewMapping = new DynamicImageMapping
			{
				Width = 400,
				Height = 400,
				ResizeMode = DynamicResizeMode.UniformFill,
				Format = DynamicImageFormat.Jpeg
			};

            mappingsConfig.Settings.Add(thumbnailMapping);
            mappingsConfig.Settings.Add(previewMapping);
		}

        /// <summary>
        /// Used to mark a N2 property as being an image gallery. In Edit Mode, the Image Gallery interface will be rendered.
        /// </summary>
        /// <param name="title">The title of the property displayed in edit mode</param>
        /// <param name="sortOrder">The sort order of the property in edit mode</param>
        /// <param name="dynamicImageUtilityResolverType">A type which implements <see cref="IDynamicImageUtilityResolver"/>. A singleton instance of this type will be created for the lifetime of the application.</param>
        public EditableImageGalleryAttribute(string title, int sortOrder, Type dynamicImageUtilityResolverType)
            : base(title, sortOrder)
        {
            if (!s_IDynamicImageUtilityResolverType.IsAssignableFrom(dynamicImageUtilityResolverType))
                throw new ArgumentException($"The specified type cannot be assigned to {nameof(IDynamicImageUtilityResolver)}", nameof(dynamicImageUtilityResolverType));

            m_IDynamicImageUtilityResolverType = dynamicImageUtilityResolverType;
        }
        #endregion

        #region Overridden Methods
        protected override System.Web.UI.Control AddEditor(Control container)
        {
            ImageGalleryControl c = new ImageGalleryControl();
            container.Controls.Add(c);
            return c;
        }

        public override void UpdateEditor(global::N2.ContentItem item, Control editor)
        {
            LinkItemCollection coll = LinkItemCollection.FindByPageAndPropertyName(item, Name);

            ImageGalleryControl ctrl = ((ImageGalleryControl)editor);

            IDynamicImageUtility dynamicImageUtility = GetDynamicImageUtility();

            //Need to convert the ImageItem objects to ImageGalleryItemEditDTO objects
            List<ImageGalleryItemEditDTO> lstImageGalleryItemEditDTO = coll.Cast<ImageItem>().Select(x =>
            {
                var dto = ImageGalleryAutoMapperMappings.Instance.Map<ImageGalleryItemEditDTO>(x);
                dto.ThumbnailUrl = dynamicImageUtility.GetResizedUrl(dto.Url, 150, 150, DynamicResizeMode.UniformFill, toAbsolutePath: true);

                return dto;
            }).ToList();

            ctrl.Initialize(JsonConvert.SerializeObject(lstImageGalleryItemEditDTO), coll.Count);
        }

        public override bool UpdateItem(global::N2.ContentItem item, Control editor)
        {
            string value = ((ImageGalleryControl)editor).Value;

            string jsonSave = null;

            if (!string.IsNullOrEmpty(value))
            {
                //We need to convert the JSON items from ImageGalleryItemEditDTO objects to ImageItem objects
                List<ImageGalleryItemEditDTO> lstImageGalleryItemEditDTO = JsonConvert.DeserializeObject<List<ImageGalleryItemEditDTO>>(value);

                //Now convert the items
                List<ImageItem> lstImageItem = ImageGalleryAutoMapperMappings.Instance.Map<List<ImageItem>>(lstImageGalleryItemEditDTO);

                //Now convert to JSON for storage in the N2 database
                jsonSave = JsonConvert.SerializeObject(lstImageItem);
            }

            item.SetDetail(Name, jsonSave, typeof(string));

            return true;
        }
        #endregion

        public string Rebase(string value, string fromAppPath, string toAppPath)
        {
            //The value being passed in is the raw JSON value stored for the property
            //We need to identify all links that need to be rebased
            LinkItemCollection coll = LinkItemCollection.Empty;
            if (LinkItemCollection.TryParse(value, out coll))
            {
                foreach (LinkItemBase item in coll)
                    item.RebaseLinkItem(fromAppPath, toAppPath);
            }

            return coll.ToJSONString();
        }

        public RelativityMode RelativeWhen => RelativityMode.Always;

        #region Private Methods
        private IDynamicImageUtility GetDynamicImageUtility()
        {
            IDynamicImageUtilityResolver resolver = s_ResolverDictionary.GetOrAdd(m_IDynamicImageUtilityResolverType, x => (IDynamicImageUtilityResolver)Activator.CreateInstance(x));

            return resolver.GetInstance();
        }
        #endregion
    }
}
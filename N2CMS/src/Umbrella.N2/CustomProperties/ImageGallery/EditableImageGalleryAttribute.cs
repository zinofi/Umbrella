using AutoMapper;
using N2.Details;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using Umbrella.N2.CustomProperties.ImageGallery.Controls;
using Umbrella.N2.CustomProperties.LinkEditor;
using Umbrella.N2.CustomProperties.LinkEditor.Items;
using Umbrella.WebUtilities.DynamicImage.Configuration;
using Umbrella.WebUtilities.DynamicImage.Enumerations;

namespace Umbrella.N2.CustomProperties.ImageGallery
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EditableImageGalleryAttribute : AbstractEditableAttribute, IRelativityTransformer
    {
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

        public EditableImageGalleryAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
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

            //Need to convert the ImageItem objects to ImageGalleryItemEditDTO objects
            List<ImageGalleryItemEditDTO> lstImageGalleryItemEditDTO = Mapper.Map<List<ImageGalleryItemEditDTO>>(coll.Cast<ImageItem>());

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
                List<ImageItem> lstImageItem = Mapper.Map<List<ImageItem>>(lstImageGalleryItemEditDTO);

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

        public RelativityMode RelativeWhen
        {
            get { return RelativityMode.Always; }
        }
    }
}
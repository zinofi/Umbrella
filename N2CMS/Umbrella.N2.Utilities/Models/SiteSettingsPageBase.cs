using N2;
using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Umbrella.N2.BaseModels
{
    /// <summary>
    /// This is the base model for Site Settings pages containing common properties
    /// shared across multiple sites.
    /// </summary>
    public abstract class SiteSettingsPageBase : PageModelBase
    {
        #region Error Pages
        [EditableLink("Error Page", 50)]
        public virtual ContentItem ErrorPage { get; set; }

        [EditableLink("Not Found Page", 51)]
        public virtual ContentItem NotFoundPage { get; set; }
        #endregion

        #region Facebook
        [EditableText("App Id", 401, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphAppId { get; set; }

        [EditableText("Type", 402, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphType { get; set; }

        [EditableText("Description", 403, ContainerName = ContainerNames.Facebook, TextMode = TextBoxMode.MultiLine, Rows = 10)]
        public virtual string FacebookDescription { get; set; }

        [EditableText("Title", 404, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphTitle { get; set; }

        [EditableImage("Image Url", 405, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphImageUrl { get; set; }

        [EditableText("Street Address", 406, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphBusinessStreetAddress { get; set; }

        [EditableText("Locality / City", 407, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphBusinessLocality { get; set; }

        [EditableText("Region / County", 408, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphBusinessRegion { get; set; }

        [EditableText("Postcode", 409, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphBusinessPostcode { get; set; }

        [EditableText("Country", 410, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphBusinessCountry { get; set; }

        [EditableText("Email Address", 411, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphBusinessEmail { get; set; }

        [EditableText("Phone Number", 412, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphBusinessPhone { get; set; }

        [EditableText("Fax Number", 413, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphBusinessFax { get; set; }

        [EditableText("Latitude", 414, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphPlaceLatitude { get; set; }

        [EditableText("Longitude", 415, ContainerName = ContainerNames.Facebook)]
        public virtual string FacebookOpenGraphPlaceLongitude { get; set; }
        #endregion

        #region Helpers
        public ContentItem ErrorPageInstance
        {
            get
            {
                return Find.Query<SiteSettingsPageBase>().First().ErrorPage;
            }
        }

        public ContentItem NotFoundPageInstance
        {
            get
            {
                return Find.Query<SiteSettingsPageBase>().First().NotFoundPage;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using N2.Resources;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Text;
using System.IO;
using Umbrella.N2.CustomProperties.LinkEditor.Extensions;
using System.Reflection;
using System.Web.Configuration;
using Umbrella.N2.CustomProperties.LinkEditor.Controls.Templates;
using N2;
using Newtonsoft.Json;
using Umbrella.Legacy.WebUtilities.Controls;
using Umbrella.Legacy.WebUtilities.Extensions;
using N2.Web.UI.WebControls;

namespace Umbrella.N2.CustomProperties.LinkEditor.Controls
{
    public class LinkEditorControl : WebControl, INamingContainer
    {
        #region Controls
        private HtmlInputHidden hdnJsonData;
        private HtmlInputHidden hdnJsonButtonOptions;
        #endregion

        #region Private Members
        private int m_LinksCount;
        #endregion

        #region Public Properties
        public string Value
        {
            get
            {
                return !string.IsNullOrEmpty(hdnJsonData.Value)
                    ? hdnJsonData.Value
                    : null;
            }
        }
        #endregion

        #region Public Methods
        public void Initialize(ContentItem editItem, string json, List<ILinkItemCollectionPlugin> plugins, int links = 0, bool showLinksOption = true, bool showImageOption = true)
        {
            EnsureChildControls();

            //Set the Virtual Application Url Prefix JavaScript value
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "VirtualApplicationUrlPrefix", string.Format("virtualAppUrlPrefix = \"{0}\";", HttpRuntime.AppDomainAppVirtualPath), true);

            //We need to ensure that there is only a single instance of this control per edit page!
            LinkListPopup ctrl = Page.FindFirstControl<ItemEditor>().FindControl("ucLinkList") as LinkListPopup;
            if (ctrl == null)
            {
                ctrl = new LinkListPopup
                {
                    ID = "ucLinkList"
                };
                Page.FindFirstControl<ItemEditor>().Controls.Add(ctrl);

                //Register stylesheets and scripts - in theory these registrations will not be duplicated
                this.Page.StyleSheet("~/Content/themes/base/all.css");

                Page.StyleSheet(Page.ClientScript.GetWebResourceUrl(GetType(), "Umbrella.N2.CustomProperties.LinkEditor.Resources.LinkEditor.css"));
                Page.StyleSheet(Page.ClientScript.GetWebResourceUrl(typeof(EmbeddedUserControl), "Umbrella.Legacy.WebUtilities.GlobalResources.DialogModal.css"));
                Page.JavaScript("~/N2/Resources/Js/UrlSelection.js");
                Page.JavaScript(Page.ClientScript.GetWebResourceUrl(typeof(EmbeddedUserControl), "Umbrella.Legacy.WebUtilities.GlobalResources.DialogModal.js"));
                Page.JavaScript(Page.ClientScript.GetWebResourceUrl(GetType(), "Umbrella.N2.CustomProperties.LinkEditor.Resources.LinkEditor.js"));

                Page.ClientScript.RegisterClientScriptBlock(GetType(), "up-arrow", string.Format("upArrowUrl = \"{0}\";", Page.ClientScript.GetWebResourceUrl(GetType(), "Umbrella.N2.CustomProperties.LinkEditor.Resources.up-arrow.png")), true);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "down-arrow", string.Format("downArrowUrl = \"{0}\";", Page.ClientScript.GetWebResourceUrl(GetType(), "Umbrella.N2.CustomProperties.LinkEditor.Resources.down-arrow.png")), true);
            }

            List<PopupButton> buttons = new List<PopupButton>();

            //Only add the Link Editor Popup if we need it, and we haven't already registered it
            if (showLinksOption)
            {
                buttons.Add(new PopupButton { Name = "Link", FriendlyName = "Link" });

                if (Page.FindFirstControl<ItemEditor>().Controls.OfType<Control>().FirstOrDefault(x => x is LinkEditorPopup) == null)
                    Page.FindFirstControl<ItemEditor>().Controls.Add(new LinkEditorPopup());
            }

            //Only add the Image Editor Popup if we need it, and we haven't already registered it
            if (showImageOption)
            {
                buttons.Add(new PopupButton { Name = "Image", FriendlyName = "Image" });

                if (Page.FindFirstControl<ItemEditor>().Controls.OfType<Control>().FirstOrDefault(x => x is ImageEditorPopup) == null)
                    Page.FindFirstControl<ItemEditor>().Controls.Add(new ImageEditorPopup());
            }

            //Register all the plugin controls - but only if they haven't already been registered
            foreach (ILinkItemCollectionPlugin plugin in plugins)
            {
                if (Page.FindFirstControl<ItemEditor>().Controls.OfType<Control>().FirstOrDefault(x => x.GetType() == plugin.GetPluginControl().GetType()) == null)
                {
                    LinkItemCollectionUserControlBase control = plugin.GetPluginControl();
                    control.CurrentItem = editItem;

                    Page.FindFirstControl<ItemEditor>().Controls.Add(control);
                    RegisterPluginJavaScript(plugin);
                }

                buttons.Add(new PopupButton { Name = plugin.Name, FriendlyName = plugin.FriendlyName });
            }

            //Set hidden field value to JSON value
            hdnJsonData.Value = json;
            m_LinksCount = links;

            //We now need to set a hidden field to indicate which buttons should be shown for this control
            //We can configure which buttons are shown on a per property basis
            hdnJsonButtonOptions.Value = JsonConvert.SerializeObject(buttons);
        }
        #endregion

        #region Private Properties
        private string LinksCountText
        {
            get
            {
                if (m_LinksCount == 1)
                    return m_LinksCount + " item";
                else
                    return m_LinksCount + " items";
            }
        }
        #endregion

        #region Overridden Methods
        protected override void CreateChildControls()
        {
            hdnJsonData = new HtmlInputHidden
            {
                ID = "hdnJsonData"
            };

            hdnJsonButtonOptions = new HtmlInputHidden
            {
                ID = "hdnJsonButtonOptions"
            };

            this.Controls.Add(hdnJsonData);
            this.Controls.Add(hdnJsonButtonOptions);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            //Outer div
            TagBuilder outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("link-item-collection clearfix");

            //Links Count Div
            TagBuilder linksCountDiv = new TagBuilder("div");
            linksCountDiv.AddCssClass("divLinksProperty");
            linksCountDiv.SetInnerText(LinksCountText);

            //Ellipsis
            TagBuilder button = new TagBuilder("input");
            button.AddCssClass("btnViewLinks");
            button.MergeAttribute("type", "button");
            button.MergeAttribute("value", "...");

            //Clear Links
            TagBuilder clearLinks = new TagBuilder("a");
            clearLinks.AddCssClass("lnkClearAllItems");
            clearLinks.MergeAttribute("href", "#");
            clearLinks.SetInnerText("Clear items");

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(linksCountDiv.ToString());
            builder.AppendLine(button.ToString());
            builder.AppendLine(RenderControl(hdnJsonData));
            builder.AppendLine(RenderControl(hdnJsonButtonOptions));
            builder.AppendLine(clearLinks.ToString());

            outerDiv.InnerHtml = builder.ToString();

            writer.Write(outerDiv.ToString());
        }
        #endregion

        #region Private Methods
        private string RenderControl(Control ctrl)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            ctrl.RenderControl(hw);
            return sb.ToString();
        }

        private void RegisterPluginJavaScript(ILinkItemCollectionPlugin plugin)
        {
            StringBuilder script = new StringBuilder(@"$(document).ready(function ()
            {
                $('#divAddEdit{0}').dialog(
                {
                    autoOpen: false,
                    modal: true,
                    resizable: false,
                    width: {2},
                    buttons:
                    [
                        {
                            text: 'Save',
                            click: function() { Save{0}(); }, 
                        },
                        {
                            text: 'Cancel',
                            click: function() { $(this).dialog('close'); }
                        }
                    ],
                    close: function ()
                    {
                        ResetAddEdit{0}Dialog();

                        ToggleAllValidatorsEnabledState($(this), false);
                    }
                });
            });

            function Add{0}_Click(event)
            {
                event.preventDefault();

                ToggleAllValidatorsEnabledState($('#divAddEdit{0}'), true);

                ResetAddEdit{0}Dialog();

                OpenAddEdit{0}Dialog('Add {1}');
            }

            function OpenAddEdit{0}Dialog(title)
            {
                $('#divAddEdit{0}').dialog('option', 'title', title);
                $('#divAddEdit{0}').dialog('open');  
            }");

            script.Replace("{0}", plugin.Name);
            script.Replace("{1}", plugin.FriendlyName);
            script.Replace("{2}", plugin.DialogWidth.ToString());

            //This ensures that the script for the plugin will not be registered twice
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "jsN2LinkEditorPlugin-" + plugin.Name, script.ToString(), true);
        }
        #endregion
    }
}
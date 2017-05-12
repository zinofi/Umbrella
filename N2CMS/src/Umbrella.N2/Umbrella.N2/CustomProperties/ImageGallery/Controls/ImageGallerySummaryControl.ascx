<%@ Control Language="C#" %>

<asp:Panel ID="pnlImageGallerySummary" runat="server" CssClass="image-gallery-summary">
    <span class="count"><asp:Literal ID="litImagesCount" runat="server" /> images</span>
    <input type="button" class="btnEditGallery" title="Edit Gallery" value="..." />
    <asp:HiddenField ID="hdnJSONData" runat="server" />
    <a href="#" class="clear-items">Clear images</a>
</asp:Panel>
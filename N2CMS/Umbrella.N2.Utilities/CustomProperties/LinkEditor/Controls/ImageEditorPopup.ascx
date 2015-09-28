<%@ Control Language="C#" %>

<div id="divAddEditImage" class="editLinkItemPopup" style="display:none;">
    <fieldset>
        <legend>Information</legend>

        <asp:ValidationSummary ID="vsAddEditImage" runat="server" HeaderText="Please correct the following errors:" CssClass="validationSummary" ValidationGroup="vgAddEditImage" />

        <table>
            <tr>
                <td>Image Url</td>
                <td>
                    <input id="txtImageUrl" runat="server" style="width:300px;"/><input type="button" id="btnSelectImage" value="..." data-input-element="txtImageUrl" />
                    <asp:RequiredFieldValidator ID="rfvImageUrl" runat="server" ControlToValidate="txtImageUrl" Display="None" ErrorMessage="Please select an image" Enabled="false" ValidationGroup="vgAddEditImage" />
                </td>
            </tr>
            <tr>
                <td>Alternative Text</td>
                <td><input id="txtImageAltText" style="width:300px;"/></td>
            </tr>
        </table>
    </fieldset>
</div>
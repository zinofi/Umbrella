<%@ Control Language="C#" %>

<div id="divAddEditLink" class="editLinkItemPopup" style="display:none;">
    <fieldset>
        <legend>Information</legend>

        <asp:ValidationSummary ID="vsAddEditLink" runat="server" HeaderText="Please correct the following errors:" CssClass="validationSummary" ValidationGroup="vgAddEditLink" />

        <table>
            <tr>
                <td>
                    <input id="rbClickableText" type="radio" value="ClickableText" name="linkInfoGroup" checked="checked" />
                    <label for="rbClickableText">Clickable Text</label>
                </td>
                <td>
                    <input id="txtLinkText" runat="server" type="text" style="width:300px;" />
                    <asp:RequiredFieldValidator ID="rfvLinkText" runat="server" ControlToValidate="txtLinkText" Display="None" ErrorMessage="Please specify the clickable text" Enabled="false" ValidationGroup="vgAddEditLink" />
                </td>
            </tr>
            <tr>
                <td>
                    <input id="rbClickableImage" type="radio" value="ClickableImage" name="linkInfoGroup" />
                    <label for="rbClickableImage">Clickable Image</label>
                </td>
                <td>
                    <input id="txtLinkImageUrl" runat="server" type="text" style="width:270px;" disabled="disabled" />
                    <input type="button" id="btnSelectLinkImageUrl" value="..." data-input-element="txtLinkImageUrl" disabled="disabled" />
                    <asp:RequiredFieldValidator ID="rfvLinkImageUrl" runat="server" ControlToValidate="txtLinkImageUrl" Display="None" ErrorMessage="Please specify a URL for the clickable image" Enabled="false" ValidationGroup="vgAddEditLink" />
                </td>
            </tr>
            <tr>
                <td>Title</td>
                <td><input id="txtLinkTitle" name="txtLinkTitle" type="text" style="width:300px;" /></td>
            </tr>
            <tr>
                <td>Access Key</td>
                <td><input id="txtLinkAccessKey" type="text" /></td>
            </tr>
            <tr>
                <td>Target Frame</td>
                <td>
                    <select id="ddlLinkTargetFrame">
                        <option value="_self" selected="selected">Open in same window</option>
                        <option value="_blank">Open in new window</option>
                    </select>
                </td>
            </tr>
        </table>

    </fieldset>
    
    <fieldset>
        <legend>Link Target</legend>

        <input id="rbInternal" type="radio" value="Internal" name="linkTargetGroup" checked="checked" /> <label for="rbInternal">Internal</label>
        <input id="rbExternal" type="radio" value="External" name="linkTargetGroup" /> <label for="rbExternal">External</label>
        <input id="rbDocument" type="radio" value="Document" name="linkTargetGroup" /> <label for="rbDocument">Document</label>
        <br />
        <table>
            <tr>
                <td>Url</td>
                <td>
                    <input id="txtLinkUrl" runat="server" type="text" style="width:300px;" />
                    <input type="button" id="btnSelectLink" value="..." />
                    <input type="button" id="btnSelectDocument" value="..." data-input-element="txtLinkUrl" />
                    <asp:RequiredFieldValidator ID="rfvLinkUrl" runat="server" ControlToValidate="txtLinkUrl" Display="None" ErrorMessage="Please specify the link URL" Enabled="false" ValidationGroup="vgAddEditLink" />
                </td>
            </tr>
            <tr>
                <td>Additional Parameters</td>
                <td>
                    <input id="txtAdditionalParameters" runat="server" style="width:300px;" />
                </td>
            </tr>
            <tr>
                <td>No Follow</td>
                <td><input id="chkLinkNoFollow" type="checkbox" /></td>
            </tr>
            <tr>
                <td>OnClick Code</td>
                <td><textarea id="txtLinkOnClickCode" style="width:300px;height:100px;"></textarea></td>
            </tr>
        </table>
    </fieldset>
</div>
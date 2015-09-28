<%@ Control Language="C#" %>

<div class="image-gallery-items"></div>

<div class="image-gallery-edit-image"></div>

<div class="image-gallery-add"></div>

<script id="tmplImageGalleryItems" type="text/x-handlebars-template">
    {{#if this}}
        <ul class="clearfix">
            {{#each this}}
            <li>
                <a href="#" title="Edit" class="edit-item"><img src="<asp:Literal ID='litEditIconUrl' runat='server' />" /></a>
                <a href="#" title="Delete" class="delete-item"><img src="<asp:Literal ID='litDeleteIconUrl' runat='server' />" /></a>
                <img src="{{ThumbnailUrl}}" alt="{{AltText}}" />
            </li>
            {{/each}}
        </ul>
        <br />
    {{/if}}
</script>

<script id="tmplImageGalleryEditImage" type="text/x-handlebars-template">
    <img src="{{PreviewUrl}}" />
    <div class="field">
        <input id="txtImageGalleryAlt" name="txtImageGalleryAlt" type="text" value="{{AltText}}" />
    </div>
</script>

<script id="tmplImageGalleryAdd" type="text/x-handlebars-template">
    <div class="clearfix">
        <label for="option1">Select Image</label>
        <input id="option1" type="radio" name="rdoAddImage" value="Single" checked />
    </div>
    <div class="clearfix">
        <label for="option2">Select Folder</label>
        <input id="option2" type="radio" name="rdoAddImage" value="Folder" />
    </div>
    <div class="clearfix">
        <input id="txtSelectedPath" class="selectedPath" disabled />
        <input id="select-path" type="button" value="..." />
    </div>
</script>
var data;
var hiddenJSON;
var divLinksProperty;
var virtualAppUrlPrefix;
var upArrowUrl;
var downArrowUrl;

$(document).ready(function ()
{
    $("#divLinks").dialog(
        {
            autoOpen: false,
            modal: true,
            title: "Edit Links",
            resizable: false,
            width: 700,
            closeText: "X",
            dialogClass: "dialog-modal"
        });

    $("#popup-close").click(function (event)
    {
    	$(this).parents("#divLinks").dialog("close");
    });

    $("#divAddEditLink").dialog(
        {
            autoOpen: false,
            modal: true,
            resizable: false,
            width: 500,
            closeText: "X",
            dialogClass: "dialog-modal",
            buttons:
            [
                {
                    text: "Save",
                    click: function () { SaveLink(); }
                },
                {
                    text: "Cancel",
                    click: function () { $(this).dialog("close"); }
                }
            ],
            close: function ()
            {
                ResetAddEditLinkDialog();
                ToggleAllValidatorsEnabledState($(this), false);
            }
        });

    $("#divAddEditImage").dialog(
        {
            autoOpen: false,
            modal: true,
            resizable: false,
            width: 500,
            closeText: "X",
            dialogClass: "dialog-modal",
            buttons:
            [
                {
                    text: "Save",
                    click: function () { SaveImage(); }
                },
                {
                    text: "Cancel",
                    click: function () { $(this).dialog("close"); }
                }
            ],
            close: function ()
            {
                ResetAddEditImageDialog();
                ToggleAllValidatorsEnabledState($(this), false);
            }
        });

    $(".btnViewLinks").click(function (event)
    {
        event.preventDefault();

        divLinksProperty = $(this).siblings("div.divLinksProperty");
        hiddenJSON = $(this).siblings("input:hidden[name$='hdnJsonData']");

        if (hiddenJSON.val().length > 0)
        	data = $.parseJSON(hiddenJSON.val());
        else
        	data = null;

        if (data == null)
            data = new Array();

        PopulateLinksListDisplay();

        var buttonOptionsJSON = $(this).siblings("input:hidden[name$='hdnJsonButtonOptions']");
        var buttonOptionsData = $.parseJSON(buttonOptionsJSON.val());
        EnsurePopupButtons(buttonOptionsData);

        $("#divLinks").dialog("open");
    });

    $(".lnkClearAllItems").click(function (event)
    {
        event.preventDefault();

        if (confirm("Are you sure you want to clear all items?"))
        {
            $(this).siblings("div.divLinksProperty").html("0 items");
            $(this).siblings("input:hidden").val("");
        }
    });

    $("input").each(function (idx, element)
    {
        if ($(this).data("selectContentLink"))
        {
            $(this).click(function (event)
            {
                event.preventDefault();
                openUrlSelectorPopup(GetApplicationUrl('/N2/Content/Navigation/Tree.aspx?location=contentselection'), $(this).data("inputElement"), 'height=600,width=400,resizable=yes,status=yes,scrollbars=yes', 'Items', 'Items', 'ContentItem', '');
            });
        }
        else if ($(this).data("selectFileLink"))
        {
            $(this).click(function (event)
            {
                event.preventDefault();
                openUrlSelectorPopup(GetApplicationUrl('/N2/Content/Navigation/Tree.aspx?location=filesselection'), $(this).data("inputElement"), 'height=600,width=400,resizable=yes,status=yes,scrollbars=yes', 'Files', 'Files', 'IFileSystemFile', '');
            });
        }
    });

    $("#btnSelectLink").click(function (event)
    {
        event.preventDefault();
        openUrlSelectorPopup(GetApplicationUrl('/N2/Content/Navigation/Tree.aspx?location=contentselection'), 'txtLinkUrl', 'height=600,width=400,resizable=yes,status=yes,scrollbars=yes', 'Items', 'Items', 'ContentItem', '');
    });

    $("#btnSelectLinkImageUrl, #btnSelectImage, #btnSelectDocument").click(function (event)
    {
        event.preventDefault();
        openUrlSelectorPopup(GetApplicationUrl('/N2/Content/Navigation/Tree.aspx?location=filesselection'), $(this).data("inputElement"), 'height=600,width=400,resizable=yes,status=yes,scrollbars=yes', 'Files', 'Files', 'IFileSystemFile', '');
    });

    $("#txtLinkText").focus(function (event)
    {
    	var currentValue = $(this).val();
    	var selectedUrl = $("#txtLinkUrl").val();

    	if (currentValue.length == 0 && selectedUrl .length > 0 && $("input:radio[name=linkTargetGroup]:checked").val() == "Internal")
    	{
    		CallServiceGet(GetApplicationUrl("/api/PageIdFromUrl?url=" + selectedUrl), function (data)
    		{
    			$("#txtLinkText").val(data.Name);
    		});
    	}
    });

    $("input:radio[name=linkInfoGroup]").click(function ()
    {
        var linkTextInput = $("#txtLinkText");
        var linkImageUrlInput = $("#txtLinkImageUrl");

        linkTextInput.val("");
        linkImageUrlInput.val("");

        var value = $(this).val();
        if (value == "ClickableText")
        {
            linkTextInput.prop("disabled", false);
            linkImageUrlInput.prop("disabled", true);
            $("#btnSelectLinkImageUrl").prop("disabled", true);

            ToggleValidatorEnabledState($("#txtLinkText"), true);
            ToggleValidatorEnabledState($("#txtLinkImageUrl"), false);
        }
        else if (value == "ClickableImage")
        {
            linkTextInput.prop("disabled", true);
            linkImageUrlInput.prop("disabled", false);
            $("#btnSelectLinkImageUrl").prop("disabled", false);

            ToggleValidatorEnabledState($("#txtLinkText"), false);
            ToggleValidatorEnabledState($("#txtLinkImageUrl"), true);
        }
    });

    $("input:radio[name=linkTargetGroup]").click(function ()
    {
        $("#txtLinkUrl").val("");

        var value = $(this).val();
        if (value == "Internal")
        {
            $("#txtLinkUrl").prop("readonly", true);
            $("#btnSelectLink").show();
            $("#btnSelectDocument").hide();
        }
        else if (value == "External")
        {
            $("#txtLinkUrl").prop("readonly", false);
            $("#btnSelectLink").hide();
            $("#btnSelectDocument").hide();
        }
        else if (value == "Document")
        {
            $("#txtLinkUrl").prop("readonly", true);
            $("#btnSelectLink").hide();
            $("#btnSelectDocument").show();
        }
    });
});

function AddLink_Click(event)
{
    event.preventDefault();

    ToggleAllValidatorsEnabledState($("#divAddEditLink"), true);

    //We need to make sure that the clickable image validator is disabled
    $("#rbClickableText").click();

    ResetAddEditLinkDialog();
    OpenAddEditLinkDialog("Add Link");
}

function AddImage_Click(event)
{
    event.preventDefault();

    ToggleAllValidatorsEnabledState($("#divAddEditImage"), true);

    ResetAddEditImageDialog();
    OpenAddEditImageDialog("Add Image");
}

function SaveLink()
{
    //Ensure the form is valid before submitting
    //We are hijacking the ASP.NET validation mechanism to accomplish this
    Page_ClientValidate("vgAddEditLink");

    if (!Page_IsValid)
        return;

    //Save the details
    var editItem = GetAddEditItem();

    var currentUrl = editItem.Url;

    editItem.Text = $("#txtLinkText").val();
    editItem.ImageUrl = $("#txtLinkImageUrl").val();
    editItem.Title = $("#txtLinkTitle").val();
    editItem.AccessKey = $("#txtLinkAccessKey").val();
    editItem.TargetFrame = $("#ddlLinkTargetFrame option:selected").val();

    editItem.LinkTypeString = $("input:radio[name=linkTargetGroup]:checked").val();
    editItem.Url = $("#txtLinkUrl").val();
    editItem.AdditionalParameters = $("#txtAdditionalParameters").val();
    editItem.NoFollow = $("#chkLinkNoFollow").prop("checked");
    editItem.OnClickCode = $("#txtLinkOnClickCode").val();

    if (editItem.LinkTypeString == "Internal" && currentUrl != editItem.Url)
    {
        UpdatePageId(editItem, editItem.Url);
    }

    Save("divAddEditLink");
}

function SaveImage()
{
    //Ensure the form is valid before submitting
    //We are hijacking the ASP.NET validation mechanism to accomplish this
    Page_ClientValidate("vgAddEditImage");

    if (!Page_IsValid)
        return;

    //Save the details
    var editItem = GetAddEditItem();

    editItem.LinkTypeString = "Image";
    editItem.Url = $("#txtImageUrl").val();
    editItem.AltText = $("#txtImageAltText").val();

    Save("divAddEditImage");
}

function GetAddEditItem()
{
    var editIndexStr = $("#hdnEditIndex").val();

    var editIndex = -1;

    if (editIndexStr.length > 0)
        editIndex = parseInt(editIndexStr);

    var editItem;
    if (editIndex == -1)
    {
        //Add a new image
        editItem = new Object();
        data[data.length] = editItem;
    }
    else
    {
        //Update existing image
        editItem = data[editIndex];
    }

    return editItem;
}

function Save(popupId)
{
    SaveData();

    PopulateLinksListDisplay();
    UpdatePropertyDisplay();

    $("#" + popupId).dialog("close");
}

function EnsurePopupButtons(buttonOptionsData)
{
    //Find the buttons container and clear any existing buttons
    var container = $("#divButtonOptions");
    container.empty();

    $.each(buttonOptionsData, function (index, value)
    {
        var optionButton = $("<button>");
        optionButton.id = "btnAdd" + value.Name;
        optionButton.html("Add " + value.FriendlyName);

        var fnName = "Add" + value.Name + "_Click";

        optionButton.click(function (event)
        {
            eval(fnName + "(event);");
        });

        optionButton.button();

        container.append(optionButton);
    });
}

function PopulateLinksListDisplay()
{
    var tbody = $("#divLinks table tbody");
    tbody.empty();

    var moveUpLinkTemplate = $("<a>").attr("href", "#").attr("title", "up").addClass("lnkMoveUp");
    moveUpLinkTemplate.append($("<img />").attr("src", upArrowUrl));

    var moveDownLinkTemplate = $("<a>").attr("href", "#").attr("title", "down").addClass("lnkMoveDown");
    moveDownLinkTemplate.append($("<img />").attr("src", downArrowUrl));

    var editLinkTemplate = $("<a>").attr("href", "#").attr("title", "edit").addClass("lnkEdit");
    editLinkTemplate.append($("<img />").attr("src", GetApplicationUrl("/N2/Resources/icons/pencil.png")));

    var deleteLinkTemplate = $("<a>").attr("href", "#").attr("title", "delete").addClass("lnkDelete");
    deleteLinkTemplate.append($("<img />").attr("src", GetApplicationUrl("/N2/Resources/icons/delete.png")));

    $.each(data, function (index, value)
    {
        var row = $("<tr>");

        if (index > 0)
        {
            var moveUpLink = moveUpLinkTemplate.clone().data("moveIndex", index);
            row.append($("<td>").append(moveUpLink));
        }
        else
        {
            row.append($("<td></td>"));
        }

        if ((index == 0 && data.length > 1) || (index > 0 && index < data.length - 1))
        {
            var moveDownLink = moveDownLinkTemplate.clone().data("moveIndex", index);
            row.append($("<td>").append(moveDownLink));
        }
        else
        {
            row.append($("<td></td>"));
        }

        row.append($("<td>").html(value.LinkTypeString));

        if (value.LinkTypeString == "Image")
        {
            row.append($("<td>").append(GeneratePreviewLink(value.Url)));
            row.append($("<td>").html("N/A"));
        }
        else if (value.LinkTypeString == "Internal" || value.LinkTypeString == "External" || value.LinkTypeString == "Document")
        {
            if (value.Text.length > 0)
                row.append($("<td>").html(value.Text));
            else if (value.ImageUrl.length > 0)
                row.append($("<td>").append(GeneratePreviewLink(value.ImageUrl)));

            row.append($("<td>").append(GeneratePreviewLink(value.Url)));
        }
        else
        {
            //Check for use of a custom plugin and populate the appropriate dialog
            var fnName = "Populate" + value.LinkTypeString + "ListItemDisplay";
            if (typeof eval(fnName) == "function")
                eval(fnName + "(row, value)");
        }

        var editLink = editLinkTemplate.clone().data("editIndex", index);
        var deleteLink = deleteLinkTemplate.clone().data("deleteIndex", index);

        row.append($("<td>").append(editLink));
        row.append($("<td>").append(deleteLink));

        tbody.append(row);
    });

    $(".lnkMoveUp").click(function (event)
    {
        event.preventDefault();

        var moveIndex = parseInt($(this).data("moveIndex"));

        var dataItemMovingUp = data[moveIndex];

        data[moveIndex] = data[moveIndex - 1];
        data[moveIndex - 1] = dataItemMovingUp;

        SaveData();

        PopulateLinksListDisplay();
    });

    $(".lnkMoveDown").click(function (event)
    {
        event.preventDefault();

        var moveIndex = parseInt($(this).data("moveIndex"));

        var dataItemMovingDown = data[moveIndex];

        data[moveIndex] = data[moveIndex + 1];
        data[moveIndex + 1] = dataItemMovingDown;

        SaveData();

        PopulateLinksListDisplay();
    });

    $(".lnkEdit").click(function (event)
    {
        event.preventDefault();

        var editIndex = parseInt($(this).data("editIndex"));

        var dataItem = data[editIndex];

        $("#hdnEditIndex").val(editIndex);

        if (dataItem.LinkTypeString == "Image")
        {
            PopulateAddEditImageDialog(dataItem);
        }
        else if (dataItem.LinkTypeString == "Internal" || dataItem.LinkTypeString == "External" || dataItem.LinkTypeString == "Document")
        {
            PopulateAddEditLinkDialog(dataItem);
        }
        else
        {
            //Check for use of a custom plugin and populate the appropriate dialog
            var fnName = "PopulateAddEdit" + dataItem.LinkTypeString + "Dialog";
            if (typeof eval(fnName) == "function")
                eval(fnName + "(dataItem)");
        }
    });

    $(".lnkDelete").click(function (event)
    {
        event.preventDefault();
        var del = confirm("Are you sure you want to delete this link?");
        if (del)
        {
            //Delete the link
            var deleteIndex = $(this).data("deleteIndex");

            data = $.grep(data, function (value, index)
            {
                return index != deleteIndex;
            });

            SaveData();

            PopulateLinksListDisplay();
            UpdatePropertyDisplay();
        }
    });
}

function PopulateAddEditLinkDialog(dataItem)
{
    ToggleAllValidatorsEnabledState($("#divAddEditLink"), true);

    if (dataItem.Text.length > 0)
    {
        $("#rbClickableText").click();
        $("#txtLinkText").val(dataItem.Text);
    }
    else if (dataItem.ImageUrl.length > 0)
    {
        $("#rbClickableImage").click();
        $("#txtLinkImageUrl").val(dataItem.ImageUrl);
    }

    $("#txtLinkTitle").val(dataItem.Title);
    $("#txtLinkAccessKey").val(dataItem.AccessKey);
    $("#ddlLinkTargetFrame option[value='" + dataItem.TargetFrame + "']").prop("selected", true);

    if (dataItem.LinkTypeString == "External")
        $("#rbExternal").click();
    else if (dataItem.LinkTypeString == "Internal")
        $("#rbInternal").click();
    else if (dataItem.LinkTypeString == "Document")
        $("#rbDocument").click();

    $("#txtLinkUrl").val(dataItem.Url);
    $("#txtAdditionalParameters").val(dataItem.AdditionalParameters);
    $("#chkLinkNoFollow").prop("checked", dataItem.NoFollow);
    $("#txtLinkOnClickCode").val(dataItem.OnClickCode);

    //Open add/edit link dialog
    OpenAddEditLinkDialog("Edit Link");
}

function PopulateAddEditImageDialog(dataItem)
{
    ToggleAllValidatorsEnabledState($("#divAddEditImage"), true);

    $("#txtImageUrl").val(dataItem.Url);
    $("#txtImageAltText").val(dataItem.AltText);

    //Open add/edit link dialog
    OpenAddEditImageDialog("Edit Image");
}

function GeneratePreviewLink(url)
{
    return $("<a></a>").attr("href", url).attr("target", "_blank").attr("title", "Click to preview").html(url);
}

function OpenAddEditLinkDialog(title)
{
    $("#divAddEditLink").dialog("option", "title", title);
    $("#divAddEditLink").dialog("open");
}

function OpenAddEditImageDialog(title)
{
    $("#divAddEditImage").dialog("option", "title", title);
    $("#divAddEditImage").dialog("open");
}

function SaveData()
{
    hiddenJSON.val(JSON.stringify(data));
}

function UpdatePropertyDisplay()
{
    var suffix = data.length == 1 ? " item" : " items";
    divLinksProperty.html(data.length + suffix);
}

function ResetAddEditLinkDialog()
{
    $("#rbClickableText").click();
    $("#txtLinkTitle").val("");
    $("#txtLinkAccessKey").val("");
    $("#ddlLinkTargetFrame option:first").prop("selected", true);

    $("#rbInternal").click();
    $("#txtLinkUrl").val("");
    $("#txtAdditionalParameters").val("");
    $("#chkLinkNoFollow").prop("checked", false);
    $("#txtLinkOnClickCode").val("");
    $("#hdnEditIndex").val(-1);

    ResetAllValidators($("#divAddEditLink"));
}

function ResetAddEditImageDialog()
{
    $("#txtImageUrl").val("");
    $("#txtImageAltText").val("");

    $("#hdnEditIndex").val(-1);

    ResetAllValidators($("#divAddEditImage"));
}

function UpdatePageId(dataItem, url)
{
	CallServiceGet(GetApplicationUrl("/api/PageIdFromUrl?url=" + url), function (data)
	{
		dataItem.PageId = data.Id;
	});
}

function ResetAllValidators(popup)
{
    popup.find("input, textarea, select").each(function (index, value)
    {
        ResetValidators($(value));
    });

    popup.find(".validationSummary").hide();
}

function ResetValidators(control)
{
    var ctrl = control.get(0);

    if (ctrl.Validators != null)
    {
        $.each(ctrl.Validators, function (index, value)
        {
            ResetValidatorDisplay(value);
        });
    }
}

function ResetValidatorDisplay(val)
{
    val.isvalid = true;
    ValidatorUpdateDisplay(val);
}

function ToggleAllValidatorsEnabledState(popup, enabled)
{
    popup.find("input, textarea, select").each(function (index, value)
    {
        ToggleValidatorEnabledState($(value), enabled);
    });
}

function ToggleValidatorEnabledState(control, enabled)
{
    var ctrl = control.get(0);

    if (ctrl.Validators != null)
    {
        $.each(ctrl.Validators, function (index, value)
        {
            ValidatorEnable(value, enabled);
        });
    }
}

function GetApplicationUrl(relativeUrl)
{
    if(virtualAppUrlPrefix.length > 1)
        return virtualAppUrlPrefix + relativeUrl;

    return relativeUrl;
}
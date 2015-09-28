$(document).ready(function ()
{
	var viewModel = null;
	var hdnJSONData = null;
	var countElement = null;
	var editIndex = 0;
	var addImageType = null;
	var fnImageGalleryItemsTemplate = Handlebars.compile($("#tmplImageGalleryItems").html());
	var fnImageGalleryEditImageTemplate = Handlebars.compile($("#tmplImageGalleryEditImage").html());
	var fnImageGalleryAddTemplate = Handlebars.compile($("#tmplImageGalleryAdd").html());

	$(".image-gallery-items").dialog(
	{
		autoOpen: false,
		modal: true,
		resizable: true,
		width: 810,
		closeText: "X",
		dialogClass: "dialog-modal",
		title: "Edit Image Gallery",
		buttons:
		[
			{
				text: "Add Images",
				click: function ()
				{
					$(".image-gallery-add").html(fnImageGalleryAddTemplate());
					$(".image-gallery-add").dialog("open");
				}
			},
			{
				text: "Save",
				click: function ()
				{
					countElement.text(viewModel.length + " images");
					hdnJSONData.val(JSON.stringify(viewModel));

					$(this).dialog("close");
				}
			},
			{
				text: "Cancel",
				click: function () { $(this).dialog("close"); }
			}
		],
		close: function ()
		{

		}
	});

	$(".image-gallery-edit-image").dialog(
	{
		autoOpen: false,
		modal: true,
		resizable: false,
		width: 620,
		closeText: "X",
		dialogClass: "dialog-modal",
		title: "Edit Image",
		buttons:
		[
			{
				text: "Save",
				click: function ()
				{
					viewModel[editIndex].AltText = $("#txtImageGalleryAlt").val();

					$(this).dialog("close");
				}
			},
			{
				text: "Cancel",
				click: function () { $(this).dialog("close"); }
			}
		],
		close: function ()
		{

		}
	});

	$(".image-gallery-add").dialog(
	{
		autoOpen: false,
		modal: true,
		resizable: false,
		width: 620,
		closeText: "X",
		dialogClass: "dialog-modal",
		title: "Add Images",
		buttons:
		[
			{
				text: "Save",
				click: function ()
				{
					var selectedPath = $("#txtSelectedPath").val();

					if (selectedPath.length > 0)
					{
						if (addImageType == "Single")
						{
							//Go to the server and get a get a single item for the image url
							var url = $(".image-gallery-summary").data("uploadFileUrl") + "?path=" + selectedPath;

							CallServiceGet(url, function (data)
							{
								viewModel.push(data);

								InitializeGalleryGrid();
							});
						}
						else
						{
							var url = $(".image-gallery-summary").data("uploadFolderUrl") + "?folderPath=" + selectedPath;

							CallServiceGet(url, function (data)
							{
								$.each(data, function (idx, item)
								{
									viewModel.push(item);
								});

								InitializeGalleryGrid();
							});
						}

						$(this).dialog("close");
					}
					else
					{
						alert("Please select an image or folder");
					}
				}
			},
			{
				text: "Cancel",
				click: function () { $(this).dialog("close"); }
			}
		],
		close: function ()
		{

		}
	});

	$(".btnEditGallery").click(function (event)
	{
		event.preventDefault();

		countElement = $(this).siblings(".count");
		hdnJSONData = $(this).siblings("input[type='hidden']");

		var json = hdnJSONData.val();

		viewModel = json.length > 0 ? $.parseJSON(json) : new Array();

		InitializeGalleryGrid();

		$(".image-gallery-items").dialog("open");
	});

	$(".clear-items").click(function (event)
	{
		event.preventDefault();

		if (confirm("Are you sure?"))
		{
			$(this).siblings(".count").text("0 images");
			$(this).siblings("input[type='hidden']").val("");
		}
	});

	$(".image-gallery-items").on("click", ".edit-item", function (event)
	{
		event.preventDefault();

		editIndex = $(this).parents("li").index();

		$(".image-gallery-edit-image").html(fnImageGalleryEditImageTemplate(viewModel[editIndex]));

		$(".image-gallery-edit-image").dialog("open");
	});

	$(".image-gallery-items").on("click", ".delete-item", function (event)
	{
		event.preventDefault();

		if (confirm("Are you sure you want to delete this image?"))
		{
			var parent = $(this).parents("li")

			parent.fadeOut("fast", function ()
			{
				viewModel.splice(parent.index(), 1);

				parent.remove();
			});
		}
	});

	$(".image-gallery-add").on("click", "#select-path", function (event)
	{
		event.preventDefault();

		addImageType = $("input[name='rdoAddImage']:checked").val();

		var popupType = addImageType === "Single" ? "IFileSystemFile" : "IFileSystemDirectory";

		openUrlSelectorPopup('/N2/Content/Navigation/Tree.aspx?location=filesselection', "txtSelectedPath", 'height=600,width=400,resizable=yes,status=yes,scrollbars=yes', 'Files', 'Files', popupType, '');
	});

	function InitializeGalleryGrid()
	{
		$(".image-gallery-items").html(fnImageGalleryItemsTemplate(viewModel));

		var startIndex = 0;
		$(".image-gallery-items ul").sortable({
			start: function (event, ui)
			{
				startIndex = $(ui.item).index();
			},
			update: function (event, ui)
			{
				var endIndex = $(ui.item).index();

				viewModel.move(startIndex, endIndex);
			}
		});
	}
});
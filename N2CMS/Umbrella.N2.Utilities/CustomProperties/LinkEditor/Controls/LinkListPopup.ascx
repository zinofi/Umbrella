<%@ Control Language="C#" %>

<%@ Import Namespace="Umbrella.N2.CustomProperties.LinkEditor.Controls" %>

<div id="divLinks" class="linksPopup" style="display:none;">
    <div class="inner">
        <table>
            <thead>
                <tr>
                    <th></th>
                    <th></th>
                    <th style="width:15%">Type</th>
                    <th style="width:35%">Text / Custom</th>
                    <th style="width:40%">Link Url</th>
                    <th style="width:5%"></th>
                    <th style="width:5%"></th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
    </div>
    <input id="hdnEditIndex" type="hidden" />

    <div id="divButtonOptions">
    </div>
    <button id="popup-close">Close</button>

</div>
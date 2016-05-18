function CallServiceGet(url, callback)
{
	$.ajax(
    {
    	type: "GET",
    	async: false,
    	url: url,
    	dataType: "json",
    	success: function (data) { callback(data); },
    	error: function (exc, textStatus, error) { alert("An error has occurred. Please try again."); },
    	cache: false
    });
}

Array.prototype.move = function (old_index, new_index)
{
	if (new_index >= this.length)
	{
		var k = new_index - this.length;
		while ((k--) + 1)
		{
			this.push(undefined);
		}
	}
	this.splice(new_index, 0, this.splice(old_index, 1)[0]);
	return this; // for testing purposes
};
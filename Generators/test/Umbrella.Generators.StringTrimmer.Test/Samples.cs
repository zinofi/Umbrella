using Umbrella.Utilities.Text;

namespace Umbrella.Generators.StringTrimmer.Test;

public partial class Samples : IUmbrellaTrimmable
{
	public required string RequiredString { get; set; }
	public string? NullableString { get; set; }
	public TrimmableChild RequiredTrimmableChild { get; set; } = new();
	public TrimmableChild? NullableTrimmableChild { get; set; }
	public Child RequiredChild { get; set; } = new();
	public Child? NullableChild { get; set; }

	public IReadOnlyCollection<TrimmableChild> TrimmableChildren { get; set; } = [];
	public IReadOnlyCollection<Child> NonTrimmableChildren { get; set; } = [];
}

public partial class TrimmableChild : IUmbrellaTrimmable
{
	public string RequiredString { get; set; } = "";
	public string? NullableString { get; set; }
}

public class Child
{
	public string RequiredString { get; set; } = "";
	public string? NullableString { get; set; }
}
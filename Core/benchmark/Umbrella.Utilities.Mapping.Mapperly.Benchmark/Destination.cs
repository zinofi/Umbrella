namespace Umbrella.Utilities.Mapping.Mapperly.Benchmark;

public record Destination
{
	public required short ShortNumber { get; set; }
	public required int WholeNumber { get; set; }
	public required long LongNumber { get; set; }
	public required float SinglePrecisionNumber { get; set; }
	public required double DoublePrecisionNumber { get; set; }
	public required Half HalfPrecisionNumber { get; set; }
	public required decimal DecimalNumber { get; set; }
	public required string SomeString { get; set; }
	public required Guid Id { get; set; }

	public required Destination? Child { get; set; }
	public required IReadOnlyCollection<Destination>? Children { get; set; }
}
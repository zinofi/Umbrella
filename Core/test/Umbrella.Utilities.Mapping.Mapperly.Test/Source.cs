namespace Umbrella.Utilities.Mapping.Mapperly.Test;

public record Source
{
	public required short ShortNumber { get; init; }
	public required int WholeNumber { get; init; }
	public required long LongNumber { get; init; }
	public required float SinglePrecisionNumber { get; init; }
	public required double DoublePrecisionNumber { get; init; }
	public required Half HalfPrecisionNumber { get; init; }
	public required decimal DecimalNumber { get; init; }
	public required string SomeString { get; init; }
	public required Guid Id { get; init; }

	public required Source? Child { get; init; }
	public required IReadOnlyCollection<Source>? Children { get; init; }
}
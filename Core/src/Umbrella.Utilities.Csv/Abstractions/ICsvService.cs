using CsvHelper.Configuration;

namespace Umbrella.Utilities.Csv.Abstractions;

/// <summary>
/// A service for generating and parsing CSV data.
/// </summary>
public interface ICsvService
{
	/// <summary>
	/// Generates a CSV file from the specified items.
	/// </summary>
	/// <typeparam name="TModel">The type of the model to generate the CSV from.</typeparam>
	/// <param name="items">The items to generate the CSV from.</param>
	/// <param name="map">The class map to use for the model. If <see langword="null"/>, the default mapping will be used.</param>
	/// <param name="cancellationToken">The cancellation token to observe while generating the CSV.</param>
	/// <returns>The generated CSV file as a byte array.</returns>
	ValueTask<byte[]> GenerateAsync<TModel>(IEnumerable<TModel> items, ClassMap<TModel>? map = null, CancellationToken cancellationToken = default);

	/// <summary>
	/// Parses a CSV file from the specified stream into a collection of items.
	/// </summary>
	/// <typeparam name="T">The type of the items to parse from the CSV.</typeparam>
	/// <param name="stream">The stream containing the CSV data.</param>
	/// <param name="config">The configuration to use for parsing the CSV. If <see langword="null"/>, the default configuration will be used.</param>
	/// <param name="cancellationToken">The cancellation token to observe while parsing the CSV.</param>
	/// <returns>The parsed items as a read-only collection.</returns>
	Task<IReadOnlyCollection<T>> ParseFromStreamAsync<T>(Stream stream, CsvConfiguration? config = default, CancellationToken cancellationToken = default);
}
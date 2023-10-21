using Riok.Mapperly.Abstractions;
using Umbrella.Utilities.Mapping.Mapperly.Abstractions;

namespace Umbrella.Utilities.Mapping.Mapperly.Test;

[Mapper]
public partial class Mapper : IUmbrellaMapperlyMapper<Source, Destination>
{
	public partial Destination Map(Source source);
	public partial void Map(Source source, Destination destination);
	public partial IReadOnlyCollection<Destination> MapAll(IEnumerable<Source> source);
}
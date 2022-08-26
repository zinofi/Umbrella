// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.Utilities.Mapping.Mapperly;

public interface IUmbrellaMapperlyMapper<TSource, TDestination>
{
	TDestination Map(TSource source);
	void Map(TSource source, TDestination destination);
}
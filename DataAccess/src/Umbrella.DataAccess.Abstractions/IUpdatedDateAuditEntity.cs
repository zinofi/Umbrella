using System;

namespace Umbrella.DataAccess.Abstractions
{
	public interface IUpdatedDateAuditEntity
	{
		DateTime UpdatedDate { get; set; }
	}
}
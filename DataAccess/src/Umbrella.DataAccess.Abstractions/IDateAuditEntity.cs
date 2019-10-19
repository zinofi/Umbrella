using System;

namespace Umbrella.DataAccess.Abstractions
{
	// TODO: Split this up into 2 interfaces
	public interface IDateAuditEntity
	{
		DateTime CreatedDate { get; set; }
		DateTime UpdatedDate { get; set; }
	}
}
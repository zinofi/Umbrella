using System;

namespace Umbrella.DataAccess.Abstractions
{
	public interface IDateAuditEntity
	{
		DateTime CreatedDate { get; set; }
		DateTime UpdatedDate { get; set; }
	}
}
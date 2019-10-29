using System;

namespace Umbrella.DataAccess.Abstractions
{
	public interface ICreatedDateAuditEntity
	{
		DateTime CreatedDate { get; set; }
	}
}
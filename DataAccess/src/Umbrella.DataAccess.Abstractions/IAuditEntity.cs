using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions
{
	public interface IAuditEntity : IAuditEntity<int>
	{
        
    }

	public interface IAuditEntity<T> : IEntity<T>, ICreatedDateAuditEntity, ICreatedUserAuditEntity<T>, IUpdatedDateAuditEntity, IUpdatedUserAuditEntity<T>
	{

	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Interfaces
{
	// TODO: V3 - Rename to ICurrentUserIdAccessor
    public interface IUserAuditDataFactory : IUserAuditDataFactory<int>
    {
    }

    public interface IUserAuditDataFactory<T>
    {
        T CurrentUserId { get; }
    }
}
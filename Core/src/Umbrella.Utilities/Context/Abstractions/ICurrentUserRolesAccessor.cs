using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Context.Abstractions
{
	public interface ICurrentUserRolesAccessor<T>
    {
		IReadOnlyCollection<string> RoleNames { get; }
		IReadOnlyCollection<T> Roles { get; }
	}
}
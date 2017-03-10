using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Interfaces
{
    public interface IUserAuditDataFactory<T>
    {
        T CurrentUserId { get; }
    }
}

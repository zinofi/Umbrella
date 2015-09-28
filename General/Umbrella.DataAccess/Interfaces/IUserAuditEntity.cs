using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IUserAuditEntity<T>
    {
        T CreatedById { get; set; }
        T UpdatedById { get; set; }
    }
}

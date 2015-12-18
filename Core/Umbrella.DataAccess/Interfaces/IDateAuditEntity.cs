using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IDateAuditEntity
    {
        DateTime CreatedDate { get; set; }
        DateTime UpdatedDate { get; set; }
    }
}
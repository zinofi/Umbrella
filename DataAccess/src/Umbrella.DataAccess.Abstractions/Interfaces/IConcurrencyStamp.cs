using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions.Interfaces
{
    public interface IConcurrencyStamp
    {
        string ConcurrencyStamp { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
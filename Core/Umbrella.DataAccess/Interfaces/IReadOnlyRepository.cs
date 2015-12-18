using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IReadOnlyRepository<T> where T : class
    {
        List<T> FindAll();
    }
}

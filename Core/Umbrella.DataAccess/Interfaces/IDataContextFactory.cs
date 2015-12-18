using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Data.Entity;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IDataContextFactory<T> where T : DbContext
    {
        T ContextInstance { get; set; }
    }
}

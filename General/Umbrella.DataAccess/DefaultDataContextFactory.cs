using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DataAccess.Interfaces;

namespace Umbrella.DataAccess
{
    /// <summary>
    /// This is the default data context factory and simply and deals with a single instance of a DbContext.
    /// It doesn't do anything special. It's just a wrapper for a the ContextInstance property with default getters and setters.
    /// </summary>
    /// <typeparam name="T">The type of the DbContext</typeparam>
    public class DefaultDataContextFactory<T> : IDataContextFactory<T> where T : DbContext
    {
        public T ContextInstance { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DataAccess.Interfaces;

namespace Umbrella.DataAccess
{
	/// <summary>
	/// A data context factory which stores the DbContext as a static instance suitable for use with unit testing.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TestDataContextFactory<T> : IDataContextFactory<T> where T : DbContext, new()
    {
        private static T m_Context = new T();

        public T ContextInstance
        {
            get { return m_Context; }
            set { m_Context = value; }
        }
    }
}
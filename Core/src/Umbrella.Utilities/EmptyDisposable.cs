using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.Utilities
{
    public class EmptyDisposable : IDisposable
    {
        #region Public Static Properties
        public static IDisposable Instance { get; } = new EmptyDisposable();
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
        }
        #endregion
    }
}
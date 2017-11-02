using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Checks if the <see cref="Object"/> is a null reference or if it has been destroyed.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to check.</param>
        /// <returns>A boolean indicating if the <see cref="Object"/> has been destroyed.</returns>
        public static bool IsNullOrDestroyed(this Object obj)
            => obj == null || obj.Equals(null);
    }
}
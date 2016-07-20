using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DataAccess.Interfaces;

namespace Umbrella.DataAccess
{
    /// <summary>
    /// A default RepoOptions type that contains a single property for ValidateEntity with a default value of true.
    /// </summary>
    public class RepoOptions
    {
        public virtual bool ValidateEntity { get; set; } = true;
    }
}
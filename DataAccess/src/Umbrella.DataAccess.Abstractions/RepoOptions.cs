using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions
{
    /// <summary>
    /// A default RepoOptions type that contains virtual properties for <see cref="SanitizeEntity"/> and <see cref="ValidateEntity"/> with default values of <see langword="true"/>,
	/// and another property <see cref="ProcessChildren"/> defaulted to <see langword="false" />.
    /// </summary>
    public class RepoOptions
    {
        public virtual bool SanitizeEntity { get; set; } = true;
        public virtual bool ValidateEntity { get; set; } = true;

		// TODO: V3 This currently isn't used by the EF6 repos. Look at doing something with it.
		public virtual bool ProcessChildren { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Interfaces
{
    public interface IIncludeMap<TEntity>
    {
        HashSet<Expression<Func<TEntity, object>>> Includes { get; }
    }
}

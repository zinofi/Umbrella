using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DataAccess.Abstractions.Interfaces;

namespace Umbrella.DataAccess.Abstractions
{
	// TODO: V3 - Rename to NullUserIdAccessor
    /// <summary>
    /// A default implementation of the <see cref="IUserAuditDataFactory{T}"/> that essentially does nothing except return the default value for <typeparamref name="T"/>.
    /// This type is useful when working with entities that do not implement the <see cref="IUserAuditEntity{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type used for the user id</typeparam>
    public sealed class NoopUserAuditDataFactory<T> : IUserAuditDataFactory<T>
    {
        public T CurrentUserId => default;
    }
}
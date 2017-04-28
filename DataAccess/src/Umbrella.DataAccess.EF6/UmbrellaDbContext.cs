using System.Data.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DataAccess.Abstractions.Interfaces;
using Umbrella.Utilities.Extensions;
using System.Threading;

namespace Umbrella.DataAccess.EF6
{
    public class UmbrellaDbContext : DbContext
    {
        #region Private Members
        private readonly Dictionary<object, Func<Task>> m_PostSaveChangesSaveActionDictionary = new Dictionary<object, Func<Task>>(); 
        #endregion

        #region Protected Properties
        protected ILogger Log { get; }
        #endregion

        #region Constructors
        public UmbrellaDbContext(ILogger logger)
        {
            Log = logger;
        }

        public UmbrellaDbContext(ILogger logger, string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Log = logger;
        }
        #endregion

        #region Public Methods
        public virtual void RegisterPostSaveChangesAction(object entity, Action action)
        {
            m_PostSaveChangesSaveActionDictionary[entity] = () => Task.FromResult(action);
        }

        public virtual void RegisterPostSaveChangesActionAsync(object entity, Func<Task> wrappedAction)
        {
            m_PostSaveChangesSaveActionDictionary[entity] = wrappedAction;
        }
        #endregion

        #region Internal Methods
        internal virtual async Task ExecutePostSaveChangesActionsAsync()
        {
            //There is the potential that if this code is being executed whilst
            //delegates are still being registered that this will throw up an error.
            //Realistically though I can't see this happening. Not worth building in locking
            //because of the overheads unless we encounter problems.
            foreach (var func in m_PostSaveChangesSaveActionDictionary.Values)
            {
                Task task = func?.Invoke();

                if (task != null)
                    await task;
            }

            //Now that all items have been processed, clear the dictionary
            m_PostSaveChangesSaveActionDictionary.Clear();
        }
        #endregion

        #region Overridden Methods
        public override int SaveChanges()
        {
            try
            {
                int result = base.SaveChanges();

                //Run this on a thread pool thread to ensure when this is executed where we have an available
                //SynchronizationContext that it does not cause deadlock
                Task t = Task.Run(() => ExecutePostSaveChangesActionsAsync());
                t.Wait();

                return result;
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public override async Task<int> SaveChangesAsync()
        {
            try
            {
                int result = await base.SaveChangesAsync();

                await ExecutePostSaveChangesActionsAsync();

                return result;
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                int result = await base.SaveChangesAsync(cancellationToken);

                await ExecutePostSaveChangesActionsAsync();

                return result;
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
        #endregion
    }
}
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
        #region Protected Properties
        protected ILogger Log { get; }
        protected Dictionary<object, Func<Task>> PostSaveChangesSaveActionDictionary { get; } = new Dictionary<object, Func<Task>>();
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
            PostSaveChangesSaveActionDictionary[entity] = () => Task.FromResult(action);

            if (Log.IsEnabled(LogLevel.Debug))
                Log.WriteDebug(message: "Post save callback registered");
        }

        public virtual void RegisterPostSaveChangesActionAsync(object entity, Func<Task> wrappedAction)
        {
            PostSaveChangesSaveActionDictionary[entity] = wrappedAction;

            if (Log.IsEnabled(LogLevel.Debug))
                Log.WriteDebug(message: "Post save callback registered");
        }
        
        public virtual async Task ExecutePostSaveChangesActionsAsync()
        {
            if (Log.IsEnabled(LogLevel.Debug))
                Log.WriteDebug(new { StartPostSaveChangesActionsCount = PostSaveChangesSaveActionDictionary.Count }, "Started executing post save callbacks");

            //There is the potential that if this code is being executed whilst
            //delegates are still being registered that this will throw up an error.
            //Realistically though I can't see this happening. Not worth building in locking
            //because of the overheads unless we encounter problems.
            foreach (var func in PostSaveChangesSaveActionDictionary.Values)
            {
                Task task = func?.Invoke();

                if (task != null)
                {
                    if (Log.IsEnabled(LogLevel.Debug))
                        Log.WriteDebug(message: "Post save callback found to execute");

                    await task;
                }
            }

            //Now that all items have been processed, clear the dictionary
            PostSaveChangesSaveActionDictionary.Clear();

            if (Log.IsEnabled(LogLevel.Debug))
                Log.WriteDebug(new { EndPostSaveChangesActionsCount = PostSaveChangesSaveActionDictionary.Count }, "Finished executing post save callbacks");
        }
        #endregion

        #region Overridden Methods
        public override int SaveChanges()
        {
            try
            {
                if (Log.IsEnabled(LogLevel.Debug))
                    Log.WriteDebug(message: "Started SaveChanges()");

                int result = base.SaveChanges();

                //Run this on a thread pool thread to ensure when this is executed where we have an available
                //SynchronizationContext that it does not cause deadlock
                Task t = Task.Run(() => ExecutePostSaveChangesActionsAsync());
                t.Wait();

                if (Log.IsEnabled(LogLevel.Debug))
                    Log.WriteDebug(message: "Finished SaveChanges()");

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
                if (Log.IsEnabled(LogLevel.Debug))
                    Log.WriteDebug(message: "Started SaveChangesAsync()");

                int result = await base.SaveChangesAsync();

                await ExecutePostSaveChangesActionsAsync();

                if (Log.IsEnabled(LogLevel.Debug))
                    Log.WriteDebug(message: "Finished SaveChangesAsync()");

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
                if (Log.IsEnabled(LogLevel.Debug))
                    Log.WriteDebug(message: "Started SaveChangesAsync()");

                int result = await base.SaveChangesAsync(cancellationToken);

                await ExecutePostSaveChangesActionsAsync();

                if (Log.IsEnabled(LogLevel.Debug))
                    Log.WriteDebug(message: "Finished SaveChangesAsync()");

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
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DataAccess.Interfaces;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DataAccess.EntityFrameworkCore
{
    public class UmbrellaDbContext : DbContext
    {
        #region Private Static Members
        private static readonly Type s_ConcurrencyTypeStamp = typeof(IConcurrencyStamp);
        #endregion

        #region Protected Members
        protected readonly ILogger Log;
        #endregion

        #region Constructors
        public UmbrellaDbContext(ILogger logger)
        {
            Log = logger;
        }
        #endregion

        #region Overridden Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                bool isDebug = Log.IsEnabled(LogLevel.Debug);

                if (isDebug)
                    Log.WriteDebug("Start applying Concurrency Token to entity types.");

                var entityTypes = modelBuilder.Model.GetEntityTypes();

                foreach (var type in entityTypes)
                {
                    if (s_ConcurrencyTypeStamp.IsAssignableFrom(type.ClrType))
                    {
                        type.FindProperty(nameof(IConcurrencyStamp.ConcurrencyStamp)).IsConcurrencyToken = true;

                        if (isDebug)
                            Log.WriteDebug($"Concurrency Token applied to Entity Type: {type.Name}");
                    }
                }

                if (isDebug)
                    Log.WriteDebug("End applying Concurrency Token to entity types.");
            }
            catch(Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
        #endregion
    }
}
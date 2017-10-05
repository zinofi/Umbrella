using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Umbrella.Unity.Utilities
{
    public class GameObjectUtility : IGameObjectUtility
    {
        #region Private Members
        private readonly ILogger<GameObjectUtility> m_Logger;
        #endregion

        #region Constructors
        public GameObjectUtility(ILoggerFactory loggerFactory)
        {
            m_Logger = loggerFactory.CreateLogger<GameObjectUtility>();
        }
        #endregion

        #region IGameObjectUtility Members
        public GameObject FindOrCreate(string name, Transform parent = null)
        {
            try
            {
                var goToFind = GameObject.Find(name);

                if (goToFind != null)
                    return goToFind;

                goToFind = new GameObject(name);

                if (parent != null)
                    goToFind.transform.parent = parent;

                return goToFind;
            }
            catch (Exception exc)
            {
                m_Logger.WriteError(exc);
                throw;
            }
        }

        public T FindOrCreate<T>(Transform parent = null)
            where T : UnityEngine.Component
        {
            try
            {
                T goToFind = GameObject.FindObjectOfType<T>();

                if (goToFind != null)
                    return goToFind;

                var goToCreate = new GameObject(typeof(T).Name);

                if (parent != null)
                    goToCreate.transform.parent = parent;

                goToFind = goToCreate.AddComponent<T>();

                return goToFind;
            }
            catch (Exception exc)
            {
                m_Logger.WriteError(exc);
                throw;
            }
        } 
        #endregion
    }
}
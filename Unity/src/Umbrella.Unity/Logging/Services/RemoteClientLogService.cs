using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.Unity.Networking.Abstractions;
using System.Threading;
using Umbrella.Utilities;
using System.Net;
using UnityEngine;
using Umbrella.Unity.Logging.Models;

namespace Umbrella.Unity.Logging.Services
{
    public class RemoteClientLogService : IRemoteClientLogService
    {
        #region Results
        public struct RemoteClientLogServiceResult
        {
            public RemoteClientLogServiceResult(bool success, string message) : this()
            {
                Success = success;
                Message = message;
            }

            public bool Success { get; set; }
            public string Message { get; set; }
        }
        #endregion

        #region Private Members
        private readonly IUnityNetworkManager m_NetworkManager;
        private readonly string m_ApiUrl;
        #endregion

        #region Constructors
        public RemoteClientLogService(IUnityNetworkManager networkManager, RemoteClientLogServiceOptions options)
        {
            m_NetworkManager = networkManager;
            m_ApiUrl = options.ApiUrl;
        }
        #endregion

        #region IRemoteClientLogService Members
        public virtual async Task<RemoteClientLogServiceResult> PostAsync(RemoteClientLogModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Guard.ArgumentNotNull(model, nameof(model));

                var response = await m_NetworkManager.PerformRequest(m_ApiUrl, HttpMethodType.Post, model, requiresAuthentication: false, showLoadingScreen: false);

                switch (response?.Status)
                {
                    case HttpStatusCode.OK:
                        return new RemoteClientLogServiceResult(true, null);
                    default:
                        return new RemoteClientLogServiceResult(false, response?.Text);
                }
            }
            catch (Exception exc)
            {
                Debug.LogError(exc.Message);
                throw;
            }
        } 
        #endregion
    }
}
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Net;
using UnityEngine;
using Umbrella.Unity.Utilities.Async;
using Umbrella.Unity.Networking.Abstractions;
using System.Collections.Generic;
using Umbrella.Utilities;

namespace Umbrella.Unity.Networking.WebRequest
{
    public class UnityNetworkManager : IUnityNetworkManager
    {
        private struct ServerErrorMessage
        {
            public string Message { get; set; }
        }

        private readonly Dictionary<string, AssetBundle> m_CachedAssetBundleDictionary = new Dictionary<string, AssetBundle>();

        public event EventHandler<UnityNetworkRequestEventArgs> OnBeginRequest;
        public event EventHandler<UnityNetworkRequestEventArgs> OnEndRequest;

        protected Microsoft.Extensions.Logging.ILogger Log { get; }
        protected ITaskCompletionSourceProcessor TaskCompletionSourceProcessor { get; }
        protected IUnityAuthenticationAccessor AuthenticationAccessor { get; }

        public UnityNetworkManager(ILoggerFactory loggerFactory,
            ITaskCompletionSourceProcessor tcsProcessor,
            IUnityAuthenticationAccessor authenticationAccessor)
        {
            Log = loggerFactory.CreateLogger<UnityNetworkManager>();
            TaskCompletionSourceProcessor = tcsProcessor;
            AuthenticationAccessor = authenticationAccessor;
        }

        public virtual async Task<UnityNetworkResponse> PerformRequest(string url, HttpMethodType method = HttpMethodType.Get, object bodyContent = null, BodyEncodingType bodyEncodingType = BodyEncodingType.Json, bool requiresAuthentication = true, bool showLoadingScreen = true)
            => await PerformRequest<object>(url, method, bodyContent, bodyEncodingType, requiresAuthentication, showLoadingScreen);

        public virtual async Task<UnityNetworkResponse<TResponse>> PerformRequest<TResponse>(string url, HttpMethodType method = HttpMethodType.Get, object bodyContent = null, BodyEncodingType bodyEncodingType = BodyEncodingType.Json, bool requiresAuthentication = true, bool showLoadingScreen = true)
        {
            UnityWebRequest request = null;
            UploadHandler uploadHandler = null;
            DownloadHandler downloadHandler = null;
            
            try
            {
                Guard.ArgumentNotNullOrWhiteSpace(url, nameof(url));

                if (requiresAuthentication && !AuthenticationAccessor.IsAuthenticated)
                    throw new UnityNetworkAuthorizationException();

                string methodName = method.ToString().ToUpperInvariant();
                url = url.Trim().ToLowerInvariant();

                if(typeof(TResponse) == typeof(AssetBundle))
                {
                    //Before doing anything check if the asset bundle has already been loaded from the server.
                    AssetBundle bundle = m_CachedAssetBundleDictionary.ContainsKey(url) ? m_CachedAssetBundleDictionary[url] : null;

                    if (bundle != null)
                    {
                        return new UnityNetworkResponse<TResponse>
                        {
                            Status = HttpStatusCode.OK,
                            Result = (TResponse)(object)bundle
                        };
                    }

                    downloadHandler = new DownloadHandlerAssetBundle(url, 0);
                }
                else
                {
                    downloadHandler = new DownloadHandlerBuffer();
                }

                if(bodyContent != null)
                {
                    switch(bodyEncodingType)
                    {
                        case BodyEncodingType.FormUrlEncoded:
                            uploadHandler = new UnityFormUrlEncodedUploadHandler(bodyContent);
                            break;
                        case BodyEncodingType.Json:
                            uploadHandler = new UnityJsonUploadHandler<object>(bodyContent);
                            break;
                    }
                }

                request = new UnityWebRequest(url, methodName, downloadHandler, uploadHandler)
                {
                    disposeDownloadHandlerOnDispose = false,
                    disposeUploadHandlerOnDispose = false
                };

                var eventArgs = new UnityNetworkRequestEventArgs(url, request, requiresAuthentication, showLoadingScreen);

                OnBeginRequest?.Invoke(this, eventArgs);

                AsyncOperation op = request.Send();

                if(Log.IsEnabled(LogLevel.Debug))
                    Log.LogDebug("Request.Send");

                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(eventArgs);
                TaskCompletionSourceProcessor.Enqueue(tcs, () => op.isDone);

                await tcs.Task;

                OnEndRequest?.Invoke(this, eventArgs);

                long statusCode = request.responseCode;

                UnityNetworkResponse<TResponse> response = new UnityNetworkResponse<TResponse>();

                //Check for an internet connection first
                if (request.isNetworkError)
                {
                    response.Text = "We are having problems connecting to the server. Please check your internet connection is working and try again.";
                }
                else
                {
                    response.Status = (HttpStatusCode)statusCode;

                    //Only handle 401 responses where authentication was explicitly required. Allow the caller to handle the
                    //401 code themselves if it wasn't required, e.g. for a login endpoint.
                    if (response.Status == HttpStatusCode.Unauthorized && requiresAuthentication)
                        throw new UnityNetworkAuthorizationException();

                    string contentType = request.GetResponseHeader("Content-Type");

                    if (response.Status != HttpStatusCode.NoContent && !string.IsNullOrWhiteSpace(contentType))
                    {
                        if (contentType.Contains("application/json"))
                        {
                            string body = downloadHandler.text;

                            response.Text = body;

                            if(Log.IsEnabled(LogLevel.Debug))
                                Log.LogDebug($"Response Body JSON: {body}");

                            try
                            {
                                if (statusCode == 500)
                                {
                                    //500 responses from the server are wrapped in a JSON object with a message property. This needs to be read and set as the text instead.
                                    response.Text = UmbrellaStatics.DeserializeJson<ServerErrorMessage>(body).Message;
                                }
                                else
                                {
                                    //Try and parse the text from JSON
                                    response.Result = UmbrellaStatics.DeserializeJson<TResponse>(body);
                                }

                                //If we get this far then the text is definitely valid JSON
                                response.IsJsonResult = true;
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    //Try and deserialize the JSON to a string - we may have got back a simple error message
                                    string message = UmbrellaStatics.DeserializeJson<string>(body);

                                    if (!string.IsNullOrWhiteSpace(message))
                                        response.Text = message;
                                }
                                catch (Exception exc)
                                {
                                    Log.WriteError(exc, message: "The server response has content-type 'application/json' but could not be parsed to a JSON object.");

                                    throw new UnityNetworkException(exc.Message, exc);
                                }
                            }
                        }
                        else if(downloadHandler is DownloadHandlerAssetBundle)
                        {
                            var assetBundleHandler = downloadHandler as DownloadHandlerAssetBundle;

                            //Check again to ensure the bundle is not in the cache
                            AssetBundle bundle = m_CachedAssetBundleDictionary.ContainsKey(url) ? m_CachedAssetBundleDictionary[url] : null;

                            if(bundle == null)
                            {
                                bundle = assetBundleHandler.assetBundle;
                                m_CachedAssetBundleDictionary[url] = bundle;
                            }

                            response.Result = (TResponse)(object)bundle;
                        }
                        else
                        {
                            //If the response isn't JSON, just assign the raw bytes to the response to be dealt with by the caller
                            response.Bytes = downloadHandler.data;
                        }
                    }
                }

                return response;
            }
            catch (Exception exc)
            {
                if (exc is UnityNetworkException == false)
                    Log.WriteError(exc);

                throw;
            }
            finally
            {
                request?.Dispose();
                downloadHandler?.Dispose();
                uploadHandler?.Dispose();
            }
        }
    }
}
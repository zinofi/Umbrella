using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Umbrella.Unity.Networking.Abstractions
{
    public interface IUnityNetworkManager
    {
        event EventHandler<UnityNetworkRequestEventArgs> OnBeginRequest;
        event EventHandler<UnityNetworkRequestEventArgs> OnEndRequest;
        Task<UnityNetworkResponse> PerformRequest(string url, HttpMethodType method = HttpMethodType.Get, object bodyContent = null, BodyEncodingType bodyEncodingType = BodyEncodingType.Json, bool requiresAuthentication = true, bool showLoadingScreen = true);
        Task<UnityNetworkResponse<TResponse>> PerformRequest<TResponse>(string url, HttpMethodType method = HttpMethodType.Get, object bodyContent = null, BodyEncodingType bodyEncodingType = BodyEncodingType.Json, bool requiresAuthentication = true, bool showLoadingScreen = true);
    }
}
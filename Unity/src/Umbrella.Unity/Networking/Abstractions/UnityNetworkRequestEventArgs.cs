using UnityEngine.Networking;

namespace Umbrella.Unity.Networking.Abstractions
{
    public struct UnityNetworkRequestEventArgs
    {
        public string Url { get; }
        public UnityWebRequest Request { get; }
        public bool RequiresAuthentication { get; }
        public bool ShowLoadingScreen { get; }

        public UnityNetworkRequestEventArgs(string url, UnityWebRequest request, bool requiresAuthentication, bool showLoadingScreen)
        {
            Url = url;
            Request = request;
            RequiresAuthentication = requiresAuthentication;
            ShowLoadingScreen = showLoadingScreen;
        }
    }
}
using UnityEngine.Networking;

namespace Umbrella.Unity.Networking.Abstractions
{
    public struct UnityNetworkRequestEventArgs
    {
        public string Url { get; set; }
        public UnityWebRequest Request { get; set; }
        public bool RequiresAuthentication { get; set; }

        public UnityNetworkRequestEventArgs(string url, UnityWebRequest request, bool requiresAuthentication)
        {
            Url = url;
            Request = request;
            RequiresAuthentication = requiresAuthentication;
        }
    }
}
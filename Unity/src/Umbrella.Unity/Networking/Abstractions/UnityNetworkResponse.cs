using System.Net;

namespace Umbrella.Unity.Networking.Abstractions
{
    public class UnityNetworkResponse
    {
        public HttpStatusCode? Status { get; set; }
        public string Text { get; set; }
        public byte[] Bytes { get; set; }
    }

    public class UnityNetworkResponse<T> : UnityNetworkResponse
    {
        public T Result { get; set; }
        public bool IsJsonResult { get; set; }
    }
}
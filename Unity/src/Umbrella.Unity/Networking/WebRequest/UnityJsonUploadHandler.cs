using System.Text;
using Umbrella.Utilities;
using UnityEngine.Networking;

namespace Umbrella.Unity.Networking.WebRequest
{
    internal class UnityJsonUploadHandler<T>
    {
        public T Object { get; }

        public UnityJsonUploadHandler(T data)
        {
            Guard.ArgumentNotNull(data, nameof(data));

            Object = data;
        }

        public static implicit operator UploadHandlerRaw(UnityJsonUploadHandler<T> handler)
        {
            Guard.ArgumentNotNull(handler, nameof(handler));
            
            string json = UmbrellaStatics.SerializeJson(handler.Object);
            byte[] bytes = Encoding.Default.GetBytes(json);
            
            var uploadHandler = new UploadHandlerRaw(bytes)
            {
                contentType = "application/json"
            };

            return uploadHandler;
        }
    }
}
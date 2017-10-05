using System.Collections.Generic;
using Umbrella.Utilities;
using UnityEngine.Networking;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using Umbrella.Utilities.Extensions;
using System.Reflection;

namespace Umbrella.Unity.Networking.WebRequest
{
    internal class UnityFormUrlEncodedUploadHandler
    {
        private readonly NameValueCollection m_DataDictionary = new NameValueCollection();

        public UnityFormUrlEncodedUploadHandler()
        {
        }

        public UnityFormUrlEncodedUploadHandler(object data)
        {
            var type = data.GetType();

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var fieldMappings = fields.Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(data)));
            var propertyMappings = properties.Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(data)));

            foreach (var item in fieldMappings.Concat(propertyMappings))
                AddItem(item.Key, item.Value);
        }

        public UnityFormUrlEncodedUploadHandler(Dictionary<string, object> data)
        {
            Guard.ArgumentNotNullOrEmpty(data, nameof(data));

            foreach (var item in data)
                AddItem(item.Key, item.Value);
        }

        public UnityFormUrlEncodedUploadHandler AddItem<T>(string key, T value)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));
            Guard.ArgumentNotNull(value, nameof(value));

            m_DataDictionary.Add(key, value.ToString());

            return this;
        }

        public UnityFormUrlEncodedUploadHandler RemoveItem(string key)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

            string keyLowered = key.ToLowerInvariant();

            if (m_DataDictionary.AllKeys.Any(x => x == keyLowered))
                m_DataDictionary.Remove(keyLowered);

            return this;
        }

        //Can't extend the UploadHandler class as it's constructor is private - this is the next best thing.
        public static implicit operator UploadHandler(UnityFormUrlEncodedUploadHandler handler)
        {
            Guard.ArgumentNotNull(handler, nameof(handler));

            string body = handler.m_DataDictionary.SerializeToString();
            byte[] bytes = Encoding.Default.GetBytes(body);

            var rawHandler = new UploadHandlerRaw(bytes)
            {
                contentType = "application/x-www-form-urlencoded"
            };

            return rawHandler;
        }
    }
}
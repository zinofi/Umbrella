using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities;

namespace UnityEngine
{
    public static class TransformExtensions
    {
        public static T Find<T>(this Transform transform, string name)
            where T : Object
        {
            Guard.ArgumentNotNull(transform, nameof(transform));
            Guard.ArgumentNotNullOrWhiteSpace(name, nameof(name));

            Transform t = transform.Find(name);

            if (t == null)
                return null;

            return t.GetComponent<T>();
        }

        public static GameObject FindGameObject(this Transform transform, string name)
        {
            Guard.ArgumentNotNull(transform, nameof(transform));
            Guard.ArgumentNotNullOrWhiteSpace(name, nameof(name));

            return transform.Find(name)?.gameObject;
        }
    }
}
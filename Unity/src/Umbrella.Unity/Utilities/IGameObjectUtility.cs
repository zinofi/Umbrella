using UnityEngine;

namespace Umbrella.Unity.Utilities
{
    public interface IGameObjectUtility
    {
        GameObject FindOrCreate(string name, Transform parent = null);
        T FindOrCreate<T>(Transform parent = null) where T : Component;
    }
}
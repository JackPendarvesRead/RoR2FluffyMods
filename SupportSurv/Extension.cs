using RoR2;
using UnityEngine;

namespace SupportSurv
{
    public static class Extension
    {
        public static T AddOrGetComponent<T>(this GameObject prefab)
            where T : MonoBehaviour
        {
            var component = prefab.GetComponent<T>();
            if (component)
            {
                return component;
            }
            else
            {
                return prefab.AddComponent<T>();
            }
        }
    }
}

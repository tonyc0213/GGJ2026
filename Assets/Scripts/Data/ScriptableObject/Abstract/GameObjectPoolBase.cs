using UnityEngine;

namespace Data.ScriptableObject.Abstract
{
    public abstract class GameObjectPoolBase : UnityEngine.ScriptableObject
    {
        protected static Transform PoolRoot => _PoolRoot ??= InitializeRoot();
        private static Transform _PoolRoot;
        
        private static Transform InitializeRoot(){
            var root = new GameObject("PoolRoot");
            DontDestroyOnLoad(root);
            return root.transform;
        }
    }
}
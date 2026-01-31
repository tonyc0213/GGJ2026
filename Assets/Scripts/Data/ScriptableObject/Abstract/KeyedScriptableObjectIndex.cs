using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Data.ScriptableObject.Abstract
{
    public abstract class KeyedScriptableObjectIndex<TKey, TItem> : UnityEngine.ScriptableObject
    {
        public List<TItem> itemList;

        public abstract TKey GetKey(TItem item);
        
        public IReadOnlyDictionary<TKey, TItem> Index => GetIndex();
        private Dictionary<TKey, TItem> index;

        private IReadOnlyDictionary<TKey, TItem> GetIndex()
        {
            if (index == null)
            {
                index = itemList.ToDictionary(GetKey);
            }
            
            return index;
        }
        
        private void OnValidate()
        {
            HashSet<TKey> keys = new HashSet<TKey>();
            for (var i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                var key = GetKey(item);
                var valid = ValidateItem(item) && keys.Add(key);
                Assert.IsTrue(valid, $"Duplicated Keys {key}");
            }
        }

        protected virtual bool ValidateItem(TItem item)
        {
            return true;
        }
    }
}
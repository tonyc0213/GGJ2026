using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Data.ScriptableObject.Abstract
{
    public abstract class ScriptableObjectIndex<TKey, TItem> : UnityEngine.ScriptableObject where TKey : IEquatable<TKey>
    {
        [Serializable]
        public struct IndexItem
        {
            public TKey key;
            public TItem item;
        }
        
        [SerializeField]
        private IndexItem defaultItem;
        public List<IndexItem> itemList;
        
        
        public IndexItem DefaultItem => defaultItem;
        public IReadOnlyDictionary<TKey, TItem> Index => GetIndex();
        private Dictionary<TKey, TItem> index;

        private IReadOnlyDictionary<TKey, TItem> GetIndex()
        {
            if (index == null)
            {
                index = itemList.ToDictionary(x => x.key, x => x.item);
                index.Add(defaultItem.key, defaultItem.item);
            }
            
            return index;
        }
        
        private void OnValidate()
        {
            Assert.IsTrue(defaultItem.item != null, "Default Item is null");
            
            HashSet<TKey> keys = new HashSet<TKey>();
            keys.Add(defaultItem.key);
            
            for (var i = 0; i < itemList.Count; i++)
            {
                var key = itemList[i].key;
                var valid = keys.Add(key);
                Assert.IsTrue(valid, $"Duplicated Keys {key}");
            }
        }
        
        public TItem GetItem(TKey key)
        {
            if (!Index.TryGetValue(key, out TItem item))
            {
                Debug.LogError($"Cannot find item of key {key}");
                return defaultItem.item;
            }
            return item;
        }

    }
}
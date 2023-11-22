using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<KeyValueEntry> entries = new List<KeyValueEntry>();
    private List<TKey> keys = new List<TKey>();

    [System.Serializable]
    class KeyValueEntry
    {
        public TKey key;
        public TValue value;
    }

    public void OnAfterDeserialize()
    {
        Clear();

        for (int i = 0; i < entries.Count; i++)
        {
            this.Add(entries[i].key, entries[i].value);
        }
    }

    public void OnBeforeSerialize()
    {
        if (entries == null)
        {
            return;
        }

        keys.Clear();

        for (int i = 0; i < entries.Count; i++)
        {
            keys.Add(entries[i].key);
        }

        var result = keys.GroupBy(x => x)
                         .Where(g => g.Count() > 1)
                         .Select(x => new { Element = x.Key, Count = x.Count() })
                         .ToList();

        if (result.Count > 0)
        {
            var duplicates = string.Join(", ", result);
            Debug.LogError($"Warning {GetType().FullName} keys has duplicates {duplicates}");
        }
    }
}
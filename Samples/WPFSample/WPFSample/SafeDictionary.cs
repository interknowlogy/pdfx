using System.Collections.Generic;

namespace WPFSample
{
	public class SafeDictionary<TKey, TValue>
	{
		Dictionary<TKey, TValue> _innerDictionary = new Dictionary<TKey, TValue>();

		public void Add(TKey key, TValue value)
		{
			_innerDictionary.Add(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			return _innerDictionary.ContainsKey(key);
		}

		public void Clear()
		{
			_innerDictionary.Clear();
		}

		public TValue this[TKey key]
		{
			get
			{
				if (!_innerDictionary.ContainsKey(key))
					return default(TValue);
					
				return _innerDictionary[key];
			}
			set { _innerDictionary[key] = value; }
		}
	}
}
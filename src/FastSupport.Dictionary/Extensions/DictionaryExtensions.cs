namespace FastSupport.Dictionary;

public static class DictionaryExtensions
{
	public static void AddRange<TKey, TData, TEntity>(
		this IDictionary<TKey, TData> dictionary,
		IEnumerable<TEntity> entities,
		Func<TEntity, KeyValuePair<TKey, TData>> convert)
	{
		foreach(var item in entities) dictionary.Add(convert(item));
	}

	public static void AddRange<TKey, TData>(
		this IEnumerable<KeyValuePair<TKey, TData>> pairs,
		IDictionary<TKey, TData> dictionary)
	{
		foreach(var item in pairs) dictionary.Add(item);
	}
}

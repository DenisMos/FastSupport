namespace FastSupport.File.Handlers;

public static class PairToEntity
{
	public static TEntity ToEntity<TEntity, TKey, TValue>(
		this IEnumerable<KeyValuePair<TKey, TValue>> data,
		KeyValuePair<TKey, TValue> header,
		Func<KeyValuePair<TKey, TValue>, TEntity> constructor,
		params (TKey, Action<TEntity, KeyValuePair<TKey, TValue>>)[] funcs)
	{
		var cstr = constructor(header);

		foreach(var pair in data)
		{
			var func = funcs.FirstOrDefault(x => x.Item1?.Equals(pair.Key) ?? false);
			if(func.Item2 != null)
			{
				func.Item2(cstr, pair);
			}
		}

		return cstr;
	}

	public static IEnumerable<TEntity> ToEntityMany<TEntity, TKey, TValue>(
		this IEnumerable<KeyValuePair<TKey, TValue>> data,
		(TKey, Func<KeyValuePair<TKey, TValue>, TEntity>) constructor,
		params (TKey, Action<TEntity, KeyValuePair<TKey, TValue>>)[] funcs)
	{
		TEntity? entity = default;
		foreach(var pair in data)
		{
			if(pair.Key?.Equals(constructor.Item1) ?? false)
			{
				if(entity != null)
				{ 
					yield return entity;
				}

				entity = constructor.Item2(pair);
			}

			if(entity != null)
			{ 
				var func = funcs.FirstOrDefault(x => x.Item1?.Equals(pair.Key) ?? false);
				if(func.Item2 != null)
				{
					func.Item2(entity, pair);
				}
			}
		}

		if(entity != null)
		{
			yield return entity;
		}
	}
}

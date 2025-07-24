namespace FastSupport.File.Handlers;

/// <summary>Позволяет разбивать класс на пары <c>Ключ</c>/<c>Значение</c>.</summary>
public static class EntityToPair
{
	/// <summary>Разбивает и возвращает наборы пар указанным способом.</summary>
	/// <typeparam name="TKey">Ключ.</typeparam>
	/// <typeparam name="TValue">Знаечние.</typeparam>
	/// <typeparam name="TSource">Источник данных.</typeparam>
	/// <param name="source">Источник данных.</param>
	/// <param name="funcs">Способы разбиения.</param>
	/// <returns></returns>
	public static IEnumerable<KeyValuePair<TKey, TValue>> ToPair<TKey, TValue, TSource>(
		this TSource source,
		params Func<TSource, (TKey, TValue)>[] funcs)
	{
		foreach(var func in funcs)
		{
			var val = func(source);
			yield return new KeyValuePair<TKey, TValue>(val.Item1, val.Item2);
		}
	}

	/// <summary>Разбивает и возвращает наборы пар указанным способом для множества источников.</summary>
	/// <typeparam name="TKey">Ключ.</typeparam>
	/// <typeparam name="TValue">Знаечние.</typeparam>
	/// <typeparam name="TSource">Источник данных.</typeparam>
	/// <param name="sources">Источники данных.</param>
	/// <param name="header">Заголовок.</param>
	/// <param name="funcs">Способы разбиения.</param>
	/// <returns></returns>
	public static IEnumerable<KeyValuePair<TKey, TValue>> ToPairMany<TKey, TValue, TSource>(
		this IEnumerable<TSource> sources,
		Func<TSource, (TKey, TValue)> header,
		params Func<TSource, (TKey, TValue)>[] funcs)
	{
		foreach(var source in sources)
		{
			var headerData = header(source);
			yield return new KeyValuePair<TKey, TValue>(headerData.Item1, headerData.Item2);
			
			foreach(var data in ToPair(source, funcs)) 
			{
				yield return data;
			}
		}
	}
}

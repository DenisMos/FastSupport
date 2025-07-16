using System.ComponentModel;

namespace FastSupport.File.Handlers;

public static class FlatDictionaryIOExtensions
{
	public static IEnumerable<KeyValuePair<TKey, TValue>> ReadFileDictionaryAsEnums<TKey, TValue>(
		this FlatDictionaryIO<TKey, TValue> parser,
		string fileName,
		Func<string, TValue> valueParse) where TKey : struct
	{
		foreach(var item in parser.ReadFileDictionary(
			fileName,
			dataKey => (Enum.TryParse(dataKey, true, out TKey result), result),
			val => valueParse(val)))
		{
			yield return item;
		}
	}

	public static void SaveFileDictionaryAsEnums<TKey, TValue>(
		this FlatDictionaryIO<TKey, TValue> parser,
		string fileName,
		Dictionary<TKey, TValue> dict) where TKey : struct
	{
		SaveFileDictionaryAsEnums(parser, fileName, dict, valueSaver: val => val?.ToString() ?? throw new ArgumentNullException());
	}

	public static void SaveFileDictionaryAsEnums<TKey, TValue>(
		this FlatDictionaryIO<TKey, TValue> parser,
		string fileName,
		Dictionary<TKey, TValue> dict,
		Func<TValue, string> valueSaver) where TKey : struct
	{
		parser.SaveFileDictionary(
			fileName,
			dict,
			keyParse: key => key.ToString() ?? throw new InvalidEnumArgumentException(),
			valueParse: val => valueSaver(val));
	}
}

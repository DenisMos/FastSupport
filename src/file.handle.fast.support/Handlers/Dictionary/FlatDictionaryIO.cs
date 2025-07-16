namespace FastSupport.File.Handlers;


/// <summary>Построчный обработчик файлов со словарной структурой <c>Key</c>:<c>Value</c>.</summary>
public class FlatDictionaryIO<TKey, TValue> : IDictionaryFileIO
{
	private readonly char[] _separator;
	private readonly string[] _ignoreRowIfStartWith;

	/// <summary>Initializes a new instance of the <see cref="DictionaryFileHandle"/> class.</summary>
	/// <param name="separator">The separator.</param>
	/// <param name="ignoreRowIfStartWith">The ignore row if start with.</param>
	public FlatDictionaryIO(
		char[]? separator = null,
		string[]? ignoreRowIfStartWith = null)
	{
		_separator = separator ?? FlatDictionaryIOConstants.Splitter;
		_ignoreRowIfStartWith = ignoreRowIfStartWith ?? FlatDictionaryIOConstants.Ignorable;
	}

	/// <summary>Reads the file dictionary.</summary>
	/// <param name="fileName">The file name.</param>
	/// <param name="keyParse">The key parse.</param>
	/// <param name="valueParse">The value parse.</param>
	/// <returns>A list of KeyValuePair.</returns>
	public IEnumerable<KeyValuePair<TKey, TValue>> ReadFileDictionary(
		string fileName,
		Func<string, (bool, TKey)> keyParse,
		Func<string, TValue> valueParse)
	{
		using(var sr = new StreamReader(fileName))
		{
			while(!sr.EndOfStream)
			{
				var row = sr.ReadLine();

				if(string.IsNullOrEmpty(row) || _ignoreRowIfStartWith.Any(x => row.StartsWith(x))) continue;

				var columns = row.Split(_separator);
				if(columns.Length <= 1) continue;

				var keyData = keyParse(columns[0].Trim());
				if(keyData.Item1)
				{
					yield return new KeyValuePair<TKey, TValue>(keyData.Item2, valueParse(columns[1].Trim()));
				}
			}
		}
	}

	/// <summary>Saves the file dictionary.</summary>
	/// <param name="fileName">The file name.</param>
	/// <param name="dict">The dict.</param>
	/// <param name="keyParse">The key parse.</param>
	/// <param name="valueParse">The value parse.</param>
	public void SaveFileDictionary(
		string fileName,
		IDictionary<TKey, TValue> dict,
		Func<TKey, string> keyParse,
		Func<TValue, string> valueParse)
	{
		var sep = _separator.First();

		using(var sr = new StreamWriter(fileName))
		{
			foreach(var row in dict)
			{
				var key = row.Key;
				var value = row.Value;

				sr.WriteLine($"{keyParse(key)}{sep}{valueParse(value)}");
			}
		}
	}
}

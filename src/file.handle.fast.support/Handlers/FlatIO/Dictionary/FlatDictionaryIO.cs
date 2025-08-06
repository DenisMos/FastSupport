namespace FastSupport.File.Handlers;


/// <summary>Построчный обработчик файлов со словарной структурой <c>Key</c>:<c>Value</c>.</summary>
public class FlatDictionaryIO<TKey, TValue> : FlatIO
{
	/// <summary>Initializes a new instance of the <see cref="DictionaryFileHandle"/> class.</summary>
	/// <param name="separator">The separator.</param>
	/// <param name="ignoreRowIfStartWith">The ignore row if start with.</param>
	public FlatDictionaryIO(
		char[]? separator = null,
		string[]? ignoreRowIfStartWith = null) : base(separator, ignoreRowIfStartWith)
	{
	}

	/// <summary>Reads the file dictionary.</summary>
	/// <param name="fileName">The file name.</param>
	/// <param name="keyParse">The key parse.</param>
	/// <param name="valueParse">The value parse.</param>
	/// <returns>A list of KeyValuePair.</returns>
	public IEnumerable<KeyValuePair<TKey, TValue>> ReadFile(
		string fileName,
		Func<string, (bool, TKey)> keyParse,
		Func<string, TValue> valueParse)
	{
		using var sr = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		foreach(var item in ReadFile(sr, keyParse, valueParse))
		{ 
			yield return item;
		}
	}

	/// <summary>Reads the file dictionary.</summary>
	/// <param name="fileStream">The file name.</param>
	/// <param name="keyParse">The key parse.</param>
	/// <param name="valueParse">The value parse.</param>
	/// <returns>A list of KeyValuePair.</returns>
	public IEnumerable<KeyValuePair<TKey, TValue>> ReadFile(
		FileStream fileStream,
		Func<string, (bool, TKey)> keyParse,
		Func<string, TValue> valueParse)
	{
		using(var sr = new StreamReader(fileStream))
		{
			foreach(var row in ParseFlatRow(sr))
			{
				var keyData = keyParse(row.Key);
				if(keyData.Item1)
				{
					yield return new KeyValuePair<TKey, TValue>(keyData.Item2, valueParse(row.Value));
				}
			}
		}
	}

	/// <summary>Saves the file dictionary.</summary>
	/// <param name="fileName">The file name.</param>
	/// <param name="dict">The dict.</param>
	/// <param name="keyParse">The key parse.</param>
	/// <param name="valueParse">The value parse.</param>
	public void SaveFile(
		FileStream fileStream,
		IEnumerable<KeyValuePair<TKey, TValue>> dict,
		Func<TKey, string> keyParse,
		Func<TValue, string> valueParse)
	{
		using(var sr = new StreamWriter(fileStream))
		{
			foreach(var row in dict)
			{
				var key   = row.Key;
				var value = row.Value;

				PushInFile(sr, keyParse(key), valueParse(value));
			}
		}
	}

	/// <summary>Saves the file dictionary.</summary>
	/// <param name="fileName">The file name.</param>
	/// <param name="dict">The dict.</param>
	/// <param name="keyParse">The key parse.</param>
	/// <param name="valueParse">The value parse.</param>
	public void SaveFile(
		string fileName,
		IEnumerable<KeyValuePair<TKey, TValue>> dict,
		Func<TKey, string> keyParse,
		Func<TValue, string> valueParse)
	{
		using(var fileStrem = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
		{
			SaveFile(fileStrem, dict, keyParse, valueParse);
		}
	}
}

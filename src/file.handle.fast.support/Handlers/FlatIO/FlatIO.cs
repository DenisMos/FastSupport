namespace FastSupport.File.Handlers;

public abstract class FlatIO
{
	protected readonly char[] _separator;
	protected readonly string[] _ignoreRowIfStartWith;

	private readonly char _sep;

	public FlatIO(
		char[]? separator = null,
		string[]? ignoreRowIfStartWith = null)
	{
		_separator = separator ?? FlatIOConstants.Splitter;
		_ignoreRowIfStartWith = ignoreRowIfStartWith ?? FlatIOConstants.Ignorable;

		_sep = _separator.First();
	}

	protected void PushInFile(StreamWriter sw, string key, object val)
	{
		sw.WriteLine($"{key}{_sep}{val}");
	}

	protected IEnumerable<KeyValuePair<string, string>> ParseFlatRow(StreamReader sr)
	{
		while(!sr.EndOfStream)
		{
			var row = sr.ReadLine();

			if(string.IsNullOrEmpty(row) || _ignoreRowIfStartWith.Any(x => row.StartsWith(x))) continue;

			var columns = row.Split(_separator);
			if(columns.Length <= 1) continue;

			var key = columns[0].Trim();
			var val = columns[1].Trim();

			yield return new(key, val);
		}
	}
}
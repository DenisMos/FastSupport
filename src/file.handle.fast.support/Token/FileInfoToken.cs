namespace FastSupport.File;

public sealed class FileInfoToken
{
	private readonly string _filename;
	private FileStream? _stream;

	public string FileName => _filename;


	public FileInfoToken(string fileName) 
	{
		_filename = fileName;
	}

	public FileStream OpenOrCreate()
	{
		if(_stream != null)
		{
			_stream?.Dispose();
		}
		return _stream = new FileStream(_filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
	}

	public static FileInfoToken GetDefaultConfigToken(string name) => Create($"{name}.cfg");

	public static FileInfoToken GetDefaultTextToken(string name) => Create($"{name}.txt");

	private static FileInfoToken Create(string filename) => new FileInfoToken(filename);
}

using FastSupport.File.Handlers;

var data = new Data()
{ 
	Dx = 1,
	Dy = 2,
	H  = 3,
	W  = 4,
};

using(var fs = new FileStream("data.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
{ 
	var handle = new FlatEntityIO();
	handle.SaveFile(data, fs, connectWithCtr: true);
}

using(var fs = new FileStream("data.txt", FileMode.Open, FileAccess.Read))
{
	var handle = new FlatEntityIO();
	var converters = new Dictionary<Type, Func<object, object>>()
	{ 
		{ typeof(Data), x => new Data() }
	};
	var res = handle.ReadFile<Data>(fs, converters, connectWithCtr: true);
}

record Data
{
	public int Dx;
	public int Dy;
	public int W;
	public int H;

	public Data(int DX = 5, string Name = "")
	{
		this.DX   = DX;
		this.Name = Name;
	}

	public int DX { get; }

	public string Name { get; }
}
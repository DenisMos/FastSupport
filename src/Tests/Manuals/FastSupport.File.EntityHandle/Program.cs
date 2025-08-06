using FastSupport.File.Handlers;

var data = new Data()
{ 
	Dx = 1,
	Dy = 2,
	H  = 3,
	W  = 4,
	DX = 5,
	Data2 = new Data()
};

using(var fs = new FileStream("data.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
{ 
	var handle = new FlatEntityIO();
	handle.SaveFile(data, fs);
}

using(var fs = new FileStream("data.txt", FileMode.Open, FileAccess.Read))
{
	var handle = new FlatEntityIO();
	var converters = new Dictionary<Type, Func<object, object>>()
	{ 
		{ typeof(Data), x => new Data() }
	};
	var res = handle.ReadFile<Data>(fs, converters);
}

class Data
{
	public int Dx;
	public int Dy;
	public int W;
	public int H;


	public int DX { get; set; }

	public Data Data2 { get; set; }

	public override string ToString()
	{
		return $"Это строка";
	}
}
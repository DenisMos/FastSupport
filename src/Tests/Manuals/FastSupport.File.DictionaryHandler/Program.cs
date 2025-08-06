using FastSupport.File;
using FastSupport.File.Handlers;

using FastSupport.Dictionary.Pairs;

var dictionary = new Dictionary<string, object>
{
	{ nameof(IDictionary<string, object>), typeof(IDictionary<string, object>) },
	{ nameof(Int32), typeof(int) },
	{ nameof(Int64), typeof(long) }
};

var dictionaryFileHandler = new FlatDictionaryIO<string, object>();
var fileTest1 = FileInfoToken.GetDefaultConfigToken("test1");
using(var stream = fileTest1.OpenOrCreate())
{
	dictionaryFileHandler.SaveFile(
		stream,
		dictionary,
		key => key,
		val => val?.ToString() ?? string.Empty);
}

//Чтение
using(var stream = fileTest1.OpenOrCreate())
{
	foreach(var key in dictionaryFileHandler.ReadFile(
			stream,
			key => (true, key),
			val => val))
	{
		Console.WriteLine($"{key.Key} : {key.Value}");
	}
}

// Test.2 Запись enums объектов
var dictEnums = new Dictionary<GrdVD_BindParameter, uint>
{
	{ GrdVD_BindParameter.WindowsParameters, 12 },
	{ GrdVD_BindParameter.DVD_0, 22 },
	{ GrdVD_BindParameter.HDD_0, 52 },
	{ GrdVD_BindParameter.CPU, 12 }
};

var dictionaryFileHandler2 = new FlatDictionaryIO<GrdVD_BindParameter, uint>();
dictionaryFileHandler2.SaveFileDictionaryAsEnums("test2.cfg", dictEnums);

foreach(var item in dictionaryFileHandler2.ReadFileDictionaryAsEnums(
	"test2.cfg",
	val => Convert.ToUInt32(val)))
{
	Console.WriteLine($"{item.Key} : {item.Value}");
}

// Test.3


var flatContainer = new FlatDictionaryIO<string, int>();
var con = flatContainer.ReadFile(
	@"F:\Git\Repositories\Mallenoms\multitracking\src\Test\multitracking.calibration\bin\Debug\net7.0\calib.txt",
	key => (true, key), 
	val => Convert.ToInt32(val));

var resdata = con.ToEntityMany(
	constructor: ("ChannelId", pair => new Data()),
	("dx", (d, p) => d.Dx = p.Value),
	("dy", (d, p) => d.Dy = p.Value),
	("sizeX",  (d, p) => d.W = p.Value),
	("sizeY",  (d, p) => d.H = p.Value)).ToArray();


class Data
{
	public int Dx;
	public int Dy;
	public int W;
	public int H;
}
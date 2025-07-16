// Test.1 Запись any объектов
using FastSupport.File.DictionaryHandler;
using FastSupport.File.Handlers;

var dictionary = new Dictionary<string, object>
{
	{ nameof(IDictionary<string, object>), typeof(IDictionary<string, object>) },
	{ nameof(Int32), typeof(int) },
	{ nameof(Int64), typeof(Int64) }
};

var dictionaryFileHandler = new FlatDictionaryIO<string, object>();
dictionaryFileHandler.SaveFileDictionary(
	"test1.cfg", 
	dictionary,
	key => key,
	val => val?.ToString() ?? string.Empty);

//Чтение
foreach(var key in dictionaryFileHandler.ReadFileDictionary(
	"test1.cfg", 
	key => (true, key),
	val => val))
{ 
	Console.WriteLine($"{key.Key} : {key.Value}");	
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




using System.Reflection;

namespace FastSupport.File.Handlers;

/// <summary>Построчный обработчик файлов с классовой структурой.</summary>
/// <remarks>Работает на основе рефлексии. Не учитывает вложения и новые типы, если не переопределены конвертеры.</remarks>
/// <remarks>Для иерархии или более сложных решений используйте стандартные средства (JSON, XML и прочее) серелизации.</remarks>
public sealed class FlatEntityIO : FlatIO
{
	/// <summary>Initializes a new instance of the <see cref="FlatEntityIO"/> class.</summary>
	/// <param name="separator">The separator.</param>
	/// <param name="ignoreRowIfStartWith">The ignore row if start with.</param>
	public FlatEntityIO(
		char[]? separator = null,
		string[]? ignoreRowIfStartWith = null) : base(separator, ignoreRowIfStartWith)
	{
	}

	public TEntity ReadFile<TEntity>(
		FileStream stream,
		IDictionary<Type, Func<object, object>>? converters = null,
		bool readFields = true,
		bool readProperties = true)
	{
		var type = typeof(TEntity);

		FieldInfo[] fieldInfos   = Array.Empty<FieldInfo>();
		PropertyInfo[] propInfos = Array.Empty<PropertyInfo>();

		if(readFields)
		{
			fieldInfos = type.GetFields();
		}
		if(readProperties)
		{ 
			propInfos = type.GetProperties();
		}

		object? obj;
		try
		{
			obj = Activator.CreateInstance(type);
		}
		catch(Exception exc)
		{
			throw new Exception($".ctor can be empty!");
		}

		using(var sr = new StreamReader(stream))
		{
			foreach(var row in ParseFlatRow(sr))
			{
				var key = row.Key;
				var val = row.Value;

				var field = fieldInfos.FirstOrDefault(x => x.Name.Equals(key));
				if(field != null)
				{
					Push(field.FieldType, field.SetValue, converters, obj, val);
				}
				else
				{
					var prop = propInfos.FirstOrDefault(x => x.Name.Equals(key));
					if(prop != null)
					{
						Push(prop.PropertyType, prop.SetValue, converters, obj, val);
					}
				}
			}
		}

		return (TEntity)obj;
	}

	private void Push(Type type, Action<object, object> action, IDictionary<Type, Func<object, object>>? converters, object obj, object val)
	{
		switch(type)
		{
			case Type t when(t == typeof(int) || t == typeof(decimal)):
				action(obj, Convert.ToInt32(val)); break;
			case Type t when(t == typeof(float) || t == typeof(decimal)):
				action(obj, Convert.ToSingle(val)); break;
			case Type t when(t == typeof(string)):
				action(obj, val); break;
			case Type t when(t == typeof(long) || t == typeof(decimal)):
				action(obj, Convert.ToInt64(val)); break;
			case Type t when(t == typeof(byte)):
				action(obj, Convert.ToByte(val)); break;
			case Type t when(t == typeof(bool)):
				action(obj, Convert.ToBoolean(val)); break;
			default:
				if(converters != null && converters.TryGetValue(type, out Func<object, object> converter))
				{
					action(obj, converter.Invoke(val)); break;
				}
				throw new ArgumentException($"Converter for type '{type.Name}' not founded! Added self converter in 'converters'!");
		}
	}

	public void SaveFile<TEntity>(
		TEntity entity, 
		FileStream fileStream,
		bool saveFields = true, 
		bool saveProperties = true) where TEntity : class
	{ 
		var type = typeof(TEntity);

		using(var sw = new StreamWriter(fileStream))
		{
			if(saveFields)
			{
				var fields = type.GetFields();

				foreach(var field in fields)
				{
					PushInFile(sw, field.Name, field.GetValue(entity));
				}
			}
			if(saveProperties)
			{
				var bindings = System.Reflection.BindingFlags.Instance
					| System.Reflection.BindingFlags.Public
					| System.Reflection.BindingFlags.NonPublic
					| System.Reflection.BindingFlags.GetProperty
					| System.Reflection.BindingFlags.SetProperty;

				var props = type.GetProperties(bindings);

				foreach(var porp in props)
				{
					PushInFile(sw, porp.Name, porp.GetValue(entity));
				}
			}
		}
	}
}

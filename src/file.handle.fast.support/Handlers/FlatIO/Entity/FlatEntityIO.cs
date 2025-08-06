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
		bool readProperties = true,
		bool connectCtorAndFields = false)
	{
		var type = typeof(TEntity);
		object? obj = default;

		List<IDataField> fieldInfos = new();
		if(readFields)
		{
			fieldInfos.AddRange(type.GetFields().Select(x => new FieldDataField(x)));
		}
		if(readProperties)
		{
			fieldInfos.AddRange(type.GetProperties().Select(x => new PropertyDataField(x)));
		}

		if(connectCtorAndFields)
		{
			var listArgs = new List<object>();
			var schema = GetCtrSchema(type, fieldInfos);
			var count = schema.Count();

			using(var sr = new StreamReader(stream))
			{
				foreach(var row in ParseFlatRow(sr))
				{
					var key = row.Key;
					var val = row.Value;

					if(obj != null)
					{
						var sch = fieldInfos.FirstOrDefault(x => x.Name.Equals(key));
						if(sch!= null)
						{
							DataTypeConvert.Push(sch, converters, obj, val);
						}
					}

					if(count > 0)
					{
						var sch = schema.FirstOrDefault(x => x.Key.Equals(key));
						if(sch.Value != null)
						{
							listArgs.Add(DataTypeConvert.ConvertTo(sch.Value, converters, val));
						}
					}
					count--;
					if(count == 0)
					{
						obj = Activator.CreateInstance(type, listArgs.ToArray());
					}
				}
			}
		}
		else
		{
			obj = Activator.CreateInstance(type, 5);
			using(var sr = new StreamReader(stream))
			{
				foreach(var row in ParseFlatRow(sr))
				{
					var key = row.Key;
					var val = row.Value;

					var field = fieldInfos.FirstOrDefault(x => x.Name.Equals(key));
					if(field != null)
					{
						DataTypeConvert.Push(field, converters, obj, val);
					}
				}
			}
			
		}

		if(obj is null)
		{
			throw new Exception();
		}

		return (TEntity)obj;
	}

	private IEnumerable<KeyValuePair<string, IDataField>> GetCtrSchema(Type type, IEnumerable<IDataField> fieldInfos)
	{
		var ctrs = type.GetConstructors().First();
		foreach(var @param in ctrs.GetParameters())
		{
			var name = @param.Name;

			var field = fieldInfos.FirstOrDefault(x => x.Name.Equals(name));
			if(field != null)
			{
				yield return new KeyValuePair<string, IDataField>(name, field);
			}
		}
	}

	public void SaveFile<TEntity>(
		TEntity entity, 
		FileStream fileStream,
		bool saveFields = true, 
		bool saveProperties = true,
		bool connectCtorAndFields = false) where TEntity : class
	{ 
		var type = typeof(TEntity);

		using(var sw = new StreamWriter(fileStream))
		{
			List<IDataField> fieldInfos = new();
			if(saveFields)
			{
				fieldInfos.AddRange(type.GetFields().Select(x => new FieldDataField(x)));
			}
			if(saveProperties)
			{
				fieldInfos.AddRange(type.GetProperties().Select(x => new PropertyDataField(x)));
			}

			if(connectCtorAndFields)
			{
				foreach(var schema in GetCtrSchema(type, fieldInfos))
				{
					PushInFile(sw, schema.Value.Name, schema.Value.GetValue(entity));
					fieldInfos.Remove(schema.Value);
				}
				foreach(var item in fieldInfos)
				{
					PushInFile(sw, item.Name, item.GetValue(entity));
				}
			}
			else
			{
				foreach(var item in fieldInfos)
				{
					PushInFile(sw, item.Name, item.GetValue(entity));
				}
			}
		}
	}
}

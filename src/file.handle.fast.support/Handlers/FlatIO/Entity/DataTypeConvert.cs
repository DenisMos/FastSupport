namespace FastSupport.File.Handlers;

internal static class DataTypeConvert
{
	internal static object ConvertTo(
		IDataField action,
		IDictionary<Type, Func<object, object>>? converters,
		object val)
	{
		switch(action.DataType)
		{
			case Type t when(t == typeof(int) || t == typeof(decimal)):
				return Convert.ToInt32(val);
			case Type t when(t == typeof(float) || t == typeof(decimal)):
				return Convert.ToSingle(val);
			case Type t when(t == typeof(string)):
				return val;
			case Type t when(t == typeof(long) || t == typeof(decimal)):
				return Convert.ToInt64(val);
			case Type t when(t == typeof(double) || t == typeof(decimal)):
				return Convert.ToDouble(val);
			case Type t when(t == typeof(DateTime)):
				return Convert.ToDateTime(val);
			case Type t when(t == typeof(byte)):
				return Convert.ToByte(val);
			case Type t when(t == typeof(bool)):
				return Convert.ToBoolean(val);
			case Type t when(t == typeof(uint)):
				return Convert.ToUInt32(val);

			//Добавляем типы по надобности
			default:
				if(converters != null && converters.TryGetValue(action.DataType, out Func<object, object> converter))
				{
					return converter.Invoke(val);
				}
				throw new ArgumentException($"Converter for type '{action.DataType.Name}' not founded! Added self converter in 'converters'!");
		}
	}

	internal static void Push(
		IDataField action, 
		IDictionary<Type, Func<object, object>>? converters, 
		object obj, 
		object val)
	{
		action.SetValue(obj, ConvertTo(action, converters, val));
	}
}

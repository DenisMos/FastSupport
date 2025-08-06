using System.Reflection;

namespace FastSupport.File.Handlers;

internal interface IDataField
{
	string Name { get; }

	Type DataType { get; }

	object GetValue(object instance);

	void SetValue(object instance, object value);
}

internal class PropertyDataField : IDataField
{
	private readonly PropertyInfo _data;

	public PropertyDataField(PropertyInfo propertyInfo)
	{
		_data = propertyInfo;
	}

	public string Name => _data.Name;

	public Type DataType => _data.PropertyType;

	public object GetValue(object instance) => _data.GetValue(instance);

	public void SetValue(object instance, object value)
	{
		if(_data.CanWrite)
		{ 
			_data.SetValue(instance, value);
		}
	}
}

internal class FieldDataField : IDataField
{
	private readonly FieldInfo _data;

	public FieldDataField(FieldInfo propertyInfo)
	{
		_data = propertyInfo;
	}

	public Type DataType => _data.FieldType;

	public string Name => _data.Name;

	public object GetValue(object instance) => _data.GetValue(instance);

	public void SetValue(object instance, object value) => _data.SetValue(instance, value);
}

using System;
using System.Reflection;

namespace Victor.Tools
{
	/// <summary>
	/// Provides an mechanism to access properties through the <see cref="IDynamicAccessor" /> abstraction.
	/// </summary>
	internal class DynamicProperty : IDynamicAccessor
	{
		private readonly PropertyInfo _propertyInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicProperty" /> class wrapping the specified property.
		/// </summary>
		/// <param name="property">The property info to wrap.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="property" /> is <c>null</c>.</exception>
		internal DynamicProperty(PropertyInfo property)
		{
			if (property == null)
			{
				throw new ArgumentNullException(nameof(property));
			}

			_propertyInfo = property;
		}

		public Type PropertyType => _propertyInfo.PropertyType;

		string IDynamicAccessor.Name => _propertyInfo.Name;

		object IDynamicAccessor.GetValue(object obj, object[] index)
		{
			return _propertyInfo.GetValue(obj, index);
		}

		void IDynamicAccessor.SetValue(object obj, object value, object[] index)
		{
			_propertyInfo.SetValue(obj, value, index);
		}
	}
}
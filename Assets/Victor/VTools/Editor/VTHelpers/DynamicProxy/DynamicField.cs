using System;
using System.Reflection;

namespace Victor.Tools
{
	/// <summary>
	/// Provides a mechanism to access fields through the <see cref="IDynamicAccessor" /> abstraction.
	/// </summary>
	internal class DynamicField : IDynamicAccessor
	{
		private readonly FieldInfo _fieldInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicField" /> class wrapping the specified field.
		/// </summary>
		/// <param name="field">The field info to wrap.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="field" /> is <c>null</c>.</exception>
		internal DynamicField(FieldInfo field)
		{
			if (field == null)
			{
				throw new ArgumentNullException(nameof(field));
			}

			_fieldInfo = field;
		}

		public Type PropertyType => _fieldInfo.FieldType;

		string IDynamicAccessor.Name => _fieldInfo.Name;

		object IDynamicAccessor.GetValue(object obj, object[] index)
		{
			return _fieldInfo.GetValue(obj);
		}

		void IDynamicAccessor.SetValue(object obj, object value, object[] index)
		{
			_fieldInfo.SetValue(obj, value);
		}
	}
}
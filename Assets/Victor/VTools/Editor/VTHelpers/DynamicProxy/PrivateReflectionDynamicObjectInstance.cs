using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Victor.Tools
{
	public class PrivateReflectionDynamicObjectInstance : PrivateReflectionDynamicObjectBase
	{
		private static readonly ConcurrentDictionary<Type, IDictionary<string, IDynamicAccessor>> _accessorsOnType = new ConcurrentDictionary<Type, IDictionary<string, IDynamicAccessor>>();

		private readonly object _instance;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrivateReflectionDynamicObjectInstance" /> class, wrapping the specified
		/// object.
		/// </summary>
		/// <param name="instance">The object to wrap.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="instance" /> is <c>null</c>.</exception>
		public PrivateReflectionDynamicObjectInstance(object instance)
		{
			_instance = instance ?? throw new ArgumentNullException(nameof(instance));
		}

		protected override IDictionary<Type, IDictionary<string, IDynamicAccessor>> AccessorsOnType => _accessorsOnType;

		// For instance calls, we get the type from the instance
		protected override Type TargetType => _instance.GetType();

		protected override object Instance => _instance;

		public override object RealObject => Instance;

		protected override BindingFlags BindingFlags => BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public bool Is(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			return type.IsInstanceOfType(Instance);
		}
	}
}
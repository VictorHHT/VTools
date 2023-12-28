using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Victor.Tools
{
	public abstract class PrivateReflectionDynamicObjectBase : DynamicObject
	{
#if NET45
        private static readonly Type[] _emptyTypes = new Type[0];
#endif

		// We need to virtualize this so we use a different cache for instance and static props
		protected abstract IDictionary<Type, IDictionary<string, IDynamicAccessor>> AccessorsOnType { get; }

		protected abstract Type TargetType { get; }

		protected abstract object Instance { get; }

		protected abstract BindingFlags BindingFlags { get; }

		public abstract object RealObject { get; }

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (binder == null)
			{
				throw new ArgumentNullException(nameof(binder));
			}

			var accessor = GetAccessor(binder.Name);

			// Get the property value
			result = accessor.GetValue(Instance, index: null);

			// Wrap the sub object if necessary. This allows nested anonymous objects to work.
			result = result.AsDynamic();

			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (binder == null)
			{
				throw new ArgumentNullException(nameof(binder));
			}

			var accessor = GetAccessor(binder.Name);

			// Set the property value.  Make sure to unwrap it first if it's one of our dynamic objects
			accessor.SetValue(Instance, Unwrap(value), index: null);

			return true;
		}

		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			if (binder == null)
			{
				throw new ArgumentNullException(nameof(binder));
			}

			var prop = GetIndexProperty();
			result = prop.GetValue(Instance, indexes);

			// Wrap the sub object if necessary. This allows nested anonymous objects to work.
			result = result.AsDynamic();

			return true;
		}

		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
		{
			if (binder == null)
			{
				throw new ArgumentNullException(nameof(binder));
			}

			var prop = GetIndexProperty();
			prop.SetValue(Instance, Unwrap(value), indexes);

			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			if (binder == null)
			{
				throw new ArgumentNullException(nameof(binder));
			}

			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			for (var i = 0; i < args.Length; i++)
			{
				args[i] = Unwrap(args[i]);
			}

			var typeArgs = GetGenericMethodArguments(binder);

			result = InvokeMethodOnType(TargetType, Instance, binder.Name, args, typeArgs);

			// Wrap the sub object if necessary. This allows nested anonymous objects to work.
			result = result.AsDynamic();

			return true;
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			if (binder == null)
			{
				throw new ArgumentNullException(nameof(binder));
			}

			result = binder.Type.GetTypeInfo().IsInstanceOfType(RealObject) ? RealObject : Convert.ChangeType(RealObject, binder.Type);

			return true;
		}

		public override string ToString()
		{
			return Instance.ToString();
		}

		private IDynamicAccessor GetIndexProperty()
		{
			// The index property is always named "Item" in C#
			return GetAccessor("Item");
		}

		private IDynamicAccessor GetAccessor(string propertyName)
		{
			// Get the list of properties and fields for this type
			var typeProperties = GetTypeAccessors(TargetType);

			// Look for the one we want
			if (typeProperties.TryGetValue(propertyName, out var property))
			{
				return property;
			}

			// The property doesn't exist

			// Get a list of supported properties and fields and show them as part of the exception message
			// For fields, skip the auto property backing fields (which name start with <)
			var propNames = typeProperties.Keys.Where(name => name[0] != '<').OrderBy(name => name);

			throw new MissingMemberException
			(
				$"The property {propertyName} doesn\'t exist on type {TargetType}. Supported properties are: {string.Join(", ", propNames)}"
			);
		}

		private IDictionary<string, IDynamicAccessor> GetTypeAccessors(Type type)
		{
			// First, check if we already have it cached
			if (AccessorsOnType.TryGetValue(type, out var typeAccessors))
			{
				return typeAccessors;
			}

			// Not cached, so we need to build it
			typeAccessors = new Dictionary<string, IDynamicAccessor>();

			// First, recurse on the base class to add its fields
			if (type.GetTypeInfo().BaseType != null)
			{
				foreach (var accessor in GetTypeAccessors(type.GetTypeInfo().BaseType).Values)
				{
					typeAccessors[accessor.Name] = accessor;
				}
			}

			// Then, add all the properties from the current type
			foreach (var property in type.GetTypeInfo().GetProperties(BindingFlags))
			{
				if (property.DeclaringType == type)
				{
					typeAccessors[property.Name] = new DynamicProperty(property);
				}
			}

			// Finally, add all the fields from the current type
			foreach (var field in type.GetTypeInfo().GetFields(BindingFlags))
			{
				if (field.DeclaringType == type)
				{
					typeAccessors[field.Name] = new DynamicField(field);
				}
			}

			// Cache it for next time
			AccessorsOnType[type] = typeAccessors;

			return typeAccessors;
		}

		private static bool ParametersCompatible(MethodInfo method, object[] passedArguments)
		{
			Debug.Assert(method != null);
			Debug.Assert(passedArguments != null);

			var parametersOnMethod = method.GetParameters();

			if (parametersOnMethod.Length != passedArguments.Length)
			{
				return false;
			}

			for (var i = 0; i < parametersOnMethod.Length; ++i)
			{
				var parameterType = parametersOnMethod[i].ParameterType.GetTypeInfo();
				ref var argument = ref passedArguments[i];

				if (argument == null && parameterType.IsValueType)
				{
					// Value types can not be null.
					return false;
				}

				if (!parameterType.IsInstanceOfType(argument))
				{
					// Parameters should be instance of the parameter type.
					if (parameterType.IsByRef)
					{
						var typePassedByRef = parameterType.GetElementType().GetTypeInfo();

						Debug.Assert(typePassedByRef != null);

						if (typePassedByRef.IsValueType && argument == null)
						{
							return false;
						}

						if (argument != null)
						{
							var argumentType = argument.GetType().GetTypeInfo();
							var argumentByRefType = argumentType.MakeByRefType().GetTypeInfo();

							if (parameterType != argumentByRefType)
							{
								try
								{
									argument = Convert.ChangeType(argument, typePassedByRef.AsType());
								}
								catch (InvalidCastException)
								{
									return false;
								}
							}
						}
					}
					else if (argument == null)
					{
						continue;
					}
					else
					{
						return false;
					}
				}
			}

			return true;
		}

		private static object InvokeMethodOnType(Type type, object target, string name, object[] args, Type[] typeArgs)
		{
			Debug.Assert(type != null);
			Debug.Assert(args != null);
			Debug.Assert(typeArgs != null);

			const BindingFlags allMethods =
				BindingFlags.Public | BindingFlags.NonPublic
				                    | BindingFlags.Instance | BindingFlags.Static;

			MethodInfo method = null;
			var currentType = type;

			while (method == null && currentType != null)
			{
				var methods = currentType.GetTypeInfo().GetMethods(allMethods);

				MethodInfo candidate;

				for (var i = 0; i < methods.Length; ++i)
				{
					candidate = methods[i];

					if (candidate.Name == name)
					{
						// Check if the method is called as a generic method.
						if (typeArgs.Length > 0 && candidate.ContainsGenericParameters)
						{
							var candidateTypeArgs = candidate.GetGenericArguments();

							if (candidateTypeArgs.Length == typeArgs.Length)
							{
								candidate = candidate.MakeGenericMethod(typeArgs);
							}
						}

						if (ParametersCompatible(candidate, args))
						{
							method = candidate;
							break;
						}
					}
				}

				if (method == null)
				{
					// Move up in the type hierarchy.
					currentType = currentType.GetTypeInfo().BaseType;
				}
			}

			if (method == null)
			{
				throw new MissingMethodException($"Method with name '{name}' not found on type '{type.FullName}'.");
			}

			return method.Invoke(target, args);
		}

		private static object Unwrap(object o)
		{
			// If it's a wrap object, unwrap it and return the real thing
			if (o is PrivateReflectionDynamicObjectBase wrappedObj)
			{
				return wrappedObj.RealObject;
			}

			// Otherwise, return it unchanged
			return o;
		}

		private static Type[] GetGenericMethodArguments(InvokeMemberBinder binder)
		{
			var csharpInvokeMemberBinderType = binder
				.GetType().GetTypeInfo()
				.GetInterface("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder")
				.GetTypeInfo();

			var typeArgsList = (IList<Type>)csharpInvokeMemberBinderType.GetProperty("TypeArguments").GetValue(binder, null);

			Type[] typeArgs;

			if (typeArgsList.Count == 0)
			{
#if NET45
                typeArgs = _emptyTypes;
#else
				typeArgs = Array.Empty<Type>();
#endif
			}
			else
			{
				typeArgs = typeArgsList.ToArray();
			}

			return typeArgs;
		}
	}
}
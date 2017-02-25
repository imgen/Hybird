using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace Hybird.iOS.Renderers
{
	class Registrar<TRegistrable> where TRegistrable : class
	{
		readonly Dictionary<Type, Type> _handlers = new Dictionary<Type, Type>();

		public void Register(Type tview, Type trender)
		{
			_handlers[tview] = trender;
		}

		internal TRegistrable GetHandler(Type type)
		{
			Type handlerType = GetHandlerType(type);
			if (handlerType == null)
				return null;

			object handler = Activator.CreateInstance(handlerType);
			return (TRegistrable)handler;
		}

		internal TOut GetHandler<TOut>(Type type) where TOut : TRegistrable
		{
			return (TOut)GetHandler(type);
		}

		internal Type GetHandlerType(Type viewType)
		{
			Type type = LookupHandlerType(viewType);
			if (type != null)
				return type;

			// lazy load render-view association with RenderWithAttribute (as opposed to using ExportRenderer)
			// TODO: change Registrar to a LazyImmutableDictionary and pass this logic to ctor as a delegate.
			var attribute = viewType.GetTypeInfo().GetCustomAttribute<RenderWithAttribute>();
			if (attribute == null)
				return null;
			type = attribute.Type;

			if (type.Name.StartsWith("_"))
			{
				// TODO: Remove attribute2 once renderer names have been unified across all platforms
				var attribute2 = type.GetTypeInfo().GetCustomAttribute<RenderWithAttribute>();
				if (attribute2 != null)
					type = attribute2.Type;

				if (type.Name.StartsWith("_"))
				{
					//var attrs = type.GetTypeInfo ().GetCustomAttributes ().ToArray ();
					return null;
				}
			}

			Register(viewType, type);
			return LookupHandlerType(viewType);
		}

		Type LookupHandlerType(Type viewType)
		{
			Type type = viewType;

			while (true)
			{
				if (_handlers.ContainsKey(type))
					return _handlers[type];

				type = type.GetTypeInfo().BaseType;
				if (type == null)
					break;
			}

			return null;
		}
	}

	static class Registrar
	{
		static Registrar()
		{
			Registered = new Registrar<IRegisterable>();
		}

		internal static Dictionary<string, Type> Effects { get; } = new Dictionary<string, Type>();

		internal static IEnumerable<Assembly> ExtraAssemblies { get; set; }

		internal static Registrar<IRegisterable> Registered { get; }
	}
}
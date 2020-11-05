using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altomatic.UI.Utilities
{
	public static class Memoizer
	{
		public static Func<TReturn> Memoize<TReturn>(Func<TReturn> func)
		{
			object cache = null;
			return () =>
			{
				cache = cache ?? func();
				return (TReturn)cache;
			};
		}

		public static Func<TArg1, TReturn> Memoize<TArg1, TReturn>(Func<TArg1, TReturn> func)
		{
			var cache = new ConcurrentDictionary<TArg1, TReturn>();
			return arg1 =>
			{
				if (cache.TryGetValue(arg1, out TReturn value))
				{
					return value;
				}
				else
				{
					value = cache.GetOrAdd(arg1, func(arg1));
					return value;
				}
			};
		}

		public static Func<TArg1, TArg2, TReturn> Memoize<TArg1, TArg2, TReturn>(Func<TArg1, TArg2, TReturn> func)
		{
			var cache = new ConcurrentDictionary<(TArg1, TArg2), TReturn>();
			return (arg1, arg2) =>
			{
				if (cache.TryGetValue((arg1, arg2), out TReturn value))
				{
					return value;
				}
				else
				{
					value = cache.GetOrAdd((arg1, arg2), func(arg1, arg2));
					return value;
				}
			};
		}

		public static Func<TArg1, TArg2, TArg3, TReturn> Memoize<TArg1, TArg2, TArg3, TReturn>(Func<TArg1, TArg2, TArg3, TReturn> func)
		{
			var cache = new ConcurrentDictionary<(TArg1, TArg2, TArg3), TReturn>();
			return (arg1, arg2, arg3) =>
			{
				if (cache.TryGetValue((arg1, arg2, arg3), out TReturn value))
				{
					return value;
				}
				else
				{
					value = cache.GetOrAdd((arg1, arg2, arg3), func(arg1, arg2, arg3));
					return value;
				}
			};
		}

		public static Func<A, R> ThreadSafeMemoize<A, R>(Func<A, R> func)
		{
			var cache = new ConcurrentDictionary<A, R>();
			return argument => cache.GetOrAdd(argument, func);
		}
	}
}

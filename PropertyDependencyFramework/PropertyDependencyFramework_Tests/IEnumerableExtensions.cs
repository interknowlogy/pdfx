using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyDependencyFramework_Tests
{
	public static class IEnumerableExtensions
	{
		public static bool SequenceEqualIgnoreOrder<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2)
		{
			ICollection<T> c1 = collection1.ToArray();
			ICollection<T> c2 = collection2.ToArray();

			if (c1.Count != c2.Count)
				return false;

			if (c1.All(c2.Contains))
				return true;

			return false;
		}

		/// <summary>
		/// Loops and executes the action for each item in the IEnumerable. (Like the List.ForEach)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The collection.</param>
		/// <param name="action">The action.</param>
		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (var item in collection)
			{
				action.Invoke(item);
			}
		}
		
		public static decimal? SumOrNull(this IEnumerable<decimal?> collection)
		{
			if (!collection.Any() || collection.Any(k => k.HasValue == false))
				return null;

			return collection.Select(k => k.Value).Sum();
		}


		public static decimal SumOrZero(this IEnumerable<decimal> collection)
		{
			if (!collection.Any())
				return 0;

			return collection.Sum();
		}

		public static decimal? LastOrNull(this IEnumerable<decimal> collection)
		{
			if (!collection.Any())
				return null;

			return collection.Last();
		}

		public static T MaxOrZero<T>(this IEnumerable<T> collection)
		{
			if (!collection.Any())
				return default(T);

			return collection.Max();
		}


		public static T MinOrZero<T>(this IEnumerable<T> collection)
		{
			if (!collection.Any())
				return default(T);

			return collection.Min();
		}

		public static IEnumerable<T> Union<T>(this IEnumerable<T> collectionLeft, IEnumerable<T> collectionRight, Func<T, T, bool> equalityCompareFunc, Func<T, int> getHashCode = null)
		{
			if (getHashCode == null)
			{
				getHashCode = t => 0;
			}

			return collectionLeft.Union(collectionRight, new CustomEqualityComparer<T>(equalityCompareFunc, getHashCode));
		}

		public static bool None<T>(this IEnumerable<T> collection, Func<T, bool> predicate = null)
		{
			if (predicate == null)
			{
				return !collection.Any();
			}
			return !collection.Any(predicate);
		}

		private class CustomEqualityComparer<T> : IEqualityComparer<T>
		{
			private readonly Func<T, T, bool> _equalityCompareFunc;
			private readonly Func<T, int> _getHashCode;

			public CustomEqualityComparer(Func<T, T, bool> equalityCompareFunc, Func<T, int> getHashCode = null)
			{
				_equalityCompareFunc = equalityCompareFunc;
				_getHashCode = getHashCode;
			}

			public bool Equals(T x, T y)
			{
				return _equalityCompareFunc(x, y);
			}

			public int GetHashCode(T obj)
			{
				return _getHashCode(obj);
			}
		}

	}
}

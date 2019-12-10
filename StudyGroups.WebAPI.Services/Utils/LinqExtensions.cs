using System;
using System.Collections.Generic;
using System.Linq;

namespace StudyGroups.WebAPI.Services.Utils
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> list, Func<T, TKey> lookup) where TKey : struct
        {
            return list.Distinct(new StructEqualityComparer<T, TKey>(lookup));
        }

    }

    class StructEqualityComparer<T, TKey> : IEqualityComparer<T> where TKey : struct
    {

        private readonly Func<T, TKey> lookup;

        public StructEqualityComparer(Func<T, TKey> lookup)
        {
            this.lookup = lookup;
        }

        public bool Equals(T x, T y)
        {
            return lookup(x).Equals(lookup(y));
        }

        public int GetHashCode(T obj)
        {
            return lookup(obj).GetHashCode();
        }
    }
}

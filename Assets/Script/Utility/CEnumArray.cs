using System;
using System.Linq.Expressions;

public class CEnumArray<TEnum, TValue>
{
    TValue[] data;

    public CEnumArray(int count)
    {
        data = new TValue[count];
    }

    public TValue this[TEnum key]
    {
        get { return data[ConvertToIndex(key)]; }
        set { }
    }

    int ConvertToIndex(TEnum key)
    {
        return CastTo<int>.From(key);
    }
}

public static class CastTo<T>
{
    public static T From<S>(S s)
    {
        return Cache<S>.caster(s);
    }

    private static class Cache<S>
    {
        public static readonly Func<S, T> caster = Get();

        private static Func<S, T> Get()
        {
            var p = Expression.Parameter(typeof(S));
            var c = Expression.ConvertChecked(p, typeof(T));
            return Expression.Lambda<Func<S, T>>(c, p).Compile();
        }
    }
}

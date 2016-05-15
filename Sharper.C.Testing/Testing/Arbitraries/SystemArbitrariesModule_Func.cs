using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;

namespace Sharper.C.Testing.Arbitraries
{

public static partial class SystemArbitrariesModule
{
    public const int DefaultFunc1CacheSize = 1000;

    public static Arbitrary<Func<A, B>> AnyFunc1<A, B>
      ( Arbitrary<B> arbB
      , IEqualityComparer<A> comp
      , int cacheSize = DefaultFunc1CacheSize
      )
    =>
        Arb.From(GenAnyFunc1(arbB.Generator, comp, cacheSize));

    public static Arbitrary<Func<A, B>> AnyFunc1<A, B>
      ( Arbitrary<B> arbB
      , int cacheSize = DefaultFunc1CacheSize
      )
      where A : IEquatable<A>
    =>
        AnyFunc1(arbB, EqualityComparer<A>.Default, cacheSize);

    public static Arbitrary<Func<A, B>> AnyFunc1Unchecked<A, B>
      ( Arbitrary<B> arbB
      , int cacheSize = DefaultFunc1CacheSize
      )
    =>
        AnyFunc1(arbB, EqualityComparer<A>.Default, cacheSize);

    public static Gen<Func<A, B>> GenAnyFunc1<A, B>
      ( Gen<B> genB
      , IEqualityComparer<A> comp
      , int cacheSize
      )
    =>
        from bs in Gen.ListOf(cacheSize, genB)
            .Select(e => e.AsEnumerable().GetEnumerator())
        from cache in
            Gen
            .Constant
              ( new Func<IDictionary<A, B>>
                  ( () => new Dictionary<A, B>(cacheSize, comp)
                  )
              )
            .Select(f => f())
        select
            new Func<A, B>
              ( a =>
                {
                    if (!cache.ContainsKey(a))
                    {
                        bs.MoveNext();
                        cache.Add(a, bs.Current);
                    }
                    return cache[a];
                });
}

}

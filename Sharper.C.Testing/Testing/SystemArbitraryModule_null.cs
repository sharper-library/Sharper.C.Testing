using System;
using System.Linq;
using FsCheck;

namespace Sharper.C.Testing
{

public static partial class SystemArbitraryModule
{
    /// <summary>
    /// Generate values of a reference type with a null:not-null ratio of 1:7.
    /// </summary>
    public static Arbitrary<A> WithNull<A>(this Arbitrary<A> arbA)
      where A : class
    =>
        Arb.From
          ( Gen.Frequency
              ( Tuple.Create(1, Gen.Constant<A>(null))
              , Tuple.Create(7, arbA.Generator)
              )
          , a =>
                ReferenceEquals(null, a)
                ? Enumerable.Empty<A>()
                : new A[] {null}.Concat(arbA.Shrinker(a))
          );
}

}
using System;
using System.Linq;
using FsCheck;

namespace Sharper.C.Testing.Arbitraries
{

public static partial class SystemArbitrariesModule
{
    /// <summary>
    /// Generate a Nullable[A] with an empty:full ration of 1:7.
    /// </summary>
    public static Arbitrary<A?> Nullable<A>(this Arbitrary<A> arbA)
      where A : struct
    =>
        Arb.From
          ( Gen.Frequency
              ( Tuple.Create(1, Gen.Constant(default(A?)))
              , Tuple.Create(7, arbA.Generator.Select(a => new A?(a)))
              )
          , a =>
                a.HasValue
                ? new[] {default(A?)}
                  .Concat(arbA.Shrinker(a.Value).Select(x => new A?(x)))
                : Enumerable.Empty<A?>()
          );
}

}

using System;
using System.Collections.Generic;
using FsCheck;

namespace Sharper.C.Testing.Laws
{

using static PropertyModule;
using static Properties.FunctionPropertiesModule;

public static class HashingLaws
{
    public static Invariant For<A>
      ( Func<A, int> hash
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Hashing Laws"
        .All
          ( IsBinaryRelationHomomorphism(hash, eq, (x, y) => x == y, arbA)
          , EqualityLaws.For(eq, arbA)
          );

    public static Invariant For<A>(Arbitrary<A> arbA)
      where A : IEquatable<A>
    =>
        For(a => a.GetHashCode(), (x, y) => x.Equals(y), arbA);

    public static Invariant For<A>
      ( IEqualityComparer<A> comparer
      , Arbitrary<A> arbA
      )
    =>
        For(comparer.GetHashCode, comparer.Equals, arbA);
}

}

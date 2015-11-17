using System;
using System.Collections.Generic;
using FsCheck;

namespace Sharper.C.Testing.Laws
{

using Fuchu;
using static PropertyModule;
using static Properties.FunctionPropertiesModule;

public static class HashingLaws
{
    public static Test For<A>
      ( Func<A, int> hash
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Hashing Laws"
        .Group
          ( IsBinaryRelationHomomorphism(hash, eq, (x, y) => x == y, arbA)
          , EqualityLaws.For(eq, arbA)
          );

    public static Test For<A>(Arbitrary<A> arbA)
      where A : IEquatable<A>
    =>
        For(a => a.GetHashCode(), (x, y) => x.Equals(y), arbA);

    public static Test For<A>
      ( IEqualityComparer<A> comparer
      , Arbitrary<A> arbA
      )
    =>
        For(comparer.GetHashCode, comparer.Equals, arbA);
}

}

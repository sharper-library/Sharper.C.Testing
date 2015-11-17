using System;
using System.Collections.Generic;
using FsCheck;

namespace Sharper.C.Testing.Laws
{

using Fuchu;
using static PropertyModule;
using static Properties.RelationPropertiesModule;

public static class OrderingLaws
{
    public static Test For<A>
      ( Func<A, A, bool> ltEq
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Ordering Laws"
        .Group
          ( IsTotalOrder(ltEq, eq, arbA)
          , EqualityLaws.For(eq, arbA)
          );

    public static Test For<A>(Arbitrary<A> arbA)
      where A : IEquatable<A>, IComparable<A>
    =>
        For
          ( (x, y) => x.CompareTo(y) <= 0
          , (x, y) => x.Equals(y)
          , arbA
          );

    public static Test For<A>
      ( IComparer<A> comparer
      , IEqualityComparer<A> eqComparer
      , Arbitrary<A> arbA
      )
    =>
        For
          ( (x, y) => comparer.Compare(x, y) <= 0
          , eqComparer.Equals
          , arbA
          );
}

}

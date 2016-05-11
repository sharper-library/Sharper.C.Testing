using System;
using System.Collections.Generic;
using FsCheck;

namespace Sharper.C.Testing.Laws
{

using static Properties.RelationPropertiesModule;

public static class OrderingLaws
{
    public static Invariant For<A>
      ( Func<A, A, bool> ltEq
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Ordering Laws"
        .All
          ( IsTotalOrder(ltEq, eq, arbA)
          , EqualityLaws.For(eq, arbA)
          );

    public static Invariant For<A>(Arbitrary<A> arbA)
      where A : IEquatable<A>, IComparable<A>
    =>
        For
          ( (x, y) => x.CompareTo(y) <= 0
          , (x, y) => x.Equals(y)
          , arbA
          );

    public static Invariant For<A>
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

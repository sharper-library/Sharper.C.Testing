using System;
using System.Collections.Generic;
using FsCheck;
using Sharper.C.Data;

namespace Sharper.C.Testing.Laws
{

using static Properties.RelationPropertiesModule;

public static class EqualityLaws
{
    public static Invariant For<A, EqA>
      ( Arbitrary<A> arbA
      , EqA eqA = default(EqA)
      )
      where EqA : Eq<A>
    =>  For(eqA.Equal, arbA);

    public static Invariant For<A>
      ( Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Equality Laws"
        .All(IsEquivalence(eq, arbA));

    public static Invariant For<A>(Arbitrary<A> arbA)
      where A : IEquatable<A>
    =>
        For((x, y) => x.Equals(y), arbA);

    public static Invariant For<A>
      ( IEqualityComparer<A> comparer
      , Arbitrary<A> arbA
      )
    =>
        For(comparer.Equals, arbA);
}

}

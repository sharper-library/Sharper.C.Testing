using System;
using System.Collections.Generic;
using FsCheck;
using Fuchu;

namespace Sharper.C.Testing.Laws
{

using static PropertyModule;
using static Properties.RelationPropertiesModule;

public static class EqualityLaws
{
    public static Test For<A>
      ( Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Equality Laws"
        .Group(IsEquivalence(eq, arbA));

    public static Test For<A>(Arbitrary<A> arbA)
      where A : IEquatable<A>
    =>
        For((x, y) => x.Equals(y), arbA);

    public static Test For<A>
      ( IEqualityComparer<A> comparer
      , Arbitrary<A> arbA
      )
    =>
        For(comparer.Equals, arbA);
}

}

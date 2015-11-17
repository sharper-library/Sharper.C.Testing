using System;
using FsCheck;
using Fuchu;

namespace Sharper.C.Testing.Properties
{

using static PropertyModule;

public static class RelationPropertiesModule
{
    public static Invariant IsReflexive<A>
      ( Func<A, A, bool> r
      , Arbitrary<A> arbA
      )
    =>
        "Reflexive:  a ~ a"
        .ForAll
          ( arbA
          , a => r(a, a)
          );

    public static Invariant IsSymmetric<A>
      ( Func<A, A, bool> r
      , Arbitrary<A> arbA
      )
    =>
        "Symmetric:  a ~ b  <=>  b ~ a"
        .ForAll
          ( arbA, arbA
          , (x, y) => r(x, y) == r(y, x)
          );

    public static Invariant IsAntisymmetric<A>
      ( Func<A, A, bool> r
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Antisymmetric:  a ~ b  &&  b ~ a  =>  a == b"
        .ForAll
          ( arbA, arbA
          , (x, y) => When(r(x, y) && r(y, x), () => eq(x, y))
          );

    public static Invariant IsTransitive<A>
      ( Func<A, A, bool> r
      , Arbitrary<A> arbA
      )
    =>
        "Transitive:  a ~ b  &&  b ~ c  =>  a ~ c"
        .ForAll
          ( arbA , arbA , arbA
          , (x, y, z) => When(r(x, y) && r(y, z), () => r(x, z))
          );

    public static Invariant IsEquivalence<A>
      ( Func<A, A, bool> r
      , Arbitrary<A> arbA
      )
    =>
        "Equivalence Relation: reflexive, symmetric, and transitive"
        .All
          ( IsReflexive(r, arbA)
          , IsSymmetric(r, arbA)
          , IsTransitive(r, arbA)
          );

    public static Invariant IsTotalOrder<A>
      ( Func<A, A, bool> r
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Total Order: reflextive, antisymmetric, and transitive"
        .All
          ( IsReflexive(r, arbA)
          , IsAntisymmetric(r, eq, arbA)
          , IsTransitive(r, arbA)
          );

    public static Invariant AreEquivalent<A>
      ( Func<A, A, bool> r1
      , Func<A, A, bool> r2
      , Arbitrary<A> arbA
      )
    =>
        "Relations are equivalent"
        .ForAll
          ( arbA, arbA
          , (x, y) => r1(x, y) == r2(x, y)
          );
}

}

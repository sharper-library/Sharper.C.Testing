using System;
using FsCheck;

namespace Sharper.C.Testing.Properties
{

using static PropertyModule;

public static class OperationPropertiesModule
{
    public static Invariant IsAssociative<A>
      ( Func<A, A, A> op
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Associative:  (a * b) * c  ==  a * (b * c)"
        .ForAll
          ( arbA, arbA, arbA
          , (x, y, z) => eq(op(x, op(y, z)), op(op(x, y), z))
          );

    public static Invariant IsCommutative<A>
      ( Func<A, A, A> op
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Commutative:  a * b  ==  b * c"
        .ForAll
          ( arbA, arbA
          , (x, y) => eq(op(x, y), op(y, x))
          );

    public static Invariant IsLeftDistributive<A>
      ( Func<A, A, A> star
      , Func<A, A, A> plus
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Left Distributive:  x * (y + z)  ==  (x * y) + (x * z)"
        .ForAll
          ( arbA, arbA, arbA
          , (x, y, z) => eq(star(x, plus(y, z)), plus(star(x, y), star(x, z)))
          );

    public static Invariant IsRightDistributive<A>
      ( Func<A, A, A> star
      , Func<A, A, A> plus
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Right Distributive:  (y + z) * x  ==  (y * x) + (z * x)"
        .ForAll
          ( arbA, arbA, arbA
          , (x, y, z) => eq(star(plus(y, z), x), plus(star(y, x), star(z, x)))
          );

    public static Invariant IsDistributive<A>
      ( Func<A, A, A> star
      , Func<A, A, A> plus
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Distributive"
        .All
          ( IsLeftDistributive(star, plus, eq, arbA)
          , IsRightDistributive(star, plus, eq, arbA)
          );

    public static Invariant IsPolymorphicDistributive<A, B>
      ( Func<A, B> f
      , Func<A, A, A> gA
      , Func<B, B, B> gB
      , Func<B, B, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Distributive:  f(g(x, y))  ==  g(f(x), f(y))"
        .ForAll
          ( arbA, arbA
          , (x, y) => eq(f(gA(x, y)), gB(f(x), f(y)))
          );

    public static Invariant HasLeftIdentity<A>
      ( Func<A, A, A> op
      , A id
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Left Identity:  a  ==  id * a"
        .ForAll
          ( arbA
          , a => eq(a, op(id, a))
          );

    public static Invariant HasRightIdentity<A>
      ( Func<A, A, A> op
      , A id
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Right Identity:  a  ==  a * id"
        .ForAll
          ( arbA
          , a => eq(a, op(a, id))
          );

    public static Invariant HasIdentity<A>
      ( Func<A, A, A> op
      , A id
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Identity:  id * a  ==  a  ==  a * id"
        .All
          ( HasLeftIdentity(op, id, eq, arbA)
          , HasRightIdentity(op, id, eq, arbA)
          );
}

}

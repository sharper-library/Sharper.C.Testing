using System;
using FsCheck;

namespace Sharper.C.Testing.Properties
{

using static PropertyModule;

public static class FunctionPropertiesModule
{
    public static Invariant IsInjection<A, B>
      ( Func<A, B> f
      , Func<B, A> g
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Injection:  g . f  ==  id"
        .ForAll
          ( arbA
          , a => eq(a, g(f(a)))
          );

    public static Invariant IsSurjection<A, B>
      ( Func<A, B> f
      , Func<B, A> g
      , Func<B, B, bool> eq
      , Arbitrary<B> arbB
      )
    =>
        "Surjection:  f . g  ==  id"
        .ForAll
          ( arbB
          , b => eq(f(g(b)), b)
          );

    public static Invariant IsBijection<A, B>
      ( Func<A, B> f
      , Func<B, A> g
      , Func<A, A, bool> eqA
      , Func<B, B, bool> eqB
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      )
    =>
        "Bijection:  injective and surjective".All
          ( IsInjection(f, g, eqA, arbA)
          , IsSurjection(f, g, eqB, arbB)
          );

    public static Invariant IsBinaryRelationHomomorphism<A, B>
      ( Func<A, B> f
      , Func<A, A, bool> rA
      , Func<B, B, bool> rB
      , Arbitrary<A> arbA
      )
    =>
        "Binary Relation Homomorphism:  x ~ y  =>  f(x) ~ f(y)"
        .ForAll
          ( arbA, arbA
          , (x, y) => When(rA(x, y), () => rB(f(x), f(y)))
          );
}

}

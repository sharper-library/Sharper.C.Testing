using System;
using FsCheck;

namespace Sharper.C.Testing.Properties
{

using static PropertyModule;

public static class FunctionPropertiesModule
{
    public static Invariant IsMonomorphism<A, B>
      ( Func<A, B> f
      , Func<B, A> g
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Monomorphism (injective homomorphism):  g . f  ==  id"
        .ForAll
          ( arbA
          , a => eq(g(f(a)), a)
          );

    public static Invariant IsEpimorphism<A, B>
      ( Func<A, B> f
      , Func<B, A> g
      , Func<B, B, bool> eq
      , Arbitrary<B> arbB
      )
    =>
        "Eipmorphism (surjective homomorphism):  f . g  ==  id"
        .ForAll
          ( arbB
          , b => eq(f(g(b)), b)
          );

    public static Invariant IsIsomorphism<A, B>
      ( Func<A, B> f
      , Func<B, A> g
      , Func<A, A, bool> eqA
      , Func<B, B, bool> eqB
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      )
    =>
        "Isomorphism (bijective homomorphism):  g . f  ==  id  ==  f . g"
        .All
          ( IsMonomorphism(f, g, eqA, arbA)
          , IsEpimorphism(f, g, eqB, arbB)
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

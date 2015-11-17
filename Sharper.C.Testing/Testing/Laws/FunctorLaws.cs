using System;
using FsCheck;

namespace Sharper.C.Testing.Laws
{

using static Properties.OperationPropertiesModule;
using static PropertyModule;

public static class FunctorLaws
{
    public static Invariant For<FA, A>
      ( Func<Func<A, A>, Func<FA, FA>> map
      , Func<FA, FA, bool> eq
      , Arbitrary<FA> arbFA
      , Arbitrary<Func<A, A>> arbAA
      )
    =>
        "Functor Laws"
        .All
          ( HasMapIdentity(map, eq, arbFA)
          , MapDistributesOverComposition(map, eq, arbFA, arbAA)
          );

    public static Invariant HasMapIdentity<FA, A>
      ( Func<Func<A, A>, Func<FA, FA>> map
      , Func<FA, FA, bool> eq
      , Arbitrary<FA> arbFA
      )
    =>
        "Map Identity:  map id  ==  id"
        .ForAll
          ( arbFA
          , fa => eq(map(x => x)(fa), fa)
          );

    public static Invariant MapDistributesOverComposition<FA, A>
      ( Func<Func<A, A>, Func<FA, FA>> map
      , Func<FA, FA, bool> eq
      , Arbitrary<FA> arbFA
      , Arbitrary<Func<A, A>> arbAA
      )
    =>
        "Map Distributes Over Composition:  map (f . g)  ==  map f . map g"
        .ForAll
          ( arbFA
          , fa =>
                IsPolymorphicDistributive
                  ( map
                  , Compose
                  , Compose
                  , (r1, r2) => eq(r1(fa), r2(fa))
                  , arbAA
                  )
          );

    private static Func<A, C> Compose<A, B, C>(Func<B, C> f, Func<A, B> g)
    =>
        a => f(g(a));
}

}

using System;
using FsCheck;

namespace Sharper.C.Testing.Laws
{

using static PropertyModule;
using static Properties.OperationPropertiesModule;

public static class MonadLaws
{
    public static Invariant For<MA, A>
      ( Func<A, MA> pure
      , Func<Func<A, A>, Func<MA, MA>> map
      , Func<Func<A, MA>, Func<MA, MA>> flatMap
      , Func<MA, MA, bool> eq
      , Arbitrary<MA> arbMA
      , Arbitrary<Func<A, MA>> arbAMA
      , Arbitrary<Func<A, A>> arbAA
      , Arbitrary<A> arbA
      )
    {
        Func<Func<A, MA>, Func<A, MA>, Func<A, MA>> kleisli =
            (f, g) => a => flatMap(g)(f(a));
        return
            "Monad Laws"
            .All
              ( KleisliLeftIdentity(pure, kleisli, eq, arbAMA, arbA)
              , KleisliRightIdentity(pure, kleisli, eq, arbAMA, arbA)
              , KleisliIsAssociative(kleisli, eq, arbAMA, arbA)
              , FunctorLaws.For
                  ( map
                  , eq
                  , arbMA
                  , arbAA
                  )
              );
    }

    public static Invariant KleisliLeftIdentity<MA, A>
      ( Func<A, MA> pure
      , Func<Func<A, MA>, Func<A, MA>, Func<A, MA>> kleisli
      , Func<MA, MA, bool> eq
      , Arbitrary<Func<A, MA>> arbAMA
      , Arbitrary<A> arbA
      )
    =>
        "Kleisli Left Identity:  pure >=> g  ==  g"
        .ForAll
          ( arbAMA, arbA
          , (ama, a) => eq(kleisli(pure, ama)(a), ama(a))
          );

    public static Invariant KleisliRightIdentity<MA, A>
      ( Func<A, MA> pure
      , Func<Func<A, MA>, Func<A, MA>, Func<A, MA>> kleisli
      , Func<MA, MA, bool> eq
      , Arbitrary<Func<A, MA>> arbAMA
      , Arbitrary<A> arbA
      )
    =>
        "Kleisli Right Identity:  g >=> pure  ==  g"
        .ForAll
          ( arbAMA, arbA
          , (ama, a) => eq(kleisli(ama, pure)(a), ama(a))
          );

    public static Invariant KleisliIsAssociative<MA, A>
      ( Func<Func<A, MA>, Func<A, MA>, Func<A, MA>> kleisli
      , Func<MA, MA, bool> eq
      , Arbitrary<Func<A, MA>> arbAMA
      , Arbitrary<A> arbA
      )
    =>
        "Kleisli Associative:  (f >=> g) >=> h  ==  f >=> (g >=> h)"
        .ForAll
          ( arbA
          , a => IsAssociative(kleisli, (k1, k2) => eq(k1(a), k2(a)), arbAMA)
          );
}

}

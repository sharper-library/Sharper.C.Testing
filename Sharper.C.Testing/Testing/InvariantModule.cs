using System;
using System.Collections.Immutable;
using FsCheck;
using FsProp = FsCheck.Prop;

namespace Sharper.C.Testing
{

public static class InvariantModule
{
    public static Invariant ForUnit(this string label, Func<Testable> f)
    =>  Invariant.Single
          ( FsProp.ForAll(Arb.Default.Unit(), _ => f().Prop)
          , label
          );

    public static Invariant All(this string label, params Invariant[] tests)
    =>  Invariant.Multiple
          ( tests.ToImmutableList()
          , label
          );

    public static Invariant ForAll<A>
      ( this string label
      , Arbitrary<A> arbA
      , Func<A, Testable> f
      )
    =>  Invariant.Single
          ( FsProp.ForAll(arbA, a => f(a).Prop)
          , label
          );

    public static Invariant ForAll<A, B>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Func<A, B, Testable> f
      )
    =>  Invariant.Single
          ( FsProp.ForAll(arbA, arbB, (a, b) => f(a, b).Prop)
          , label
          );

    public static Invariant ForAll<A, B, C>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Arbitrary<C> arbC
      , Func<A, B, C, Testable> f
      )
    =>  Invariant.Single
          ( FsProp.ForAll(arbA, arbB, arbC, (a, b, c) => f(a, b, c).Prop)
          , label
          );

    public static Invariant ForAll<A, B, C, D>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Arbitrary<C> arbC
      , Arbitrary<D> arbD
      , Func<A, B, C, D, Testable> f
      )
    =>  Invariant.Single
          ( FsProp.ForAll
              ( arbD
              , d =>
                    FsProp.ForAll
                      ( arbA
                      , arbB
                      , arbC
                      , (a, b, c) => f(a, b, c, d).Prop
                      )
              )
          , label
          );
}

}

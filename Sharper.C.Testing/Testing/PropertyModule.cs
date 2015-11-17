using System;
using System.Linq;
using Microsoft.FSharp.Collections;
using FsCheck;
using Fuchu;

namespace Sharper.C.Testing
{

using FsProp = Prop;

public static class PropertyModule
{
    public static Property When(bool condition, Action a)
    =>
        PropertyExtensions.When(a, condition);

    public static Property When(bool condition, Func<bool> f)
    =>
        PropertyExtensions.When(f, condition);

    public static Property When(bool condition, Property p)
    =>
        FsProp.given
          ( condition
          , p
          , FsProp.ofTestable
              ( new Result
                  ( Outcome.Rejected
                  , FSharpList<string>.Empty
                  , new FSharpSet<string>(Enumerable.Empty<string>())
                  , FSharpList<object>.Empty
                  )
              )
          );

    public static Invariant ForAll<A>
      ( this string label
      , Arbitrary<A> arbA
      , Func<A, bool> f
      )
    =>
        Invariant.Mk(FsProp.ForAll(arbA, f), label);

    public static Invariant ForAll<A>
      ( this string label
      , Arbitrary<A> arbA
      , Func<A, Property> f
      )
    =>
        Invariant.Mk(FsProp.ForAll(arbA, f), label);

    public static Invariant ForAll<A, B>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Func<A, B, bool> f
      )
    =>
        Invariant.Mk(FsProp.ForAll(arbA, arbB, f), label);

    public static Invariant ForAll<A, B>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Func<A, B, Property> f
      )
    =>
        Invariant.Mk(FsProp.ForAll(arbA, arbB, f), label);

    public static Invariant ForAll<A, B, C>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Arbitrary<C> arbC
      , Func<A, B, C, bool> f
      )
    =>
        Invariant.Mk(FsProp.ForAll(arbA, arbB, arbC, f), label);

    public static Invariant ForAll<A, B, C>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Arbitrary<C> arbC
      , Func<A, B, C, Property> f
      )
    =>
        Invariant.Mk(FsProp.ForAll(arbA, arbB, arbC, f), label);

    public static Invariant ForAll<A, B, C, D>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Arbitrary<C> arbC
      , Arbitrary<D> arbD
      , Func<A, B, C, D, bool> f
      )
    =>
        Invariant.Mk
          ( FsProp.ForAll
              ( arbD
              , d => FsProp.ForAll(arbA, arbB, arbC, (a, b, c) => f(a, b, c, d))
              )
          , label
          );

    public static Invariant ForAll<A, B, C, D>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Arbitrary<C> arbC
      , Arbitrary<D> arbD
      , Func<A, B, C, D, Property> f
      )
    =>
        Invariant.Mk
          ( FsProp.ForAll
              ( arbD
              , d => FsProp.ForAll(arbA, arbB, arbC, (a, b, c) => f(a, b, c, d))
              )
          , label
          );

    public static Invariant All(this string label, params Property[] tests)
    =>
        Invariant.Mk(tests.Aggregate(PropertyExtensions.And), label);

    public static Test Group(this string label, params Test[] tests)
    =>
        Test.List(label, tests);
}

public sealed class Invariant
{
    public Property Property { get; }
    public string Label { get; }

    private Invariant(Property p, string l)
    {
        Property = p;
        Label = l;
    }

    public static Invariant Mk(Property p, string label)
    =>
        new Invariant(p, label);

    public Test Test
    =>
        FuchuFsCheckModule.testProperty<Property>(Label).Invoke(Property);

    public static implicit operator Test(Invariant p)
    =>
        p.Test;

    public static implicit operator Property(Invariant p)
    =>
        p.Property.Label(p.Label);
}

}

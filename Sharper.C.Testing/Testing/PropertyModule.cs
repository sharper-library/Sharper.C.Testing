using System;
using System.Linq;
using System.Collections.Immutable;
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
        InvariantProperty(FsProp.ForAll(arbA, f), label);

    public static Invariant ForAll<A>
      ( this string label
      , Arbitrary<A> arbA
      , Func<A, Property> f
      )
    =>
        InvariantProperty(FsProp.ForAll(arbA, f), label);

    public static Invariant ForAll<A, B>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Func<A, B, bool> f
      )
    =>
        InvariantProperty(FsProp.ForAll(arbA, arbB, f), label);

    public static Invariant ForAll<A, B>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Func<A, B, Property> f
      )
    =>
        InvariantProperty(FsProp.ForAll(arbA, arbB, f), label);

    public static Invariant ForAll<A, B, C>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Arbitrary<C> arbC
      , Func<A, B, C, bool> f
      )
    =>
        InvariantProperty(FsProp.ForAll(arbA, arbB, arbC, f), label);

    public static Invariant ForAll<A, B, C>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Arbitrary<C> arbC
      , Func<A, B, C, Property> f
      )
    =>
        InvariantProperty(FsProp.ForAll(arbA, arbB, arbC, f), label);

    public static Invariant ForAll<A, B, C, D>
      ( this string label
      , Arbitrary<A> arbA
      , Arbitrary<B> arbB
      , Arbitrary<C> arbC
      , Arbitrary<D> arbD
      , Func<A, B, C, D, bool> f
      )
    =>
        InvariantProperty
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
        InvariantProperty
          ( FsProp.ForAll
              ( arbD
              , d => FsProp.ForAll(arbA, arbB, arbC, (a, b, c) => f(a, b, c, d))
              )
          , label
          );

    public static Invariant All(this string label, params Invariant[] tests)
    =>
        Invariants.Mk(tests.ToImmutableList(), label);

    public static Test All(this string label, params Test[] tests)
    =>  Test.List(label, tests); 

    public static Invariant InvariantProperty(Property p, string label)
    =>
        new Invariant1(p, label);

    public abstract class Invariant
    {
        internal Invariant()
        {
        }

        public abstract string Label { get; }
        public abstract Property Property { get; }
        public abstract Test Test { get; }

        public static implicit operator Test(Invariant p)
        =>
            p.Test;

        public static implicit operator Property(Invariant p)
        =>
            p.Property.Label(p.Label);
    }

    private sealed class Invariant1
      : Invariant
    {
        internal Invariant1(Property p, string l)
        {
            Label = l;
            Property = p;
        }

        public override string Label { get; }
        public override Property Property { get; }

        public override Test Test
        =>
            FuchuFsCheckModule.testProperty<Property>(Label).Invoke(Property);
    }

    private sealed class Invariants
      : Invariant
    {
        internal Invariants(IImmutableList<Invariant> children, string l)
        {
            Children = children;
            Label = l;
        }

        public IImmutableList<Invariant> Children { get; }
        public override string Label { get; }

        public static Invariants Mk(IImmutableList<Invariant> children, string label)
        =>
            new Invariants(children, label);

        public override Property Property
        =>
            Children.Select(i => i.Property).Aggregate(PropertyExtensions.And).Label(Label);

        public override Test Test
        =>
            Test.List(Label, Children.Select(c => c.Test).ToArray());
    }
}

}

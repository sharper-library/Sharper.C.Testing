using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.FSharp.Collections;
using FsCheck;
using StdGen = FsCheck.Random.StdGen;
using Microsoft.FSharp.Core;
using FsProp = FsCheck.Prop;

namespace Sharper.C.Testing
{

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

    public static Invariant Wrap(this string label, Invariant inv)
    =>  Invariants.Mk(ImmutableList.Create(inv), label);

    public static Invariant All(this string label, params Invariant[] tests)
    =>
        Invariants.Mk(tests.ToImmutableList(), label);

    public static StdGen MkStdGen(int a, int b)
    =>  StdGen.NewStdGen(a, b);

    public delegate InvariantConfig ConfigF(InvariantRunner runner);

    public struct Testable
    {
        private Testable(Property p)
        {   Prop = p;
        }

        public Property Prop { get; }

        public static implicit operator Testable(Property p)
        =>  new Testable(p);

        public static implicit operator Testable(bool b)
        =>  new Testable(b.ToProperty());

        public static implicit operator Testable(Action a)
        =>  new Testable(a.ToProperty());

        public static implicit operator Testable(Invariant i)
        =>  new Testable(i.LabeledProperty);
    }

    public abstract class Invariant
    {
        internal Invariant()
        {
        }

        public abstract string Label { get; }
        public abstract Property Property { get; }

        public Property LabeledProperty
        =>  Property.Label(Label);

        public A Match<A>
          ( Func<Property, string, A> inv1
          , Func<IImmutableList<Invariant>, string, A> invs
          )
        =>  this is Invariant1
            ? inv1
                ( Property
                , Label
                )
            : invs(((Invariants)this).Children, Label);

        public IEnumerable<InvariantResult> Results(ConfigF config = null)
        {   var runner = InvariantRunner.Mk();
            var conf = (config ?? DefaultConfigF)(runner);
            foreach (var i in Linearize())
            {   i.Check(conf);
            }
            return runner.Results;
        }

        public IEnumerable<InvariantResult> Failures(ConfigF config = null)
        =>  Results(config).Where(r => r.Failed);

        public bool Passes()
        {   var runner = InvariantRunner.Mk();
            LabeledProperty.Check(new Configuration { Runner = runner });
            return runner.Results.All(r => r.Passed);
        }

        public bool Fails()
        => !Passes();

        public bool Check(ConfigF config = null)
        =>  !Failures(config).Any();

        public void CheckThrow(ConfigF config = null)
        {   var xs = Failures(config).ToImmutableList();
            if (xs.Any())
            {   throw new Exception
                  ( string.Join("\n", xs.Select(f => f.Message))
                  );
            }
        }

        public static implicit operator Property(Invariant p)
        =>
            p.LabeledProperty;

        public IEnumerable<Invariant> AsSeq()
        =>  Linearize().Cast<Invariant>();

        private IEnumerable<Invariant1> Linearize()
        =>  Linearize(ImmutableList.Create<string>());

        private IEnumerable<Invariant1> Linearize
          ( IImmutableList<string> prefix
          )
        =>  Match
              ( (p, l) =>
                    new[]
                    { new Invariant1(p, string.Join("/", prefix.Add(l)))
                    }
              , (xs, _) => xs.SelectMany(x => x.Linearize(prefix.Add(Label)))
              );
    }

    private sealed class Invariant1
      : Invariant
    {
        internal Invariant1
          ( Property p
          , string l
          )
        {
            Label = l;
            Property = p;
        }

        public override string Label { get; }
        public override Property Property { get; }

        public void Check(Configuration config)
        =>  LabeledProperty.Check(config);
    }

    private sealed class Invariants
      : Invariant
    {
        internal Invariants(IImmutableList<Invariant> children, string l)
        {   Children = children;
            Label = l;
        }

        public IImmutableList<Invariant> Children { get; }

        public override string Label { get; }

        public static Invariants Mk
          ( IImmutableList<Invariant> children
          , string label
          )
        =>  new Invariants(children, label);

        public override Property Property
        =>  Children
            .Select(i => i.LabeledProperty)
            .Aggregate(PropertyExtensions.And)
            .Label(Label);
    }

    public struct InvariantResult
    {
        public string Label { get; }

        private TestResult Result { get; }

        private InvariantResult(string label, TestResult result)
        {   Label = label;
            Result = result;
        }

        public string Message
        =>  Runner.onFinishedToString(Label, Result);

        public bool Passed
        =>  Result.IsTrue;

        public bool Failed
        =>  !Passed;

        public bool Exhausted
        =>  Result.IsExhausted;

        public static InvariantResult Mk
          ( string label
          , TestResult result
          )
        =>  new InvariantResult(label, result);
    }

    public struct InvariantRunner
      : IRunner
    {
        private readonly List<InvariantResult> results;

        private InvariantRunner(List<InvariantResult> results)
        {   this.results = results;
        }

        public static InvariantRunner Mk()
        =>  new InvariantRunner(new List<InvariantResult>());

        public IEnumerable<InvariantResult> Results
        =>  results;

        public void OnArguments
          ( int ntest
          , FSharpList<object> args
          , FSharpFunc<int, FSharpFunc<FSharpList<object>, string>> every
          )
        =>  Runner.consoleRunner.OnArguments(ntest, args, every);

        public void OnFinished(string name, TestResult result)
        =>  results.Add(InvariantResult.Mk(name, result));

        public void OnShrink
          ( FSharpList<object> args
          , FSharpFunc<FSharpList<object>, string> everyShrink
          )
        =>  Runner.consoleRunner.OnShrink(args, everyShrink);

        public void OnStartFixture(Type t)
        =>  Runner.consoleRunner.OnStartFixture(t);
    }

    public sealed class InvariantConfig
      : Configuration
    {
        public new InvariantRunner Runner
        {
            get { return (InvariantRunner) base.Runner; }
            set { base.Runner = value; }
        }
    }

    internal static Invariant InvariantProperty(Property p, string label)
    =>  new Invariant1(p, label);

    private static Configuration Config
      ( IRunner runner
      , int? maxTests = null
      , StdGen replay = null
      )
    =>  new Configuration
        { Runner = runner
        , Replay = replay
        , MaxNbOfTest = maxTests ?? FsCheck.Config.Default.MaxTest
        };

    private static InvariantConfig DefaultConfigF(InvariantRunner r)
    =>  new InvariantConfig { Runner = r };
}

}

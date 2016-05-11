using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FsCheck;

namespace Sharper.C.Testing
{
    using ConfigF = Func<InvariantRunner, InvariantConfig>;

    public abstract class Invariant
    {
        internal Invariant()
        {
        }

        public static Invariant Single(Property prop, string label)
        =>  new SingleCase(prop, label);

        public static Invariant Multiple
          ( IImmutableList<Invariant> children
          , string label
          )
        =>  new MultipleCase(children, label);

        public abstract string Label { get; }

        public Property LabeledProperty
        =>  Match
              ( (p, l) => p.Label(l)
              , (xs, l) =>
                    xs
                    .Select(x => (Property) x)
                    .Aggregate(PropertyExtensions.And)
                    .Label(l)
              );

        public A Match<A>
          ( Func<Property, string, A> inv1
          , Func<IImmutableList<Invariant>, string, A> invs
          )
        =>  this is SingleCase
            ? inv1
                ( ((SingleCase)this).Property
                , Label
                )
            : invs(((MultipleCase)this).Children, Label);

        public IEnumerable<InvariantResult> Results(ConfigF config = null)
        {   var runner = InvariantRunner.Mk();
            var conf = (config ?? InvariantConfig.Default)(runner);
            foreach (var i in Linearize())
            {   i.RunCheck(conf);
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

        public static implicit operator Property(Invariant i)
        =>  i.LabeledProperty;

        public IEnumerable<Invariant> AsSeq()
        =>  Linearize().Cast<Invariant>();

        private IEnumerable<SingleCase> Linearize()
        =>  Linearize(ImmutableList.Create<string>());

        private IEnumerable<SingleCase> Linearize
          ( IImmutableList<string> prefix
          )
        =>  Match
              ( (p, l) =>
                    new[]
                    { new SingleCase(p, string.Join("/", prefix.Add(l)))
                    }
              , (xs, _) => xs.SelectMany(x => x.Linearize(prefix.Add(Label)))
              );

        private sealed class SingleCase
          : Invariant
        {
            internal SingleCase
              ( Property p
              , string l
              )
            {
                Label = l;
                Property = p;
            }

            public override string Label { get; }
            public Property Property { get; }

            public void RunCheck(Configuration config)
            =>  LabeledProperty.Check(config);
        }

        private sealed class MultipleCase
          : Invariant
        {
            internal MultipleCase(IImmutableList<Invariant> children, string l)
            {   Children = children;
                Label = l;
            }

            public IImmutableList<Invariant> Children { get; }

            public override string Label { get; }

            public static MultipleCase Mk
              ( IImmutableList<Invariant> children
              , string label
              )
            =>  new MultipleCase(children, label);
        }
    }
}

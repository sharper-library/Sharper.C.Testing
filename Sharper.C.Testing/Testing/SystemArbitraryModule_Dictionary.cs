using System.Collections.Generic;
using System.Linq;
using FsCheck;

namespace Sharper.C.Testing
{

public static partial class SystemArbitraryModule
{
    public static Arbitrary<Dictionary<A, B>> AnyDict<A, B>
      ( Arbitrary<A> arbA
      , Arbitrary<B> arbB
      )
    =>
        Arb.From(DictGen(arbA, arbB), DictShrink);

    public static Arbitrary<IDictionary<A, B>> AsIDict<A, B>
      ( this Arbitrary<Dictionary<A, B>> arb
      )
    =>
        arb.Cast<Dictionary<A, B>, IDictionary<A, B>>();

    private static Gen<Dictionary<A, B>> DictGen<A, B>
      ( Arbitrary<A> arbA
      , Arbitrary<B> arbB
      )
    =>
        from keys in
            Gen.ArrayOf(arbA.Generator)
            .Select
              ( seq =>
                    seq
                    .Where(a => !ReferenceEquals(null, a))
                    .Distinct()
                    .ToList()
              )
        from values in Gen.ArrayOf(keys.Count(), arbB.Generator)
        select
            keys
            .Zip(values, (k, v) => new {k, v})
            .ToDictionary(x => x.k, x => x.v);

    private static IEnumerable<Dictionary<A, B>> DictShrink<A, B>
      ( Dictionary<A, B> dict
      )
    {
        var keys = dict.Keys.ToList();
        for (var n = keys.Count() - 2; n >= 0; --n)
        {
            var ks = keys.Take(n);
            var vs = dict.Values.Take(n);
            yield return
                ks
                .Zip(vs, (k, v) => new {k, v})
                .ToDictionary(x => x.k, x => x.v);
        }
    }

}

}

